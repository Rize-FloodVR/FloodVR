using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace SSF_Particle2Fluid_ShaderUtil
{
    public class SSF_RenderPass : ScriptableRenderPass
    {
        private static readonly int ParticleBufferID = Shader.PropertyToID("ParticleBuffer");
        private static readonly int QuadVertexBufferID = Shader.PropertyToID("quadVertexBuffer");
        private static readonly int ModelMatrixID = Shader.PropertyToID("modelmatrix");
        private static readonly int ThicknessAmplifierID = Shader.PropertyToID("_thicknessAmplifier");
        private static readonly int NoiseAmplifierID = Shader.PropertyToID("_noiseAmplifier");
        private static readonly int NoiseTexID = Shader.PropertyToID("_noiseTex");
        private  Material _depthColorThicknessMaterial;
        private Material _fluidRenderMaterial;
        private  ComputeShader _normalPosShader;
        private  ComputeShader _smoothDepthShader;

        private  GraphicsBuffer _quadVerticesBuffer;
        private int _downSampleRatio;
        private SSF_SmoothAlgoParams _smoothAlgoParams;
        private Texture2D _noiseTexture;

        public bool DebugVisualize = false;
        public SSF_TextureType DebugTextureType = SSF_TextureType.Depth;
        
        
        
        static readonly MaterialPropertyBlock SPropertyBlock = new();
        private static readonly int DataSourceIndexID = Shader.PropertyToID("_dataSourceIndex");

        public void Setup(Material depthColorMat, 
            Texture2D noiseTexture,
            Material fluidRenderMaterial, 
            ComputeShader normalPos, 
            ComputeShader smoothDepth, 
            int downSampleRatio,
            SSF_SmoothAlgoParams smoothAlgoParams
            )
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
            _depthColorThicknessMaterial = depthColorMat;
            _noiseTexture = noiseTexture;
            _fluidRenderMaterial = fluidRenderMaterial;
            _normalPosShader = normalPos;
            _smoothDepthShader = smoothDepth;
            _quadVerticesBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 6,16);
            _quadVerticesBuffer.SetData(new[]
            {
                new Vector4(-0.5f, 0.5f),
                new Vector4(0.5f, 0.5f),
                new Vector4(0.5f, -0.5f),
                new Vector4(0.5f, -0.5f),
                new Vector4(-0.5f, -0.5f),
                new Vector4(-0.5f, 0.5f),
            }); 
            _downSampleRatio = downSampleRatio;
            _smoothAlgoParams = smoothAlgoParams;
            
            ConfigureInput(ScriptableRenderPassInput.Depth);
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext)
        {

            UniversalCameraData cameraData = frameContext.Get<UniversalCameraData>();
            
            BufferHandle quadVertexBufferHandle = renderGraph.ImportBuffer(_quadVerticesBuffer);
            int textureWidth = cameraData.scaledWidth/_downSampleRatio;
            int textureHeight = cameraData.scaledHeight/_downSampleRatio;
            
            // create texture descriptors
            var 
            depthTextureDescriptor = new RenderTextureDescriptor(textureWidth,textureHeight, RenderTextureFormat.RFloat, 0)
            {
                enableRandomWrite = true
            };
            var depthTextureTempDescriptor = new RenderTextureDescriptor(textureWidth,textureHeight, RenderTextureFormat.Depth, 16);
            var colorTextureDescriptor = new RenderTextureDescriptor(textureWidth,textureHeight, RenderTextureFormat.ARGB32, 0);
            var thicknessTextureDescriptor = new RenderTextureDescriptor(textureWidth,textureHeight, RenderTextureFormat.RFloat, 0);
            var particleDataTextureDescriptor = new RenderTextureDescriptor(textureWidth,textureHeight, RenderTextureFormat.ARGB32, 0);
            var smoothedDepthTextureDescriptor = new RenderTextureDescriptor(textureWidth,textureHeight, RenderTextureFormat.RFloat, 0)
                {
                    enableRandomWrite = true
                };
            var eyeSpaceNormalTextureDescriptor = new RenderTextureDescriptor(textureWidth,textureHeight, RenderTextureFormat.ARGB32, 0)
                {
                    enableRandomWrite = true
                };
            // create texture handles 
            TextureHandle depthTextureHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, depthTextureDescriptor, "SSF_DepthTexture",true);
            TextureHandle depthTextureHandleTemp = UniversalRenderer.CreateRenderGraphTexture(renderGraph, depthTextureTempDescriptor, "SSF_DepthTextureTemp",true);
            TextureHandle colorTextureHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, colorTextureDescriptor, "SSF_ColorTexture",true);
            TextureHandle thicknessTextureHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, thicknessTextureDescriptor, "SSF_ThicknessTexture",true);
            TextureHandle smoothedDepthTextureHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, smoothedDepthTextureDescriptor, "SSF_SmoothedDepthTexture",true);
            TextureHandle eyeSpaceNormalTextureHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, eyeSpaceNormalTextureDescriptor, "SSF_EyeSpaceNormalTexture",true);
            
             
            TextureHandle particleDataTextureHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, particleDataTextureDescriptor, "SSF_ParticleData",true);

            
            var debugTextureDescriptor = new RenderTextureDescriptor(512,512, RenderTextureFormat.ARGB32, 0);
            TextureHandle debugTextureHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, debugTextureDescriptor, "SSF_DebugTexture",true);
            
            UniversalResourceData resourceData = frameContext.Get<UniversalResourceData>();

            
            var cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(cameraData.camera);

            if (SSF_Manager.Instance==null || SSF_Manager.Instance.particleSources==null|| SSF_Manager.Instance.particleSources.Count<=0)
            {
                return;
            }
            var validSources = SSF_Manager.Instance.particleSources.FindAll(source =>
                source.isActiveAndEnabled && GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, source.GetBounds()));
            if (validSources.Count <= 0)
            {
                return;
            }
            bool isFirstToRender = true;
            
            for(int i =0 ;i<validSources.Count;i++)
            {
                var source = validSources[i];
                // set material params
                var particleBuffer = source.GetParticleBuffer();
                // draw color and depth
                DrawSpheresFromQuad(renderGraph,
                    new []{colorTextureHandle,depthTextureHandle,particleDataTextureHandle},depthTextureHandleTemp, new DrawSpheresFromQuadPassData
                {
                    ThicknessAmplifierValue = source.thickness,
                    ModelMatrixValue = source.GetModelMatrix(),
                    DataSourceIndex = i,
                    QuadVertexBufferHandle = quadVertexBufferHandle,
                    NoiseAmplifierValue = source.noise,
                    ParticleBuffer = particleBuffer,
                    ParticleCount = source.GetParticleNum(),
                    ShaderPassIndex = 0,
                    ShouldClear = isFirstToRender,
                },"SSF_DrawColorDepthNoise");
                DrawSpheresFromQuad(renderGraph,new []{thicknessTextureHandle},depthTextureHandleTemp, new DrawSpheresFromQuadPassData
                {
                    ThicknessAmplifierValue = source.thickness,
                    ModelMatrixValue = source.GetModelMatrix(),
                    DataSourceIndex = i,
                    QuadVertexBufferHandle = quadVertexBufferHandle,
                    NoiseAmplifierValue = source.noise,
                    ParticleBuffer = particleBuffer,
                    ParticleCount = source.GetParticleNum(),
                    ShaderPassIndex = 1,
                    ShouldClear = isFirstToRender,
                },"SSF_DrawThickness");
                if (isFirstToRender)
                {
                    isFirstToRender = false;
                }
                
            }
            
            SmoothDepthV2(
                renderGraph,
                new SmoothDepthPassDataV2
                {
                    Cs = _smoothDepthShader,
                    DepthTexInput = depthTextureHandle,
                    DepthTexOutput = smoothedDepthTextureHandle,
                    ParticleDataTexInput = particleDataTextureHandle,
                    FilterSize = _smoothAlgoParams.filterSize,
                    VerticalFov = Mathf.Deg2Rad*cameraData.camera.fieldOfView,
                    TextureWidth = textureWidth,
                    TextureHeight = textureHeight,
                }
            );
            CalculateEyeSpaceNormal(renderGraph,new CalculateEyeSpacePosAndNormalPassData
            {
                NormalPosShader = _normalPosShader,
                SmoothedDepth = smoothedDepthTextureHandle,
                EyeSpaceNormal = eyeSpaceNormalTextureHandle,
                TextureWidth = textureWidth,
                TextureHeight = textureHeight,
                InvProjMatrix = cameraData.GetProjectionMatrix().inverse,
                Far = cameraData.camera.farClipPlane,
                Near = cameraData.camera.nearClipPlane
            });
            
            
            // render to screen 
            DrawToScreen(renderGraph,resourceData,new DrawToScreenPassData
            {
                Material = _fluidRenderMaterial,
                ColorTex = colorTextureHandle,
                EyeDepthTex = smoothedDepthTextureHandle,
                EyeNormalTex = eyeSpaceNormalTextureHandle,
                ParticleDataTex = particleDataTextureHandle,
                ThicknessTex = thicknessTextureHandle,
                RenderParamsBuffer = SSF_Manager.Instance.RenderParamsBuffer
            }
            );
                
                
