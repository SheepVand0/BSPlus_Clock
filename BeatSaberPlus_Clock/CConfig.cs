using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace BeatSaberPlus_Clock
{
    /// <summary>
    /// Config class helper
    /// </summary>
    internal class CConfig : CP_SDK.Config.JsonConfig<CConfig>
    {
        [JsonProperty] internal bool Enabled = false;

        [JsonProperty] internal List<ClockConfig> Profiles = new List<ClockConfig>();
        [JsonProperty] internal int SelectedProfileIndex = 0;

        internal class ClockConfig
        {
            [JsonConstructor]
            internal ClockConfig(string p_ProfileName)
            {
                ProfileName = p_ProfileName;
            }

            [JsonProperty] internal string ProfileName = "Default";

            [JsonProperty] internal SerializableVector3 MenuClockPosition = new SerializableVector3(0, 2, 0);
            [JsonProperty] internal SerializableVector3 GameClockPosition = new SerializableVector3(0, 2, 0);

            [JsonProperty] internal SerializableVector3 MenuClockRotationEuler = new SerializableVector3(0, 90, 0);
            [JsonProperty] internal SerializableVector3 GameClockRotationEuler = new SerializableVector3(0, 90, 0);

            [JsonProperty] internal bool EnableClockGrabbing = false;
            [JsonProperty] internal bool EnableAnchors = false;

            [JsonProperty] internal bool SeparateDayHours = false;
            [JsonProperty] internal bool BoolAmPm         = false;

            [JsonProperty] internal string       Separator   = ":";
            [JsonProperty] internal List<string> FormatOrder = new List<string>() { "hh", "mn", "ss" };

            [JsonProperty] internal float  FontSize  = 2f;
            [JsonProperty] internal string FontName  = "Arial";

            [JsonProperty] internal bool  UseGradient           = false;
            [JsonProperty] internal bool  UseFourColorsGradient = false;
            [JsonProperty] internal bool  GradientHorizontal    = false;
            [JsonProperty] internal SerializableColor ClockColor            = new SerializableColor(1, 1, 1, 1);
            [JsonProperty] internal SerializableColor ClockGradientColor1   = new SerializableColor(1, 0, 1, 1);
            [JsonProperty] internal SerializableColor ClockGradientColor2   = new SerializableColor(1, 0, 1, 1);
            [JsonProperty] internal SerializableColor ClockGradientColor3   = new SerializableColor(1, 0, 0.8f, 1);
            [JsonProperty] internal SerializableColor ClockGradientColor4   = new SerializableColor(1, 0, 0.8f, 1);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get relative config path
        /// </summary>
        /// <returns></returns>
        public override string GetRelativePath()
            => $"{CP_SDK.ChatPlexSDK.ProductName}/Clock/Config";

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On config init
        /// </summary>
        /// <param name="p_OnCreation">On creation</param>
        protected override void OnInit(bool p_OnCreation)
        {
            Save();
            if (Profiles.Count == 0)
            {
                Profiles.Add(new ClockConfig("Default"));
                SelectedProfileIndex = 0;
            }
        }
    }

    public struct Anchor
    {
        public Anchor(string p_Name, Vector3 p_Position, Vector3 p_RotationEuler, float p_Radius)
        {
            AnchorName = p_Name;
            AnchorPosition = p_Position;
            AnchorRotationEuler = p_RotationEuler;
            AnchorRadius = p_Radius;
        }
        public string AnchorName { get; set; }
        public Vector3 AnchorPosition { get; set; }
        public Vector3 AnchorRotationEuler { get; set; }
        public float AnchorRadius { get; set; }
    }

    internal enum ConfigAccessibility
    {
        AllPlayers, CurrentPlayerOnly
    }

    internal class SerializableVector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public SerializableVector3(float p_X, float p_Y, float p_Z)
        {
            x = p_X;
            y = p_Y;
            z = p_Z;
        }

        public Vector3 ToUnityVector3()
        {
            return new Vector3(x,y,z);
        }

        public static SerializableVector3 ToSerializableVector(Vector3 p_Vector)
        {
            return new SerializableVector3(p_Vector.x, p_Vector.y, p_Vector.z);
        }
    }

    internal class SerializableColor
    {
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public float a { get; set; }

        public SerializableColor(float p_R, float p_G, float p_B, float p_A)
        {
            r = p_R;
            g = p_G;
            b = p_B;
            a = p_A;
        }

        public Color ToUnityColor()
        {
            return new Color(r, g, b, 1);
        }

        public static SerializableColor ToSerializableColor(Color p_Color)
        {
            return new SerializableColor(p_Color.r, p_Color.g, p_Color.b, 1);
        }
    }
}
