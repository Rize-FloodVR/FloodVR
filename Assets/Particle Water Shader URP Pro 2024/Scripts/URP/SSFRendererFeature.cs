using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SSF_Particle2Fluid_ShaderUtil
{
    [System.Serializable]
    public class SSF_SmoothAlgoParams
    {
        [Tooltip("FilterSize in world space, etc X meters")]
        public float filterSize = 5;
        // Following is not recommend to change, these are algorithm specific parameters
        [HideInInspector]public float maxFilterSize = 100;
        [HideInInspector]public int fixedFilterRadius = 4;
        [HideInInspector] public float thresholdRatio = 10.5f;
        [HideInInspector] public float clampRatio = 1;
    }
    public class SSFRendererFeature:ScriptableRendererFeature
    {
        public Shader depthColorThicknessShader;
        public Shader fluidRenderShader;
        public ComputeShader normalPosShader;
        public ComputeShader smoothDepthShader;
        [Range(1, 4)]
        public int downSampleRatio = 1;
        public SSF_SmoothAlgoParams algorithmParams;
        public Texture2D noiseTexture;

        private Material _depthColorThicknessMaterial;
        private Material _fluidRenderMaterial;
        
        public bool debugVisualize = false;
        public SSF_TextureType debugTextureType = SSF_TextureType.Depth;
        
        
        private SSF_RenderPass _renderPass;
        public override void Create()
        {
            if (depthColorThicknessShader==null)
            {
                depthColorThicknessShader = Resources.Load<Shader>("Shaders/DepthColorThicknessShader");
            }
            if (fluidRenderShader==null)
            {
                fluidRenderShader = Resources.Load<Shader>("Shaders/FluidRender");
            }
            if (normalPosShader==null)
            {
                normalPosShader = Resources.Load<ComputeShader>("Shaders/NormalPos_EYE");
            }
            if (smoothDepthShader==null)
            {
                smoothDepthShader = Resources.Load<ComputeShader>("Shaders/SmoohDepthV2");
            }
            if(noiseTexture==null)
            {
                noiseTexture = Resources.Load<Texture2D>("Textures/noiseTexture");
            }
            _depthColorThicknessMaterial = CoreUtils.CreateEngineMaterial(depthColorThicknessShader);
            _fluidRenderMaterial = CoreUtils.CreateEngineMaterial(fluidRenderShader);

            _renderPass = new SSF_RenderPass();
        }
        

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _renderPass.Setup(
                _depthColorThicknessMaterial, 
                noiseTexture,
                _fluidRenderMaterial,
                normalPosShader, 
                smoothDepthShader,
                downSampleRatio, 
                algorithmParams
                );
            _renderPass.DebugVisualize = debugVisualize;
            _renderPass.DebugTextureType = debugTextureType;
            renderer.EnqueuePass(_renderPass);
        }
    }
}
