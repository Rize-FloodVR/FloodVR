
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SSF_Particle2Fluid_ShaderUtil
{
        public class SSF_LoadParticlesFromFile : SSF_ParticleSource
    {
        public UnityEngine.Object particleFile;
        public float particleRadius;
        public Color particleColor;
        [FormerlySerializedAs("positionOrder_0")] [Range(0,2)]
        public int positionOrder0;
        [FormerlySerializedAs("positionOrder_1")] [Range(0,2)]
        public int positionOrder1=2;
        [FormerlySerializedAs("positionOrder_2")] [Range(0,2)]
        public int positionOrder2=1;

        protected override void SetupParticleBufferData()
        {
            base.SetupParticleBufferData();
            if (particleFile != null)
            {
                TextAsset asset = particleFile as TextAsset;
                if (asset != null)
                {
                    string[] striparr = asset.text.Split(new string[] { "\r\n", " " }, StringSplitOptions.RemoveEmptyEntries);
                    ParticleNum = striparr.Length / 3;
                    print("[SSF] Loaded particles : " + ParticleNum);
                    ParticlesData = new SSF_particle[ParticleNum];
                    for (int i = 0; i < ParticleNum; i++)
                    {
                        ParticlesData[i].Position = new Vector3(Convert.ToSingle(striparr[3 * i+positionOrder0]),
                            Convert.ToSingle(striparr[3 * i + positionOrder1]), Convert.ToSingle(striparr[3 * i + positionOrder2]));
                        ParticlesData[i].Radius = particleRadius;
                        ParticlesData[i].Color = particleColor;
                    }
                }
            }
        }

        protected override void UpdateParticleBufferData()
        {
            base.UpdateParticleBufferData();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(Bounds.center,Bounds.size);
        }

        public override bool NeedsToUpdate()
        {
            // load from file need not update
            return false;
        }

        protected override void SetupBounds()
        {
            
            Vector3 boundMin = Vector3.one * float.MaxValue;
            Vector3 boundMax = Vector3.one * float.MinValue;
            
            if (particleFile != null)
            {
                TextAsset asset = particleFile as TextAsset;
                if (asset != null)
                {
                    string[] striparr = asset.text.Split(new string[] { "\r\n", " " }, StringSplitOptions.RemoveEmptyEntries);
                    ParticleNum = striparr.Length / 3;
                    for (int i = 0; i < ParticleNum; i++)
                    {
                        Vector3 particlePosition = new Vector3(Convert.ToSingle(striparr[3 * i+positionOrder0]),
                            Convert.ToSingle(striparr[3 * i + positionOrder1]), Convert.ToSingle(striparr[3 * i + positionOrder2]));

                        boundMin = Vector3.Min(boundMin, particlePosition-Vector3.one * particleRadius );
                        boundMax = Vector3.Max(boundMax, particlePosition+Vector3.one * particleRadius);
                    }
                }
            }

            Bounds = new Bounds((boundMax + boundMin) / 2, (boundMax - boundMin));
        }
    }
}