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
        public const float DEFAULT_RADIUS = 0.4f;

        GameObject m_Sphere;

        public float m_Radius = 1.0f;

        public static Material m_AnchorMaterial;

        public static Anchor CreateAnchor(Vector3 p_Position, Vector3 p_RotationEuler, float p_Radius)
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

        public static void DestroysAnchors()
        {
            foreach (Anchor l_Current in Resources.FindObjectsOfTypeAll<Anchor>())
            {
                GameObject.DestroyImmediate(l_Current.m_Sphere.gameObject);
                GameObject.DestroyImmediate(l_Current.gameObject);
            }
        }

        public void SetPosition(Vector3 p_Position, Vector3 p_RotationEuler)
        {
            m_Sphere.transform.localPosition = p_Position;
            m_Sphere.transform.localRotation = Quaternion.Euler(p_RotationEuler);
            m_Sphere.transform.localScale = Vector3.one * Math.Abs(m_Radius);
            transform.localPosition = p_Position;
            transform.localRotation = Quaternion.Euler(p_RotationEuler);
        }

        public void Awake()
        {
            m_Sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshRenderer l_Renderer = m_Sphere.GetComponent<MeshRenderer>();
            l_Renderer.material = new Material(m_AnchorMaterial);

        }
    }
}
