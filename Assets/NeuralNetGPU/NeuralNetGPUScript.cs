using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// GPU neural net is possible, however:
// Fixed Topology only  -- evolving topo would be extremely complicated
// might be a bottleneck for update worldState...


    // TO-DO:
    // ======================================================================================
    // Fitness Measurement Shader
    // Weight Mutation Shader
    // Selection & Generation of nextGen Population
    // 2D Grid display of Agent's resultsQuads (Population)
    // Expand Functionality of Neural Networks (hidden layers, input samples, etc.)
    // Working Evolution --> toy case darker == better

    // Figure out better image comparison metrics ! + fitness
    // Figure out how to expand network expressivity  -- Lots of layers? CPPN??
    // incorporate texture coords uv into network inputs

    // Better evolution settings -- + novelty search / species / champions?

    // Clean up and compartmentalize code

public class NeuralNetGPUScript : MonoBehaviour {

    public Button button1;
    private bool displayWeights = false;

    public ComputeShader agentBrainComputeShader;
    public ComputeShader imageMeasurementComputeShader;
    public ComputeShader weightMutationComputeShader;

    public Material displayMat;
    public Material displayMat3D;
    public Shader blitShader;

    private int populationSize = 24;
    private FilterAgentConvolve[] population;
    private FilterAgentConvolve[] tempPopulation;
    
    // Need a pair of these per evaluation instance:
    //private RenderTexture[] weightsRT;
    //private RenderTexture[] swapWeightsRT;
    public Texture2D referenceTexture;  // image/pattern trying to match
    private Texture2D sourceNoiseTexture;  // the input to the agent networks (noise)
    // v v v Eventually replace with 2 RT Arrays that alternate roles to avoid extra Blit() each round...
    //private RenderTexture[] sourceTexturesRT;  // the active modified version of each agent's texture  
    //private RenderTexture[] generatedTexturesRT;  // output of each agent(filter)

    private RenderTexture debugRT;

    //private ComputeBuffer[] inputNeuronsCB;
    //private ComputeBuffer[] outputNeuronsCB;    

    public GameObject displayQuadBestWeights;
    public GameObject displayQuadReference;
    private GameObject[] displayQuadsGO;
    private Material[] displayQuadMaterials;

    //private ComputeBuffer inputMappingCB;  // tells agent's inputNeurons what worldData index they are linked to
    //private ComputeBuffer outputMappingCB; // tells agent's outputNeurons what worldData index they are linked to

    int imageResolution = 64;
    
    int maxTimeSteps = 1;
    int curTimeStep = 0;

    int curGeneration = 0;
    int maxGenerations = 5000;

    bool run = true;

    public float mutationRate = 0.05f;
    public float mutationSize = 1f;

    // Use this for initialization
    void Start () {

        debugRT = new RenderTexture(32, 32, 1, RenderTextureFormat.ARGBFloat);
        debugRT.filterMode = FilterMode.Bilinear;
        debugRT.enableRandomWrite = true;
        debugRT.Create();
        FirstTimeInit();


    }

