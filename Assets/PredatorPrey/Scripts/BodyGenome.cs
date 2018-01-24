using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BodyGenome {

    //public AgentBodyType bodyType;
    // Modules:
    
    //public List<HealthGenome> healthModuleList;
    //public List<OscillatorGenome> oscillatorInputList;
    //public List<ValueInputGenome> valueInputList;

    public BodyGenome() {

    }

    public void InitializeGenomeAsDefault() {

    }

    public void CopyBodyGenomeFromTemplate(BodyGenome templateGenome) {
        /*
        // This method creates a clone of the provided BodyGenome - should have no shared references!!!
        bodyType = templateGenome.bodyType;
        // copy module lists:
        healthModuleList = new List<HealthGenome>();
        for (int i = 0; i < templateGenome.healthModuleList.Count; i++) {
            HealthGenome genomeCopy = new HealthGenome(templateGenome.healthModuleList[i]);
            healthModuleList.Add(genomeCopy);
        }
        oscillatorInputList = new List<OscillatorGenome>();
        for (int i = 0; i < templateGenome.oscillatorInputList.Count; i++) {
            OscillatorGenome genomeCopy = new OscillatorGenome(templateGenome.oscillatorInputList[i]);
            oscillatorInputList.Add(genomeCopy);
        }
        valueInputList = new List<ValueInputGenome>();
        for (int i = 0; i < templateGenome.valueInputList.Count; i++) {
            ValueInputGenome genomeCopy = new ValueInputGenome(templateGenome.valueInputList[i]);
            valueInputList.Add(genomeCopy);
        }
        */
    }

    public int GetCurrentHighestInnoValue() {
        int highestInno = -1;
        /*
        for (int i = 0; i < healthModuleList.Count; i++) {
            if (healthModuleList[i].inno > highestInno)
                highestInno = healthModuleList[i].inno;
        }
        for (int i = 0; i < oscillatorInputList.Count; i++) {
            if (oscillatorInputList[i].inno > highestInno)
                highestInno = oscillatorInputList[i].inno;
        }        
        */
        return highestInno;
    }
}
