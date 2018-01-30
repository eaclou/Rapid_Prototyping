using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    //public AgentGenome genome;    
    public Brain brain;

    public TestModule testModule;
    private Rigidbody2D rigidBody2D;
    //[System.NonSerialized]
    //public List<HealthModule> healthModuleList;
    //[System.NonSerialized]
    //public List<InputValue> valueList;
    
    public bool isVisible = false;

    public float horizontalMovementInput = 0f;
    public float verticalMovementInput = 0f;

    public float speed = 150f;

    public bool humanControlled = false;

    // Use this for initialization
    void Start() {
        //Debug.Log("New Agent!");
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    public void MapNeuronToModule(NID nid, Neuron neuron) {
        testModule.MapNeuron(nid, neuron);
        // Hidden nodes!
        if (nid.moduleID == -1) {
            //Debug.Log("Map Hidden Neuron #" + nid.neuronID.ToString());

            neuron.currentValue = new float[1];
            neuron.neuronType = NeuronGenome.NeuronType.Hid;
            neuron.previousValue = 0f;
        }

        /*
        for (int i = 0; i < healthModuleList.Count; i++) {
            healthModuleList[i].MapNeuron(nid, neuron);
        }        
        for (int i = 0; i < valueList.Count; i++) {
            valueList[i].MapNeuron(nid, neuron);
        }
        
        // Hidden nodes!
        if (nid.moduleID == -1) {
            //Debug.Log("Map Hidden Neuron #" + nid.neuronID.ToString());

            neuron.currentValue = new float[1];
            neuron.neuronType = NeuronGenome.NeuronType.Hid;
            neuron.previousValue = 0f;
        }
        */

        /*
        public void MapNeuron(NID nid, Neuron neuron) {
            if (inno == nid.moduleID) {
                if (nid.neuronID == 0) {
                    neuron.currentValue = healthSensor;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
                if (nid.neuronID == 1) {
                    neuron.currentValue = takingDamage;
                    neuron.neuronType = NeuronGenome.NeuronType.In;
                }
            }
        }
         * */
    }

    public void TickBrain() {        
        brain.BrainMasterFunction();
    }
    public void RunModules() {
        testModule.Tick();

        //Vector3 agentPos = new Vector3(testModule.ownPosX[0], testModule.ownPosY[0], 0f);
        //this.transform.localPosition = agentPos;

        if (humanControlled) {
            horizontalMovementInput = 0f;
            if (Input.GetKey("left") || Input.GetKey("a")) {
                horizontalMovementInput -= 1f;
            }
            if (Input.GetKey("right") || Input.GetKey("d")) {
                horizontalMovementInput += 1f;
            }

            verticalMovementInput = 0f;
            if (Input.GetKey("up") || Input.GetKey("w")) {
                verticalMovementInput += 1f;
            }
            if (Input.GetKey("down") || Input.GetKey("s")) {
                verticalMovementInput -= 1f;
            }
        }
        else {
            horizontalMovementInput = Mathf.Round(testModule.throttleX[0] * 3f / 2f);
            verticalMovementInput = Mathf.Round(testModule.throttleY[0] * 3f / 2f);
        }

        // MOVEMENT HERE:
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(speed * horizontalMovementInput * Time.deltaTime, 0f), ForceMode2D.Impulse);
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, speed * verticalMovementInput * Time.deltaTime), ForceMode2D.Impulse);
    }
    public void RunModules(int timeStep, Environment currentEnvironment) {
        testModule.Tick();

        //Vector3 agentPos = new Vector3(testModule.ownPosX[0], testModule.ownPosY[0], 0f);
        //this.transform.localPosition = agentPos;

        if(humanControlled) {
            horizontalMovementInput = 0f;
            if (Input.GetKey("left") || Input.GetKey("a")) {
                horizontalMovementInput -= 1f;
            }
            if (Input.GetKey("right") || Input.GetKey("d")) {
                horizontalMovementInput += 1f;
            }

            verticalMovementInput = 0f;
            if (Input.GetKey("up") || Input.GetKey("w")) {
                verticalMovementInput += 1f;
            }
            if (Input.GetKey("down") || Input.GetKey("s")) {
                verticalMovementInput -= 1f;
            }
        }
        else {
            horizontalMovementInput = Mathf.Round(testModule.throttleX[0] * 3f / 2f);
            verticalMovementInput = Mathf.Round(testModule.throttleY[0] * 3f / 2f);
        }

        // MOVEMENT HERE:
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(speed * horizontalMovementInput * Time.deltaTime, 0f), ForceMode2D.Impulse);
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, speed * verticalMovementInput * Time.deltaTime), ForceMode2D.Impulse);
        

        //for (int i = 0; i < healthModuleList.Count; i++) {
        //    healthModuleList[i].Tick();
        //}        
        //for (int i = 0; i < valueList.Count; i++) {            
        //}        
    }

    public void InitializeModules(AgentGenome genome, Agent agent, StartPositionGenome startPos) {
        testModule = new TestModule();
        testModule.Initialize(genome.bodyGenome.testModuleGenome, agent, startPos);

        
        /*
        healthModuleList = new List<HealthModule>();        
        valueList = new List<InputValue>();
        
        for (int i = 0; i < genome.bodyGenome.healthModuleList.Count; i++) {
            HealthModule healthModule = new HealthModule();
            //agent.segmentList[genome.healthModuleList[i].parentID].AddComponent<HealthModuleComponent>();
            healthModule.Initialize(genome.bodyGenome.healthModuleList[i], agent);
            healthModuleList.Add(healthModule);
            //healthModuleList[i].Initialize(genome.healthModuleList[i]);
        }        
        for (int i = 0; i < genome.bodyGenome.valueInputList.Count; i++) {
            InputValue inputValue = new InputValue();
            inputValue.Initialize(genome.bodyGenome.valueInputList[i], agent);
            valueList.Add(inputValue);
            //valueList[i].Initialize(genome.valueInputList[i]);
        } 
        */
    }

    public void InitializeAgentFromTemplate(AgentGenome genome, StartPositionGenome startPos) {
        // Initialize Modules --
        //Debug.Log("Agent Initialize Modules() segment count: " + segmentList.Count.ToString() + ", visCount: " + visibleObjectList.Count.ToString());
        // -- Setup that used to be done in the constructors
        InitializeModules(genome, this, startPos);

        // Visible/Non-Visible:
        //if (isVisible) {
        //    for (int i = 0; i < visibleObjectList.Count; i++) {
        //        visibleObjectList[i].SetActive(true);
        //    }
        //}
        // Construct Brain:
        brain = new Brain(genome.brainGenome, this);
    }
}