    private void FirstTimeInit() {



        //weightsRT = new RenderTexture[populationSize];
        //swapWeightsRT = new RenderTexture[populationSize];

        //generatedTexturesRT = new RenderTexture[populationSize];
        //sourceTexturesRT = new RenderTexture[populationSize];

        //worldDataCB = new ComputeBuffer[populationSize];
        //inputNeuronsCB = new ComputeBuffer[populationSize];
        //outputNeuronsCB = new ComputeBuffer[populationSize];

        // Create Population:
        population = new FilterAgentConvolve[populationSize];
        tempPopulation = new FilterAgentConvolve[populationSize];

        GenerateReferenceTexture();
        RegenerateSourceNoiseTexture();

        for (int i = 0; i < populationSize; i++) {
            // create each agent:

            FilterAgentConvolve filterAgent = new FilterAgentConvolve();
            // Create weight layer textures with initial values
            filterAgent.InitializeGenome(agentBrainComputeShader, weightMutationComputeShader, imageResolution);
            population[i] = filterAgent;


            // Won't be used until selection / reproduction, but will reuse this array to avoid creation/destruction of agents every Generation
            FilterAgentConvolve tempFilterAgent = new FilterAgentConvolve();
            tempFilterAgent.InitializeGenome(agentBrainComputeShader, weightMutationComputeShader, imageResolution);
            tempPopulation[i] = tempFilterAgent;



            //ComputeBuffer inNeuronsCB = new ComputeBuffer(numInputsX * numInputsY, sizeof(float));
            //inputNeuronsCB[i] = inNeuronsCB;
            //ComputeBuffer outNeuronsCB = new ComputeBuffer(numOutputs, sizeof(float));
            //outputNeuronsCB[i] = outNeuronsCB;

            /*RenderTexture weights = new RenderTexture(numInputsX, numInputsY, 1, RenderTextureFormat.ARGBFloat);
            weights.filterMode = FilterMode.Point;
            weights.enableRandomWrite = true;
            weights.autoGenerateMips = true;
            weights.useMipMap = true;
            // Initial Weights:
            Texture2D weightsTex = new Texture2D(numInputsX, numInputsY, TextureFormat.RGBAFloat, true);
            for (int x = 0; x < numInputsX; x++) {
                for (int y = 0; y < numInputsY; y++) {
                    float randomWeight0 = UnityEngine.Random.Range(-0.1f, 0.1f);
                    float randomWeight1 = UnityEngine.Random.Range(-0.1f, 0.1f);
                    float randomWeight2 = UnityEngine.Random.Range(-0.1f, 0.1f);
                    //randomWeight = 1.0f / 9.0f;
                    weightsTex.SetPixel(x, y, new Color(randomWeight0, randomWeight1, randomWeight2));
                }
            }
            weightsTex.Apply();
            Graphics.Blit(weightsTex, weights);  // initialize Weights matrix
            weightsRT[i] = weights;

            RenderTexture swapWeights = new RenderTexture(numInputsX, numInputsY, 1, RenderTextureFormat.ARGBFloat);
            swapWeights.filterMode = FilterMode.Point;
            swapWeights.enableRandomWrite = true;
            swapWeights.autoGenerateMips = true;
            swapWeights.useMipMap = true;
            swapWeights.Create();
            swapWeightsRT[i] = swapWeights;

            //generatedTexturesRT = new RenderTexture
            RenderTexture generatedTex = new RenderTexture(imageResX, imageResY, 1, RenderTextureFormat.ARGBFloat);
            generatedTex.enableRandomWrite = true;
            generatedTex.filterMode = FilterMode.Bilinear;
            generatedTex.autoGenerateMips = false;
            generatedTex.useMipMap = true;
            generatedTex.Create();
            generatedTexturesRT[i] = generatedTex;

            RenderTexture sourceTex = new RenderTexture(imageResX, imageResY, 1, RenderTextureFormat.ARGBFloat);
            sourceTex.enableRandomWrite = true;
            sourceTex.filterMode = FilterMode.Bilinear;
            sourceTex.autoGenerateMips = true;
            sourceTex.useMipMap = true;
            //sourceTex.Create();
            Graphics.Blit(sourceNoiseTexture, sourceTex);
            sourceTexturesRT[i] = sourceTex;
            */
        }

        CreateDisplayQuads();
        SetTexturesDisplayQuads();


        //testRT = new RenderTexture(imageResX, imageResY, 1, RenderTextureFormat.ARGBFloat);
        //testRT.enableRandomWrite = true;
        //testRT.filterMode = FilterMode.Point;
        //testRT.Create();
        displayMat.SetTexture("_MainTex", sourceNoiseTexture);

        Material bestAgentMat = new Material(displayMat3D);
        displayQuadBestWeights.GetComponent<MeshRenderer>().material = bestAgentMat;
        bestAgentMat.SetTexture("_MainTex", population[0].GetWeights());

        Material refImgMat = new Material(displayMat);
        displayQuadReference.GetComponent<MeshRenderer>().material = refImgMat;
        refImgMat.SetTexture("_MainTex", referenceTexture);


        //Material blitMat = new Material(blitShader);
        //Graphics.Blit(generatedTexturesRT[0], testRT, blitMat);

        //ComputeBuffer tempCB = new ComputeBuffer(1, sizeof(float));
        //float[] tempArray = new float[1];
        //tempArray[0] = 33.7f;
        //tempCB.SetData(tempArray);

        //int kernelFeedForward = agentBrainComputeShader.FindKernel("CSMain");
        //agentBrainComputeShader.SetTexture(kernelFeedForward, "sourceNoiseTex", sourceNoiseTexture);
        //agentBrainComputeShader.SetBuffer(kernelFeedForward, "inputNeuronsCB", inputNeuronsCB[i]);
        //agentBrainComputeShader.SetBuffer(kernelFeedForward, "outputNeuronsCB", outputNeuronsCB[i]);
        //agentBrainComputeShader.SetTexture(kernelFeedForward, "weightsRT", weightsRT[i]);
        //agentBrainComputeShader.SetBuffer(kernelFeedForward, "tempCB", tempCB);
        //agentBrainComputeShader.SetTexture(kernelFeedForward, "ResultRT", testRT);
        //agentBrainComputeShader.Dispatch(kernelFeedForward, imageResX, imageResY, 1);

        //tempCB.GetData(tempArray);
        //Debug.Log("" + tempArray[0].ToString());
        //displayMat.SetTexture("_MainTex", testRT);



        //inputNeuronsCB = new ComputeBuffer(numInputs, sizeof(float));
        //outputNeuronsCB = new ComputeBuffer(numOutputs, sizeof(float));
        //weightsRT = new RenderTexture(numInputs, numOutputs, 1, RenderTextureFormat.ARGBFloat);
        //worldDataCB = new ComputeBuffer(7, sizeof(float)); // agentPos xy + targetPos xy + bias = 5

        /*inputMappingCB = new ComputeBuffer(numInputs, sizeof(int));
        outputMappingCB = new ComputeBuffer(numOutputs, sizeof(int));

        int[] inputMappingArray = new int[numInputs];
        inputMappingArray[0] = 0;  // index of worldDataBuffer to read from
        inputMappingArray[1] = 1;
        inputMappingArray[2] = 4;
        inputMappingArray[3] = 5;
        inputMappingArray[4] = 6;
        inputMappingCB.SetData(inputMappingArray);

        int[] outputMappingArray = new int[numOutputs];
        outputMappingArray[0] = 2;  // index of worldDataBuffer to write to
        outputMappingArray[1] = 3;
        outputMappingCB.SetData(outputMappingArray);
        */

        /*Texture2D weightsTex = new Texture2D(numInputs, numOutputs, TextureFormat.RGBAFloat, false);
        for(int x = 0; x < numInputs; x++) {
            for(int y = 0; y < numOutputs; y++) {
                float randomWeight = UnityEngine.Random.Range(-1f, 1f);
                weightsTex.SetPixel(x, y, new Color(randomWeight, 0f, 0f));
            }
        }*/
        //weightsTex.Apply();
        //Graphics.Blit(weightsTex, weightsRT);  // initialize Weights matrix
        // weightsRT.filterMode = FilterMode.Point;

        //displayMat.SetTexture("_MainTex", weightsRT);

        // World Data and World <--> Brain Mappings:

        /*float[] worldDataArray = new float[7];
        worldDataArray[0] = 0f;  // agentPosX
        worldDataArray[1] = 0f;  // agentPosY
        worldDataArray[2] = 0f;  // agentThrottleX
        worldDataArray[3] = 0f;  // agentThrottleY
        worldDataArray[4] = 1f;  // targetPosX
        worldDataArray[5] = -1f;  // targetPosY
        worldDataArray[6] = 1f;  // bias
        worldDataArray[7] = 0f;  // timestep
        worldDataArray[8] = 0f; // time of capture
        worldDataCB.SetData(worldDataArray);*/
    }

