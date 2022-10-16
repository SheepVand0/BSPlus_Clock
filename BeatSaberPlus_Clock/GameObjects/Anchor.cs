using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace BeatSaberPlus_Clock.GameObjects
{
    class Anchor : MonoBehaviour
    {
        internal const float DEFAULT_RADIUS = 0.4f;

        GameObject m_Sphere;

        internal float m_Radius = 1.0f;

        internal static Material m_AnchorMaterial;

        internal static Anchor CreateAnchor(Vector3 p_Position, Vector3 p_RotationEuler, float p_Radius)
        {
            if (m_AnchorMaterial == null) {
                AssetBundle l_Bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{Plugin.AssemblyName}.Bundle.bspclockbundle"));
                m_AnchorMaterial = l_Bundle.LoadAsset<Material>("M_Anchor");
                l_Bundle.Unload(false);
            }

            Anchor l_Anchor;
            (l_Anchor = new GameObject("Anchor").AddComponent<Anchor>()).m_Radius = p_Radius;
            l_Anchor.SetPosition(p_Position, p_RotationEuler);
            return l_Anchor;
        }

        internal static void DestroysAnchors()
        {
            foreach (Anchor l_Current in Resources.FindObjectsOfTypeAll<Anchor>())
            {
                GameObject.DestroyImmediate(l_Current.m_Sphere.gameObject);
                GameObject.DestroyImmediate(l_Current.gameObject);
            }
        }

        internal void SetPosition(Vector3 p_Position, Vector3 p_RotationEuler)
        {
            m_Sphere.transform.localPosition = p_Position;
            m_Sphere.transform.localRotation = Quaternion.Euler(p_RotationEuler);
            m_Sphere.transform.localScale = Vector3.one * Math.Abs(m_Radius);
            transform.localPosition = p_Position;
            transform.localRotation = Quaternion.Euler(p_RotationEuler);
        }

        internal void Awake()
        {
            m_Sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshRenderer l_Renderer = m_Sphere.GetComponent<MeshRenderer>();
            l_Renderer.material = new Material(m_AnchorMaterial);

        }
    }
}
