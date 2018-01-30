using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImitationLearningManager : MonoBehaviour {

    public ImitationUI imitationUI;

    public GameMode gameMode;
    public enum GameMode {
        Off,
        DataCollection,
        Training
    }
    public bool isPaused = true;

    public int numDataCollectionRounds = 8;
    public int curDataCollectionRound = 0;

    public int curTrainingGen = 0;
    public float avgFitnessLastGen = 0f;

    public int maxTrialTimeSteps = 512;
    public int curTimeStep = 0;

    public float targetRadius = 3f;
    public float minSpawnRadius = 3f;
    public float maxSpawnRadius = 18f;

    public GameObject playerGO;
    public GameObject targetGO;

    public Agent playerAgent;
    public Agent targetAgent;

    //public PlayerController playerScript;

    public List<DataSample> dataSamplesList;
    private int dataCollectionFrequency = 18;

    public int trainingPopulationSize = 64;
    private List<AgentGenome> agentGenomeList;
    //private Brain agentBrainList;
    private TrainingSettingsManager trainingSettingsManager;

    public int curTestingAgent = 0;
    public int curTestingSample = 0;

    private Agent dummyAgent;

    public GameObject displayGO;
    private Agent displayAgent;
    private StartPositionGenome displayStartPos;

    public float[] rawFitnessScoresArray;

    private StartPositionGenome dummyStartGenome;

    private bool training = true;

    private float lastHorizontalInput = 0f;
    private float lastVerticalInput = 0f;

    // Use this for initialization
    void Start () {
        gameMode = GameMode.Off;
        Time.timeScale = 0f;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void CreateFakeData() {
        dataSamplesList = new List<DataSample>();

        DataSample sample1 = new DataSample();
        sample1.inputDataArray[0] = 1f;
        sample1.inputDataArray[1] = 0f;
        sample1.inputDataArray[2] = 0f;
        sample1.inputDataArray[3] = 10f;
        sample1.inputDataArray[4] = 10f;
        sample1.inputDataArray[5] = 0f;
        sample1.inputDataArray[6] = 0f;        
        sample1.inputDataArray[7] = 0.77f;
        sample1.inputDataArray[8] = 0.77f;
        sample1.inputDataArray[9] = 0f;
        sample1.inputDataArray[10] = 0f;
        sample1.inputDataArray[11] = 0f;
        sample1.inputDataArray[12] = 0f;
        
        sample1.outputDataArray[0] = 1f;
        sample1.outputDataArray[1] = 1f;

        dataSamplesList.Add(sample1);  // Add to master List

        DataSample sample2 = new DataSample();
        sample2.inputDataArray[0] = 1f;
        sample2.inputDataArray[1] = 0f;
        sample2.inputDataArray[2] = 0f;
        sample2.inputDataArray[3] = 10f;
        sample2.inputDataArray[4] = -10f;
        sample2.inputDataArray[5] = 0f;
        sample2.inputDataArray[6] = 0f;
        sample2.inputDataArray[7] = 0.77f;
        sample2.inputDataArray[8] = -0.77f;
        sample2.inputDataArray[9] = 0f;
        sample2.inputDataArray[10] = 0f;
        sample2.inputDataArray[11] = 0f;
        sample2.inputDataArray[12] = 0f;

        sample2.outputDataArray[0] = 1f;
        sample2.outputDataArray[1] = -1f;

        dataSamplesList.Add(sample2);  // Add to master List

        DataSample sample3 = new DataSample();
        sample3.inputDataArray[0] = 1f;
        sample3.inputDataArray[1] = 0f;
        sample3.inputDataArray[2] = 0f;
        sample3.inputDataArray[3] = -10f;
        sample3.inputDataArray[4] = 10f;
        sample3.inputDataArray[5] = 0f;
        sample3.inputDataArray[6] = 0f;
        sample3.inputDataArray[7] = -0.77f;
        sample3.inputDataArray[8] = 0.77f;
        sample3.inputDataArray[9] = 0f;
        sample3.inputDataArray[10] = 0f;
        sample3.inputDataArray[11] = 0f;
        sample3.inputDataArray[12] = 0f;

        sample3.outputDataArray[0] = -1f;
        sample3.outputDataArray[1] = 1f;

        dataSamplesList.Add(sample3);  // Add to master List

        DataSample sample4 = new DataSample();
        sample4.inputDataArray[0] = 1f;
        sample4.inputDataArray[1] = 0f;
        sample4.inputDataArray[2] = 0f;
        sample4.inputDataArray[3] = -10f;
        sample4.inputDataArray[4] = -10f;
        sample4.inputDataArray[5] = 0f;
        sample4.inputDataArray[6] = 0f;
        sample4.inputDataArray[7] = -0.77f;
        sample4.inputDataArray[8] = -0.77f;
        sample4.inputDataArray[9] = 0f;
        sample4.inputDataArray[10] = 0f;
        sample4.inputDataArray[11] = 0f;
        sample4.inputDataArray[12] = 0f;

        sample4.outputDataArray[0] = -1f;
        sample4.outputDataArray[1] = -1f;

        dataSamplesList.Add(sample4);  // Add to master List
    }
    

    private void RecordPlayerData() {
        /*
        0 bias
        1 ownVelX
        2 ownVelY
        3 targetPosX
        4 targetPosY
        5 targetVelX
        6 targetVelY
        7 targetDirX
        8 targetDirY
        9 distLeft
        10 distRight
        11 distUp
        12 distDown

        0 throttleX
        1 throttleY
        */
        DataSample sample = new DataSample();
        sample.inputDataArray[0] = 1f;
        sample.inputDataArray[1] = playerGO.GetComponent<Rigidbody2D>().velocity.x / 15f;
        sample.inputDataArray[2] = playerGO.GetComponent<Rigidbody2D>().velocity.y / 15f;
        sample.inputDataArray[3] = (targetGO.transform.position.x - playerGO.transform.position.x) / 21f;
        sample.inputDataArray[4] = (targetGO.transform.position.y - playerGO.transform.position.y) / 21f;
        sample.inputDataArray[5] = 0f;
        sample.inputDataArray[6] = 0f;
        Vector2 targetDir = (new Vector2(targetGO.transform.position.x, targetGO.transform.position.y) - new Vector2(playerGO.transform.position.x, playerGO.transform.position.y)).normalized;
        sample.inputDataArray[7] = targetDir.x;
        sample.inputDataArray[8] = targetDir.y;
        sample.inputDataArray[9] = Mathf.Abs(-21f - playerGO.transform.position.x) / 21f;
        sample.inputDataArray[10] = Mathf.Abs(21f - playerGO.transform.position.x) / 21f;
        sample.inputDataArray[11] = Mathf.Abs(21f - playerGO.transform.position.y) / 21f;
        sample.inputDataArray[12] = Mathf.Abs(-21f - playerGO.transform.position.y) / 21f;

        // @$!@$#!#% REVISIT THIS!! REDUNDANT CODE!!!!!!!!!!!!!!!!!!!!!!!!!!  movement script on Agent also does this....
        float outputHorizontal = 0f;
        if (Input.GetKey("left") || Input.GetKey("a")) {
            outputHorizontal += -1f;
        }
        if (Input.GetKey("right") || Input.GetKey("d")) {
            outputHorizontal += 1f;
        }
        float outputVertical = 0f;
        if (Input.GetKey("down") || Input.GetKey("s")) {
            outputVertical += -1f;
        }
        if (Input.GetKey("up") || Input.GetKey("w")) {
            outputVertical += 1f;
        }
        sample.outputDataArray[0] = outputHorizontal;
        sample.outputDataArray[1] = outputVertical;

        dataSamplesList.Add(sample);  // Add to master List
    }

    private void FixedUpdate() {
        if(gameMode == GameMode.DataCollection) {
            if(curDataCollectionRound < numDataCollectionRounds) {
                if (curTimeStep < maxTrialTimeSteps) {
                    //playerScript.Movement();
                    playerAgent.RunModules();

                    float curHorizontalInput = 0f;
                    float curVerticalInput = 0f;
                    if (Input.GetKey("left") || Input.GetKey("a")) {
                        curHorizontalInput += -1f;
                    }
                    if (Input.GetKey("right") || Input.GetKey("d")) {
                        curHorizontalInput += 1f;
                    }
                    float outputVertical = 0f;
                    if (Input.GetKey("down") || Input.GetKey("s")) {
                        curVerticalInput += -1f;
                    }
                    if (Input.GetKey("up") || Input.GetKey("w")) {
                        curVerticalInput += 1f;
                    }

                    // Record Data
                    //if (curTimeStep % dataCollectionFrequency == 0 && curTimeStep > 30) {
                    //    RecordPlayerData();
                    //}

                    if(curHorizontalInput != lastHorizontalInput || curVerticalInput != lastVerticalInput) {
                        RecordPlayerData();
                    }

                    lastHorizontalInput = curHorizontalInput;
                    lastVerticalInput = curVerticalInput;

                    curTimeStep++;
                }
                else {
                    curDataCollectionRound++;

                    if (curDataCollectionRound < numDataCollectionRounds) {
                        StartNewDataCollectionRound();
                    }
                    else {
                        Debug.Log("DATA COLLECTION COMPLETE!!!");
                        /*string debugTxt = "";
                        for(int x = 0; x < 1; x++) {
                            for(int i = 0; i < dataSamplesList[32].inputDataArray.Length; i++) {
                                debugTxt += i.ToString() + ": " + dataSamplesList[32].inputDataArray[i].ToString() + "\n";
                            }
                            for (int o = 0; o < dataSamplesList[32].outputDataArray.Length; o++) {
                                debugTxt += o.ToString() + ": " + dataSamplesList[32].outputDataArray[o].ToString() + "\n";
                            }
                        }
                        Debug.Log(debugTxt);*/
                    }
                }
            }
            else {

            }            
        } 
        
        if(gameMode == GameMode.Training) {
            if(training) {
                if(dataSamplesList != null) {
                    if (curTrainingGen < 2048) {
                        for(int i = 0; i < 256; i++) {
                            TickTrainingMode();
                        }                        
                    }
                    else {
                        Debug.Log("TrainingComplete!");
                        training = false;
                    }
                }
                
                //Vector3 agentPos = new Vector3(displayAgent.testModule.ownPosX[0], displayAgent.testModule.ownPosY[0], 0f);
                //displayAgent.gameObject.transform.localPosition = agentPos;

                //  INPUT AND MOVEMENT MODEL HERE !!!!  (OR INSIDE AGENT...)


                //displayAgent.RunModules()
                //if (currentTimeStep % 1 == 0) {
                //    currentAgentsArray[i].TickBrain();
                //}
                //currentAgentsArray[i].RunModules(currentTimeStep, currentEnvironment);
            }
            // Run Agent:
            displayAgent.TickBrain();
            //displayAgent.testModule.Tick();
            displayAgent.RunModules();
        }
    }

    public void EnterDataCollectionMode() {
        Debug.Log("EnterDataCollectionMode()");

        gameMode = GameMode.DataCollection;
        curDataCollectionRound = 0;
        dataSamplesList = new List<DataSample>();

        StartNewDataCollectionRound();
    }
    public void EnterTrainingMode() {
        Debug.Log("EnterTrainingMode()");

        gameMode = GameMode.Training;
        //CreateFakeData();
        StartNewTrainingMode();
    }

    public void StartNewDataCollectionRound() {
        
        curTimeStep = 0;
        imitationUI.TogglePause();

        // Spawn Player & Target Randomly
        float randomAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
        float randomRadius = UnityEngine.Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 playerPos = new Vector3(Mathf.Cos(randomAngle) * randomRadius, Mathf.Sin(randomAngle) * randomRadius, 0f);
        Vector3 targetPos = new Vector3(Mathf.Cos(randomAngle + Mathf.PI) * randomRadius, Mathf.Sin(randomAngle + Mathf.PI) * randomRadius, 0f);

        BodyGenome templateBodyGenome = new BodyGenome();
        templateBodyGenome.InitializeGenomeAsDefault();
        AgentGenome newGenome = new AgentGenome(-1);
        newGenome.InitializeBodyGenomeFromTemplate(templateBodyGenome);
        newGenome.InitializeRandomBrainFromCurrentBody(0.0f);

        if (playerGO == null) {                        
            //ResetTrainingForNewGen();
            // Create Visible Display Agent to observe behavior
            //displayStartPos = new StartPositionGenome(Vector3.zero, Quaternion.identity);
            //playerGO.transform.localPosition = displayStartPos.agentStartPosition;
            //playerGO.transform.localRotation = displayStartPos.agentStartRotation;
        }
        else {
            //playerGO.transform.position = playerPos;
            Destroy(playerGO); // NOT IDEAL
        }
        // Instantiate Player (Predator) Human-Controlled:
        playerGO = Instantiate(Resources.Load("PredatorPrey/PredatorPrefab")) as GameObject;
        playerAgent = playerGO.AddComponent<Agent>();
        playerAgent.humanControlled = true;
        StartPositionGenome playerStartPos = new StartPositionGenome(playerPos, Quaternion.identity);
        playerGO.transform.position = playerPos;        

        if (targetGO == null) {
            //targetGO = Instantiate(Resources.Load("PredatorPrey/PreyPrefab")) as GameObject;
            //targetGO.transform.position = targetPos;
        }
        else {
            //targetGO.transform.position = targetPos;
            Destroy(targetGO); // NOT IDEAL
        }
        targetGO = Instantiate(Resources.Load("PredatorPrey/PreyPrefab")) as GameObject;
        targetAgent = targetGO.AddComponent<Agent>();
        targetAgent.humanControlled = false;
        StartPositionGenome targetStartPos = new StartPositionGenome(targetPos, Quaternion.identity);
        targetGO.transform.position = targetPos;



        // Hook-Ups PLAYER:
        playerAgent.isVisible = true;
        playerAgent.InitializeAgentFromTemplate(newGenome, playerStartPos);  // creates TestModule
        // Hook-Ups TARGET:
        targetAgent.isVisible = true;
        targetAgent.InitializeAgentFromTemplate(newGenome, targetStartPos);  // creates TestModule

        // hook modules PLAYER:
        playerAgent.testModule.ownRigidBody2D = playerGO.GetComponent<Rigidbody2D>();
        playerAgent.testModule.bias[0] = 1f;
        playerAgent.testModule.enemyTestModule = targetAgent.testModule; // self as target, to zero out targetvel
        // hook modules TARGET:
        targetAgent.testModule.ownRigidBody2D = targetGO.GetComponent<Rigidbody2D>();
        targetAgent.testModule.bias[0] = 1f;
        targetAgent.testModule.enemyTestModule = playerAgent.testModule; // self as target, to zero out targetvel
    }

    public void StartNewTrainingMode() {
        Destroy(playerGO);
        
        // Create population brains
        agentGenomeList = new List<AgentGenome>();

        BodyGenome templateBodyGenome = new BodyGenome();
        templateBodyGenome.InitializeGenomeAsDefault();
        //BodyGenome bodyGenomeTemplate = new BodyGenome();
        //bodyGenomeTemplate.CopyBodyGenomeFromTemplate(bodyTemplate);

        for (int i = 0; i < trainingPopulationSize; i++) {
            AgentGenome newGenome = new AgentGenome(i);
            newGenome.InitializeBodyGenomeFromTemplate(templateBodyGenome);
            newGenome.InitializeRandomBrainFromCurrentBody(0.033f);
            agentGenomeList.Add(newGenome);
        }

        // CrossoverSettings:
        trainingSettingsManager = new TrainingSettingsManager(0.06f, 0.35f, 0.1f, 0.01f);
        GameObject dummyAgentGO = new GameObject("DummyAgent");
        dummyAgent = dummyAgentGO.AddComponent<Agent>();
        dummyStartGenome = new StartPositionGenome(Vector3.zero, Quaternion.identity);

        ResetTrainingForNewGen();
        //Brain brain = new Brain()        
        //Debug.Log(dummyAgent.testModule.);

        // Create Visible Display Agent to observe behavior
        /*displayStartPos = new StartPositionGenome(Vector3.zero, Quaternion.identity);
        GameObject agentGO = Instantiate(Resources.Load("PredatorPrey/PredatorPrefab")) as GameObject; ; // GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //displayAgent
        agentGO.name = "Guinea Pig";
        //agentGO.transform.parent = gameObject.transform;
        agentGO.transform.localPosition = displayStartPos.agentStartPosition;
        agentGO.transform.localRotation = displayStartPos.agentStartRotation;
        //agentGO.GetComponent<Collider>().enabled = false;
        displayAgent = agentGO.AddComponent<Agent>();
        displayAgent.isVisible = true;
        displayAgent.InitializeAgentFromTemplate(agentGenomeList[0], displayStartPos);
        // hook modules:
        displayAgent.testModule.enemyTransform = targetGO.transform;
        displayAgent.testModule.bias[0] = 1f;
        displayAgent.testModule.enemyTestModule = displayAgent.testModule; // self as target, to zero out targetvel
        */
        ResetTrainingAgentAndEnvironment();
    }

    private void CopyDataSampleToModule(DataSample sample, TestModule module) {
        module.bias[0] = sample.inputDataArray[0];
        module.ownVelX[0] = sample.inputDataArray[1];
        module.ownVelY[0] = sample.inputDataArray[2];
        module.enemyPosX[0] = sample.inputDataArray[3];
        module.enemyPosY[0] = sample.inputDataArray[4];
        module.enemyVelX[0] = sample.inputDataArray[5];
        module.enemyVelY[0] = sample.inputDataArray[6];
        module.enemyDirX[0] = sample.inputDataArray[7];
        module.enemyDirY[0] = sample.inputDataArray[8];
        module.distLeft[0] = sample.inputDataArray[9];
        module.distRight[0] = sample.inputDataArray[10];
        module.distUp[0] = sample.inputDataArray[11];
        module.distDown[0] = sample.inputDataArray[12];
    }

    private float CompareDataSampleToBrainOutput(DataSample sample, TestModule module) {
        float throttleX = Mathf.Round(module.throttleX[0] * 3f / 2f);// + module.throttleX[0];
        float throttleY = Mathf.Round(module.throttleY[0] * 3f / 2f);// + module.throttleY[0];
        float deltaX = sample.outputDataArray[0] - throttleX + (sample.outputDataArray[0] - module.throttleX[0]) * 0.5f;
        float deltaY = sample.outputDataArray[1] - throttleY + (sample.outputDataArray[1] - module.throttleY[0]) * 0.5f;
        float distSquared = deltaX * deltaX + deltaY * deltaY;
        return distSquared;
    }

    public void TickTrainingMode() {
        // Can do one genome & 1 dataSample per frame
        if (curTestingAgent < agentGenomeList.Count) {
            if(curTestingSample < dataSamplesList.Count) {
                CopyDataSampleToModule(dataSamplesList[curTestingSample], dummyAgent.testModule);  // load training data into module (and by extension, brain)
                // Run Brain ( few ticks? )
                for(int t = 0; t < 2; t++) {
                    dummyAgent.TickBrain();
                }
                //Debug.Log(CompareDataSampleToBrainOutput(dataSamplesList[curTestingSample], dummyAgent.testModule).ToString());
                rawFitnessScoresArray[curTestingAgent] += CompareDataSampleToBrainOutput(dataSamplesList[curTestingSample], dummyAgent.testModule);
                curTestingSample++;
            }
            else {
                curTestingSample = 0;
                
                curTestingAgent++;
                if(curTestingAgent < agentGenomeList.Count) { // this can clearly be done better
                    dummyAgent.InitializeAgentFromTemplate(agentGenomeList[curTestingAgent], dummyStartGenome);
                }                
            }
        }
        else {
            // New Generation!!!
            //
            NextGeneration();
            
        }

        // Loop through all Genomes in pop
        // Build Brain from genome
        // Test Brain vs all dataSamples
        // keep track of fitness score
        // Sort by fitness
        // Crossover
        // Set new population


    }

    private void NextGeneration() {
        string fitTxt = "Fitness Scores:\n";
        float totalFitness = 0f;
        float minScore = float.PositiveInfinity;
        float maxScore = float.NegativeInfinity;
        for (int f = 0; f < trainingPopulationSize; f++) {
            fitTxt += f.ToString() + ": " + rawFitnessScoresArray[f].ToString() + "\n";
            totalFitness += rawFitnessScoresArray[f];
            minScore = Mathf.Min(minScore, rawFitnessScoresArray[f]);
            maxScore = Mathf.Max(maxScore, rawFitnessScoresArray[f]);
        }
        Debug.Log(fitTxt);
        avgFitnessLastGen = totalFitness / (float)trainingPopulationSize;

        // Process Fitness (bigger is better so lottery works)
        float scoreRange = maxScore - minScore;
        if(scoreRange == 0f) {
            scoreRange = 1f;  // avoid divide by zero
        }
        for (int f = 0; f < trainingPopulationSize; f++) {            
            rawFitnessScoresArray[f] = 1f - ((rawFitnessScoresArray[f] - minScore) / scoreRange); // normalize and invert scores for fitness lottery
        }

        // Sort Fitness Scores
        int[] rankedIndicesList = new int[rawFitnessScoresArray.Length];
        float[] rankedFitnessList = new float[rawFitnessScoresArray.Length];

        // populate arrays:
        for (int i = 0; i < rawFitnessScoresArray.Length; i++) {
            rankedIndicesList[i] = i;
            rankedFitnessList[i] = rawFitnessScoresArray[i];
        }
        for (int i = 0; i < rawFitnessScoresArray.Length - 1; i++) {
            for (int j = 0; j < rawFitnessScoresArray.Length - 1; j++) {
                float swapFitA = rankedFitnessList[j];
                float swapFitB = rankedFitnessList[j + 1];
                int swapIdA = rankedIndicesList[j];
                int swapIdB = rankedIndicesList[j + 1];

                if (swapFitA < swapFitB) {  // bigger is better now after inversion
                    rankedFitnessList[j] = swapFitB;
                    rankedFitnessList[j + 1] = swapFitA;
                    rankedIndicesList[j] = swapIdB;
                    rankedIndicesList[j + 1] = swapIdA;
                }
            }
        }
        string fitnessRankText = "";
        for (int i = 0; i < rawFitnessScoresArray.Length; i++) {
            fitnessRankText += "[" + rankedIndicesList[i].ToString() + "]: " + rankedFitnessList[i].ToString() + "\n";
        }

        // CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER CROSSOVER 
        List<BrainGenome> newGenBrainGenomeList = new List<BrainGenome>(); // new population!        

        //FitnessManager fitnessManager = teamsConfig.playersList[playerIndex].fitnessManager;
        //TrainingSettingsManager trainingSettingsManager = teamsConfig.playersList[playerIndex].trainingSettingsManager;

        // Keep top-half peformers + mutations:
        for (int x = 0; x < agentGenomeList.Count; x++) {
            if (x == 0) {
                BrainGenome parentGenome = agentGenomeList[rankedIndicesList[x]].brainGenome;  // keep top performer as-is
                newGenBrainGenomeList.Add(parentGenome);
            }
            else {
                BrainGenome newBrainGenome = new BrainGenome();
                int parentIndex = GetAgentIndexByLottery(rankedFitnessList, rankedIndicesList);
                BrainGenome parentGenome = agentGenomeList[parentIndex].brainGenome;
                newBrainGenome.SetToMutatedCopyOfParentGenome(parentGenome, trainingSettingsManager);
                newGenBrainGenomeList.Add(newBrainGenome);
            }
        }

        for (int i = 0; i < agentGenomeList.Count; i++) {
            agentGenomeList[i].brainGenome = newGenBrainGenomeList[i];
        }

        // Reset
        ResetTrainingForNewGen();

        trainingSettingsManager.mutationChance *= 0.996f;
        trainingSettingsManager.mutationStepSize *= 0.998f;

        curTrainingGen++;
    }

    public int GetAgentIndexByLottery(float[] rankedFitnessList, int[] rankedIndicesList) {
        int selectedIndex = 0;
        // calculate total fitness of all agents
        float totalFitness = 0f;
        for (int i = 0; i < rankedFitnessList.Length; i++) {
            totalFitness += rankedFitnessList[i];
        }
        // generate random lottery value between 0f and totalFitness:
        float lotteryValue = UnityEngine.Random.Range(0f, totalFitness);
        float currentValue = 0f;
        for (int i = 0; i < rankedFitnessList.Length; i++) {
            if (lotteryValue >= currentValue && lotteryValue < (currentValue + rankedFitnessList[i])) {
                // Jackpot!
                selectedIndex = rankedIndicesList[i];
                //Debug.Log("Selected: " + selectedIndex.ToString() + "! (" + i.ToString() + ") fit= " + currentValue.ToString() + "--" + (currentValue + (1f - rankedFitnessList[i])).ToString() + " / " + totalFitness.ToString() + ", lotto# " + lotteryValue.ToString() + ", fit= " + (1f - rankedFitnessList[i]).ToString());
            }
            currentValue += rankedFitnessList[i]; // add this agent's fitness to current value for next check            
        }

        return selectedIndex;
    }

    private void ResetTrainingForNewGen() {
        rawFitnessScoresArray = new float[trainingPopulationSize];

        curTestingAgent = 0;
        curTestingSample = 0;
        
        dummyAgent.InitializeAgentFromTemplate(agentGenomeList[0], dummyStartGenome);
    }

    public void ResetTrainingAgentAndEnvironment() {

        // Spawn Player & Target Randomly
        float randomAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
        float randomRadius = UnityEngine.Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 displayPos = new Vector3(Mathf.Cos(randomAngle) * randomRadius, Mathf.Sin(randomAngle) * randomRadius, 0f);
        Vector3 targetPos = new Vector3(Mathf.Cos(randomAngle + Mathf.PI) * randomRadius, Mathf.Sin(randomAngle + Mathf.PI) * randomRadius, 0f);

        BodyGenome templateBodyGenome = new BodyGenome();
        templateBodyGenome.InitializeGenomeAsDefault();
        AgentGenome newGenome = new AgentGenome(-1);
        newGenome.InitializeBodyGenomeFromTemplate(templateBodyGenome);
        newGenome.InitializeRandomBrainFromCurrentBody(0.0f);

        if (displayGO != null) {
            Destroy(displayGO); // NOT IDEAL
        }
        displayGO = Instantiate(Resources.Load("PredatorPrey/PredatorPrefab")) as GameObject;
        displayAgent = displayGO.AddComponent<Agent>();
        displayAgent.humanControlled = false;
        StartPositionGenome displayStartPos = new StartPositionGenome(displayPos, Quaternion.identity);
        displayGO.transform.position = displayPos;

        if (targetGO != null) {
            Destroy(targetGO); // NOT IDEAL
        }
        targetGO = Instantiate(Resources.Load("PredatorPrey/PreyPrefab")) as GameObject;
        targetAgent = targetGO.AddComponent<Agent>();
        targetAgent.humanControlled = false;
        StartPositionGenome targetStartPos = new StartPositionGenome(targetPos, Quaternion.identity);
        targetGO.transform.position = targetPos;

        // Hook-Ups PLAYER:
        displayAgent.isVisible = true;
        displayAgent.InitializeAgentFromTemplate(agentGenomeList[0], displayStartPos);  // creates TestModule
        // Hook-Ups TARGET:
        targetAgent.isVisible = true;
        targetAgent.InitializeAgentFromTemplate(newGenome, targetStartPos);  // creates TestModule

        // hook modules PLAYER:
        displayAgent.testModule.ownRigidBody2D = displayGO.GetComponent<Rigidbody2D>();
        displayAgent.testModule.bias[0] = 1f;
        displayAgent.testModule.enemyTestModule = targetAgent.testModule; // self as target, to zero out targetvel
        // hook modules TARGET:
        targetAgent.testModule.ownRigidBody2D = targetGO.GetComponent<Rigidbody2D>();
        targetAgent.testModule.bias[0] = 1f;
        targetAgent.testModule.enemyTestModule = displayAgent.testModule; // self as target, to zero out targetvel

        /*
        // Spawn Player & Target Randomly
        float randomAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
        float randomRadius = UnityEngine.Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 playerPos = new Vector3(Mathf.Cos(randomAngle) * randomRadius, Mathf.Sin(randomAngle) * randomRadius, 0f);
        Vector3 targetPos = new Vector3(Mathf.Cos(randomAngle + Mathf.PI) * randomRadius, Mathf.Sin(randomAngle + Mathf.PI) * randomRadius, 0f);        
        targetGO.transform.position = targetPos;        

        displayStartPos = new StartPositionGenome(playerPos, Quaternion.identity);
        GameObject agentGO;
        if (displayAgent == null) {
            agentGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            displayAgent = agentGO.AddComponent<Agent>();
        }
        else {
            agentGO = displayAgent.gameObject;
        }
        //displayAgent
        agentGO.name = "Guinea Pig";
        agentGO.transform.parent = gameObject.transform;
        agentGO.transform.localPosition = displayStartPos.agentStartPosition;
        agentGO.transform.localRotation = displayStartPos.agentStartRotation;
        //agentGO.GetComponent<Collider>().enabled = false;        
        displayAgent.isVisible = true;
        displayAgent.InitializeAgentFromTemplate(agentGenomeList[0], displayStartPos);
        // hook modules:
        displayAgent.testModule.enemyTransform = targetGO.transform;
        displayAgent.testModule.bias[0] = 1f;
        displayAgent.testModule.enemyTestModule = displayAgent.testModule;
        */
    }
}
