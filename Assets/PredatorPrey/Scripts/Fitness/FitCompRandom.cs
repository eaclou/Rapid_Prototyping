using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCompRandom : FitCompBase {

    public float score = UnityEngine.Random.Range(0f, 1f);

    public FitCompRandom(FitnessComponentDefinition sourceDefinition) {
        this.sourceDefinition = sourceDefinition;
    }

    public override void TickScore() {

        switch (sourceDefinition.measure) {
            case FitnessComponentMeasure.Avg:
                rawScore += score;
                break;
            case FitnessComponentMeasure.Min:
                rawScore = Mathf.Min(rawScore, score);
                break;
            case FitnessComponentMeasure.Max:
                rawScore = Mathf.Max(rawScore, score);
                break;
            case FitnessComponentMeasure.Last:
                rawScore = score;
                break;
            default:
                break;
        }
    }
}