    // Update is called once per frame
    void Update () {
        if(run) {
            if (curTimeStep < maxTimeSteps) {
                Tick();
                curTimeStep++;
            }
            else {
                // End of current Generation:

                int[] agentIndices = new int[populationSize];
                float[] fitnessScores = new float[populationSize];

                // Calculate histogram of Reference Image and store:
                //
                //Graphics.Blit(referenceTexture, debugRT);
                
                
                ComputeBuffer refHistogram0CB = new ComputeBuffer(16, sizeof(uint));
                ComputeBuffer refHistogram1CB = new ComputeBuffer(16, sizeof(uint));
                ComputeBuffer refHistogramLocal0CB = new ComputeBuffer(64, sizeof(uint));
                ComputeBuffer refHistogramLocal1CB = new ComputeBuffer(64, sizeof(uint));
                int kernelRefClearHistogram = imageMeasurementComputeShader.FindKernel("CSClearHistogramBuffer");
                int kernelRefGenerateHistogram = imageMeasurementComputeShader.FindKernel("CSGenerateHistogram");
                imageMeasurementComputeShader.SetBuffer(kernelRefClearHistogram, "Histogram0CB", refHistogram0CB);
                imageMeasurementComputeShader.SetBuffer(kernelRefClearHistogram, "Histogram1CB", refHistogram1CB);
                imageMeasurementComputeShader.SetBuffer(kernelRefClearHistogram, "HistogramLocal0CB", refHistogramLocal0CB);
                imageMeasurementComputeShader.SetBuffer(kernelRefClearHistogram, "HistogramLocal1CB", refHistogramLocal1CB);

                imageMeasurementComputeShader.SetBuffer(kernelRefGenerateHistogram, "Histogram0CB", refHistogram0CB);
                imageMeasurementComputeShader.SetBuffer(kernelRefGenerateHistogram, "Histogram1CB", refHistogram1CB);
                imageMeasurementComputeShader.SetBuffer(kernelRefGenerateHistogram, "HistogramLocal0CB", refHistogramLocal0CB);
                imageMeasurementComputeShader.SetBuffer(kernelRefGenerateHistogram, "HistogramLocal1CB", refHistogramLocal1CB);
                imageMeasurementComputeShader.SetTexture(kernelRefGenerateHistogram, "Texture", referenceTexture);

                imageMeasurementComputeShader.Dispatch(kernelRefClearHistogram, 16, 1, 1);
                imageMeasurementComputeShader.Dispatch(kernelRefGenerateHistogram, imageResolution / 8, imageResolution / 8, 1);

                uint[] refHistogram0Array = new uint[16];
                refHistogram0CB.GetData(refHistogram0Array);                
                refHistogram0CB.Release();
                uint[] refHistogram1Array = new uint[16];
                refHistogram1CB.GetData(refHistogram1Array);
                refHistogram1CB.Release();
                uint[] refHistogramLocal0Array = new uint[64];
                refHistogramLocal0CB.GetData(refHistogramLocal0Array);
                refHistogramLocal0CB.Release();
                uint[] refHistogramLocal1Array = new uint[64];
                refHistogramLocal1CB.GetData(refHistogramLocal1Array);
                refHistogramLocal1CB.Release();
                //
                /*
                string debugTxt = "refHistogramLocal0Array: ";
                for (int j = 0; j < refHistogramLocal0Array.Length; j++) {
                    debugTxt += j.ToString() + ": " + refHistogramLocal0Array[j].ToString() + ", ";
                }
                Debug.Log(debugTxt);
                //
                */

                // Collect scores:
                for (int i = 0; i < populationSize; i++) {
                    //string debugTxt = "Generation " + curGeneration.ToString() + "\n";
                    //float[] dataArray = new float[worldDataCB[i].count];
                    //worldDataCB[i].GetData(dataArray);
                    //Debug.Log("Instance [" + i.ToString() + "]: " + dataArray[0].ToString() + ", " + dataArray[1].ToString());

                    //Graphics.Blit(generatedTexturesRT[i], debugRT);
                    
                    //generatedTexturesRT[i].GenerateMips();

                    ComputeBuffer histogram0CB = new ComputeBuffer(16, sizeof(uint));
                    ComputeBuffer histogram1CB = new ComputeBuffer(16, sizeof(uint));
                    ComputeBuffer histogramLocal0CB = new ComputeBuffer(64, sizeof(uint));
                    ComputeBuffer histogramLocal1CB = new ComputeBuffer(64, sizeof(uint));
                    int kernelClearHistogram = imageMeasurementComputeShader.FindKernel("CSClearHistogramBuffer");
                    int kernelGenerateHistogram = imageMeasurementComputeShader.FindKernel("CSGenerateHistogram");
                    imageMeasurementComputeShader.SetBuffer(kernelClearHistogram, "Histogram0CB", histogram0CB);
                    imageMeasurementComputeShader.SetBuffer(kernelClearHistogram, "Histogram1CB", histogram1CB);
                    imageMeasurementComputeShader.SetBuffer(kernelClearHistogram, "HistogramLocal0CB", histogramLocal0CB);
                    imageMeasurementComputeShader.SetBuffer(kernelClearHistogram, "HistogramLocal1CB", histogramLocal1CB);

                    imageMeasurementComputeShader.SetBuffer(kernelGenerateHistogram, "Histogram0CB", histogram0CB);
                    imageMeasurementComputeShader.SetBuffer(kernelGenerateHistogram, "Histogram1CB", histogram1CB);
                    imageMeasurementComputeShader.SetBuffer(kernelGenerateHistogram, "HistogramLocal0CB", histogramLocal0CB);
                    imageMeasurementComputeShader.SetBuffer(kernelGenerateHistogram, "HistogramLocal1CB", histogramLocal1CB);
                    imageMeasurementComputeShader.SetTexture(kernelGenerateHistogram, "Texture", population[i].GetResultTexture());

                    imageMeasurementComputeShader.Dispatch(kernelClearHistogram, 16, 1, 1);
                    imageMeasurementComputeShader.Dispatch(kernelGenerateHistogram, imageResolution / 8, imageResolution / 8, 1);

                    uint[] histogram0Array = new uint[16];
                    histogram0CB.GetData(histogram0Array);
                    uint[] histogram1Array = new uint[16];
                    histogram1CB.GetData(histogram1Array);
                    uint[] histogramLocal0Array = new uint[64];
                    histogramLocal0CB.GetData(histogramLocal0Array);
                    uint[] histogramLocal1Array = new uint[64];
                    histogramLocal1CB.GetData(histogramLocal1Array);

                    //debugTxt = "HistogramArray: ";
                    //for (int j = 0; j < histogram0Array.Length; j++) {
                    //    debugTxt += j.ToString() + ": " + histogram0Array[j].ToString() + ", ";
                    //}
                    //Debug.Log(debugTxt);

                    // Compare Histogram with reference img histogram to calculate score
                    float sqrDist = 0f;
                    for(int h = 0; h < histogram0Array.Length; h++) {
                        float dist0 = ((float)histogram0Array[h] - (float)refHistogram0Array[h]) / 4096f;
                        float dist1 = ((float)histogram1Array[h] - (float)refHistogram1Array[h]) / 4096f;
                        float dist2 = ((float)histogramLocal0Array[h] - (float)refHistogramLocal0Array[h]) / 4096;
                        float dist3 = ((float)histogramLocal1Array[h] - (float)refHistogramLocal1Array[h]) / 4096;

                        sqrDist += dist0 * dist0 + dist1 * dist1 + dist2 * dist2 + dist3 * dist3;
                    }
                    //Debug.Log(sqrDist.ToString());
                    

                    agentIndices[i] = i;
                    fitnessScores[i] = sqrDist; // scoreArray[0];  UnityEngine.Random.Range(0f, 1f); //

                    histogram0CB.Release();
                    histogram1CB.Release();
                    histogramLocal0CB.Release();
                    histogramLocal1CB.Release();
                }

                // Rank Agents: (brute force algo)
                for (int i = 0; i < populationSize - 1; i++) {
                    for (int j = 0; j < populationSize - 1; j++) {
                        float swapFitA = fitnessScores[j];
                        float swapFitB = fitnessScores[j + 1];
                        int swapIdA = agentIndices[j];
                        int swapIdB = agentIndices[j + 1];

                        if (swapFitA > swapFitB) {  // lower is better
                            fitnessScores[j] = swapFitB;
                            fitnessScores[j + 1] = swapFitA;
                            agentIndices[j] = swapIdB;
                            agentIndices[j + 1] = swapIdA;
                        }
                    }
                }
                string fitnessRankText = "";
                for (int i = 0; i < fitnessScores.Length; i++) {
                    fitnessRankText += "[" + agentIndices[i].ToString() + "]: " + fitnessScores[i].ToString() + "\n";
                }
                Debug.Log(fitnessRankText);

                // Store currentGen Weights in swapArray:
                for(int i = 0; i < populationSize; i++) {
                    //Graphics.Blit(weightsRT[i], swapWeightsRT[i]);

                    // copy pop into tempPop:
                    tempPopulation[i].CopyWeightsFromTemplate(population[i]);
                }

                
                // Crossover:
                for(int i = 0; i < populationSize; i++) {
                    if(i < (populationSize / 8)) {
                        //Graphics.Blit(swapWeightsRT[agentIndices[i]], weightsRT[i]);
                        population[i].CopyWeightsFromTemplate(tempPopulation[agentIndices[i]]);
                    }
                    else {
                        // Mutate:
                        population[i].MutateGenome(tempPopulation[agentIndices[i % 8]], mutationRate, mutationSize);

                        /*int kernelMutate = weightMutationComputeShader.FindKernel("CSMain");
                        weightMutationComputeShader.SetTexture(kernelMutate, "Source", swapWeightsRT[agentIndices[i % 8]]);
                        weightMutationComputeShader.SetTexture(kernelMutate, "Result", weightsRT[i]);
                        weightMutationComputeShader.SetInt("_Gen", curGeneration);
                        weightMutationComputeShader.SetFloat("_SeedA", UnityEngine.Random.Range(0f, 100f));
                        weightMutationComputeShader.SetFloat("_SeedB", UnityEngine.Random.Range(0f, 100f));
                        weightMutationComputeShader.SetFloat("_MutationRate", mutationRate);
                        weightMutationComputeShader.SetFloat("_MutationSize", mutationSize);
                        weightMutationComputeShader.Dispatch(kernelMutate, numInputsX, numInputsY, 1);
                        */
                    }
                }
                //Graphics.Blit(generatedTexturesRT[0], debugRT);
                //displayMat.SetTexture("_MainTex", debugRT);
                

                curGeneration++;

                if(curGeneration >= maxGenerations) {
                    run = false;
                }
                else {
                    // RESET:
                    ResetForNextGeneration();

                    /*curTimeStep = 0;
                    for (int j = 0; j < populationSize; j++) {                        
                        // Initialize WorldStateData:
                        // 4) Init WorldState:
                        int kernelInitWorldState = agentBrainComputeShader.FindKernel("CSInitWorldState");
                        agentBrainComputeShader.SetBuffer(kernelInitWorldState, "worldDataCB", worldDataCB[j]);
                        agentBrainComputeShader.Dispatch(kernelInitWorldState, 1, 1, 1);                        

                    }*/
                }
            }
        }		
	}

