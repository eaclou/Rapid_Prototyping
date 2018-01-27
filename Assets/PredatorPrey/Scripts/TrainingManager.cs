using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour {

    public GameManager gameManager;
    public TeamsConfig teamsConfig; // holds all the data    
    public EvaluationManager evaluationManager;  // keeps track of evaluation pairs & instances

    public bool isTraining = false; // actively evaluating
    public bool trainingPaused = false;
    public bool TrainingPaused
    {
        get
        {
            return trainingPaused;
        }
        set
        {

        }
    }
    public float playbackSpeed = 1f;

    // Training Status:
    public int playingCurGen = 0;

    public int debugFrameCounter = 0;

    // Use this for initialization
    void Start () {
		
	}

    public void NewTrainingMode() {
        
        // Initialize
        int numPlayers = 2;
        
        // environment is evolvable, 1 player:
        teamsConfig = new TeamsConfig(numPlayers, 1, 1);

        playingCurGen = 0;

        evaluationManager = new EvaluationManager();
        // Need to make sure all populations have their representatives set up before calling this:
        // Right now this is done through the teamsConfig Constructor
        evaluationManager.InitializeNewTraining(teamsConfig); // should I just roll this into the Constructor?

        isTraining = true;
        //cameraEnabled = true;

        //gameManager.uiManager.panelTraining.moduleViewUI.SetPendingGenomesFromData(this);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Tick() {
        if (isTraining) { // && debugFrameCounter < 2) {
            //Debug.Log("FixedUpdate isTraining");
            if (evaluationManager.allEvalsComplete) {
                NextGeneration();
            }
            else {
                //gameManager.cameraEnabled = true;
                evaluationManager.Tick(teamsConfig);
            }
            debugFrameCounter++;
        }
    }

    public void TogglePlayPause() {
        print("togglePlayPause");
        if (trainingPaused) {
            Time.timeScale = Mathf.Clamp(playbackSpeed, 0.1f, 100f);
        }
        else {
            Time.timeScale = 0f;
        }
        trainingPaused = !trainingPaused;
    }
    public void Pause() {
        print("Pause");
        trainingPaused = true;
        Time.timeScale = 0f;
    }

    public void IncreasePlaybackSpeed() {
        playbackSpeed = Mathf.Clamp(playbackSpeed * 1.5f, 0.1f, 100f);
        Time.timeScale = Mathf.Clamp(playbackSpeed, 0.1f, 100f);
    }
    public void DecreasePlaybackSpeed() {
        playbackSpeed = Mathf.Clamp(playbackSpeed * 0.66667f, 0.1f, 100f);
        Time.timeScale = Mathf.Clamp(playbackSpeed, 0.1f, 100f);
    }

    public void Play1X() {
        playbackSpeed = 1f;
        Time.timeScale = Mathf.Clamp(playbackSpeed, 0.1f, 100f);
        trainingPaused = false;
    }
    public void Play4X() {
        playbackSpeed = 4f;
        Time.timeScale = Mathf.Clamp(playbackSpeed, 0.1f, 100f);
        trainingPaused = false;
    }
    public void Play25X() {
        playbackSpeed = 12f;
        Time.timeScale = Mathf.Clamp(playbackSpeed, 0.1f, 100f);
        trainingPaused = false;
    }

    private void NextGeneration() {
        Debug.Log("Generation " + playingCurGen.ToString() + " Complete!");
        if(playingCurGen == 100) {
            
        }
        // Crossover:
        teamsConfig.environmentPopulation.fitnessManager.ProcessAndRankRawFitness(teamsConfig.environmentPopulation.popSize);
        for (int i = 0; i < teamsConfig.playersList.Count; i++) {
            //Debug.Log("Player " + i.ToString());
            teamsConfig.playersList[i].fitnessManager.ProcessAndRankRawFitness(teamsConfig.playersList[i].popSize);            
        }
        Crossover();

        // Cleanup for next Gen:
        // Reset fitness data:
        
        teamsConfig.environmentPopulation.fitnessManager.InitializeForNewGeneration(teamsConfig.environmentPopulation.environmentGenomeList.Count);
        teamsConfig.environmentPopulation.historicGenomePool.Add(teamsConfig.environmentPopulation.environmentGenomeList[0]);
        teamsConfig.environmentPopulation.ResetRepresentativesList();

        for (int i = 0; i < teamsConfig.playersList.Count; i++) {            
            teamsConfig.playersList[i].fitnessManager.InitializeForNewGeneration(teamsConfig.playersList[i].agentGenomeList.Count);
            //teamsConfig.playersList[i].historicGenomePool.Add(teamsConfig.playersList[i].agentGenomeList[0]);
            teamsConfig.playersList[i].AddNewHistoricalRepresentative(teamsConfig.playersList[i].agentGenomeList[0]);
            teamsConfig.playersList[i].ResetRepresentativesList();

            teamsConfig.playersList[i].trainingSettingsManager.mutationStepSize *= 0.996f;
            teamsConfig.playersList[i].trainingSettingsManager.mutationChance *= 0.996f;
            teamsConfig.playersList[i].trainingSettingsManager.newHiddenNodeChance *= 0.99f;
            teamsConfig.playersList[i].trainingSettingsManager.newLinkChance *= 0.995f;
        }

        evaluationManager.maxTimeStepsDefault += 4;
        Mathf.Clamp(evaluationManager.maxTimeStepsDefault, 128, 4096);

        // Best Performer Brain:
        //teamsConfig.playersList[0].agentGenomeList[0].PrintBrainGenome();

        // Reset default evals + exhibition
        evaluationManager.ResetForNewGeneration(teamsConfig);        

        playingCurGen++;        
    }
    private void Crossover() {
        // Query Fitness Managers to create:
        // List of ranked Fitness scores (processed data)
        // Parallel List of the indices corresponding to those fitness scores.
        // Then the crossover/mutation functions should only need to take in the Fitness Manager in order to operate...

        if (teamsConfig.environmentPopulation.isTraining) {
            EnvironmentCrossover();
        }
        for (int i = 0; i < teamsConfig.playersList.Count; i++) {
            if (teamsConfig.playersList[i].isTraining) {
                AgentCrossover(i);
            }
        }
    }
    private void EnvironmentCrossover() {

        /*int randStart = UnityEngine.Random.Range(0, 4);
        if(randStart == 0) {
            StartPositionGenome newStart0 = new StartPositionGenome(new Vector3(-5f, 0f, 0f), Quaternion.identity);
            StartPositionGenome newStart1 = new StartPositionGenome(new Vector3(5f, 0f, 0f), Quaternion.identity);
            teamsConfig.environmentPopulation.environmentGenomeList[0].agentStartPositionsList[0] = newStart0;
            teamsConfig.environmentPopulation.environmentGenomeList[0].agentStartPositionsList[1] = newStart1;
        }
        else if(randStart == 1) {
            StartPositionGenome newStart0 = new StartPositionGenome(new Vector3(5f, 0f, 0f), Quaternion.identity);
            StartPositionGenome newStart1 = new StartPositionGenome(new Vector3(-5f, 0f, 0f), Quaternion.identity);
            teamsConfig.environmentPopulation.environmentGenomeList[0].agentStartPositionsList[0] = newStart0;
            teamsConfig.environmentPopulation.environmentGenomeList[0].agentStartPositionsList[1] = newStart1;
        }
        else if (randStart == 2) {
            StartPositionGenome newStart0 = new StartPositionGenome(new Vector3(0f, -5f, 0f), Quaternion.identity);
            StartPositionGenome newStart1 = new StartPositionGenome(new Vector3(0f, 5f, 0f), Quaternion.identity);
            teamsConfig.environmentPopulation.environmentGenomeList[0].agentStartPositionsList[0] = newStart0;
            teamsConfig.environmentPopulation.environmentGenomeList[0].agentStartPositionsList[1] = newStart1;
        }
        else {
            StartPositionGenome newStart0 = new StartPositionGenome(new Vector3(0f, 5f, 0f), Quaternion.identity);
            StartPositionGenome newStart1 = new StartPositionGenome(new Vector3(0f, -5f, 0f), Quaternion.identity);
            teamsConfig.environmentPopulation.environmentGenomeList[0].agentStartPositionsList[0] = newStart0;
            teamsConfig.environmentPopulation.environmentGenomeList[0].agentStartPositionsList[1] = newStart1;
        }*/
        for(int e = 0; e < teamsConfig.environmentPopulation.environmentGenomeList.Count; e++) {
            float randAngleRad = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
            float randRadius = UnityEngine.Random.Range(2.5f, 7.5f);
            StartPositionGenome newStart0 = new StartPositionGenome(new Vector3(Mathf.Cos(randAngleRad) * randRadius, Mathf.Sin(randAngleRad) * randRadius, 0f), Quaternion.identity);
            StartPositionGenome newStart1 = new StartPositionGenome(new Vector3(Mathf.Cos(randAngleRad + Mathf.PI) * randRadius, Mathf.Sin(randAngleRad + Mathf.PI) * randRadius, 0f), Quaternion.identity);
            teamsConfig.environmentPopulation.environmentGenomeList[e].agentStartPositionsList[0] = newStart0;
            teamsConfig.environmentPopulation.environmentGenomeList[e].agentStartPositionsList[1] = newStart1;
        }
        

        /*
        List<EnvironmentGenome> newGenGenomeList = new List<EnvironmentGenome>(); // new population!     

        FitnessManager fitnessManager = teamsConfig.environmentPopulation.fitnessManager;
        TrainingSettingsManager trainingSettingsManager = teamsConfig.environmentPopulation.trainingSettingsManager;
        float mutationChance = trainingSettingsManager.mutationChance;
        float mutationStepSize = trainingSettingsManager.mutationStepSize;

        // Keep top-half peformers + mutations:
        for (int x = 0; x < teamsConfig.environmentPopulation.environmentGenomeList.Count; x++) {
            if (false) { //x == 0) {
                // Top performer stays
                EnvironmentGenome parentGenome = teamsConfig.environmentPopulation.environmentGenomeList[fitnessManager.rankedIndicesList[x]];
                parentGenome.index = 0;
                newGenGenomeList.Add(parentGenome);
            }
            else {
                int parentIndex = fitnessManager.GetAgentIndexByLottery();

                EnvironmentGenome parentGenome = teamsConfig.environmentPopulation.environmentGenomeList[parentIndex];
                EnvironmentGenome newGenome = parentGenome.BirthNewGenome(parentGenome, newGenGenomeList.Count, teamsConfig.challengeType, mutationChance, mutationStepSize);

                newGenGenomeList.Add(newGenome);
            }
        }

        for (int i = 0; i < teamsConfig.environmentPopulation.environmentGenomeList.Count; i++) {
            teamsConfig.environmentPopulation.environmentGenomeList[i] = newGenGenomeList[i];
        }
        */
    }
    private void AgentCrossover(int playerIndex) {
        
        List<BrainGenome> newGenBrainGenomeList = new List<BrainGenome>(); // new population!        

        FitnessManager fitnessManager = teamsConfig.playersList[playerIndex].fitnessManager;
        TrainingSettingsManager trainingSettingsManager = teamsConfig.playersList[playerIndex].trainingSettingsManager;
        
        // Keep top-half peformers + mutations:
        for (int x = 0; x < teamsConfig.playersList[playerIndex].agentGenomeList.Count; x++) {
            if (x == 0) {
                BrainGenome parentGenome = teamsConfig.playersList[playerIndex].agentGenomeList[fitnessManager.rankedIndicesList[x]].brainGenome;
                //parentGenome.index = 0;
                newGenBrainGenomeList.Add(parentGenome);
            }
            else {
                BrainGenome newBrainGenome = new BrainGenome();

                int parentIndex = fitnessManager.GetAgentIndexByLottery();

                BrainGenome parentGenome = teamsConfig.playersList[playerIndex].agentGenomeList[parentIndex].brainGenome;

                newBrainGenome.SetToMutatedCopyOfParentGenome(parentGenome, trainingSettingsManager);
                newGenBrainGenomeList.Add(newBrainGenome);
            }
        }

        for (int i = 0; i < teamsConfig.playersList[playerIndex].agentGenomeList.Count; i++) {
            teamsConfig.playersList[playerIndex].agentGenomeList[i].brainGenome = newGenBrainGenomeList[i];
        }
        
    }
}
