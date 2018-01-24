using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterAgentConvolve : FilterAgentBase {

    private ComputeShader agentBrainComputeShader;
    private ComputeShader weightMutationComputeShader;

    public RenderTexture[] layerWeightsArrayRT;
    private RenderTexture workingTextureA;  // input/output of each agent(filter) layer during forward pass
    private RenderTexture workingTextureB;  // input/output of each agent(filter) layer during forward pass

    private int numLayers = 12;

    int imageResolution;
    int convolutionSize = 5;

    private float initialWeightsMagnitude = 0.2f;
    private float initialWeightChance = 0.01f;

    public override void InitializeGenome(ComputeShader brainShader, ComputeShader weightMutationComputeShader, int resolution) {

        agentBrainComputeShader = brainShader;
        this.weightMutationComputeShader = weightMutationComputeShader;
        imageResolution = resolution;

        // Initialize Layers:
        layerWeightsArrayRT = new RenderTexture[numLayers];

        for(int i = 0; i < layerWeightsArrayRT.Length; i++) {
            // !!!! MAKE 3D !!!!!
            int numInputFilters = 4;
            if (i + 1 == layerWeightsArrayRT.Length) { // output layer only outputs one channel
                numInputFilters = 4;
            }
            RenderTexture weights = new RenderTexture(convolutionSize, convolutionSize, 16, RenderTextureFormat.ARGBFloat);
            weights.filterMode = FilterMode.Point;
            weights.enableRandomWrite = true;
            weights.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
            weights.volumeDepth = numInputFilters;
            //Debug.Log("DEPTH: " + weights.volumeDepth);
            weights.Create();
            layerWeightsArrayRT[i] = weights;

            // Initial Weights:
            //Texture3D weightsTex = new Texture3D(convolutionSize, convolutionSize, 1, TextureFormat.RGBAFloat, true);
            Color[] weightValues = new Color[convolutionSize * convolutionSize * numInputFilters];
            for (int c = 0; c < weightValues.Length; c++) {   
                float randCheck0 = UnityEngine.Random.Range(0f, 1f);
                float weight0 = 0f;
                float randCheck1 = UnityEngine.Random.Range(0f, 1f);
                float weight1 = 0f;
                float randCheck2 = UnityEngine.Random.Range(0f, 1f);
                float weight2 = 0f;
                float randCheck3 = UnityEngine.Random.Range(0f, 1f);
                float weight3 = 0f;
                if (randCheck0 < initialWeightChance)
                    weight0 = UnityEngine.Random.Range(-initialWeightsMagnitude, initialWeightsMagnitude);
                if (randCheck1 < initialWeightChance)
                    weight1 = UnityEngine.Random.Range(-initialWeightsMagnitude, initialWeightsMagnitude);
                if (randCheck2 < initialWeightChance)
                    weight2 = UnityEngine.Random.Range(-initialWeightsMagnitude, initialWeightsMagnitude);
                if (randCheck3 < initialWeightChance)
                    weight3 = UnityEngine.Random.Range(-initialWeightsMagnitude, initialWeightsMagnitude);

                weightValues[c] = new Color(weight0, weight1, weight2, weight3);
            }
            ComputeBuffer weightsCB = new ComputeBuffer(convolutionSize * convolutionSize * numInputFilters, sizeof(float) * 4);
            weightsCB.SetData(weightValues);
            
            // Blit doesn't work on 3D so I'll use computeShader...
            int kernelInitWeights = agentBrainComputeShader.FindKernel("InitWeights");
            agentBrainComputeShader.SetInt("_ConvWidth", layerWeightsArrayRT[i].width);
            agentBrainComputeShader.SetInt("_ConvHeight", layerWeightsArrayRT[i].height);
            agentBrainComputeShader.SetInt("_ConvDepth", layerWeightsArrayRT[i].volumeDepth);
            agentBrainComputeShader.SetTexture(kernelInitWeights, "weightsRT", weights);
            agentBrainComputeShader.SetBuffer(kernelInitWeights, "weightsCB", weightsCB);            
            agentBrainComputeShader.Dispatch(kernelInitWeights, weights.width, weights.height, weights.volumeDepth);

            //Vector4[] debugArray = new Vector4[weightsCB.count];
            //weightsCB.GetData(debugArray);

            //string debugTxt = "Initialize";
            //for (int d = 0; d < debugArray.Length; d++) {
            //    debugTxt += debugArray[d].ToString();
            //}
            //Debug.Log(debugTxt);

            weightsCB.Release();

            //weightsTex.SetPixels(weightValues);
            //weightsTex.Apply();  // Set
            //Graphics.Blit(weightsTex, weights);  // initialize Weights matrix -- Replaces .Create() function
        }
        
        // Create working textures ( store each layer's neuron activations as well as generated output image )
        workingTextureA = new RenderTexture(resolution, resolution, 1, RenderTextureFormat.ARGBFloat);
        workingTextureA.enableRandomWrite = true;
        workingTextureA.filterMode = FilterMode.Bilinear;
        workingTextureA.autoGenerateMips = false;
        workingTextureA.useMipMap = true;
        workingTextureA.Create();

        workingTextureB = new RenderTexture(resolution, resolution, 1, RenderTextureFormat.ARGBFloat);
        workingTextureB.enableRandomWrite = true;
        workingTextureB.filterMode = FilterMode.Bilinear;
        workingTextureB.autoGenerateMips = false;
        workingTextureB.useMipMap = true;
        workingTextureB.Create();
    }

    public override void MutateGenome(FilterAgentConvolve templateAgent, float mutationRate, float mutationSize) {
        for(int i = 0; i < layerWeightsArrayRT.Length; i++) {
            int kernelMutate = weightMutationComputeShader.FindKernel("CSMain");
            weightMutationComputeShader.SetTexture(kernelMutate, "SourceTex3D", templateAgent.layerWeightsArrayRT[i]);
            weightMutationComputeShader.SetTexture(kernelMutate, "ResultTex3D", layerWeightsArrayRT[i]);

            weightMutationComputeShader.SetFloat("_SeedA", UnityEngine.Random.Range(0f, 100f));
            weightMutationComputeShader.SetFloat("_SeedB", UnityEngine.Random.Range(0f, 100f));
            weightMutationComputeShader.SetFloat("_MutationRate", mutationRate);
            weightMutationComputeShader.SetFloat("_MutationSize", mutationSize);
            weightMutationComputeShader.Dispatch(kernelMutate, layerWeightsArrayRT[i].width, layerWeightsArrayRT[i].height, layerWeightsArrayRT[i].volumeDepth);
            //Debug.Log("Mutate " + i.ToString());
        } 

        /*int kernelMutate = weightMutationComputeShader.FindKernel("CSMain");
        weightMutationComputeShader.SetTexture(kernelMutate, "Source", templateAgent.layerWeightsArrayRT[0]);
        weightMutationComputeShader.SetTexture(kernelMutate, "Result", layerWeightsArrayRT[0]);
        //weightMutationComputeShader.SetInt("_Gen", curGeneration);
        weightMutationComputeShader.SetFloat("_SeedA", UnityEngine.Random.Range(0f, 100f));
        weightMutationComputeShader.SetFloat("_SeedB", UnityEngine.Random.Range(0f, 100f));
        weightMutationComputeShader.SetFloat("_MutationRate", mutationRate);
        weightMutationComputeShader.SetFloat("_MutationSize", mutationSize);
        weightMutationComputeShader.Dispatch(kernelMutate, layerWeightsArrayRT[0].width, layerWeightsArrayRT[0].height, layerWeightsArrayRT[0].volumeDepth);

        int kernelMutate2 = weightMutationComputeShader.FindKernel("CSMain");
        weightMutationComputeShader.SetTexture(kernelMutate2, "Source", templateAgent.layerWeightsArrayRT[2]);
        weightMutationComputeShader.SetTexture(kernelMutate2, "Result", layerWeightsArrayRT[2]);
        //weightMutationComputeShader.SetInt("_Gen", curGeneration);
        weightMutationComputeShader.SetFloat("_SeedA", UnityEngine.Random.Range(0f, 100f));
        weightMutationComputeShader.SetFloat("_SeedB", UnityEngine.Random.Range(0f, 100f));
        weightMutationComputeShader.SetFloat("_MutationRate", mutationRate);
        weightMutationComputeShader.SetFloat("_MutationSize", mutationSize);
        weightMutationComputeShader.Dispatch(kernelMutate2, layerWeightsArrayRT[2].width, layerWeightsArrayRT[2].height, layerWeightsArrayRT[2].volumeDepth);

        int kernelMutate3 = weightMutationComputeShader.FindKernel("CSMain");
        weightMutationComputeShader.SetTexture(kernelMutate3, "Source", templateAgent.layerWeightsArrayRT[3]);
        weightMutationComputeShader.SetTexture(kernelMutate3, "Result", layerWeightsArrayRT[3]);
        //weightMutationComputeShader.SetInt("_Gen", curGeneration);
        weightMutationComputeShader.SetFloat("_SeedA", UnityEngine.Random.Range(0f, 100f));
        weightMutationComputeShader.SetFloat("_SeedB", UnityEngine.Random.Range(0f, 100f));
        weightMutationComputeShader.SetFloat("_MutationRate", mutationRate);
        weightMutationComputeShader.SetFloat("_MutationSize", mutationSize);
        weightMutationComputeShader.Dispatch(kernelMutate3, layerWeightsArrayRT[3].width, layerWeightsArrayRT[3].height, layerWeightsArrayRT[3].volumeDepth);
        */
    }

    public override void CopyWeightsFromTemplate(FilterAgentConvolve templateAgent) {
        
        for(int i = 0; i < layerWeightsArrayRT.Length; i++) {
            int kernelCopy3D = weightMutationComputeShader.FindKernel("CSCopy3D");
            weightMutationComputeShader.SetTexture(kernelCopy3D, "SourceTex3D", templateAgent.layerWeightsArrayRT[i]);
            weightMutationComputeShader.SetTexture(kernelCopy3D, "ResultTex3D", layerWeightsArrayRT[i]);
            // Run compute shader for each output Pixel:
            weightMutationComputeShader.Dispatch(kernelCopy3D, layerWeightsArrayRT[i].width, layerWeightsArrayRT[i].height, layerWeightsArrayRT[i].volumeDepth);
        }
    }

    public override void GenerateTexture(Texture2D sourceTexture) {

        
        
        Graphics.Blit(sourceTexture, workingTextureA);
        bool sourceIsA = true;  // when true, workingTextureA is temp input and workingTextureB is temp output

        // For each NN Layer:
        for(int i = 0; i < layerWeightsArrayRT.Length; i++) {
            

            int kernelFeedForward = agentBrainComputeShader.FindKernel("CSMain");
            agentBrainComputeShader.SetFloat("_Res", imageResolution);
            agentBrainComputeShader.SetInt("_ConvWidth", layerWeightsArrayRT[i].width);
            agentBrainComputeShader.SetInt("_ConvHeight", layerWeightsArrayRT[i].height);
            agentBrainComputeShader.SetInt("_ConvDepth", layerWeightsArrayRT[i].volumeDepth);
            //Debug.Log("_ConvWidth" + layerWeightsArrayRT[i].width.ToString() +  "_ConvHeight" + layerWeightsArrayRT[i].height.ToString() + "_ConvDepth" + layerWeightsArrayRT[i].volumeDepth.ToString());
            if(sourceIsA) {
                agentBrainComputeShader.SetTexture(kernelFeedForward, "InputTexture", workingTextureA);
                agentBrainComputeShader.SetTexture(kernelFeedForward, "OutputTexture", workingTextureB);
            }
            else {
                agentBrainComputeShader.SetTexture(kernelFeedForward, "InputTexture", workingTextureB);
                agentBrainComputeShader.SetTexture(kernelFeedForward, "OutputTexture", workingTextureA);
            }            
            agentBrainComputeShader.SetTexture(kernelFeedForward, "readWeights", layerWeightsArrayRT[i]);
            // Run compute shader for each output Pixel:
            agentBrainComputeShader.Dispatch(kernelFeedForward, imageResolution, imageResolution, 1);

            sourceIsA = !sourceIsA;

            /*ComputeBuffer debugCB = new ComputeBuffer(convolutionSize * convolutionSize, sizeof(float) * 4);

            int kernelDebug = agentBrainComputeShader.FindKernel("ReadWeights");
            agentBrainComputeShader.SetTexture(kernelDebug, "weightsRT", layerWeightsArrayRT[i]);
            agentBrainComputeShader.SetBuffer(kernelDebug, "debugCB", debugCB);
            agentBrainComputeShader.Dispatch(kernelDebug, convolutionSize, convolutionSize, 1);

            Vector4[] debugArray = new Vector4[debugCB.count];
            debugCB.GetData(debugArray);

            string debugTxt = "GenerateTexture";
            for(int d = 0; d < debugArray.Length; d++) {
                debugTxt += debugArray[d].ToString();
            }
            Debug.Log(debugTxt);

            debugCB.Release();*/
        }

        if (sourceIsA) {  // set B to hold actual output
            Graphics.Blit(workingTextureA, workingTextureB);
        }
        else {

        }

        //  Iterate Through output neurons & calculate values using weightsMatrix

        //Graphics.Blit(generatedTexturesRT[i], sourceTexturesRT[i]);
        /*
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
        */

        //Graphics.Blit(generatedTexturesRT[i], sourceTexturesRT[i]);


    }

    public override RenderTexture GetWeights() {

        //RenderTexture RT = new RenderTexture(4, 4, 1);
        return layerWeightsArrayRT[0];

    }

    public override RenderTexture GetResultTexture() {
        workingTextureB.GenerateMips();
        return workingTextureB;
    }
}
