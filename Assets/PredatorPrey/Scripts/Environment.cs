using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour {

    //public EnvironmentGameplay environmentGameplay;
    //public EnvironmentRenderable environmentRenderable;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddRenderableContent(EnvironmentGenome genome) {

    }

    public void CreateCollisionAndGameplayContent(EnvironmentGenome genome) {
        //GameObject environmentGameplayGO = new GameObject("environmentGameplay");
        //environmentGameplay = environmentGameplayGO.AddComponent<EnvironmentGameplay>();
        //environmentGameplay.transform.parent = gameObject.transform;
        //environmentGameplay.transform.localPosition = new Vector3(0f, 0f, 0f);

        // Construct Ground Physics Material:
        
        //=============WALLS===========
        
        // Game-Required Modules:
        // Target !!!        
        // Obstacles:        
        // Atmosphere (WIND) !!!
        
        // Set Genome's prefab environment:
        //genome.gameplayPrefab = environmentGameplay;
    }
}
