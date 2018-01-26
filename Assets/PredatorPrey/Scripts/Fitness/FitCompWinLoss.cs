using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCompWinLoss : FitCompBase {

    public float[] score;

    public FitCompWinLoss(FitnessComponentDefinition sourceDefinition) {
        this.sourceDefinition = sourceDefinition;
        score = new float[1];
    }

    public override void TickScore() {

        switch (sourceDefinition.measure) {
            case FitnessComponentMeasure.Avg:
                rawScore += score[0];
                break;
            case FitnessComponentMeasure.Min:
                rawScore = Mathf.Min(rawScore, score[0]);
                break;
            case FitnessComponentMeasure.Max:
                rawScore = Mathf.Max(rawScore, score[0]);
                break;
            case FitnessComponentMeasure.Last:
                rawScore = score[0];
                break;
            default:
                break;
        }
    }
}
