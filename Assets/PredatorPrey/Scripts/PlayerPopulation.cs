using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerPopulation {
    public int index;

    public List<AgentGenome> agentGenomeList;  // Primary genome population

    public List<AgentGenome> representativeGenomeList;  // the list of agentGenomes that will be opponents for all other populations this round
    public List<AgentGenome> historicGenomePool;  // a collection of predecessor genomes that can be chosen from
    //public List<AgentGenome> baselineGenomePool;  // a collection of blank and random genomes for fitness comparison purposes.
    public int maxHistoricGenomePoolSize = 100;

    public int popSize;
    //public int numBaseline;
    public FitnessManager fitnessManager; // keeps track of performance data from this population's agents
    public TrainingSettingsManager trainingSettingsManager;  // keeps track of core algorithm settings, like mutation rate, thoroughness, etc.

    public bool isTraining = true;
    public int numPerformanceReps = 1;
    public int numHistoricalReps = 0;
    //public int numBaselineReps = 0;

    public BodyGenome bodyGenomeTemplate;

    // Representative system will be expanded later - for now, just defaults to Top # of performers
    public PlayerPopulation(int index, BodyGenome bodyTemplate, int numGenomes, int numReps) {
        this.index = index;
        
        // Re-Factor:
        bodyGenomeTemplate = new BodyGenome();
        bodyGenomeTemplate.CopyBodyGenomeFromTemplate(bodyTemplate);
        //graphKing = new TheGraphKing();

        popSize = numGenomes;
        //this.numBaseline = numBaseline;

        // Create blank AgentGenomes for the standard population
        agentGenomeList = new List<AgentGenome>();
        historicGenomePool = new List<AgentGenome>();
        //baselineGenomePool = new List<AgentGenome>();

        for (int j = 0; j < numGenomes; j++) {
            AgentGenome agentGenome = new AgentGenome(j);
            agentGenome.InitializeBodyGenomeFromTemplate(bodyGenomeTemplate);
            agentGenome.InitializeRandomBrainFromCurrentBody(1f);
            agentGenomeList.Add(agentGenome);
        }
        //RepopulateBaselineGenomes();
        //AppendBaselineGenomes();

        // Representatives:
        numPerformanceReps = numReps;
        ResetRepresentativesList();
        historicGenomePool.Add(agentGenomeList[0]); // init

        
        fitnessManager = new FitnessManager();
        SetUpDefaultFitnessComponents(fitnessManager, this.index);
        //fitnessManager.ResetHistoricalData();
        //fitnessManager.ResetCurrentHistoricalDataLists();
        fitnessManager.InitializeForNewGeneration(agentGenomeList.Count);
        
        trainingSettingsManager = new TrainingSettingsManager(0.15f, 0.35f, 0.0f, 0.0f);
    }

    /*public void RepopulateBaselineGenomes() {
        if (baselineGenomePool == null) {
            baselineGenomePool = new List<AgentGenome>();
        }
        else {
            baselineGenomePool.Clear();
        }


        for (int j = 0; j < numBaseline; j++) {

            AgentGenome baselineGenome = new AgentGenome(j);
            baselineGenome.InitializeBodyGenomeFromTemplate(bodyGenomeTemplate);
            float increment = (float)j / (float)(numBaseline - 1);
            float weightScale = 0f; // how much to scale initial random weights
            baselineGenome.InitializeRandomBrainFromCurrentBody(increment * (float)j * weightScale);
            baselineGenomePool.Add(baselineGenome);
        }
    }*/

    private void SetUpDefaultFitnessComponents(FitnessManager fitnessManager, int index) {
        
        FitnessComponentDefinition fitCompCombat1 = new FitnessComponentDefinition(FitnessComponentType.Random, FitnessComponentMeasure.Avg, 0.0f, true);
        fitnessManager.fitnessComponentDefinitions.Add(fitCompCombat1);
        if(index == 0) {
            FitnessComponentDefinition fitCompCombat2 = new FitnessComponentDefinition(FitnessComponentType.DistanceToEnemy, FitnessComponentMeasure.Avg, 1f, false);
            fitnessManager.fitnessComponentDefinitions.Add(fitCompCombat2);
        }
        else {
            FitnessComponentDefinition fitCompCombat3 = new FitnessComponentDefinition(FitnessComponentType.DistanceToEnemy, FitnessComponentMeasure.Avg, 1f, true);
            fitnessManager.fitnessComponentDefinitions.Add(fitCompCombat3);
        }
        
        //fitnessManager.SetPendingFitnessListFromMaster(); // make pending list a copy of the primary
    }

    public void ResetRepresentativesList() {
        if (representativeGenomeList == null) {
            representativeGenomeList = new List<AgentGenome>();
        }
        representativeGenomeList.Clear();

        for (int i = 0; i < numPerformanceReps; i++) {
            representativeGenomeList.Add(agentGenomeList[i]);
        }
        for (int i = 0; i < numHistoricalReps; i++) {
            int randIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0f, (float)historicGenomePool.Count - 1f));
            representativeGenomeList.Add(historicGenomePool[randIndex]);
        }
        /*for (int i = 0; i < numBaselineReps; i++) {
            int randIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0f, (float)baselineGenomePool.Count - 1f));
            representativeGenomeList.Add(baselineGenomePool[randIndex]);
        }*/
    }
}
