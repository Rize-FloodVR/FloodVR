using UnityEditor;
using UnityEngine;

namespace SSF_Particle2Fluid_ShaderUtil.Editor
{
    [CustomEditor(typeof(SSF_ParticleSource), true)]
    public class SSFParticleSourceEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SSF_ParticleSource source = (SSF_ParticleSource) target;
            if (GUILayout.Button("Apply RenderParam Change"))
            {
                source.OnRenderParamChange();
            }
        }
    }
}