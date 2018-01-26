using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum FitnessComponentType {
    DistanceToEnemy,
    Random,
    WinLoss
};
[System.Serializable]
public enum FitnessComponentMeasure {
    Avg,
    Min,
    Max,
    Last
};

// Experimental future:
/* // Allows definitions to be more generic and modular?
public enum ObjectTypeReference {
    primaryAgentCenterOfMass,
    targetColumn,
    nearestAgentCenterOfMass,
    hazard
};
*/

[System.Serializable]
public class FitnessComponentDefinition {
    // This Class serves as a serializable list of attributes and types for each fitness component
    // within a population's FitnessManager, to let it know which FitnessComponent Types to instantiate
    // during an Evaluation.

    public FitnessComponentType type;
    public FitnessComponentMeasure measure;
    public float weight;
    public bool biggerIsBetter;

    public FitnessComponentDefinition(FitnessComponentType type, FitnessComponentMeasure measure, float weight, bool biggerIsBetter) {
        this.type = type;
        this.measure = measure;
        this.weight = weight;
        this.biggerIsBetter = biggerIsBetter;
    }
}