    private void ResetForNextGeneration() {
        curTimeStep = 0;
        RegenerateSourceNoiseTexture();
        for(int i = 0; i < populationSize; i++) {
            //Graphics.Blit(sourceNoiseTexture, sourceTexturesRT[i]);
        }
    }

    private void Tick() {
        Debug.Log("Tick!");

        for(int i = 0; i < populationSize; i++) {

            // Encapsulate this in a Function ( RT --> RT )
            population[i].GenerateTexture(sourceNoiseTexture);
            
            /*
            //  Iterate Through output neurons & calculate values using weightsMatrix
            int kernelFeedForward = agentBrainComputeShader.FindKernel("CSMain");
            agentBrainComputeShader.SetFloat("_Res", imageResX);
            agentBrainComputeShader.SetInt("_Layer", 0);
            agentBrainComputeShader.SetTexture(kernelFeedForward, "sourceNoiseTexture", sourceTexturesRT[i]);
            agentBrainComputeShader.SetTexture(kernelFeedForward, "weightsRT", weightsRT[i]);
            agentBrainComputeShader.SetTexture(kernelFeedForward, "ResultRT", generatedTexturesRT[i]);
            agentBrainComputeShader.Dispatch(kernelFeedForward, imageResX, imageResY, 1);

            //Graphics.Blit(generatedTexturesRT[i], sourceTexturesRT[i]);

            agentBrainComputeShader.SetInt("_Layer", 1);
            agentBrainComputeShader.SetTexture(kernelFeedForward, "sourceNoiseTexture", generatedTexturesRT[i]);
            agentBrainComputeShader.SetTexture(kernelFeedForward, "weightsRT", weightsRT[i]);
            agentBrainComputeShader.SetTexture(kernelFeedForward, "ResultRT", sourceTexturesRT[i]);
            agentBrainComputeShader.Dispatch(kernelFeedForward, imageResX, imageResY, 1);

            agentBrainComputeShader.SetInt("_Layer", 2);
            agentBrainComputeShader.SetTexture(kernelFeedForward, "sourceNoiseTexture", sourceTexturesRT[i]);
            agentBrainComputeShader.SetTexture(kernelFeedForward, "weightsRT", weightsRT[i]);
            agentBrainComputeShader.SetTexture(kernelFeedForward, "ResultRT", generatedTexturesRT[i]);
            agentBrainComputeShader.Dispatch(kernelFeedForward, imageResX, imageResY, 1);

            Graphics.Blit(generatedTexturesRT[i], sourceTexturesRT[i]);
            */
        }
        
    }

