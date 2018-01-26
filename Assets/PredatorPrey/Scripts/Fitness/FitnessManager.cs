using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FitnessManager {

    // The list of types of FitnessComponents that are currently active and their weights:
    public List<FitnessComponentDefinition> fitnessComponentDefinitions;

    //[System.NonSerialized]
    //public List<FitnessComponentDefinition> pendingFitnessComponentDefinitions; // edited by UI, applied each new generation

    [System.NonSerialized]
    public float[] rankedFitnessList;
    [System.NonSerialized]
    public int[] rankedIndicesList;
    [System.NonSerialized]
    public float[] processedFitnessScores;

    /*[System.NonSerialized]
    public List<float> baselineScoresAvgList;
    [System.NonSerialized]
    public float maxRatioValue;
    [System.NonSerialized]
    public List<float> alternateRatiosAvgList;
    [System.NonSerialized]
    public List<float> alternateRatiosMaxList;
    [System.NonSerialized]
    public List<float> baselineScoresSmoothedList;*/
    //[System.NonSerialized]
    //public float blankGenomeScore; // processed score for a blank brain versus current representatives
    //int numComponents = fitnessComponentDefinitions.Count;
    //[System.NonSerialized]
    //public float[] historicalComponentRecordMinimums; // keeps track of all-time value range of each fitnessComponent
    //[System.NonSerialized]
    //public float[] historicalComponentRecordMaximums;  // keeps track of all-time value range of each fitnessComponent


    // Recorded History of Progress:
    //[System.NonSerialized]
    //public List<float> curBaselineVersusMinScoreRatiosList;
    //[System.NonSerialized]
    //public List<float> curBaselineVersusAvgScoreRatiosList; // the list of each gen's avg score vs. baseline agents avg score, for all previous gens with same fitness/maxTimeStep settings
    //[System.NonSerialized]
    //public List<float> curBaselineVersusMaxScoreRatiosList; // the list of each gen's BEST individual score vs. baseline agents avg score, for all previous gens with same fitness/maxTimeStep settings
    //[System.NonSerialized]
    //public List<float> alltimeBaselineVersusMinScoreRatiosList;
    //[System.NonSerialized]
    //public List<float> alltimeBaselineVersusAvgScoreRatiosList; // the list of each gen's avg score vs. baseline agents avg score, for ALL generations!
    //[System.NonSerialized]
    //public List<float> alltimeBaselineVersusMaxScoreRatiosList; // the list of each gen's BEST individual score vs. baseline agents avg score, for ALL generations!
    //[System.NonSerialized]
    //public float curMaxRatioValue = 1f; // the highest ratio value achieved by any of the current generation range score Ratios
    //[System.NonSerialized]
    //public float curMinRatioValue = 1f;
    //[System.NonSerialized]
    //public float alltimeMaxRatioValue = 1f; // the highest ratio value achieved by any generation
    //[System.NonSerialized]
    //public float alltimeMinRatioValue = 1f; // the highest ratio value achieved by any generation
    //[System.NonSerialized]
    //public float minRatioFloorAdd = 0.2f; // the highest ratio value achieved by any generation


    //float[] historicalComponentWeightsNormalized = new float[numComponents];
    // scores for random baseline Separated from blank genome score
    // running average for random baseline genomes -- as long as it's reset when fitness/time changes?
    // scores relative to all-time record values -- reset when fitness or training time changes!
    // show mutation rates over time

    //public int[] debugFitnessLottery;
    // TEMP OLD:
    //public List<float> rawFitnessScores;
    // New:
    // Outer list is all the population's genomes
    // Inner list contains group of fitComp's per evaluation instance for that genome
    [System.NonSerialized]
    public List<FitnessComponentEvaluationGroup>[] FitnessEvalGroupArray; // each index is one genome, each list member = trial round?

    public FitnessManager() {
        fitnessComponentDefinitions = new List<FitnessComponentDefinition>();
        //pendingFitnessComponentDefinitions = new List<FitnessComponentDefinition>();
        //baselineScoresAvgList = new List<float>();
        //baselineScoresSmoothedList = new List<float>();
        //alternateRatiosAvgList = new List<float>();
        //alternateRatiosMaxList = new List<float>();

        //InitializeAlltimeHistoricalDataLists();
        //ResetCurrentHistoricalDataLists();
        //ResetHistoricalData();
    }

    /*
    public void InitializeAlltimeHistoricalDataLists() {
        alltimeBaselineVersusMinScoreRatiosList = new List<float>();
        alltimeBaselineVersusAvgScoreRatiosList = new List<float>();
        alltimeBaselineVersusMaxScoreRatiosList = new List<float>();
    }
    public void ResetCurrentHistoricalDataLists() {
        curBaselineVersusMinScoreRatiosList = new List<float>();
        curBaselineVersusAvgScoreRatiosList = new List<float>();
        curBaselineVersusMaxScoreRatiosList = new List<float>();
    }
    public void ResetHistoricalData() {
        //Debug.Log("ResetHISTORICALDATA!");
        historicalComponentRecordMinimums = new float[fitnessComponentDefinitions.Count];  // keeps track of all-time value range of each fitnessComponent
        for (int i = 0; i < historicalComponentRecordMinimums.Length; i++) {
            historicalComponentRecordMinimums[i] = float.PositiveInfinity;
        }
        historicalComponentRecordMaximums = new float[fitnessComponentDefinitions.Count];  // keeps track of all-time value range of each fitnessComponent
        for (int i = 0; i < historicalComponentRecordMaximums.Length; i++) {
            historicalComponentRecordMaximums[i] = float.NegativeInfinity;
        }
    }
    */

    public void InitializeForNewGeneration(int populationSize) {

        //Debug.Log("InitializeForNewGeneration: " + populationSize.ToString());
        if (FitnessEvalGroupArray == null) {
            FitnessEvalGroupArray = new List<FitnessComponentEvaluationGroup>[populationSize];
            for (int i = 0; i < populationSize; i++) { // for each genome:                
                FitnessEvalGroupArray[i] = new List<FitnessComponentEvaluationGroup>(); // The list of fitness Component Groups
            }
        }
        else {  // Not null
            if (FitnessEvalGroupArray.Length != populationSize) {  // but incorrectly sized - population size has changed
                FitnessEvalGroupArray = new List<FitnessComponentEvaluationGroup>[populationSize];
            }
            for (int i = 0; i < populationSize; i++) { // for each genome:
                if (FitnessEvalGroupArray[i] != null) {
                    FitnessEvalGroupArray[i].Clear();
                }
                else {
                    FitnessEvalGroupArray[i] = new List<FitnessComponentEvaluationGroup>(); // The list of fitness Component Groups                    
                }
            }
        }        
    }    

    public void AddNewFitCompEvalGroup(FitnessComponentEvaluationGroup evalGroup, int genomeIndex) {
        FitnessComponentEvaluationGroup groupCopy = new FitnessComponentEvaluationGroup();  // Wrapper class is new memory address...
        for (int i = 0; i < evalGroup.fitCompList.Count; i++) {
            groupCopy.fitCompList.Add(evalGroup.fitCompList[i]);  // ... but they share the actual FitComp objects as ref
        }
        //Debug.Log("AddNewFitCompEvalGroup FitnessEvalGroupArray? " + FitnessEvalGroupArray[genomeIndex].Count.ToString());
        FitnessEvalGroupArray[genomeIndex].Add(groupCopy);
    }

    public void ProcessAndRankRawFitness(int popSize) {
        ProcessRawFitness(popSize);

        /*float totalBaselineScore = 0f;
        for (int i = popSize; i < processedFitnessScores.Length; i++) {
            totalBaselineScore += processedFitnessScores[i];
        }
        float baselineScoresAvg = totalBaselineScore / (processedFitnessScores.Length - popSize);

        float[] trimmedFitnessScoresArray = new float[popSize];
        for (int j = 0; j < popSize; j++) {
            trimmedFitnessScoresArray[j] = processedFitnessScores[j];
        }
        processedFitnessScores = trimmedFitnessScoresArray;
        */

        // Debug:
        /*float totalScore = 0f;
        for (int i = 0; i < popSize; i++) {
            totalScore += processedFitnessScores[i];
        }
        totalScore = totalScore / popSize;

        //Debug.Log("PrimaryScore: " + totalScore.ToString() + ", baselineScore: " + baselineScoresAvg.ToString() + ", Ratio: " + (totalScore / baselineScoresAvg).ToString());
        float scoreRatio = totalScore / baselineScoresAvg;   // compare active genome avg to baseline genome avg
        
        float weight = 0.1f; // how smoothed to make line
        if(baselineScoresSmoothedList.Count > 0) {
            baselineScoresSmoothedList.Add(baselineScoresSmoothedList[baselineScoresSmoothedList.Count - 1] * (1f - weight) + scoreRatio * weight);
        }
        else {
            baselineScoresSmoothedList.Add(scoreRatio); // start the list at first value
        }

        if (scoreRatio > alltimeMaxRatioValue) {
            //maxRatioValue = scoreRatio;
            Debug.Log("new max ratio: " + alltimeMaxRatioValue.ToString());
        }
        alltimeBaselineVersusAvgScoreRatiosList.Add(scoreRatio);
        
        //string txt = "FitnessRatios: \n";
        //int numLines = Mathf.RoundToInt(Mathf.Min(20, baselineScoresAvgList.Count));
        //for(int i = 0; i < numLines; i++) {
        //    txt += baselineScoresAvgList[baselineScoresAvgList.Count - i - 1].ToString() + "\n";
        //}
        //Debug.Log(txt);
        */

        RankProcessedFitness(popSize);
    }

    public void ProcessRawFitness(int popSize) {
        //int popSize = FitnessEvalGroupArray.Length;

        if (processedFitnessScores == null) {
            processedFitnessScores = new float[FitnessEvalGroupArray.Length];
        }
        else {
            if (processedFitnessScores.Length != FitnessEvalGroupArray.Length) {
                processedFitnessScores = new float[FitnessEvalGroupArray.Length];
            }
        }

        // Process!  -- Makes a lot of assumptions!!!
        int numComponents = fitnessComponentDefinitions.Count;
        float[][] componentRecordMinimums = new float[numComponents][];
        float[][] componentRecordMaximums = new float[numComponents][];
        float[] componentWeightsNormalized = new float[numComponents];  // Rescales fitnessComponent Weights to add up to One (1).
        //float[][] componentPerTrialWeightsNormalized = new float[numComponents][];  // Rescales fitnessComponent Weights to add up to One (1) -- For each fitComp within a trial.
        //
        //float[][] componentAverageBaselineScores = new float[numComponents][];
        float[][] componentRawTotalScore = new float[numComponents][];
        // Need the Individual version of: componentAveragePopulationScores[f][n];
        //float[][] componentWorstIndividualScores = new float[numComponents][];
        //float[][] componentBestIndividualScores = new float[numComponents][];
        // Initialize arrays:
        for (int a = 0; a < numComponents; a++) {
            componentRecordMinimums[a] = new float[FitnessEvalGroupArray[a].Count]; // questionable?
            componentRecordMaximums[a] = new float[FitnessEvalGroupArray[a].Count];
            //componentAverageBaselineScores[a] = new float[FitnessEvalGroupArray[a].Count];

            componentRawTotalScore[a] = new float[FitnessEvalGroupArray[a].Count];
            
            //componentWorstIndividualScores[a] = new float[FitnessEvalGroupArray[a].Count];
            //componentBestIndividualScores[a] = new float[FitnessEvalGroupArray[a].Count];

            for (int d = 0; d < FitnessEvalGroupArray[a].Count; d++) {
                componentRecordMinimums[a][d] = float.PositiveInfinity;
                componentRecordMaximums[a][d] = float.NegativeInfinity;
            }
        }
        //=================================================================================================
        // Loop through all fitness scores and keep track of min/max values range for each trial/component
        // GET RECORD MIN / MAX for each fitComp & Trial
        // POPULATE BASELINE & AVGPOP DATA!!!
        for (int i = 0; i < numComponents; i++) {  // for each fitness component
            //componentRecordMinimums[i] = float.PositiveInfinity;
            //componentRecordMaximums[i] = float.NegativeInfinity;
            for (int j = 0; j < FitnessEvalGroupArray.Length; j++) { // loop through each genome
                                                                     //string txt = "ProcessRawFitness()";

                for (int k = 0; k < FitnessEvalGroupArray[j].Count; k++) { // for each EvalGroup of this genome
                    float score = FitnessEvalGroupArray[j][k].fitCompList[i].rawScore;
                    // Won't work if evalTimes are different? -- among other things...
                    componentRecordMinimums[i][k] = Mathf.Min(score, componentRecordMinimums[i][k]);
                    componentRecordMaximums[i][k] = Mathf.Max(score, componentRecordMaximums[i][k]);

                    //historicalComponentRecordMinimums[i] = Mathf.Min(score, historicalComponentRecordMinimums[i]);
                    //historicalComponentRecordMaximums[i] = Mathf.Max(score, historicalComponentRecordMaximums[i]);

                    //if (j < popSize) {
                    componentRawTotalScore[i][k] += score;
                    //}
                    //else {
                    //    componentAverageBaselineScores[i][k] += score;
                    //}
                }
            }
        }
        //for(int i = 0; i < numComponents; i++ ) {
         //   Debug.Log(fitnessComponentDefinitions[i].type.ToString() + ": ");
        //}

        // Loop through components and get NORMALIZED WEIGHTS:
        float totalFitCompWeight = 0f;
        for (int f = 0; f < numComponents; f++) {
            totalFitCompWeight += fitnessComponentDefinitions[f].weight;
        }
        // store normalized component weights:
        for (int f = 0; f < numComponents; f++) {
            componentWeightsNormalized[f] = Mathf.Clamp01(fitnessComponentDefinitions[f].weight / totalFitCompWeight);
        }
        
        /*for (int i = 0; i < numComponents; i++) {
            // For each Trial:
            for (int k = 0; k < FitnessEvalGroupArray[0].Count; k++) {
                float rawAvgPopScore;
                float normalizedAvgComponentScore;
                if ((componentRecordMaximums[i][k] - componentRecordMinimums[i][k]) != 0) {
                    rawAvgPopScore = componentRawTotalScore[i][k] / popSize;
                    normalizedAvgComponentScore = (rawAvgPopScore - componentRecordMinimums[i][k]) / (componentRecordMaximums[i][k] - componentRecordMinimums[i][k]);
                    if (!FitnessEvalGroupArray[i][k].fitCompList[i].sourceDefinition.biggerIsBetter) {
                        normalizedAvgComponentScore = 1f - normalizedAvgComponentScore;  // 1=good, 0=bad
                    }
                }
                else {  // all agents are tied in score
                    normalizedAvgComponentScore = 0f;
                }
            }            
        }*/
        
        //=================================================================================================
        // FINAL PROCESSED SINGLE FLOAT SCORE::::
        // Loop through all genomes and tally up their scores from each of their evaluations
        for (int a = 0; a < FitnessEvalGroupArray.Length; a++) {  // a = genomes
            float genomeScore = 0f;
            for (int b = 0; b < FitnessEvalGroupArray[a].Count; b++) {  // b = evalGroups (i.e. trials)
                float evalScore = 0f;
                for (int c = 0; c < FitnessEvalGroupArray[a][b].fitCompList.Count; c++) {  // c = fitnessComponents
                    if ((componentRecordMaximums[c][b] - componentRecordMinimums[c][b]) > 0f) {
                        float normalizedScore = (FitnessEvalGroupArray[a][b].fitCompList[c].rawScore - componentRecordMinimums[c][b]) / (componentRecordMaximums[c][b] - componentRecordMinimums[c][b]);
                        if (!FitnessEvalGroupArray[a][b].fitCompList[c].sourceDefinition.biggerIsBetter) {
                            normalizedScore = 1f - normalizedScore;  // 1=good, 0=bad
                        }
                        evalScore += normalizedScore * componentWeightsNormalized[c];
                    }
                    else {
                        evalScore += 0f * componentWeightsNormalized[c]; // score when it's an all-way draw
                    }
                }
                genomeScore += evalScore;
            }
            genomeScore /= FitnessEvalGroupArray[a].Count; // divide by # trials

            // Store final processed score in global array -- [ 0 < score < 1 ]
            processedFitnessScores[a] = genomeScore;
        }
    }

    public void RankProcessedFitness(int popSize) {
        rankedIndicesList = new int[processedFitnessScores.Length];
        rankedFitnessList = new float[processedFitnessScores.Length];

        // populate arrays:
        for (int i = 0; i < processedFitnessScores.Length; i++) {
            rankedIndicesList[i] = i;
            rankedFitnessList[i] = processedFitnessScores[i];
        }
        for (int i = 0; i < processedFitnessScores.Length - 1; i++) {
            for (int j = 0; j < processedFitnessScores.Length - 1; j++) {
                float swapFitA = rankedFitnessList[j];
                float swapFitB = rankedFitnessList[j + 1];
                int swapIdA = rankedIndicesList[j];
                int swapIdB = rankedIndicesList[j + 1];

                if (swapFitA < swapFitB) {
                    rankedFitnessList[j] = swapFitB;
                    rankedFitnessList[j + 1] = swapFitA;
                    rankedIndicesList[j] = swapIdB;
                    rankedIndicesList[j + 1] = swapIdA;
                }
            }
        }
        string fitnessRankText = "";
        for (int i = 0; i < processedFitnessScores.Length; i++) {
            fitnessRankText += "[" + rankedIndicesList[i].ToString() + "]: " + rankedFitnessList[i].ToString() + "\n";
        }
        //Debug.Log(fitnessRankText);
        
        //if (avgRatioBest > alltimeMaxRatioValue) {
        //alltimeMaxRatioValue = avgRatioBest;
        //}
        // TEMP!
        //AddTopHalfBonus();
    }

    public void AddTopHalfBonus() {
        Debug.Log("AddTopHalfBonus()");
        for (int i = 0; i < rankedFitnessList.Length; i++) {
            //rankedFitnessList[rankedIndicesList[i]] = Mathf.Pow(rankedFitnessList[rankedIndicesList[i]], 4f);
            if (i * 2 > rankedFitnessList.Length) {
                rankedFitnessList[i] = Mathf.Lerp(0f, rankedFitnessList[i], (float)i / (float)rankedFitnessList.Length);
            }
            else {
                rankedFitnessList[i] = Mathf.Lerp(rankedFitnessList[i], 1f, (float)i / (float)rankedFitnessList.Length);
            }
            //float bonus = 0.5f;
            //rankedFitnessList[i] = Mathf.Lerp(rankedFitnessList[i], (float)i/(float)rankedFitnessList.Length, bonus);
        }
    }

    public int GetAgentIndexByLottery() {
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
}
