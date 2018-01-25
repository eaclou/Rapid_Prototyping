using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentPopulation {

    public EnvironmentGenome templateGenome;
    public List<EnvironmentGenome> environmentGenomeList;

    [System.NonSerialized]
    public List<EnvironmentGenome> representativeGenomeList;  // the list of agentGenomes that will be opponents for all other populations this round
    public List<EnvironmentGenome> historicGenomePool;  // a collection of predecessor genomes that can be chosen from
    //public List<EnvironmentGenome> baselineGenomePool;  // a collection of blank and random genomes for fitness comparison purposes.
    public int maxHistoricGenomePoolSize = 100;

    public int popSize;
    //public int numBaseline;
    public FitnessManager fitnessManager; // keeps track of performance data from this population's agents
    public TrainingSettingsManager trainingSettingsManager;  // keeps track of core algorithm settings, like mutation rate, thoroughness, etc.
    public bool isTraining = true;
    public int numPerformanceReps = 1;
    public int numHistoricalReps = 0;
    //public int numBaselineReps = 0;

    public EnvironmentPopulation(EnvironmentGenome templateGenome, int numGenomes, int numReps) {

        this.templateGenome = templateGenome;
        popSize = numGenomes;
        //this.numBaseline = numBaseline;

        environmentGenomeList = new List<EnvironmentGenome>();
        historicGenomePool = new List<EnvironmentGenome>();
        //baselineGenomePool = new List<EnvironmentGenome>();

        for (int e = 0; e < numGenomes; e++) {
            // Create new environmentGenome
            EnvironmentGenome envGenome = new EnvironmentGenome(e);
            envGenome.InitializeRandomGenomeFromTemplate(templateGenome);
            // Add to envGenomesList:
            environmentGenomeList.Add(envGenome);

            // Create parallel initial batch of genomes to be used as baseline comparison
            //EnvironmentGenome baseGenome = new EnvironmentGenome(e);
            //baseGenome.InitializeRandomGenomeFromTemplate(templateGenome);
            //baselineGenomePool.Add(baseGenome);
        }
        //AppendBaselineGenomes();

        // Representatives:
        numPerformanceReps = numReps;
        ResetRepresentativesList();
        historicGenomePool.Add(environmentGenomeList[0]); // init               

        fitnessManager = new FitnessManager();
        SetUpDefaultFitnessComponents(fitnessManager);
        //fitnessManager.ResetHistoricalData();
        fitnessManager.InitializeForNewGeneration(environmentGenomeList.Count);

        trainingSettingsManager = new TrainingSettingsManager(0.25f, 0.05f, 0f, 0f);
    }

    
    private void SetUpDefaultFitnessComponents(FitnessManager fitnessManager) {
        
        FitnessComponentDefinition newComponentCombat = new FitnessComponentDefinition(FitnessComponentType.Random, FitnessComponentMeasure.Avg, 1f, true);
        fitnessManager.fitnessComponentDefinitions.Add(newComponentCombat);
            
        //fitnessManager.SetPendingFitnessListFromMaster(); // make pending list a copy of the primary
    }

    public void ResetRepresentativesList() {

        if (representativeGenomeList == null) {
            representativeGenomeList = new List<EnvironmentGenome>();
        }
        else {
            representativeGenomeList.Clear();
        }

        for (int i = 0; i < numPerformanceReps; i++) {
            representativeGenomeList.Add(environmentGenomeList[i]);
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
