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

    public PlayerController playerScript;

    public List<DataSample> dataSamplesList;
    private int dataCollectionFrequency = 4;

    public int trainingPopulationSize = 32;
    private List<AgentGenome> agentGenomeList;
    //private Brain agentBrainList;
    private TrainingSettingsManager trainingSettingsManager;

    public int curTestingAgent = 0;
    public int curTestingSample = 0;

    private Agent dummyAgent;

    public float[] rawFitnessScoresArray;

    private StartPositionGenome dummyStartGenome;

    private bool training = true;

    // Use this for initialization
    void Start () {
        gameMode = GameMode.Off;
        Time.timeScale = 0f;

	}
	
	// Update is called once per frame
	void Update () {
		
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
        sample.inputDataArray[1] = playerGO.GetComponent<Rigidbody2D>().velocity.x;
        sample.inputDataArray[2] = playerGO.GetComponent<Rigidbody2D>().velocity.y;
        sample.inputDataArray[3] = targetGO.transform.position.x - playerGO.transform.position.x;
        sample.inputDataArray[4] = targetGO.transform.position.y - playerGO.transform.position.y;
        sample.inputDataArray[5] = 0f;
        sample.inputDataArray[6] = 0f;
        Vector2 targetDir = (new Vector2(targetGO.transform.position.x, targetGO.transform.position.y) - new Vector2(playerGO.transform.position.x, playerGO.transform.position.y)).normalized;
        sample.inputDataArray[7] = targetDir.x;
        sample.inputDataArray[8] = targetDir.y;
        sample.inputDataArray[9] = Mathf.Abs(-21f - targetGO.transform.position.x);
        sample.inputDataArray[10] = Mathf.Abs(21f - targetGO.transform.position.x);
        sample.inputDataArray[11] = Mathf.Abs(21f - targetGO.transform.position.y);
        sample.inputDataArray[12] = Mathf.Abs(-21f - targetGO.transform.position.y);

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
                    playerScript.Movement();

                    // Record Data
                    if(curTimeStep % dataCollectionFrequency == 0) {
                        RecordPlayerData();
                    }

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
                    if (curTrainingGen < 64) {
                        for(int i = 0; i < 2048; i++) {
                            TickTrainingMode();
                        }                        
                    }
                    else {
                        Debug.Log("TrainingComplete!");
                        training = false;
                    }
                }
            }
                        
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
        
        StartNewTrainingMode();
    }

    public void StartNewDataCollectionRound() {
        
        curTimeStep = 0;

        // Spawn Player & Target Randomly
        float randomAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
        float randomRadius = UnityEngine.Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 playerPos = new Vector3(Mathf.Cos(randomAngle) * randomRadius, Mathf.Sin(randomAngle) * randomRadius, 0f);
        Vector3 targetPos = new Vector3(Mathf.Cos(randomAngle + Mathf.PI) * randomRadius, Mathf.Sin(randomAngle + Mathf.PI) * randomRadius, 0f);

        if(playerGO == null) {
            playerGO = Instantiate(Resources.Load("ImitationLearning/Player")) as GameObject;
            playerGO.transform.position = playerPos;
            playerScript = playerGO.GetComponent<PlayerController>();
        }
        else {
            playerGO.transform.position = playerPos;
        }
        
        if(targetGO == null) {
            targetGO = Instantiate(Resources.Load("ImitationLearning/Target")) as GameObject;
            targetGO.transform.position = targetPos;
        }
        else {
            targetGO.transform.position = targetPos;
        }
    }

    public void StartNewTrainingMode() {
        // Create population brains
        agentGenomeList = new List<AgentGenome>();

        BodyGenome templateBodyGenome = new BodyGenome();
        templateBodyGenome.InitializeGenomeAsDefault();
        //BodyGenome bodyGenomeTemplate = new BodyGenome();
        //bodyGenomeTemplate.CopyBodyGenomeFromTemplate(bodyTemplate);

        for (int i = 0; i < trainingPopulationSize; i++) {
            AgentGenome newGenome = new AgentGenome(i);
            newGenome.InitializeBodyGenomeFromTemplate(templateBodyGenome);
            newGenome.InitializeRandomBrainFromCurrentBody(0.02f);
            agentGenomeList.Add(newGenome);
        }

        // CrossoverSettings:
        trainingSettingsManager = new TrainingSettingsManager(0.05f, 0.1f, 0.0f, 0.0f);
        GameObject dummyAgentGO = new GameObject("DummyAgent");
        dummyAgent = dummyAgentGO.AddComponent<Agent>();
        dummyStartGenome = new StartPositionGenome(Vector3.zero, Quaternion.identity);

        ResetTrainingForNewGen();        
        //Brain brain = new Brain()        
        //Debug.Log(dummyAgent.testModule.);
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
        float throttleX = module.throttleX[0];
        float throttleY = module.throttleY[0];
        float deltaX = sample.outputDataArray[0] - throttleX;
        float deltaY = sample.outputDataArray[1] - throttleY;
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
}
