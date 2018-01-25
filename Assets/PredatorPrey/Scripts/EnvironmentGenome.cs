using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentGenome {

    public int index = -1;
    
    public Vector3 arenaBounds;    

    // MODULES:    
    public List<StartPositionGenome> agentStartPositionsList;
    
    public EnvironmentGenome(int index) {
        this.index = index;
        
    }
    public void InitializeAsDefaultGenome() {
        // ARENA BOUNDS IRRELEVANT!!!
        // Currently hardcoded in:
        //  EvaluationManager --> CreateEvaluationInstances()
        arenaBounds = new Vector3(40f, 40f, 40f);

        agentStartPositionsList = new List<StartPositionGenome>();
        StartPositionGenome player1Start = new StartPositionGenome(new Vector3(0f, -5f, 0f), Quaternion.identity);
        StartPositionGenome player2Start = new StartPositionGenome(new Vector3(0f, 5f, 0f), Quaternion.identity);
        agentStartPositionsList.Add(player1Start);
        agentStartPositionsList.Add(player2Start);
    }
    public void InitializeRandomGenomeFromTemplate(EnvironmentGenome templateGenome) {
        CopyGenomeFromTemplate(templateGenome);        
    }
    public void CopyGenomeFromTemplate(EnvironmentGenome templateGenome) {
        
        arenaBounds = new Vector3(templateGenome.arenaBounds.x, templateGenome.arenaBounds.y, templateGenome.arenaBounds.z);

        agentStartPositionsList = new List<StartPositionGenome>();
        for (int i = 0; i < templateGenome.agentStartPositionsList.Count; i++) {
            StartPositionGenome genomeCopy = new StartPositionGenome(templateGenome.agentStartPositionsList[i]);
            agentStartPositionsList.Add(genomeCopy);
        }              
    }
    
    public EnvironmentGenome BirthNewGenome(EnvironmentGenome parentGenome, int index, float mutationRate, float mutationDriftAmount) {
        EnvironmentGenome newGenome = new EnvironmentGenome(index);
                
        newGenome.arenaBounds = new Vector3(parentGenome.arenaBounds.x, parentGenome.arenaBounds.y, parentGenome.arenaBounds.z);
        
        // StartPositions:
        // HACKY! DOES NOT SUPPORT EVOLVING START POSITIONS! ALL THE SAME!!!!
        newGenome.agentStartPositionsList = parentGenome.agentStartPositionsList;
        
        return newGenome;
    }
}
