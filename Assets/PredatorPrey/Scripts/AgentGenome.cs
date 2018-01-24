using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentGenome {

    public int index = -1;

    public BodyGenome bodyGenome;
    public BrainGenome brainGenome;

    // Constructor
    public AgentGenome(int index) {
        this.index = index;

        bodyGenome = new BodyGenome();  // empty constructors:
        brainGenome = new BrainGenome();
    }
    
    public void InitializeBodyGenomeFromTemplate(BodyGenome bodyGenomeTemplate) {
        bodyGenome.CopyBodyGenomeFromTemplate(bodyGenomeTemplate);
    }

    public void InitializeRandomBrainFromCurrentBody(float initialWeightsMultiplier) {
        brainGenome.InitializeRandomBrainGenome(bodyGenome, initialWeightsMultiplier);
    }

    public void PrintBrainGenome() {
        string neuronText = "";
        for (int i = 0; i < brainGenome.bodyNeuronList.Count; i++) {
            neuronText += "(" + brainGenome.bodyNeuronList[i].nid.moduleID.ToString() + "," + brainGenome.bodyNeuronList[i].nid.neuronID.ToString() + ")\n";
        }
        Debug.Log(neuronText);
        string linkText = "";
        for (int i = 0; i < brainGenome.linkList.Count; i++) {
            linkText += "(" + brainGenome.linkList[i].fromModuleID.ToString() + "," + brainGenome.linkList[i].fromNeuronID.ToString() + ") ==> (" +
                        brainGenome.linkList[i].toModuleID.ToString() + "," + brainGenome.linkList[i].toNeuronID.ToString() + ")\n";
        }
        Debug.Log(linkText);
    }
}
