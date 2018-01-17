using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterAgentBase {

	
    public virtual void InitializeGenome(ComputeShader brainShader, ComputeShader weightMutationComputeShader, int resolution) {

    }

    public virtual void MutateGenome(FilterAgentConvolve templateAgent, float mutationRate, float mutationSize) {

    }

    public virtual void GenerateTexture(Texture2D sourceTexture) {

        
    }

    public virtual void CopyWeightsFromTemplate(FilterAgentConvolve templateAgent) {

    }

    public virtual RenderTexture GetWeights() {

        RenderTexture RT = new RenderTexture(4, 4, 1);
        return RT;

    }

    public virtual RenderTexture GetResultTexture() {
        RenderTexture RT = new RenderTexture(4, 4, 1);
        return RT;

    }
}