#if UNITY_EDITOR
            if(DebugVisualize)
            {
                switch (DebugTextureType)
                {
                    case SSF_TextureType.Depth:
                        renderGraph.AddBlitPass(depthTextureHandle,debugTextureHandle,Vector2.one,Vector2.zero,passName:"SSF_DebugDepthTexture");
                        break;
                    case SSF_TextureType.SmoothedDepth:
                        renderGraph.AddBlitPass(smoothedDepthTextureHandle,debugTextureHandle,Vector2.one,Vector2.zero,passName:"SSF_DebugSmoothedDepthTexture");
                        break;
                    case SSF_TextureType.Color:
                        renderGraph.AddBlitPass(colorTextureHandle,debugTextureHandle,Vector2.one,Vector2.zero,passName:"SSF_DebugColorTexture");
                        break;
                    case SSF_TextureType.Thickness:
                        renderGraph.AddBlitPass(thicknessTextureHandle,debugTextureHandle,Vector2.one,Vector2.zero,passName:"SSF_DebugThicknessTexture");
                        break;
                    case SSF_TextureType.Normal:
                        renderGraph.AddBlitPass(eyeSpaceNormalTextureHandle,debugTextureHandle,Vector2.one,Vector2.zero,passName:"SSF_DebugNormalTexture");
                        break;
                    case SSF_TextureType.Others:
                        renderGraph.AddBlitPass(particleDataTextureHandle,debugTextureHandle,Vector2.one,Vector2.zero,passName:"SSF_DebugNoiseTexture");
                        break;
                }
                renderGraph.AddBlitPass(debugTextureHandle,resourceData.activeColorTexture,Vector2.one,Vector2.zero,passName:"SSF_DebugBlit");
            }
