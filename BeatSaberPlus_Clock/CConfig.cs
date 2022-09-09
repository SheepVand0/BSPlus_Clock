using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

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

            [JsonProperty] internal Vector3 MenuClockPosition = new Vector3(0, 2, 0);
            [JsonProperty] internal Vector3 GameClockPosition = new Vector3(0, 2, 0);

            [JsonProperty] internal Vector3 MenuClockRotationEuler = new Vector3(0, 90, 0);
            [JsonProperty] internal Vector3 GameClockRotationEuler = new Vector3(0, 90, 0);

            [JsonProperty] internal bool EnableClockGrabbing = false;
            [JsonProperty] internal bool EnableAnchors       = false;

            [JsonProperty] internal bool SeparateDayHours = false;
            [JsonProperty] internal bool BoolAmPm         = false;

            [JsonProperty] internal string       Separator   = ":";
            [JsonProperty] internal List<string> FormatOrder = new List<string>() { "hh", "mn", "ss" };

            [JsonProperty] internal float  FontSize  = 2f;
            [JsonProperty] internal string FontName  = "Arial";

            [JsonProperty] internal bool  UseGradient           = false;
            [JsonProperty] internal bool  UseFourColorsGradient = false;
            [JsonProperty] internal bool  GradientHorizontal    = false;
            [JsonProperty] internal Color ClockColor            = new Color(1, 1, 1, 1);
            [JsonProperty] internal Color ClockGradientColor1   = new Color(1, 0, 1, 1);
            [JsonProperty] internal Color ClockGradientColor2   = new Color(1, 0, 1, 1);
            [JsonProperty] internal Color ClockGradientColor3   = new Color(1, 0, 0.8f, 1);
            [JsonProperty] internal Color ClockGradientColor4   = new Color(1, 0, 0.8f, 1);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal ClockConfig GetActiveConfig()
        {
            if (Profiles.ElementAt(SelectedProfileIndex) == null)
                SelectedProfileIndex = 0;

            return Profiles.ElementAt(SelectedProfileIndex);
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

            if (Profiles.ElementAt(SelectedProfileIndex) == null)
                SelectedProfileIndex = 0;

            //m_JsonConverters[1].
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

    internal enum EClockMovementMode
    {
        Menu,
        Game
    }
}