    private void CreateAddressSourceNoiseTexture() {
        sourceNoiseTexture = new Texture2D(imageResolution, imageResolution, TextureFormat.RGBAFloat, true);
    }
    private void RegenerateSourceNoiseTexture() {
        if(sourceNoiseTexture == null) {
            CreateAddressSourceNoiseTexture();
        }
        else {
            sourceNoiseTexture.Resize(imageResolution, imageResolution);
        }
        sourceNoiseTexture.filterMode = FilterMode.Point;
        for (int x = 0; x < imageResolution; x++) {
            for (int y = 0; y < imageResolution; y++) {
                //float val = UnityEngine.Random.Range(0f, 1f);
                Color newColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
                sourceNoiseTexture.SetPixel(x, y, newColor);
            }
        }
        sourceNoiseTexture.Apply();
    }
    private void GenerateReferenceTexture() {
        if (referenceTexture == null) {
            referenceTexture = new Texture2D(imageResolution, imageResolution, TextureFormat.RGBAFloat, true);
            referenceTexture.Resize(imageResolution, imageResolution);
            for (int x = 0; x < imageResolution; x++) {
                for (int y = 0; y < imageResolution; y++) {
                    Color newColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
                    referenceTexture.SetPixel(x, y, newColor);
                }
            }
            referenceTexture.Apply();
        }
        else {
            //referenceTexture.Resize(imageResX, imageResY);
            Debug.Log(referenceTexture.mipmapCount.ToString());
        }
    }

