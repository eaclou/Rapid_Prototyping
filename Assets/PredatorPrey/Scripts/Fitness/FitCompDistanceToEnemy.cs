using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCompDistanceToEnemy : FitCompBase {

    public Vector3 ownPos;
    public Vector3 enemyPos;

    public FitCompDistanceToEnemy(FitnessComponentDefinition sourceDefinition) {
        this.sourceDefinition = sourceDefinition;        
    }

    public override void TickScore() {
        float distSquared = (enemyPos - ownPos).sqrMagnitude;
        switch (sourceDefinition.measure) {
            case FitnessComponentMeasure.Avg:
                rawScore += distSquared;
                break;
            case FitnessComponentMeasure.Min:
                rawScore = Mathf.Min(rawScore, distSquared);
                break;
            case FitnessComponentMeasure.Max:
                rawScore = Mathf.Max(rawScore, distSquared);
                break;
            case FitnessComponentMeasure.Last:
                rawScore = distSquared;
                break;
            default:
                break;
        }
        //Debug.Log("distSquared: " + distSquared.ToString());
    }
}
