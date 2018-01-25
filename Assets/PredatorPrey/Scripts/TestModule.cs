using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestModule {
    //public int parentID;
    //public int inno;
    public int parentID;
    public int inno;
    //public bool isVisible;

    //public bool destroyed = false;
    //public float maxHealth;
    //public float prevHealth;
    //public float health;
    public float[] bias;
    public float[] posX;
    public float[] posY;
    public float[] enemyPosX;
    public float[] enemyPosY;
    public float[] moveX;
    public float[] moveY;

    public float speed = 0.1f;

    public Transform enemyTransform;

    //public HealthModuleComponent component;

    public TestModule() {
        /*healthSensor = new float[1];
        takingDamage = new float[1];
        health = maxHealth;
        prevHealth = health;
        parentID = genome.parentID;
        inno = genome.inno;*/
    }

    public void Initialize(TestModuleGenome genome, Agent agent, StartPositionGenome startPos) {
        //destroyed = false;
        bias = new float[1];
        bias[0] = 1f;

        posX = new float[1];
        posX[0] = startPos.agentStartPosition.x;
        posY = new float[1];
        posY[0] = startPos.agentStartPosition.y;
        enemyPosX = new float[1];
        enemyPosY = new float[1];

        moveX = new float[1];
        moveY = new float[1];

        speed = genome.speed;
        //maxHealth = genome.maxHealth;
        //health = maxHealth;
        //prevHealth = health;

        parentID = genome.parentID;
        inno = genome.inno;
        //isVisible = agent.isVisible;

        //component = agent.segmentList[parentID].AddComponent<HealthModuleComponent>();
        //if (component == null) {
        //    Debug.LogAssertion("No existing HealthModuleComponent on segment " + parentID.ToString());
        //}
        //component.healthModule = this;
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            if (nid.neuronID == 0) {
                neuron.currentValue = bias;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 1) {
                neuron.currentValue = posX;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 2) {
                neuron.currentValue = posY;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 3) {
                neuron.currentValue = enemyPosX;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 4) {
                neuron.currentValue = enemyPosY;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }

            if (nid.neuronID == 5) {
                neuron.currentValue = moveX;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
            if (nid.neuronID == 6) {
                neuron.currentValue = moveY;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
        }
    }

    /*public void InflictDamage(float amount) {
        health -= amount;
        if (health <= 0f) {
            health = 0f;
            destroyed = true;
        }
    }*/

    public void Tick() {
        enemyPosX[0] = enemyTransform.localPosition.x - posX[0];
        enemyPosY[0] = enemyTransform.localPosition.y - posY[0];

        posX[0] += moveX[0] * speed;
        posY[0] += moveY[0] * speed;

        if(posX[0] > 10f) {
            posX[0] = 10f;
        }
        if (posY[0] > 10f) {
            posY[0] = 10f;
        }
        if (posX[0] < -10f) {
            posX[0] = -10f;
        }
        if (posY[0] < -10f) {
            posY[0] = -10f;
        }

        // OLD:
        /*
        healthSensor[0] = health / maxHealth;
        if (health != prevHealth) {
            takingDamage[0] = 1f;
        }
        else {
            takingDamage[0] = 0f;
        }
        prevHealth = health;
        takingDamage[0] = (maxHealth - health) / maxHealth;
        */
    }
}
