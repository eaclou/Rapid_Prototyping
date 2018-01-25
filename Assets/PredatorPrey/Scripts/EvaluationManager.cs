using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationManager {

    public bool allEvalsComplete;
    // Evaluations Manager
    // for each generation, what are the required evaluation pairs?
    // what is the status of each of them?
    public List<EvaluationTicket> evaluationTicketList;
    //public Queue<EvaluationTicket> evaluationTicketQueue;
    public List<EvaluationInstance> evaluationInstancesList;

    public List<EvaluationTicket> exhibitionTicketList;
    public int exhibitionTicketCurrentIndex = 0;
    public EvaluationInstance exhibitionInstance;
    //public ExhibitionParticleCurves exhibitionParticleCurves;

    private int maxInstancesX = 5;
    private int maxInstancesY = 5;
    private int maxInstancesZ = 5;
    private float instanceBufferX = 2.5f;
    private float instanceBufferY = 2.5f;
    private float instanceBufferZ = 2.5f;
    public int maxTimeStepsDefault = 128;


    public EvaluationManager() {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitializeNewTraining(TeamsConfig teamsConfig) {
        
        // Set up eval tickets:
        evaluationTicketList = new List<EvaluationTicket>();
        exhibitionTicketList = new List<EvaluationTicket>();
        //evaluationTicketQueue = new Queue<EvaluationTicket>();

        // Set up Exhibition Instance:
        GameObject exhibitionInstanceGO = new GameObject("ExhibitionInstance");
        exhibitionInstance = exhibitionInstanceGO.AddComponent<EvaluationInstance>();
        exhibitionInstance.transform.position = new Vector3(0f, 0f, 0f);
        exhibitionInstance.visible = true;
        exhibitionInstance.isExhibition = true;

        CreateEvaluationInstances();

        CreateDefaultEvaluationTickets(teamsConfig); // creates combinatorics for each Population's representatives  
        InitializeExhibitionTickets(teamsConfig);
        
        Debug.Log("EvalManager Initialized!");
    }

    public void CreateDefaultEvaluationTickets(TeamsConfig teamsConfig) {
        allEvalsComplete = false;
        
        // TEMPORARY!!! BRUTE FORCE SOLUTION -- MAX 4 Players!!!:
        int numPlayers = teamsConfig.playersList.Count;
        if (numPlayers > 4) {
            Debug.Assert(true, "More than 4 Players not currently supported! NumPlayers: " + numPlayers.ToString());
        }
        else {
            // Environment First:
            for (int e = 0; e < teamsConfig.environmentPopulation.environmentGenomeList.Count; e++) {
                // 1+ Player:
                for (int i = 0; i < teamsConfig.playersList[0].representativeGenomeList.Count; i++) {
                    if (numPlayers > 1) {
                        // 2+ Players:
                        for (int j = 0; j < teamsConfig.playersList[1].representativeGenomeList.Count; j++) {
                            if (numPlayers > 2) {
                                // 3+ Players:
                                for (int k = 0; k < teamsConfig.playersList[2].representativeGenomeList.Count; k++) {
                                    if (numPlayers > 3) {
                                        // 4 Players
                                        for (int m = 0; m < teamsConfig.playersList[3].representativeGenomeList.Count; m++) {
                                            // 4 Players
                                            //string text = "envIndex: *" + e.ToString() + "*, agentIndices: [" + teamsConfig.playersList[0].representativeGenomeList[i].index.ToString() + "," + teamsConfig.playersList[1].representativeGenomeList[j].index.ToString() + "," + teamsConfig.playersList[2].representativeGenomeList[k].index.ToString() + "," + teamsConfig.playersList[3].representativeGenomeList[m].index.ToString() + "]";
                                            //Debug.Log(text);
                                            //int[] indices = new int[5];
                                            //indices[0] = e;
                                            //indices[1] = i; // teamsConfig.playersList[0].representativeGenomeList[i].index;
                                            //indices[2] = j; // teamsConfig.playersList[1].representativeGenomeList[j].index;
                                            //indices[3] = k; // teamsConfig.playersList[2].representativeGenomeList[k].index;
                                            //indices[4] = m; // teamsConfig.playersList[3].representativeGenomeList[m].index;
                                            List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                            agentGenomesList.Add(teamsConfig.playersList[0].representativeGenomeList[i]);
                                            agentGenomesList.Add(teamsConfig.playersList[1].representativeGenomeList[j]);
                                            agentGenomesList.Add(teamsConfig.playersList[2].representativeGenomeList[k]);
                                            agentGenomesList.Add(teamsConfig.playersList[3].representativeGenomeList[m]);
                                            EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.environmentGenomeList[e], agentGenomesList, 0, maxTimeStepsDefault);
                                            evaluationTicketList.Add(evalTicket);
                                        }
                                    }
                                    else { // 3 Players
                                        //string text = "envIndex: *" + e.ToString() + "*, agentIndices: [" + teamsConfig.playersList[0].representativeGenomeList[i].index.ToString() + "," + teamsConfig.playersList[1].representativeGenomeList[j].index.ToString() + "," + teamsConfig.playersList[2].representativeGenomeList[k].index.ToString() + "]";
                                        //Debug.Log(text);
                                        //int[] indices = new int[4];
                                        //indices[0] = e;
                                        //indices[1] = i; // teamsConfig.playersList[0].representativeGenomeList[i].index;
                                        //indices[2] = j; // teamsConfig.playersList[1].representativeGenomeList[j].index;
                                        //indices[3] = k; // teamsConfig.playersList[2].representativeGenomeList[k].index;
                                        //EvaluationTicket evalTicket = new EvaluationTicket(indices, 0, maxTimeStepsDefault);

                                        List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                        agentGenomesList.Add(teamsConfig.playersList[0].representativeGenomeList[i]);
                                        agentGenomesList.Add(teamsConfig.playersList[1].representativeGenomeList[j]);
                                        agentGenomesList.Add(teamsConfig.playersList[2].representativeGenomeList[k]);
                                        EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.environmentGenomeList[e], agentGenomesList, 0, maxTimeStepsDefault);
                                        evaluationTicketList.Add(evalTicket);
                                    }
                                }
                            }
                            else {  // 2 Players:
                                //string text = "envIndex: *" + e.ToString() + "*, agentIndices: [" + teamsConfig.playersList[0].representativeGenomeList[i].index.ToString() + "," + teamsConfig.playersList[1].representativeGenomeList[j].index.ToString() + "]";
                                //Debug.Log(text);
                                //int[] indices = new int[3];
                                //indices[0] = e;
                                //indices[1] = i; // teamsConfig.playersList[0].representativeGenomeList[i].index;
                                //indices[2] = j; // teamsConfig.playersList[1].representativeGenomeList[j].index;
                                //EvaluationTicket evalTicket = new EvaluationTicket(indices, 0, maxTimeStepsDefault);

                                List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                agentGenomesList.Add(teamsConfig.playersList[0].representativeGenomeList[i]);
                                agentGenomesList.Add(teamsConfig.playersList[1].representativeGenomeList[j]);
                                EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.environmentGenomeList[e], agentGenomesList, 0, maxTimeStepsDefault);
                                evaluationTicketList.Add(evalTicket);
                            }
                        }
                    }
                    else { // 1 Player:
                        //string text = "envIndex: *" + e.ToString() + "*, agentIndex: [" + teamsConfig.playersList[0].representativeGenomeList[i].index.ToString() + "]";
                        //Debug.Log(text);
                        //int[] indices = new int[2];
                        //indices[0] = e;
                        //indices[1] = i; // teamsConfig.playersList[0].representativeGenomeList[i].index;
                        //EvaluationTicket evalTicket = new EvaluationTicket(indices, 0, maxTimeStepsDefault);

                        List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                        agentGenomesList.Add(teamsConfig.playersList[0].representativeGenomeList[i]);
                        //Debug.Log(e.ToString() + ", " + teamsConfig.environmentPopulation.environmentGenomeList[e].index.ToString());
                        EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.environmentGenomeList[e], agentGenomesList, 0, maxTimeStepsDefault);
                        evaluationTicketList.Add(evalTicket);
                    }
                }
            }
            if (numPlayers > 0) {
                // Player 1:
                for (int i = 0; i < teamsConfig.playersList[0].agentGenomeList.Count; i++) {
                    for (int e = 0; e < teamsConfig.environmentPopulation.representativeGenomeList.Count; e++) {
                        if (numPlayers > 1) {
                            for (int j = 0; j < teamsConfig.playersList[1].representativeGenomeList.Count; j++) {
                                if (numPlayers > 2) {
                                    for (int k = 0; k < teamsConfig.playersList[2].representativeGenomeList.Count; k++) {
                                        if (numPlayers > 3) {
                                            // 4 Players:
                                            for (int m = 0; m < teamsConfig.playersList[3].representativeGenomeList.Count; m++) {
                                                //string text = "envIndex: " + teamsConfig.environmentPopulation.representativeGenomeList[e].index.ToString() + ", agentIndex: [*" + i.ToString() + "*," + teamsConfig.playersList[1].representativeGenomeList[j].index.ToString() + "," + teamsConfig.playersList[2].representativeGenomeList[k].index.ToString() + "," + teamsConfig.playersList[3].representativeGenomeList[m].index.ToString() + "]";
                                                //Debug.Log(text);
                                                //int[] indices = new int[5];
                                                //indices[0] = e; // teamsConfig.environmentPopulation.representativeGenomeList[e].index;
                                                //indices[1] = i;
                                                //indices[2] = j; //teamsConfig.playersList[1].representativeGenomeList[j].index;
                                                //indices[3] = k; // teamsConfig.playersList[2].representativeGenomeList[k].index;
                                                //indices[4] = m; // teamsConfig.playersList[3].representativeGenomeList[m].index;
                                                //EvaluationTicket evalTicket = new EvaluationTicket(indices, 1, maxTimeStepsDefault);

                                                List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                                agentGenomesList.Add(teamsConfig.playersList[0].agentGenomeList[i]);
                                                agentGenomesList.Add(teamsConfig.playersList[1].representativeGenomeList[j]);
                                                agentGenomesList.Add(teamsConfig.playersList[2].representativeGenomeList[k]);
                                                agentGenomesList.Add(teamsConfig.playersList[3].representativeGenomeList[m]);
                                                EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.representativeGenomeList[e], agentGenomesList, 1, maxTimeStepsDefault);
                                                evaluationTicketList.Add(evalTicket);
                                            }
                                        }
                                        else {
                                            // 3 Players:
                                            //string text = "envIndex: " + teamsConfig.environmentPopulation.representativeGenomeList[e].index.ToString() + ", agentIndex: [*" + i.ToString() + "*," + teamsConfig.playersList[1].representativeGenomeList[j].index.ToString() + "," + teamsConfig.playersList[2].representativeGenomeList[k].index.ToString() + "]";
                                            //Debug.Log(text);
                                            //int[] indices = new int[4];
                                            //indices[0] = e; // teamsConfig.environmentPopulation.representativeGenomeList[e].index;
                                            //indices[1] = i;
                                            //indices[2] = j; // teamsConfig.playersList[1].representativeGenomeList[j].index;
                                            //indices[3] = k; // teamsConfig.playersList[2].representativeGenomeList[k].index;
                                            //EvaluationTicket evalTicket = new EvaluationTicket(indices, 1, maxTimeStepsDefault);

                                            List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                            agentGenomesList.Add(teamsConfig.playersList[0].agentGenomeList[i]);
                                            agentGenomesList.Add(teamsConfig.playersList[1].representativeGenomeList[j]);
                                            agentGenomesList.Add(teamsConfig.playersList[2].representativeGenomeList[k]);
                                            EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.representativeGenomeList[e], agentGenomesList, 1, maxTimeStepsDefault);
                                            evaluationTicketList.Add(evalTicket);
                                        }
                                    }
                                }
                                else {
                                    // 2 Players:
                                    //string text = "envIndex: " + teamsConfig.environmentPopulation.representativeGenomeList[e].index.ToString() + ", agentIndex: [*" + i.ToString() + "*," + teamsConfig.playersList[1].representativeGenomeList[j].index.ToString() + "]";
                                    //Debug.Log(text);
                                    //int[] indices = new int[3];
                                    //indices[0] = e; // teamsConfig.environmentPopulation.representativeGenomeList[e].index;
                                    //indices[1] = i;
                                    //indices[2] = j; // teamsConfig.playersList[1].representativeGenomeList[j].index;
                                    //EvaluationTicket evalTicket = new EvaluationTicket(indices, 1, maxTimeStepsDefault);

                                    List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                    agentGenomesList.Add(teamsConfig.playersList[0].agentGenomeList[i]);
                                    agentGenomesList.Add(teamsConfig.playersList[1].representativeGenomeList[j]);
                                    EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.representativeGenomeList[e], agentGenomesList, 1, maxTimeStepsDefault);
                                    evaluationTicketList.Add(evalTicket);
                                }
                            }
                        }
                        else {
                            // 1 Player:
                            //string text = "envIndex: " + teamsConfig.environmentPopulation.representativeGenomeList[e].index.ToString() + ", agentIndex: [*" + i.ToString() + "*]";
                            //Debug.Log(text);
                            //int[] indices = new int[2];
                            //indices[0] = e; // teamsConfig.environmentPopulation.representativeGenomeList[e].index;
                            //indices[1] = i;
                            //EvaluationTicket evalTicket = new EvaluationTicket(indices, 1, maxTimeStepsDefault);

                            List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                            agentGenomesList.Add(teamsConfig.playersList[0].agentGenomeList[i]);
                            EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.representativeGenomeList[e], agentGenomesList, 1, maxTimeStepsDefault);
                            evaluationTicketList.Add(evalTicket);
                        }
                    }
                }

                if (numPlayers > 1) {
                    // more than 1 player -- do player 2:
                    // 2 Players:
                    for (int j = 0; j < teamsConfig.playersList[1].agentGenomeList.Count; j++) { // Player 2 focus
                        for (int e = 0; e < teamsConfig.environmentPopulation.representativeGenomeList.Count; e++) {
                            for (int i = 0; i < teamsConfig.playersList[0].representativeGenomeList.Count; i++) {
                                if (numPlayers > 2) {
                                    for (int k = 0; k < teamsConfig.playersList[2].representativeGenomeList.Count; k++) {
                                        if (numPlayers > 3) {
                                            // 4 Players:
                                            for (int m = 0; m < teamsConfig.playersList[3].representativeGenomeList.Count; m++) {
                                                //string text = "envIndex: " + teamsConfig.environmentPopulation.representativeGenomeList[e].index.ToString() + ", agentIndex: [" + teamsConfig.playersList[0].representativeGenomeList[i].index.ToString() + ",*" + j.ToString() + "*," + teamsConfig.playersList[2].representativeGenomeList[k].index.ToString() + "," + teamsConfig.playersList[3].representativeGenomeList[m].index.ToString() + "]";
                                                //Debug.Log(text);
                                                //int[] indices = new int[5];
                                                //indices[0] = e; // teamsConfig.environmentPopulation.representativeGenomeList[e].index;
                                                //indices[1] = i; // teamsConfig.playersList[0].representativeGenomeList[i].index;
                                                //indices[2] = j;
                                                //indices[3] = k; // teamsConfig.playersList[2].representativeGenomeList[k].index;
                                                //indices[4] = m; // teamsConfig.playersList[3].representativeGenomeList[m].index;
                                                //EvaluationTicket evalTicket = new EvaluationTicket(indices, 2, maxTimeStepsDefault);

                                                List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                                agentGenomesList.Add(teamsConfig.playersList[0].representativeGenomeList[i]);
                                                agentGenomesList.Add(teamsConfig.playersList[1].agentGenomeList[j]);
                                                agentGenomesList.Add(teamsConfig.playersList[2].representativeGenomeList[k]);
                                                agentGenomesList.Add(teamsConfig.playersList[3].representativeGenomeList[m]);
                                                EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.representativeGenomeList[e], agentGenomesList, 2, maxTimeStepsDefault);
                                                evaluationTicketList.Add(evalTicket);
                                            }
                                        }
                                        else {
                                            // 3 Players:
                                            //string text = "envIndex: " + teamsConfig.environmentPopulation.representativeGenomeList[e].index.ToString() + ", agentIndex: [" + teamsConfig.playersList[0].representativeGenomeList[i].index.ToString() + ",*" + j.ToString() + "*," + teamsConfig.playersList[2].representativeGenomeList[k].index.ToString() + "]";
                                            //Debug.Log(text);
                                            //int[] indices = new int[4];
                                            //indices[0] = e; // teamsConfig.environmentPopulation.representativeGenomeList[e].index;
                                            //indices[1] = i; // teamsConfig.playersList[0].representativeGenomeList[i].index;
                                            //indices[2] = j;
                                            //indices[3] = k; // teamsConfig.playersList[2].representativeGenomeList[k].index;
                                            //EvaluationTicket evalTicket = new EvaluationTicket(indices, 2, maxTimeStepsDefault);

                                            List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                            agentGenomesList.Add(teamsConfig.playersList[0].representativeGenomeList[i]);
                                            agentGenomesList.Add(teamsConfig.playersList[1].agentGenomeList[j]);
                                            agentGenomesList.Add(teamsConfig.playersList[2].representativeGenomeList[k]);
                                            EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.representativeGenomeList[e], agentGenomesList, 2, maxTimeStepsDefault);
                                            evaluationTicketList.Add(evalTicket);
                                        }
                                    }
                                }
                                else {
                                    // 2 Players:
                                    //string text = "envIndex: " + teamsConfig.environmentPopulation.representativeGenomeList[e].index.ToString() + ", agentIndex: [" + teamsConfig.playersList[0].representativeGenomeList[i].index.ToString() + ",*" + j.ToString() + "*]";
                                    //Debug.Log(text);
                                    //int[] indices = new int[3];
                                    //indices[0] = e; // teamsConfig.environmentPopulation.representativeGenomeList[e].index;
                                    //indices[1] = i; // teamsConfig.playersList[0].representativeGenomeList[i].index;
                                    //indices[2] = j;
                                    //EvaluationTicket evalTicket = new EvaluationTicket(indices, 2, maxTimeStepsDefault);

                                    List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                    agentGenomesList.Add(teamsConfig.playersList[0].representativeGenomeList[i]);
                                    agentGenomesList.Add(teamsConfig.playersList[1].agentGenomeList[j]);
                                    EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.representativeGenomeList[e], agentGenomesList, 2, maxTimeStepsDefault);
                                    evaluationTicketList.Add(evalTicket);
                                }
                            }
                        }
                    }
                    if (numPlayers > 2) {
                        // more than 2 players: do player 3:
                        // 3 Players:
                        for (int k = 0; k < teamsConfig.playersList[2].agentGenomeList.Count; k++) { // Player 2 focus
                            for (int e = 0; e < teamsConfig.environmentPopulation.representativeGenomeList.Count; e++) {
                                for (int i = 0; i < teamsConfig.playersList[0].representativeGenomeList.Count; i++) {
                                    for (int j = 0; j < teamsConfig.playersList[1].representativeGenomeList.Count; j++) {
                                        if (numPlayers > 3) {
                                            // 4 Players:
                                            for (int m = 0; m < teamsConfig.playersList[3].representativeGenomeList.Count; m++) {
                                                //string text = "envIndex: " + teamsConfig.environmentPopulation.representativeGenomeList[e].index.ToString() + ", agentIndex: [" + teamsConfig.playersList[0].representativeGenomeList[i].index.ToString() + "," + teamsConfig.playersList[1].representativeGenomeList[j].index.ToString() + ",*" + k.ToString() + "*," + teamsConfig.playersList[3].representativeGenomeList[m].index.ToString() + "]";
                                                //Debug.Log(text);
                                                //int[] indices = new int[5];
                                                //indices[0] = e; // teamsConfig.environmentPopulation.representativeGenomeList[e].index;
                                                //indices[1] = i; // teamsConfig.playersList[0].representativeGenomeList[i].index;
                                                //indices[2] = j; // teamsConfig.playersList[1].representativeGenomeList[j].index;
                                                //indices[3] = k;
                                                //indices[4] = m; // teamsConfig.playersList[3].representativeGenomeList[m].index;
                                                //EvaluationTicket evalTicket = new EvaluationTicket(indices, 3, maxTimeStepsDefault);

                                                List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                                agentGenomesList.Add(teamsConfig.playersList[0].representativeGenomeList[i]);
                                                agentGenomesList.Add(teamsConfig.playersList[1].representativeGenomeList[j]);
                                                agentGenomesList.Add(teamsConfig.playersList[2].agentGenomeList[k]);
                                                agentGenomesList.Add(teamsConfig.playersList[3].representativeGenomeList[m]);
                                                EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.representativeGenomeList[e], agentGenomesList, 3, maxTimeStepsDefault);
                                                evaluationTicketList.Add(evalTicket);
                                            }
                                        }
                                        else {
                                            // 3 Players:
                                            //string text = "envIndex: " + teamsConfig.environmentPopulation.representativeGenomeList[e].index.ToString() + ", agentIndex: [" + teamsConfig.playersList[0].representativeGenomeList[i].index.ToString() + "," + teamsConfig.playersList[1].representativeGenomeList[j].index.ToString() + ",*" + k.ToString() + "*]";
                                            //Debug.Log(text);
                                            //int[] indices = new int[4];
                                            //indices[0] = e; // teamsConfig.environmentPopulation.representativeGenomeList[e].index;
                                            //indices[1] = i; // teamsConfig.playersList[0].representativeGenomeList[i].index;
                                            //indices[2] = j; // teamsConfig.playersList[1].representativeGenomeList[j].index;
                                            //indices[3] = k;
                                            //EvaluationTicket evalTicket = new EvaluationTicket(indices, 3, maxTimeStepsDefault);

                                            List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                            agentGenomesList.Add(teamsConfig.playersList[0].representativeGenomeList[i]);
                                            agentGenomesList.Add(teamsConfig.playersList[1].representativeGenomeList[j]);
                                            agentGenomesList.Add(teamsConfig.playersList[2].agentGenomeList[k]);
                                            EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.representativeGenomeList[e], agentGenomesList, 3, maxTimeStepsDefault);
                                            evaluationTicketList.Add(evalTicket);
                                        }
                                    }
                                }
                            }
                        }
                        if (numPlayers > 3) {
                            // 4 Players:
                            for (int m = 0; m < teamsConfig.playersList[3].agentGenomeList.Count; m++) { // Player 2 focus
                                for (int e = 0; e < teamsConfig.environmentPopulation.representativeGenomeList.Count; e++) {
                                    for (int i = 0; i < teamsConfig.playersList[0].representativeGenomeList.Count; i++) {
                                        for (int j = 0; j < teamsConfig.playersList[1].representativeGenomeList.Count; j++) {
                                            for (int k = 0; k < teamsConfig.playersList[2].representativeGenomeList.Count; k++) {
                                                // 4 Players:
                                                //string text = "envIndex: " + teamsConfig.environmentPopulation.representativeGenomeList[e].index.ToString() + ", agentIndex: [" + teamsConfig.playersList[0].representativeGenomeList[i].index.ToString() + "," + teamsConfig.playersList[1].representativeGenomeList[j].index.ToString() + "," + teamsConfig.playersList[1].representativeGenomeList[k].index.ToString() + ",*" + m.ToString() + "*]";
                                                //Debug.Log(text);
                                                //int[] indices = new int[5];
                                                //indices[0] = e; // teamsConfig.environmentPopulation.representativeGenomeList[e].index;
                                                //indices[1] = i; // teamsConfig.playersList[0].representativeGenomeList[i].index;
                                                //indices[2] = j; // teamsConfig.playersList[1].representativeGenomeList[j].index;
                                                //indices[3] = k; // teamsConfig.playersList[2].representativeGenomeList[k].index;
                                                //indices[4] = m;
                                                //EvaluationTicket evalTicket = new EvaluationTicket(indices, 4, maxTimeStepsDefault);

                                                List<AgentGenome> agentGenomesList = new List<AgentGenome>();
                                                agentGenomesList.Add(teamsConfig.playersList[0].representativeGenomeList[i]);
                                                agentGenomesList.Add(teamsConfig.playersList[1].representativeGenomeList[j]);
                                                agentGenomesList.Add(teamsConfig.playersList[2].representativeGenomeList[k]);
                                                agentGenomesList.Add(teamsConfig.playersList[3].agentGenomeList[m]);
                                                EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.representativeGenomeList[e], agentGenomesList, 4, maxTimeStepsDefault);
                                                evaluationTicketList.Add(evalTicket);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /*string evalText = "";
        for(int i = 0; i < evaluationTicketList.Count; i++) {
            evalText += "(";
            for (int j = 0; j < evaluationTicketList[i].genomeIndices.Length; j++) {
                evalText += evaluationTicketList[i].genomeIndices[j].ToString() + ",";
            }
            evalText += ")\n";
        }
        Debug.Log(evalText);*/
    }
    public void CreateEvaluationInstances() {
        // Set up Evaluation Instances:

        Vector3 arenaBounds = new Vector3(40f, 40f, 40f); //Challenge.GetChallengeArenaBounds(challengeType); // revisit this method

        evaluationInstancesList = new List<EvaluationInstance>();

        for (int x = 0; x < maxInstancesX; x++) {
            for (int y = 0; y < maxInstancesY; y++) {
                int yRowPosition = y - Mathf.RoundToInt((float)maxInstancesY * 0.5f);
                if (yRowPosition >= 0) {
                    yRowPosition++;
                }
                float yPos = (yRowPosition) * (arenaBounds.y + instanceBufferY);
                for (int z = 0; z < maxInstancesZ; z++) {
                    GameObject evalInstanceGO = new GameObject("EvaluationInstance [" + x.ToString() + "," + z.ToString() + "]");
                    EvaluationInstance evaluationInstance = evalInstanceGO.AddComponent<EvaluationInstance>();
                    //evaluationInstance.particleCurves = particleTrajectories;
                    float xPos = (x + 1) * (arenaBounds.x + instanceBufferX) - ((float)maxInstancesX * 0.5f) * (arenaBounds.x + instanceBufferX);
                    float zPos = (z + 1) * (arenaBounds.z + instanceBufferZ) - ((float)maxInstancesZ * 0.5f) * (arenaBounds.z + instanceBufferZ);
                    evalInstanceGO.transform.position = new Vector3(xPos, yPos, zPos);
                    evaluationInstancesList.Add(evaluationInstance);

                    evaluationInstance.isExhibition = false;
                }
            }
        }
    }
    public void DisableInstances() {
        for (int i = 0; i < evaluationInstancesList.Count; i++) {
            evaluationInstancesList[i].gameObject.SetActive(false);
        }
        exhibitionInstance.gameObject.SetActive(false);
    }
    public void EnableInstances() {
        for (int i = 0; i < evaluationInstancesList.Count; i++) {
            evaluationInstancesList[i].gameObject.SetActive(true);
        }
        exhibitionInstance.gameObject.SetActive(true);
    }
    public void ClearCurrentTraining() {
        ClearEvaluationTickets(); // maybe not needed, done in ResetForNewGeneration()        
        DisableInstances();
    }

    public void ClearEvaluationTickets() {
        if (evaluationTicketList != null) {
            evaluationTicketList.Clear();
        }
    }

    public void Tick(TeamsConfig teamsConfig) {
        //Debug.Log("EvaluationManager.Tick()!");

        TickEvaluations(teamsConfig);
        TickExhibition(teamsConfig);
    }

    public void TickEvaluations(TeamsConfig teamsConfig) {
        // Check training status:
        
        // Auto Parallel Mode!!!!!

        // loop through evalInstances -- if pending, start evaluating with next free evalPair
        // if inProgress, Tick()    
        int maxEvalConstructionsPerFrame = 1;
        int numEvalConstructionsThisFrame = 0;
        for (int i = 0; i < evaluationInstancesList.Count; i++) {
            if (evaluationInstancesList[i].currentEvalTicket == null) {
                if (numEvalConstructionsThisFrame >= maxEvalConstructionsPerFrame) {
                    // ignore
                }
                else {
                    // available for use
                    // Find next available evalPair:
                    int currentEvalPairIndex = GetNextPendingEvalPairIndex();
                    if (currentEvalPairIndex == -1) {
                        // no evals pending
                    }
                    else {
                        //print("evalPair: " + currentEvalPairIndex.ToString() + " (" + evaluationPairsList[currentEvalPairIndex].status.ToString() + "), [" + evaluationPairsList[currentEvalPairIndex].evalPairIndices[0].ToString() + "," + evaluationPairsList[currentEvalPairIndex].evalPairIndices[1].ToString() + "]");
                        evaluationInstancesList[i].SetUpInstance(evaluationTicketList[currentEvalPairIndex], teamsConfig);
                        numEvalConstructionsThisFrame++;
                    }
                }
            }
            else {
                if (evaluationInstancesList[i].currentEvalTicket.status == EvaluationTicket.EvaluationStatus.InProgress) {
                    // Tick 
                    evaluationInstancesList[i].Tick();
                }
                if (evaluationInstancesList[i].currentEvalTicket.status == EvaluationTicket.EvaluationStatus.PendingComplete) {
                    // CleanUp and Process
                    // Instance finished but not fully processed

                    // STORE FITNESS
                    if (evaluationInstancesList[i].currentEvalTicket.focusPopIndex == 0) {  // Environment                            
                        //teamsConfig.environmentPopulation.fitnessManager.rawFitnessScores[evaluationInstancesList[i].currentEvalTicket.genomeIndices[0]] += evaluationInstancesList[i].score;
                        //Debug.Log("evalInstance " + i.ToString() + ", Enviro rawScore=" + evaluationInstancesList[i].fitnessComponentEvaluationGroup.fitCompList[0].rawScore.ToString());
                    }
                    // Players:
                    else if (evaluationInstancesList[i].currentEvalTicket.focusPopIndex == 1) {
                        //teamsConfig.playersList[0].fitnessManager.rawFitnessScores[evaluationInstancesList[i].currentEvalTicket.genomeIndices[1]] += evaluationInstancesList[i].score;
                        //Debug.Log("evalInstance " + i.ToString() + ", Player 1 rawScore=" + evaluationInstancesList[i].fitnessComponentEvaluationGroup.fitCompList[0].rawScore.ToString());
                    }
                    else if (evaluationInstancesList[i].currentEvalTicket.focusPopIndex == 2) {
                        //teamsConfig.playersList[1].fitnessManager.rawFitnessScores[evaluationInstancesList[i].currentEvalTicket.genomeIndices[2]] += evaluationInstancesList[i].score;
                    }
                    else if (evaluationInstancesList[i].currentEvalTicket.focusPopIndex == 3) {
                        //teamsConfig.playersList[2].fitnessManager.rawFitnessScores[evaluationInstancesList[i].currentEvalTicket.genomeIndices[3]] += evaluationInstancesList[i].score;
                    }
                    else { // 4
                        //teamsConfig.playersList[3].fitnessManager.rawFitnessScores[evaluationInstancesList[i].currentEvalTicket.genomeIndices[4]] += evaluationInstancesList[i].score;
                    }
                    evaluationInstancesList[i].DeleteAllGameObjects();
                    evaluationInstancesList[i].currentEvalTicket.status = EvaluationTicket.EvaluationStatus.Complete;
                    evaluationInstancesList[i].currentEvalTicket = null;
                }
            }
        }
        // check for Gen complete:
        bool genComplete = true;
        for (int i = 0; i < evaluationTicketList.Count; i++) {
            if (evaluationTicketList[i].status == EvaluationTicket.EvaluationStatus.Complete) {

            }
            else {
                genComplete = false;
            }
        }
        if (genComplete) {
            // All evals complete!
            //TerrainConstructorGPU.ClearCustomHeightRT();
            Debug.Log("EvaluationManager allEvalsComplete!!!");
            allEvalsComplete = true;
            // NEXTGEN READY
        }
        
    }
    public void TickExhibition(TeamsConfig teamsConfig) {
        if (exhibitionInstance.currentEvalTicket == null) {
            // first-time setup:
            ResetExhibitionInstance(teamsConfig);
        }
        else {
            if (exhibitionInstance.currentEvalTicket.status == EvaluationTicket.EvaluationStatus.Pending) {

            }
            else if (exhibitionInstance.currentEvalTicket.status == EvaluationTicket.EvaluationStatus.InProgress) {
                exhibitionInstance.Tick();
            }
            else if (exhibitionInstance.currentEvalTicket.status == EvaluationTicket.EvaluationStatus.PendingComplete) {
                exhibitionInstance.ClearInstance();
            }
            else {  // Complete
                exhibitionInstance.ClearInstance();
            }
        }
    }

    public void InitializeExhibitionTickets(TeamsConfig teamsConfig) {
        int numPlayers = teamsConfig.playersList.Count;
        
        List<AgentGenome> agentGenomesList = new List<AgentGenome>();
        for (int i = 0; i < numPlayers; i++) {
            agentGenomesList.Add(teamsConfig.playersList[i].representativeGenomeList[0]);
        }
        EvaluationTicket evalTicket = new EvaluationTicket(teamsConfig.environmentPopulation.representativeGenomeList[0], agentGenomesList, 0, maxTimeStepsDefault * 100);
        exhibitionTicketList.Add(evalTicket);        
    }

    public void ResetExhibitionTicket(TeamsConfig teamsConfig) {
        // Set Genomes:
        int numPlayers = teamsConfig.playersList.Count;
        for (int i = 0; i < numPlayers; i++) {
            exhibitionTicketList[0].agentGenomesList[i] = teamsConfig.playersList[i].representativeGenomeList[0];
        }
        exhibitionTicketList[0].environmentGenome = teamsConfig.environmentPopulation.representativeGenomeList[0];
        //exhibitionTicketList[0].environmentGenome.ClearEnvironmentPrefab();
    }
    public void ResetExhibitionInstance(TeamsConfig teamsConfig) {
                
        exhibitionInstance.SetUpInstance(exhibitionTicketList[exhibitionTicketCurrentIndex], teamsConfig);
    }

    private int GetNextPendingEvalPairIndex() {        
        for (int i = 0; i < evaluationTicketList.Count; i++) {
            if (evaluationTicketList[i].status == EvaluationTicket.EvaluationStatus.Pending) {
                return i;
            }
            else {               
            }
        }
        return -1;
    }
    private int GetCurrentEvalTicketIndex() {
        for (int i = 0; i < evaluationTicketList.Count; i++) {
            if (evaluationTicketList[i].status == EvaluationTicket.EvaluationStatus.Complete) {
            }
            else {
                return i;
            }
        }
        return -1;
    }
    public void ResetEvaluationTicketList() {
        for (int i = 0; i < evaluationTicketList.Count; i++) {
            evaluationTicketList[i].status = EvaluationTicket.EvaluationStatus.Pending;
            evaluationTicketList[i].manualSelectStatus = EvaluationTicket.ManualSelectStatus.Pending;
            evaluationTicketList[i].maxTimeSteps = maxTimeStepsDefault;
        }
    }

    public void ResetForNewGeneration(TeamsConfig teamsConfig) {
        
        for (int i = 0; i < evaluationInstancesList.Count; i++) {
            evaluationInstancesList[i].ClearInstance();
        }
        ClearEvaluationTickets();
        CreateDefaultEvaluationTickets(teamsConfig);
        
        allEvalsComplete = false;
        ResetExhibitionTicket(teamsConfig);
        ResetExhibitionInstance(teamsConfig);
        EnableInstances();
        exhibitionTicketCurrentIndex = 0;

    }
}
