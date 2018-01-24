using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    //public bool isTraining = false;
    //public UIManager uiManager;
    public TrainingManager trainerRef;

    // Use this for initialization
    void Start () {
        FirstTimeInitialization();
    }

    private void FirstTimeInitialization() {
        //uiManager.InitializeUI();
        trainerRef.NewTrainingMode();
    }

    // Update is called once per frame
    void Update () {
        //uiManager.panelTraining.UpdateState();
        //SetCamera();
    }

    void FixedUpdate() {        
        trainerRef.Tick();
               
    }

    public void QuitGame() {
        Application.Quit();
    }
}
