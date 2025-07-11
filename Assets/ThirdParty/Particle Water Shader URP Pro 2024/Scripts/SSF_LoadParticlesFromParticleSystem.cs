
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace SSF_Particle2Fluid_ShaderUtil
{
    public class SSF_LoadParticlesFromParticleSystem : SSF_ParticleSource
    {
        public ParticleSystem[] particleSystems;
        private List<ParticleSystem.Particle[]> _particles;

        protected override void UpdateParticleBufferData()
        {
            base.UpdateParticleBufferData();
            CheckParticleDataLength();


            bool hasOnlyOneParticleSystem = particleSystems is { Length: 1 } && particleSystems[0] ;
            int preIndex = 0;
            for (int i = 0; i < particleSystems.Count(); i++)
            {
                if (particleSystems[i])
                {
                    int count = particleSystems[i].GetParticles(_particles[i]);
                    for (int j = 0; j < count; j++)
                    {
                        Vector3 pos;
                        if (hasOnlyOneParticleSystem || particleSystems[i].main.simulationSpace ==
                            ParticleSystemSimulationSpace.World)
                        {
                            pos = _particles[i][j].position;
                        }
                        else
                        {
                            pos = particleSystems[i].transform.localToWorldMatrix.MultiplyPoint3x4(_particles[i][j].position);
                        }
                        ParticlesData[preIndex+j].Position = pos ;
                        ParticlesData[preIndex+j].Radius = _particles[i][j].GetCurrentSize(particleSystems[i]);
                        ParticlesData[preIndex+j].Color = _particles[i][j].GetCurrentColor(particleSystems[i]);
                    }
                    preIndex += particleSystems[i].main.maxParticles;
                }
            }
        }

        public override Matrix4x4 GetModelMatrix()
        {
            bool hasOnlyOneParticleSystem = particleSystems is { Length: 1 } && particleSystems[0];
            return hasOnlyOneParticleSystem? (particleSystems[0].main.simulationSpace ==
                                              ParticleSystemSimulationSpace.World
                ? Matrix4x4.identity:
            particleSystems[0].transform.localToWorldMatrix):Matrix4x4.identity ;
        }

        void CheckParticleCount()
        {
            ParticleNum = 0;
            for (int i = 0; i < particleSystems.Count(); i++)
            {
                if (particleSystems[i] != null)
                {
                    ParticleNum += particleSystems[i].main.maxParticles;
                }
            }
        }

        protected override void SetupParticleBufferData()
        {
            base.SetupParticleBufferData();
            UpdateParticleBufferData();
        }

        protected override void SetupBounds()
        {
           UpdateBounds();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(Bounds.center,Bounds.size);
        }

        protected override void UpdateBounds()
        {
            Bounds =  new Bounds();

            // 遍历所有粒子系统
            foreach (ParticleSystem ps in particleSystems)
            {
                // 确保粒子系统已经启动
                ps.Play();

                // 获取粒子系统的Renderer组件
                Renderer particleRenderer = ps.GetComponent<Renderer>();

                // 检查是否有Renderer组件
                if (particleRenderer != null)
                {

                    // 将当前粒子系统的边界包含到整体边界中
                    Bounds.Encapsulate(particleRenderer.bounds);
                }
                else
                {
                    Debug.LogError("No Renderer component found on the Particle System: " + ps.name);
                }
            }
        }

        private void CheckParticleDataLength()
        {
            CheckParticleCount();
            if (ParticlesData == null || ParticlesData.Count() != ParticleNum)
            {
                ParticlesData = new SSF_particle[ParticleNum];
                print("[SSF] Particle Num changed : " + ParticleNum);
                // Checking SSF_particle Buffers
                if (_particles == null || _particles.Count < particleSystems.Count())
                {
                    _particles = new List<ParticleSystem.Particle[]>(particleSystems.Count());

                    _particles.Clear();
                    for (int i = 0; i < particleSystems.Count(); i++)
                    {
                        _particles.Add(new ParticleSystem.Particle[particleSystems[i].main.maxParticles]);
                    }
                }
                for (int i = 0; i < particleSystems.Count(); i++)
                {
                    if (_particles[i].Count() != particleSystems[i].main.maxParticles)
                    {
                        _particles[i] = new ParticleSystem.Particle[particleSystems[i].main.maxParticles];
                    }
                }
            }
        }
    }
}