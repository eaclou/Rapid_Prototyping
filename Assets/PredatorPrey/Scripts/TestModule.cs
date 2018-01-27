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
    public float[] ownPosX;
    public float[] ownPosY;
    public float[] ownVelX;
    public float[] ownVelY;
    public float[] enemyPosX;
    public float[] enemyPosY;
    public float[] enemyVelX;
    public float[] enemyVelY;
    public float[] enemyDirX;
    public float[] enemyDirY;

    public float[] distLeft;
    public float[] distRight;
    public float[] distUp;
    public float[] distDown;

    public float[] throttleX;
    public float[] throttleY;

    public float maxSpeed = 0.5f;
    public float accel = 0.05f;
    public float radius = 1f;

    public Transform enemyTransform;
    public TestModule enemyTestModule;

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
        bias[0] = 0f;

        ownPosX = new float[1];
        ownPosX[0] = startPos.agentStartPosition.x;
        ownPosY = new float[1];
        ownPosY[0] = startPos.agentStartPosition.y;
        ownVelX = new float[1];
        ownVelY = new float[1];

        enemyPosX = new float[1];
        enemyPosY = new float[1];
        enemyVelX = new float[1];
        enemyVelY = new float[1];
        enemyDirX = new float[1];
        enemyDirY = new float[1];

        distLeft = new float[1];
        distRight = new float[1];
        distUp = new float[1];
        distDown = new float[1];

        throttleX = new float[1];
        throttleY = new float[1];

        maxSpeed = genome.maxSpeed;
        accel = genome.accel;
        radius = genome.radius;

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
            //if (nid.neuronID == 1) {
            //    neuron.currentValue = ownPosX;
            //    neuron.neuronType = NeuronGenome.NeuronType.In;
            //}
            //if (nid.neuronID == 2) {
            //    neuron.currentValue = ownPosY;
            //    neuron.neuronType = NeuronGenome.NeuronType.In;
            //}
            if (nid.neuronID == 3) {
                neuron.currentValue = ownVelX;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 4) {
                neuron.currentValue = ownVelY;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 5) {
                neuron.currentValue = enemyPosX;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 6) {
                neuron.currentValue = enemyPosY;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 7) {
                neuron.currentValue = enemyVelX;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 8) {
                neuron.currentValue = enemyVelY;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 9) {
                neuron.currentValue = enemyDirX;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 10) {
                neuron.currentValue = enemyDirY;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }

            if (nid.neuronID == 11) {
                neuron.currentValue = distLeft;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 12) {
                neuron.currentValue = distRight;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 13) {
                neuron.currentValue = distUp;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 14) {
                neuron.currentValue = distDown;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }

            if (nid.neuronID == 15) {
                neuron.currentValue = throttleX;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
            if (nid.neuronID == 16) {
                neuron.currentValue = throttleY;
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
        //Vector2 enemyPos = 
        

        enemyPosX[0] = (enemyTransform.localPosition.x - ownPosX[0]) / 10f;
        enemyPosY[0] = (enemyTransform.localPosition.y - ownPosY[0]) / 10f;

        Vector2 enemyDir = new Vector2(enemyTransform.localPosition.x - ownPosX[0], enemyTransform.localPosition.y - ownPosY[0]).normalized;
        enemyDirX[0] = enemyDir.x;
        enemyDirY[0] = enemyDir.y;

        enemyVelX[0] = enemyTestModule.ownVelX[0] - ownVelX[0];
        enemyVelY[0] = enemyTestModule.ownVelY[0] - ownVelY[0];

        ownVelX[0] = Mathf.Clamp(ownVelX[0] + throttleX[0] * accel, -maxSpeed, maxSpeed);
        ownVelY[0] = Mathf.Clamp(ownVelY[0] + throttleY[0] * accel, -maxSpeed, maxSpeed);

        ownPosX[0] += ownVelX[0];
        ownPosY[0] += ownVelY[0];

        /*
        if(ownPosX[0] > 10f) {
            ownPosX[0] = 10f;
            ownVelX[0] = 0f;
        }
        if (ownPosY[0] > 10f) {
            ownPosY[0] = 10f;
            ownVelY[0] = 0f;
        }
        if (ownPosX[0] < -10f) {
            ownPosX[0] = -10f;
            ownVelX[0] = 0f;
        }
        if (ownPosY[0] < -10f) {
            ownPosY[0] = -10f;
            ownVelY[0] = 0f;
        }
        distLeft[0] = 2f - (Mathf.Abs(-10f - ownPosX[0]) / 10f);
        distRight[0] = 2f - (Mathf.Abs(10f - ownPosX[0]) / 10f);
        distUp[0] = 2f - (Mathf.Abs(10f - ownPosY[0]) / 10f);
        distDown[0] = 2f - (Mathf.Abs(-10f - ownPosY[0]) / 10f);
        */

        if (ownPosX[0] > 21f) {
            ownPosX[0] = 21f;
            ownVelX[0] = 0f;
        }
        if (ownPosY[0] > 21f) {
            ownPosY[0] = 21f;
            ownVelY[0] = 0f;
        }
        if (ownPosX[0] < -21f) {
            ownPosX[0] = -21f;
            ownVelX[0] = 0f;
        }
        if (ownPosY[0] < -21f) {
            ownPosY[0] = -21f;
            ownVelY[0] = 0f;
        }
        distLeft[0] = Mathf.Abs(-21f - ownPosX[0]);
        distRight[0] = Mathf.Abs(21f - ownPosX[0]);
        distUp[0] = Mathf.Abs(21f - ownPosY[0]);
        distDown[0] = Mathf.Abs(-21f - ownPosY[0]);

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
