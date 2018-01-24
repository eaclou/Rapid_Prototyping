using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    //public AgentGenome genome;
    public Brain brain;
    
    //[System.NonSerialized]
    //public List<HealthModule> healthModuleList;
    //[System.NonSerialized]
    //public List<InputValue> valueList;
    
    public bool isVisible = false;

    // Use this for initialization
    void Start() {
        //Debug.Log("New Agent!");
    }

    public void MapNeuronToModule(NID nid, Neuron neuron) {
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
    }

    public void TickBrain() {        
        brain.BrainMasterFunction();
    }
    public void RunModules(int timeStep, Environment currentEnvironment) {        
        //for (int i = 0; i < healthModuleList.Count; i++) {
        //    healthModuleList[i].Tick();
        //}        
        //for (int i = 0; i < valueList.Count; i++) {            
        //}        
    }

    public void InitializeModules(AgentGenome genome, Agent agent) {
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

    public void InitializeAgentFromTemplate(AgentGenome genome) {
        // Initialize Modules --
        //Debug.Log("Agent Initialize Modules() segment count: " + segmentList.Count.ToString() + ", visCount: " + visibleObjectList.Count.ToString());
        // -- Setup that used to be done in the constructors
        InitializeModules(genome, this);

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
