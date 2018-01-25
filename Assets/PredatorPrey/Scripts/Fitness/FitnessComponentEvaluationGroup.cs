using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessComponentEvaluationGroup {

    public List<FitCompBase> fitCompList;

    public FitnessComponentEvaluationGroup() {
        fitCompList = new List<FitCompBase>();
    }

    public void CreateFitnessComponentEvaluationGroup(FitnessManager fitnessManager, int genomeIndex) {

        for (int i = 0; i < fitnessManager.fitnessComponentDefinitions.Count; i++) {
            switch (fitnessManager.fitnessComponentDefinitions[i].type) {
                case FitnessComponentType.DistanceToEnemy:
                    FitCompDistanceToEnemy fitCompDistanceToTargetSquared = new FitCompDistanceToEnemy(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompDistanceToTargetSquared);
                    break;                
                case FitnessComponentType.Random:
                    FitCompRandom fitCompRandom = new FitCompRandom(fitnessManager.fitnessComponentDefinitions[i]);
                    fitCompList.Add(fitCompRandom);
                    break;
                default:
                    Debug.LogError("No such component type! (" + fitnessManager.fitnessComponentDefinitions[i].type.ToString() + ")");
                    break;
            }
        }

        fitnessManager.AddNewFitCompEvalGroup(this, genomeIndex);
    }
}
