using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImitationUI : MonoBehaviour {

    public ImitationLearningManager imitationLearningManager;

    public Button buttonTogglePause;
    public Button buttonDataCollectionMode;
    public Button buttonTrainingMode;
    public Button buttonResetEnv;
    public Text textDataCollection;
    public Text textTrainingProgress;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UpdateDataCollectionText();
        UpdateTrainingProgressText();
	}

    private void UpdateDataCollectionText() {
        string txt = "Current Data Collection Round:\n";
        txt += imitationLearningManager.curDataCollectionRound.ToString() + " / " + imitationLearningManager.numDataCollectionRounds.ToString() + " Rounds\n";
        txt += imitationLearningManager.curTimeStep.ToString() + " / " + imitationLearningManager.maxTrialTimeSteps.ToString() + " Time Steps\n";
        int samplesCollected = 0;
        if(imitationLearningManager.dataSamplesList != null) {
            samplesCollected = imitationLearningManager.dataSamplesList.Count;
        }
        txt += samplesCollected.ToString() + " Data Samples Recorded";
        textDataCollection.text = txt;
    }
    private void UpdateTrainingProgressText() {
        string txt = "Current Training Progress:\n";
        txt += "Agent: " + imitationLearningManager.curTestingAgent.ToString() + ", Sample: " + imitationLearningManager.curTestingSample.ToString() + "\n";
        txt += "Generation " + imitationLearningManager.curTrainingGen.ToString() + "\n";
        txt += "Fitness: " + imitationLearningManager.avgFitnessLastGen.ToString("F2");
        textTrainingProgress.text = txt;
    }

    public void TogglePause() {
        Debug.Log("TogglePause() " + imitationLearningManager.isPaused.ToString());
        if (imitationLearningManager.isPaused) {
            Time.timeScale = 1f;
            buttonTogglePause.GetComponentInChildren<Text>().text = "PAUSE";
        }
        else {
            Time.timeScale = 0f;
            buttonTogglePause.GetComponentInChildren<Text>().text = "PLAY";
        }
        imitationLearningManager.isPaused = !imitationLearningManager.isPaused;
    }

    public void DataCollectionMode() {
        Debug.Log("DataCollectionMode()");
        imitationLearningManager.EnterDataCollectionMode();
    }

    public void TrainingMode() {
        Debug.Log("TrainingMode()");
        imitationLearningManager.EnterTrainingMode();
    }

    public void ResetEnv() {
        Debug.Log("ResetEnv()");
        imitationLearningManager.ResetTrainingAgentAndEnvironment();
    }
}
