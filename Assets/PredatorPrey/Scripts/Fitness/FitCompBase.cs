using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Fitness Components should be created for each EvaluationInstance in which they are involved, per population.
// They should be stored in a list inside their population's FitnessManager, organized by genome.
// So if a given genome has undergone 3 evaluations in a generation, the fitnessManager should have 3 copies
// of the specific FitComp associated with that Genome.
// They serve 2 main purposes:
// 1) Keep track of evaluation raw scores from all evals, within the FitnessManager
// 2) Hook up into the Evals and calculate the score update per evaluation Tick()

public class FitCompBase {

    public FitnessComponentDefinition sourceDefinition;
    public float rawScore;

    public FitCompBase() {

    }

    public virtual void TickScore() {

    }
}
