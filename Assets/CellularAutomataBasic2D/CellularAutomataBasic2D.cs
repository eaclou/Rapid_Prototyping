using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataBasic2D : MonoBehaviour {

    public Texture2D initTex;
    public Shader blitShader;
    public RenderTexture mainRT;
    private RenderTexture tempRT;
    private Material blitMat;

	// Use this for initialization
	void Start () {
		if(mainRT) {
            tempRT = new RenderTexture(mainRT.width, mainRT.height, 1, mainRT.format);
        }

        blitMat = new Material(blitShader);

        if(initTex) {
            Graphics.Blit(initTex, mainRT);
        }

        Tick();
    }

    private void Tick() {
        Graphics.Blit(mainRT, tempRT, blitMat, 0);
        Graphics.Blit(tempRT, mainRT);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Pressed left click.");
            Tick();
        }
            
    }
}
