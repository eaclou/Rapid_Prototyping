using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuronGenome {

    public NeuronType neuronType;
    public enum NeuronType {
        In,
        Hid,
        Out
    }

    public NID nid;
    //public int moduleID;  // id of source module
    //public int neuronID;  // id within that specific module

    public NeuronGenome() {

    }

    public NeuronGenome(NeuronType type, int moduleID, int neuronID) {
        neuronType = type;
        nid = new NID(moduleID, neuronID);
    }

    public NeuronGenome(NeuronType type, NID nid) {
        neuronType = type;
        this.nid = nid;
    }

    public NeuronGenome(NeuronGenome template) {
        neuronType = template.neuronType;
        nid = template.nid;
    }
}
