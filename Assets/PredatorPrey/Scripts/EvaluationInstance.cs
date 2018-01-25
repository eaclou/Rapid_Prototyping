using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationInstance : MonoBehaviour {
    public bool visible = false;
    public bool isExhibition = false;
    public bool gameWonOrLost = false;    
    
    public EvaluationTicket currentEvalTicket;
    private TeamsConfig teamsConfig;
    
    public FitnessComponentEvaluationGroup fitnessComponentEvaluationGroup;

    public Agent[] currentAgentsArray;
    public float[][] agentGameScoresArray;
    private Environment currentEnvironment;
    
    public int maxTimeSteps;
    public int currentTimeStep = 0;

    public void Tick() {
        //print("Tick! " + currentTimeStep.ToString());

        CalculateGameScores();
        CalculateFitnessScores();

        //currentEnvironment.RunModules();
        for (int i = 0; i < currentAgentsArray.Length; i++) {
            if (currentTimeStep % 1 == 0) {
                currentAgentsArray[i].TickBrain();
            }
            currentAgentsArray[i].RunModules(currentTimeStep, currentEnvironment);            
        }

        if (CheckForEvaluationEnd()) {
            currentEvalTicket.status = EvaluationTicket.EvaluationStatus.PendingComplete;
            
            if (!isExhibition) {
                AverageFitnessComponentsByTimeSteps();
            }
            else {
                //Debug.Log("isExhibition!!!CheckForEvaluationEnd");
                if (gameWonOrLost) {
                    //Debug.Log("isExhibition!!!gameWonOrLost!!!CheckForEvaluationEnd");
                    if (agentGameScoresArray.Length > 1) {
                        if (agentGameScoresArray[0][0] > 0f) {
                            //Debug.Log("Player 1 WINS!!!");

                        }
                        else {
                            //Debug.Log("Player 2 WINS!!!");

                        }
                    }
                    else {
                        if (agentGameScoresArray[0][0] < 0f) {
                            //Debug.Log("Player 1 DIED!!!");
                        }
                    }
                }
            }
        }
        else {
            currentTimeStep++;
        }        
    }

    private bool CheckForEvaluationEnd() {
        bool isEnded = false;

        if (currentTimeStep > maxTimeSteps) {
            isEnded = true;
        }
        if (gameWonOrLost) {
            isEnded = true;
        }

        return isEnded;
    }
    public void ClearInstance() {
        if (currentEvalTicket != null) {
            currentEvalTicket.status = EvaluationTicket.EvaluationStatus.Pending;
            currentEvalTicket = null;
        }
        DeleteAllGameObjects();
    }
    public void DeleteAllGameObjects() {
        if (currentAgentsArray != null) {
            for (int i = 0; i < currentAgentsArray.Length; i++) {
                if (currentAgentsArray[i] != null) {
                    currentAgentsArray[i].gameObject.SetActive(false);
                }
            }
        }
        var children = new List<GameObject>();
        foreach (Transform child in gameObject.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }

    public void SetUpInstance(EvaluationTicket evalTicket, TeamsConfig teamsConfig) {
        this.teamsConfig = teamsConfig;        
        this.maxTimeSteps = evalTicket.maxTimeSteps;
        
        currentEvalTicket = evalTicket;

        BruteForceInit();

        currentEvalTicket.status = EvaluationTicket.EvaluationStatus.InProgress;
    }

    private void BruteForceInit() {
        // REFACTOR THIS!!!!!!!!!!!!!!
        //Debug.Log("BruteForceInit!" + currentTimeStep.ToString());

        // Clear Everything:
        DeleteAllGameObjects();

        currentTimeStep = 0;
        gameWonOrLost = false; // <-- revisit this shit

        currentAgentsArray = new Agent[currentEvalTicket.agentGenomesList.Count];
        agentGameScoresArray = new float[currentEvalTicket.agentGenomesList.Count][];
        for (int i = 0; i < agentGameScoresArray.Length; i++) {
            agentGameScoresArray[i] = new float[1];
        }

        // Create Environment:
        CreateEnvironment();

        // Create Agents:        
        for (int i = 0; i < currentAgentsArray.Length; i++) {

            // Create Agent Base Body:
            //GameObject agentGO = Instantiate(Resources.Load(AgentBodyGenomeTemplate.GetAgentBodyTypeURL(currentEvalTicket.agentGenomesList[i].bodyGenome.bodyType))) as GameObject;
            //GameObject agentGO = new GameObject("Agent_" + currentEvalTicket.agentGenomesList[i].index.ToString());
            GameObject agentGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            agentGO.name = "Player[" + i.ToString() + "] Agent[" + currentEvalTicket.agentGenomesList[i].index.ToString() + "]";
            agentGO.transform.parent = gameObject.transform;
            agentGO.transform.localPosition = currentEvalTicket.environmentGenome.agentStartPositionsList[i].agentStartPosition;
            agentGO.transform.localRotation = currentEvalTicket.environmentGenome.agentStartPositionsList[i].agentStartRotation;
            agentGO.GetComponent<Collider>().enabled = false;
            Agent agentScript = agentGO.AddComponent<Agent>();            
            agentScript.isVisible = visible;

            agentScript.InitializeAgentFromTemplate(currentEvalTicket.agentGenomesList[i], currentEvalTicket.environmentGenome.agentStartPositionsList[i]);

            currentAgentsArray[i] = agentScript;
        }
        
        HookUpModules();

        //SetInvisibleTraverse(gameObject);
        if (visible) {
            currentEnvironment.AddRenderableContent(currentEvalTicket.environmentGenome);
            SetVisibleTraverse(gameObject);
        }
        else {
            SetInvisibleTraverse(gameObject);
        }

        if (isExhibition) {

        }
        else {
            // Fitness Crap only if NON-exhibition!:
            
            FitnessManager fitnessManager;
            int genomeIndex;
            if (currentEvalTicket.focusPopIndex == 0) {  // environment
                fitnessManager = teamsConfig.environmentPopulation.fitnessManager;
                genomeIndex = currentEvalTicket.environmentGenome.index;
            }
            else {  // a player
                fitnessManager = teamsConfig.playersList[currentEvalTicket.focusPopIndex - 1].fitnessManager;
                genomeIndex = currentEvalTicket.agentGenomesList[currentEvalTicket.focusPopIndex - 1].index;
            }
            fitnessComponentEvaluationGroup = new FitnessComponentEvaluationGroup();
            // Creates a copy inside this, and also a copy in the FitnessManager, but they share refs to the FitComps themselves:            
            
            fitnessComponentEvaluationGroup.CreateFitnessComponentEvaluationGroup(fitnessManager, genomeIndex);
            //Debug.Log("currentEvalTicket.focusPopIndex: " + currentEvalTicket.focusPopIndex.ToString() + ", index: " + currentEvalTicket.genomeIndices[currentEvalTicket.focusPopIndex].ToString());
            HookUpFitnessComponents();
            
        }
    }
    private void CreateEnvironment() {

        GameObject environmentGO = new GameObject("environment");
        Environment environmentScript = environmentGO.AddComponent<Environment>();
        currentEnvironment = environmentScript;
        environmentGO.transform.parent = gameObject.transform;
        environmentGO.transform.localPosition = new Vector3(0f, 0f, 0f);

        // OLD ( REFACTORING for non-physx)
        /*
        if (currentEvalTicket.environmentGenome.gameplayPrefab == null) {
            // This might only work if environment is completely static!!!! otherwise it could change inside original evalInstance and then that
            // changed environment would be instantiated as fresh Environments for subsequent Evals!            
            environmentScript.CreateCollisionAndGameplayContent(currentEvalTicket.environmentGenome);
        }
        else {
            // Already built            
            EnvironmentGameplay environmentGameplayScript = Instantiate<EnvironmentGameplay>(currentEvalTicket.environmentGenome.gameplayPrefab) as EnvironmentGameplay;
            
            currentEnvironment.environmentGameplay = environmentGameplayScript;            
            currentEnvironment.environmentGameplay.gameObject.transform.parent = currentEnvironment.gameObject.transform;
            currentEnvironment.environmentGameplay.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        */
    }

    public void HookUpModules() {
        for(int p = 0; p < currentAgentsArray.Length; p++) {
            for(int e = 0; e < currentAgentsArray.Length; e++) {
                if(e != p) {  // not vs self:
                    currentAgentsArray[p].testModule.enemyTransform = currentAgentsArray[e].transform;
                    //currentAgentsArray[p].testModule.enemyPosX[0] = currentAgentsArray[e].testModule.posX[0] - currentAgentsArray[p].testModule.posX[0];
                    //currentAgentsArray[p].testModule.enemyPosY[0] = currentAgentsArray[e].testModule.posY[0] - currentAgentsArray[p].testModule.posY[0];
                }
            }
        }
        
        /*switch (challengeType) {
            case Challenge.Type.Test:
                for (int i = 0; i < currentAgentsArray[0].targetSensorList.Count; i++) {
                    currentAgentsArray[0].targetSensorList[i].targetPosition = currentEnvironment.environmentGameplay.targetColumn.gameObject.transform;

                    if (isExhibition) {
                        DoodadManager doodadManager = currentAgentsArray[i].gameObject.GetComponent<DoodadManager>();
                        if (doodadManager != null) {

                            doodadManager.neuronID_01 = UnityEngine.Random.Range(0, currentAgentsArray[i].brain.neuronList.Count);
                            doodadManager.neuronID_02 = UnityEngine.Random.Range(0, currentAgentsArray[i].brain.neuronList.Count);
                            doodadManager.neuronID_03 = UnityEngine.Random.Range(0, currentAgentsArray[i].brain.neuronList.Count);
                        }
                    }
                }
                break;
            case Challenge.Type.Racing:
                // TEMP!
                for (int i = 0; i < currentAgentsArray[0].targetSensorList.Count; i++) {
                    currentAgentsArray[0].targetSensorList[i].targetPosition = currentEnvironment.transform;
                }
                break;
            case Challenge.Type.Combat:
                for (int i = 0; i < currentAgentsArray[0].targetSensorList.Count; i++) {
                    currentAgentsArray[0].targetSensorList[i].targetPosition = currentAgentsArray[1].rootObject.transform;
                }
                for (int i = 0; i < currentAgentsArray[1].targetSensorList.Count; i++) {
                    currentAgentsArray[1].targetSensorList[i].targetPosition = currentAgentsArray[0].rootObject.transform;
                }
                break;
            default:
                break;
        }*/
    }
    public void HookUpFitnessComponents() {
        // Implementing Fitness Later Differently
        
        for (int i = 0; i < fitnessComponentEvaluationGroup.fitCompList.Count; i++) {
            int populationIndex = 0; // defaults to player1
            if (currentEvalTicket.focusPopIndex != 0) {  // if environment is not the focus Pop, set correct playerIndex:
                populationIndex = currentEvalTicket.focusPopIndex - 1;
            }
            switch (fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.type) {
                case FitnessComponentType.DistanceToEnemy:
                    FitCompDistanceToEnemy fitCompDistToTargetSquared = (FitCompDistanceToEnemy)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompDistanceToEnemy;
                    fitCompDistToTargetSquared.ownPos = currentAgentsArray[populationIndex].transform.localPosition;
                    fitCompDistToTargetSquared.enemyPos = currentAgentsArray[populationIndex].testModule.enemyTransform.localPosition;
                    break;                
                case FitnessComponentType.Random:
                    // handled fully within the FitCompRandom class
                    break;                
                default:
                    Debug.LogError("ERROR!!! Fitness Type not found!!! " + fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.type.ToString());
                    break;
            }
        }
    }
    private void CalculateFitnessScores() {        
        if (!isExhibition) {
            // Temp for now: in order to update positions...
            HookUpFitnessComponents();

            for (int i = 0; i < fitnessComponentEvaluationGroup.fitCompList.Count; i++) {
                fitnessComponentEvaluationGroup.fitCompList[i].TickScore();
            }
        }
    }
    // !#$!#$!@ HARDCODED FOR 1 or 2 players only!!!!
    public void CalculateGameScores() {  // only applies to players for now...
        //float winLossDraw = 0f;

        if (gameWonOrLost) {  // if game is over

        }
        else {
            //float dotUp = Vector3.Dot(currentAgentsArray[0].rootObject.transform.up, new Vector3(0f, 1f, 0f));
            //if (dotUp < 0.3) {
            //    agentGameScoresArray[0][0] = -5f;
            //    gameWonOrLost = false;
            //}
        }

        if (isExhibition) {
            gameWonOrLost = false;
        }
    }
    public void AverageFitnessComponentsByTimeSteps() {
        
        for (int i = 0; i < fitnessComponentEvaluationGroup.fitCompList.Count; i++) {
            if (fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.measure == FitnessComponentMeasure.Avg) {
                if (currentTimeStep > 1)
                    fitnessComponentEvaluationGroup.fitCompList[i].rawScore /= (currentTimeStep - 1);
            }
        }
    }

    public void SetInvisibleTraverse(GameObject obj) {
        //obj.layer = LayerMask.NameToLayer("Hidden");
        
        var children = new List<GameObject>();
        foreach (Transform child in obj.transform) children.Add(child.gameObject);
        children.ForEach(child => SetInvisibleTraverse(child.gameObject));
    }
    public void SetVisibleTraverse(GameObject obj) {
        //obj.layer = LayerMask.NameToLayer("Default");
        
        var children = new List<GameObject>();
        foreach (Transform child in obj.transform) children.Add(child.gameObject);
        children.ForEach(child => SetVisibleTraverse(child.gameObject));
    }
}
