using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestModuleGenome {
    public int parentID;
    public int inno;
    
    public float speed;

    public TestModuleGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
        this.speed = 0.5f;
    }

    public TestModuleGenome(TestModuleGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.speed = template.speed;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        NeuronGenome bias = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 0);
        NeuronGenome posX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 1);
        NeuronGenome posY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 2);
        NeuronGenome enemyPosX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 3);
        NeuronGenome enemyPosY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 4);

        NeuronGenome moveX = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 5);
        NeuronGenome moveY = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 6);

        neuronList.Add(bias);
        neuronList.Add(posX);
        neuronList.Add(posY);
        neuronList.Add(enemyPosX);
        neuronList.Add(enemyPosY);

        neuronList.Add(moveX);
        neuronList.Add(moveY);
    }
}