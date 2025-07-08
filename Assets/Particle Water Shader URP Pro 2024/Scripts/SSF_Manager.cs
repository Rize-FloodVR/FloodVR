using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SSF_Particle2Fluid_ShaderUtil
{
    public class SSF_Manager : MonoBehaviour
    {
        public static SSF_Manager Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<SSF_Manager>();
                }
                if(_instance == null)
                {
                    GameObject go = new GameObject("SSF_Manager");
                    _instance = go.AddComponent<SSF_Manager>();
                }
                return _instance;
            }
        }

        public List<SSF_ParticleSource> particleSources;
        private static SSF_Manager _instance;

        public GraphicsBuffer RenderParamsBuffer;

        private void OnEnable()
        {
            CreateParamsBufferIfNeeded();
        }
        
        private void OnDisable()
        {
            RenderParamsBuffer.Release();
            
            RenderParamsBuffer.Dispose();
        }

        private void CreateParamsBufferIfNeeded()
        {
            
            if (RenderParamsBuffer == null)
            {
                // 28 is the byte count of SSF_RenderParams
                RenderParamsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 10,
                    18 * 4);
            }
        }

        public void RegisterParticleSource(SSF_ParticleSource source)
        {
            if (particleSources == null)
            {
                particleSources = new List<SSF_ParticleSource>();
            }
            particleSources.Add(source);
            source.OnRenderParamChange += UpdateRenderParamsBuffer;
            UpdateRenderParamsBuffer();
        }
        public void UnregisterParticleSource(SSF_ParticleSource source)
        {
            if (particleSources == null)
            {
                return;
            }
            particleSources.Remove(source);
            source.OnRenderParamChange -= UpdateRenderParamsBuffer;
            UpdateRenderParamsBuffer();
        }
        
        private void UpdateRenderParamsBuffer()
        {
            SSF_RenderParams[] renderParams = new SSF_RenderParams[particleSources.Count];
            for (int i = 0; i < particleSources.Count; i++)
            {
                renderParams[i] =particleSources[i].renderParams;
            }
            
            CreateParamsBufferIfNeeded();
            RenderParamsBuffer.SetData(renderParams);
        }


        private void Update()
        {
            foreach (var source in particleSources)
            {
                if (source.NeedsToUpdate())
                {
                    source.UpdateParticleBuffer();
                }
            }
        }

    }
}