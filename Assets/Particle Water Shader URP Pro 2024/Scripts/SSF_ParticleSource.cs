
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace SSF_Particle2Fluid_ShaderUtil
{
    

    public enum SSF_TextureType
    {
        Depth,
        SmoothedDepth,
        Color,
        Thickness,
        Normal,
        Others
    }
    [System.Serializable]
    public struct SSF_RenderParams
    {
        [Range(0,1)]
        public float isTransparent;
        
        public float ior;
        [Range(0,1)]
        public float smoothness;
        [Range(1,32)]
        public float fresnelPower;
        [Range(0,1)]
        public float minReflectionRatio;
        [Range(0,1)]
        public float maxReflectionRatio;
        
        public Color ambientColor;
        public Color diffuseColor;
        public Color specularColor;
    }
    public struct SSF_particle
    {
        public Vector3 Position;
        public Color Color;
        public float Radius;
    }
    public class SSF_ParticleSource : MonoBehaviour
    {
        public float particleBufferUpdateInterval = -1f;
        
        private GraphicsBuffer _particleBuffer;

        protected SSF_particle[] ParticlesData;
        protected int ParticleNum;
        [Range(0,1)]
        public float thickness=0.1f;
        [Range(0,1)]
        public float noise=0.5f;

        [SerializeField]protected SSF_RenderParams _renderParams;

        public SSF_RenderParams renderParams
        {
            get => _renderParams;
            set
            {
                _renderParams = value;
                OnRenderParamChange?.Invoke();
            }
        }

        protected Bounds Bounds = new();


        private readonly float _lastUpdateTime = 0.0f;
        public int GetParticleNum() { return ParticleNum; }

        public UnityAction OnRenderParamChange;


        /// Should set partcle_num and actual SSF_particle data
        protected virtual void SetupParticleBufferData()
        {
        }

        protected virtual void SetupBounds()
        {
            
        }
        void SetupParticleBuffer()
        {
            SetupParticleBufferData();
            SetupBounds();
            if(_particleBuffer!=null){
                _particleBuffer.Release();
            }
            _particleBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, GetParticleNum(), 32);
                
            _particleBuffer.SetData(ParticlesData);

        }

        private void SetupBuffers()
        {

            SetupParticleBuffer();
        }
        void OnEnable()
        {
            print("[SSF] Enabled source");
            // SSF_Manager.Instance.particleSources.Add(this);
            SetupBuffers();
            SSF_Manager.Instance.RegisterParticleSource(this);
        }

        void OnDisable()
        {
            // SSF_Manager.Instance.particleSources.Remove(this);
            print("[SSF] Disabled source");
            ReleaseBuffers();
            ParticlesData = null;
            ParticleNum=0;
            SSF_Manager.Instance.UnregisterParticleSource(this);
        }
        protected virtual void UpdateParticleBufferData()
        {
        }

        protected virtual void Update()
        {
            if (NeedsToUpdate())
            {
                UpdateParticleBuffer();
            }
        }

        public virtual bool NeedsToUpdate()
        {
            if (particleBufferUpdateInterval < 0)
            {
                return true;
            }

            return Time.time - _lastUpdateTime > particleBufferUpdateInterval;

        }
        
        protected virtual void UpdateBounds()
        {
            // should update bounds on 
        }
        public void UpdateParticleBuffer()
        {
            UpdateParticleBufferData();
            UpdateBounds();
            _particleBuffer.SetData(ParticlesData);
        }
        // Update is called once per frame
        public void ReleaseBuffers()
        {
            if (_particleBuffer != null)
                _particleBuffer.Release();
        }
        public GraphicsBuffer GetParticleBuffer()
        {
            return _particleBuffer;
        }


        public Bounds GetBounds()
        {
            return Bounds;
        }

        public virtual Matrix4x4 GetModelMatrix()
        {
            return transform.localToWorldMatrix;
        }
    }   
}