    private void CreateDisplayQuads() {
        displayQuadsGO = new GameObject[populationSize];
        displayQuadMaterials = new Material[populationSize];

        int numRows = Mathf.CeilToInt(Mathf.Sqrt(populationSize));

        for (int i = 0; i < populationSize; i++) {
            int row = Mathf.FloorToInt((float)i / numRows);
            int column = i % numRows;

            GameObject displayQuadGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
            float xPos = (float)column * 1.1f;
            float yPos = (float)row * -1.1f;

            displayQuadGO.transform.position = new Vector3(xPos, yPos, 0f);
            displayQuadsGO[i] = displayQuadGO;

            Material dispMat = new Material(displayMat);
            displayQuadMaterials[i] = dispMat;
            displayQuadGO.GetComponent<MeshRenderer>().material = dispMat;
        }
    }
    private void SetTexturesDisplayQuads() {
        if(displayWeights) {
            for (int i = 0; i < populationSize; i++) {
                Material dispMat3D = new Material(displayMat3D);
                displayQuadMaterials[i] = dispMat3D;
                displayQuadsGO[i].GetComponent<MeshRenderer>().material = dispMat3D;
                displayQuadMaterials[i].SetTexture("_MainTex", population[i].GetWeights());
                //displayQuadMaterials[i].SetTexture("_MainTex", referenceTexture);
            }
        }
        else {
            for (int i = 0; i < populationSize; i++) {
                Material dispMat = new Material(displayMat);
                displayQuadMaterials[i] = dispMat;
                displayQuadsGO[i].GetComponent<MeshRenderer>().material = dispMat;
                displayQuadMaterials[i].SetTexture("_MainTex", population[i].GetResultTexture());
                //displayQuadMaterials[i].SetTexture("_MainTex", referenceTexture);
            }
        }        
    }
    
    public void ClickButton1() {
        displayWeights = !displayWeights;
        SetTexturesDisplayQuads();
    }

    private void OnDestroy() {
        
        /*if (inputMappingCB != null) {
            inputMappingCB.Dispose();
        }
        if (outputMappingCB != null) {
            outputMappingCB.Dispose();
        }

        if(inputNeuronsCB != null) {
            for(int i = 0; i < inputNeuronsCB.Length; i++) {
                inputNeuronsCB[i].Dispose();
            }
        }
        if (outputNeuronsCB != null) {
            for (int i = 0; i < outputNeuronsCB.Length; i++) {
                outputNeuronsCB[i].Dispose();
            }
        }
        */
        /*if (worldDataCB != null) {
            for (int i = 0; i < worldDataCB.Length; i++) {
                worldDataCB[i].Dispose();
            }
        }*/
    }
}
