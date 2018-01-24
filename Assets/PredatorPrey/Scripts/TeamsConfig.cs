using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeamsConfig {

    public EnvironmentPopulation environmentPopulation;
    public List<PlayerPopulation> playersList;

    // default population sizes:
    private int numEnvironmentGenomes = 2;
    private int numAgentGenomesPerPlayer = 16;

    public TeamsConfig(int numPlayers, int numEnvironmentReps, int numPlayerReps) {
        
        //EnvironmentGenome templateEnvironmentGenome = GetDefaultTemplateEnvironmentGenome(challengeType);
        EnvironmentGenome templateEnvironmentGenome = new EnvironmentGenome(-1);
        templateEnvironmentGenome.InitializeAsDefaultGenome();  // Temporary hacky solution

        environmentPopulation = new EnvironmentPopulation(templateEnvironmentGenome, numEnvironmentGenomes, numEnvironmentReps);

        // Players:
        playersList = new List<PlayerPopulation>();
        for (int i = 0; i < numPlayers; i++) {
            // Might have to revisit how to pass agent templates per population...
            //AgentBodyGenomeTemplate templateAgentGenome = GetDefaultTemplateAgentGenome(challengeType);
            // Temporary hack solution:
            BodyGenome templateBodyGenome = new BodyGenome();
            templateBodyGenome.InitializeGenomeAsDefault();

            // List of Agent Genomes
            PlayerPopulation player = new PlayerPopulation(templateBodyGenome, numAgentGenomesPerPlayer, numPlayerReps);

            playersList.Add(player);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