#endif
                
        }
        private class DrawToScreenPassData
        {
            public Material Material;
            public TextureHandle ColorTex;
            public TextureHandle EyeDepthTex;
            public TextureHandle EyeNormalTex;
            public TextureHandle ParticleDataTex;
            public TextureHandle ThicknessTex;
            
            public TextureHandle SourceColorTex;
            public TextureHandle DestinationColorTex;
            public GraphicsBuffer RenderParamsBuffer;

        }
        private void DrawToScreen(RenderGraph renderGraph,
            UniversalResourceData resourceData,
            DrawToScreenPassData dataInput)
        {
            
            
            var source = resourceData.activeColorTexture;
            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = $"CameraColor-SSF";
            destinationDesc.clearBuffer = false;
            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);
            
            
            RenderGraphUtils.BlitMaterialParameters para = new(source, destination, dataInput.Material, 0);
            renderGraph.AddBlitPass(para, passName: "SSF_DrawToScreenBlitTexture");
            using (var builder = renderGraph.AddRasterRenderPass("SSF_DrawToScreen", out DrawToScreenPassData passData))
            {
                passData.Material = dataInput.Material;
                passData.ColorTex = dataInput.ColorTex;
                passData.EyeDepthTex = dataInput.EyeDepthTex;
                passData.EyeNormalTex = dataInput.EyeNormalTex;
                passData.ParticleDataTex = dataInput.ParticleDataTex;
                passData.ThicknessTex = dataInput.ThicknessTex;
                passData.SourceColorTex = source;
                passData.DestinationColorTex = destination;
                passData.RenderParamsBuffer = dataInput.RenderParamsBuffer;
                
                
                
                builder.UseTexture(passData.ColorTex);
                builder.UseTexture(passData.EyeDepthTex);
                builder.UseTexture(passData.EyeNormalTex);
                builder.UseTexture(passData.ParticleDataTex);
                builder.UseTexture(passData.ThicknessTex);
                
                if(passData.SourceColorTex.IsValid())
                {
                    builder.UseTexture(passData.SourceColorTex);
                }
                
                builder.SetRenderAttachment(passData.DestinationColorTex, 0);
                
                
                builder.SetRenderFunc((DrawToScreenPassData data, RasterGraphContext context) =>
                {
                    SPropertyBlock.SetTexture("_ColorTex", data.ColorTex);
                    SPropertyBlock.SetTexture("_EyeDepthTex", data.EyeDepthTex);
                    SPropertyBlock.SetTexture("_EyeNormalTex", data.EyeNormalTex);
                    SPropertyBlock.SetTexture("_ParticleDataTex", data.ParticleDataTex);
                    SPropertyBlock.SetTexture("_ThicknessTex", data.ThicknessTex);
                    SPropertyBlock.SetTexture("_BlitTexture", data.SourceColorTex);
                    SPropertyBlock.SetVector("_BlitScaleBias",new Vector4(1,1,0,0));
                    SPropertyBlock.SetTexture("unity_SpecCube0", ReflectionProbe.defaultTexture);
                    SPropertyBlock.SetVector("unity_SpecCube0_HDR", ReflectionProbe.defaultTextureHDRDecodeValues);
                    SPropertyBlock.SetBuffer("_RenderParams", data.RenderParamsBuffer);
                    // data.Material.SetTexture("_CopiedCameraDepthTex", data.CopiedCameraDepthTex);
                    context.cmd.DrawProcedural(Matrix4x4.identity, data.Material, 0, MeshTopology.Triangles, 3,1,SPropertyBlock);
                    SPropertyBlock.Clear();
                     
                });
            }
            
            resourceData.cameraColor = destination;
        }
        
        private class DrawSpheresFromQuadPassData
        {
            public BufferHandle QuadVertexBufferHandle;
            public GraphicsBuffer ParticleBuffer;
            public float ThicknessAmplifierValue,NoiseAmplifierValue;
            public Matrix4x4 ModelMatrixValue;
            public int ShaderPassIndex;
            public int ParticleCount;
            public int DataSourceIndex;
            public bool ShouldClear;

        }

        private void DrawSpheresFromQuad(RenderGraph renderGraph,
            TextureHandle[] attachments,
            TextureHandle depthTextureHandle,
            DrawSpheresFromQuadPassData dataInput,
            string drawPassName)
        {
            using (var builder =
                   renderGraph.AddRasterRenderPass(drawPassName, out DrawSpheresFromQuadPassData passData))
            {
                passData.ShaderPassIndex = dataInput.ShaderPassIndex;
                passData.ModelMatrixValue = dataInput.ModelMatrixValue;
                passData.ThicknessAmplifierValue = dataInput.ThicknessAmplifierValue;
                passData.NoiseAmplifierValue = dataInput.NoiseAmplifierValue;
                passData.QuadVertexBufferHandle = dataInput.QuadVertexBufferHandle;
                passData.ParticleBuffer = dataInput.ParticleBuffer;
                passData.ParticleCount = dataInput.ParticleCount;
                passData.DataSourceIndex = dataInput.DataSourceIndex;
                passData.ShouldClear = dataInput.ShouldClear;
                
                
                builder.UseBuffer(passData.QuadVertexBufferHandle);
                // builder.UseBuffer(passData.ParticleBufferHandle);
                for(int i= 0;i<attachments.Length;i++)
                {
                    builder.SetRenderAttachment(attachments[i], i);
                }
                builder.SetRenderAttachmentDepth(depthTextureHandle);
                builder.SetRenderFunc((DrawSpheresFromQuadPassData data, RasterGraphContext context) =>
                {
                    SPropertyBlock.Clear();
                    SPropertyBlock.SetBuffer(QuadVertexBufferID, data.QuadVertexBufferHandle);
                    SPropertyBlock.SetBuffer(ParticleBufferID, data.ParticleBuffer);
                    SPropertyBlock.SetMatrix(ModelMatrixID, data.ModelMatrixValue);
                    SPropertyBlock.SetFloat(ThicknessAmplifierID, data.ThicknessAmplifierValue);
                    SPropertyBlock.SetFloat(NoiseAmplifierID, data.NoiseAmplifierValue);
                    SPropertyBlock.SetTexture(NoiseTexID, _noiseTexture);
                    SPropertyBlock.SetInt(DataSourceIndexID, data.DataSourceIndex);
                    if (data.ShouldClear)
                    {
                        context.cmd.ClearRenderTarget(true, true, Color.clear);
                    }
                    context.cmd.DrawProcedural(Matrix4x4.identity,_depthColorThicknessMaterial, data.ShaderPassIndex, MeshTopology.Triangles, 6,data.ParticleCount,SPropertyBlock);
                });
            }
        }
        

        private class SmoothDepthPassDataV2
        {
            public ComputeShader Cs;
            public TextureHandle DepthTexInput;
            public TextureHandle DepthTexOutput;
            public TextureHandle ParticleDataTexInput;
            public float VerticalFov;
            public float FilterSize;
            public int TextureWidth;
            public int TextureHeight;
        }

        private void SmoothDepthV2(RenderGraph renderGraph, 
            SmoothDepthPassDataV2 dataInput)
        {
            
            using (var builder = renderGraph.AddComputePass<SmoothDepthPassDataV2>("SSF_SmoothDepthV2", out var passData))
            {
                passData.Cs = dataInput.Cs;
                passData.DepthTexInput = dataInput.DepthTexInput;
                passData.DepthTexOutput = dataInput.DepthTexOutput;
                passData.ParticleDataTexInput = dataInput.ParticleDataTexInput;
                passData.FilterSize = dataInput.FilterSize;
                passData.TextureWidth = dataInput.TextureWidth;
                passData.TextureHeight = dataInput.TextureHeight;
                passData.VerticalFov = dataInput.VerticalFov;
                //
                builder.UseTexture(passData.ParticleDataTexInput);
                builder.UseTexture(passData.DepthTexInput,AccessFlags.ReadWrite);
                builder.UseTexture(passData.DepthTexOutput, AccessFlags.ReadWrite);
                builder.SetRenderFunc((SmoothDepthPassDataV2 data, ComputeGraphContext cgContext) =>
                {
                    cgContext.cmd.SetComputeTextureParam(data.Cs, data.Cs.FindKernel("CSMain"), "particleDataTex", data.ParticleDataTexInput);
                    cgContext.cmd.SetComputeFloatParam(data.Cs, "filterSize", data.FilterSize);
                    cgContext.cmd.SetComputeFloatParam(data.Cs,"maxFilterSize",_smoothAlgoParams.maxFilterSize);
                    cgContext.cmd.SetComputeFloatParam(data.Cs,"fixedFilterRadius",_smoothAlgoParams.fixedFilterRadius);
                    cgContext.cmd.SetComputeFloatParam(data.Cs,"thresholdRatio",_smoothAlgoParams.thresholdRatio);
                    cgContext.cmd.SetComputeFloatParam(data.Cs,"clampRatio",_smoothAlgoParams.clampRatio);
                    cgContext.cmd.SetComputeFloatParam(data.Cs, "verticalFov", data.VerticalFov);
                    cgContext.cmd.SetComputeIntParam(data.Cs, "screenWidth", data.TextureWidth);
                    cgContext.cmd.SetComputeIntParam(data.Cs, "screenHeight", data.TextureHeight);
                    cgContext.cmd.SetComputeIntParam(data.Cs, "doFilter1D", 1);
                
                    cgContext.cmd.SetComputeTextureParam(data.Cs, data.Cs.FindKernel("CSMain"), "depthTex", data.DepthTexInput);
                    cgContext.cmd.SetComputeTextureParam(data.Cs, data.Cs.FindKernel("CSMain"), "depthTexOut", data.DepthTexOutput);
                    cgContext.cmd.SetComputeIntParam(data.Cs, "filterDirection", 0);
                    cgContext.cmd.DispatchCompute(data.Cs, data.Cs.FindKernel("CSMain"), data.TextureWidth / 8, data.TextureHeight / 8, 1);
                
                    cgContext.cmd.SetComputeTextureParam(data.Cs, data.Cs.FindKernel("CSMain"), "depthTex", data.DepthTexOutput);
                    cgContext.cmd.SetComputeTextureParam(data.Cs, data.Cs.FindKernel("CSMain"), "depthTexOut", data.DepthTexInput);
                    cgContext.cmd.SetComputeIntParam(data.Cs, "filterDirection", 1);
                    cgContext.cmd.DispatchCompute(data.Cs, data.Cs.FindKernel("CSMain"), data.TextureWidth / 8, data.TextureHeight / 8, 1);

                    
                    
                    cgContext.cmd.SetComputeIntParam(data.Cs, "doFilter1D", -1);
                    
                    cgContext.cmd.SetComputeTextureParam(data.Cs, data.Cs.FindKernel("CSMain"), "depthTex", data.DepthTexInput);
                    cgContext.cmd.SetComputeTextureParam(data.Cs, data.Cs.FindKernel("CSMain"), "depthTexOut", data.DepthTexOutput);
                    cgContext.cmd.DispatchCompute(data.Cs, data.Cs.FindKernel("CSMain"), data.TextureWidth / 8, data.TextureHeight / 8, 1);
                    
                });

            }

        }
         class CalculateEyeSpacePosAndNormalPassData
        {
            public ComputeShader NormalPosShader;
            public TextureHandle SmoothedDepth;
            public TextureHandle EyeSpaceNormal;
            public int TextureWidth;
            public int TextureHeight;
            public Matrix4x4 InvProjMatrix;
            public float Far;
            public float Near;
        }
        private void CalculateEyeSpaceNormal(RenderGraph renderGraph,CalculateEyeSpacePosAndNormalPassData dataInput)
        {
            using (var builder =
                   renderGraph.AddComputePass<CalculateEyeSpacePosAndNormalPassData>("SSF_CalPosAndNormal",
                       out var passData)){
                passData.NormalPosShader = dataInput.NormalPosShader;
                passData.SmoothedDepth = dataInput.SmoothedDepth;
                passData.EyeSpaceNormal = dataInput.EyeSpaceNormal;
                passData.TextureWidth = dataInput.TextureWidth;
                passData.TextureHeight = dataInput.TextureHeight;
                passData.InvProjMatrix = dataInput.InvProjMatrix;
                passData.Far = dataInput.Far;
                passData.Near = dataInput.Near;



                builder.UseTexture(passData.SmoothedDepth);
                builder.UseTexture(passData.EyeSpaceNormal, AccessFlags.Write);
                builder.SetRenderFunc((CalculateEyeSpacePosAndNormalPassData data, ComputeGraphContext cgContext) =>
                {
                    cgContext.cmd.SetComputeFloatParam(data.NormalPosShader, "screenWidth", data.TextureWidth);
                    cgContext.cmd.SetComputeFloatParam(data.NormalPosShader, "screenHeight", data.TextureHeight);
                    cgContext.cmd.SetComputeMatrixParam(data.NormalPosShader, "invProjMatrix", data.InvProjMatrix);
                    cgContext.cmd.SetComputeFloatParam(data.NormalPosShader, "far", data.Far);
                    cgContext.cmd.SetComputeFloatParam(data.NormalPosShader, "near", data.Near);
                    cgContext.cmd.SetComputeTextureParam(data.NormalPosShader,
                        data.NormalPosShader.FindKernel("CSMain"), "depthTex", data.SmoothedDepth);
                    cgContext.cmd.SetComputeTextureParam(data.NormalPosShader,
                        data.NormalPosShader.FindKernel("CSMain"), "eyeSpaceNormal", data.EyeSpaceNormal);
                    cgContext.cmd.DispatchCompute(data.NormalPosShader, data.NormalPosShader.FindKernel("CSMain"),
                        data.TextureWidth / 8, data.TextureHeight / 8, 1);
                });
            };
            
        }

    }
}