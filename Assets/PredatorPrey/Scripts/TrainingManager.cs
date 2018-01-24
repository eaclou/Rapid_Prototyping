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
        int numPlayers = 1;
        
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

    private void NextGeneration() {
        
        // Crossover:
        //teamsConfig.environmentPopulation.fitnessManager.ProcessAndRankRawFitness(teamsConfig.environmentPopulation.popSize);
        
        Crossover();

        // Cleanup for next Gen:
        // Reset fitness data:
        
        //teamsConfig.environmentPopulation.fitnessManager.InitializeForNewGeneration(teamsConfig.environmentPopulation.environmentGenomeList.Count);
        //teamsConfig.environmentPopulation.historicGenomePool.Add(teamsConfig.environmentPopulation.environmentGenomeList[0]);
        //teamsConfig.environmentPopulation.ResetRepresentativesList();

        for (int i = 0; i < teamsConfig.playersList.Count; i++) {            
            //teamsConfig.playersList[i].fitnessManager.InitializeForNewGeneration(teamsConfig.playersList[i].agentGenomeList.Count);
            //teamsConfig.playersList[i].historicGenomePool.Add(teamsConfig.playersList[i].agentGenomeList[0]);
            //teamsConfig.playersList[i].ResetRepresentativesList();
        }        

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
        /*
        List<BrainGenome> newGenBrainGenomeList = new List<BrainGenome>(); // new population!        

        FitnessManager fitnessManager = teamsConfig.playersList[playerIndex].fitnessManager;
        TrainingSettingsManager trainingSettingsManager = teamsConfig.playersList[playerIndex].trainingSettingsManager;
        //float mutationChance = trainingSettingsManager.mutationChance;
        //float mutationStepSize = trainingSettingsManager.mutationStepSize;

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
        */
    }
}
