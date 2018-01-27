using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentModule {

    public float[] bias;
    public float[] ownVelX;
    public float[] ownVelY;
    public float[] targetPosX;
    public float[] targetPosY;
    public float[] targetDirX;
    public float[] targetDirY;
    public float[] distLeft;
    public float[] distRight;
    public float[] distUp;
    public float[] distDown;

    public float[] throttleX;
    public float[] throttleY;

    public AgentModule() {

    }
}
