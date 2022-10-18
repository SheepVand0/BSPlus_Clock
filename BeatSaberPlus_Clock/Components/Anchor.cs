using System;
using System.Reflection;
using UnityEngine;

namespace BeatSaberPlus_Clock.Components
{
    public class Anchor : MonoBehaviour
    {
        /// <summary>
        /// Anchor config
        /// </summary>
        public struct Settings
        {
            public string   Name            { get; set; }
            public Vector3  Position        { get; set; }
            public Vector3  RotationEuler   { get; set; }
            public float    Radius          { get; set; }

            ////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="p_Name">Anchor name</param>
            /// <param name="p_Position">Anchor position</param>
            /// <param name="p_RotationEuler">Anchor rotation</param>
            /// <param name="p_Radius">Anchor radius</param>
            public Settings(string p_Name, Vector3 p_Position, Vector3 p_RotationEuler, float p_Radius)
            {
                Name            = p_Name;
                Position        = p_Position;
                RotationEuler   = p_RotationEuler;
                Radius          = p_Radius;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal const float DEFAULT_RADIUS = 0.7f;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        GameObject m_Sphere;

        internal float m_Radius = 1.0f;

        internal static Material m_AnchorMaterial;

        /// <summary>
        /// Create an anchor
        /// </summary>
        /// <param name="p_Position">Anchor position</param>
        /// <param name="p_RotationEuler">Anchor rotation</param>
        /// <param name="p_Radius">Action radius</param>
        /// <returns></returns>
        internal static Anchor CreateAnchor(Vector3 p_Position, Vector3 p_RotationEuler, float p_Radius)
        {
            if (m_AnchorMaterial == null) {
                AssetBundle l_Bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{Plugin.AssemblyName}.Bundle.bspclockbundle"));
                m_AnchorMaterial = l_Bundle.LoadAsset<Material>("M_Anchor");
                l_Bundle.Unload(false);
            }

            Anchor l_Anchor;
            (l_Anchor = new GameObject("Anchor").AddComponent<Anchor>()).m_Radius = p_Radius;
            l_Anchor.SetPositionAndRotation(p_Position, p_RotationEuler);
            return l_Anchor;
        }

        /// <summary>
        /// Destroy all anchors
        /// </summary>
        internal static void DestroysAnchors()
        {
            foreach (Anchor l_Current in Resources.FindObjectsOfTypeAll<Anchor>())
            {
                GameObject.DestroyImmediate(l_Current.m_Sphere.gameObject);
                GameObject.DestroyImmediate(l_Current.gameObject);
            }
        }
        /// <summary>
        /// Set Position and rotation of this anchor
        /// </summary>
        /// <param name="p_Position">New position</param>
        /// <param name="p_RotationEuler">New rotation</param>
        internal void SetPositionAndRotation(Vector3 p_Position, Vector3 p_RotationEuler)
        {
            m_Sphere.transform.localPosition = p_Position;
            m_Sphere.transform.localRotation = Quaternion.Euler(p_RotationEuler);
            m_Sphere.transform.localScale = Vector3.one * Math.Abs(m_Radius);
            transform.localPosition = p_Position;
            transform.localRotation = Quaternion.Euler(p_RotationEuler);
        }

        /// <summary>
        /// When an anchor is created
        /// </summary>
        internal void Awake()
        {
            m_Sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshRenderer l_Renderer = m_Sphere.GetComponent<MeshRenderer>();
            l_Renderer.material = new Material(m_AnchorMaterial);

        }
    }
}
