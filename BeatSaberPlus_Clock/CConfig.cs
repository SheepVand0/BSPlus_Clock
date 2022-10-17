using BeatSaberPlus_Clock.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

            [JsonProperty] internal Vector3 MenuClockPosition = ClockFloatingScreen.m_Anchors[0].Position;
            [JsonProperty] internal Vector3 GameClockPosition = ClockFloatingScreen.m_Anchors[0].Position;

            [JsonProperty] internal Vector3 MenuClockRotationEuler = ClockFloatingScreen.m_Anchors[0].RotationEuler;
            [JsonProperty] internal Vector3 GameClockRotationEuler = ClockFloatingScreen.m_Anchors[0].RotationEuler;

            [JsonProperty] internal bool EnableClockGrabbing = false;
            [JsonProperty] internal bool EnableAnchors       = false;

            [JsonProperty] internal bool SeparateDayHours = false;
            [JsonProperty] internal bool ShowAmPm         = false;

            [JsonProperty] internal string       Separator   = ":";
            [JsonProperty] internal List<string> FormatOrder = new List<string>() { "hh", "mn", "ss" };

            [JsonProperty] internal float  FontSize  = 75f;
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
            if (Profiles.Count == 0)
            {
                Profiles.Add(new ClockConfig("Default"));
                SelectedProfileIndex = 0;
            }

            if (Profiles.ElementAt(SelectedProfileIndex) == null)
                SelectedProfileIndex = 0;

            Save();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal ClockConfig GetActiveConfig()
        {
            if (Profiles.ElementAt(SelectedProfileIndex) == null)
                SelectedProfileIndex = 0;

            return Profiles.ElementAt(SelectedProfileIndex);
        }

        internal List<JsonConverter> GetConverters()
        {
            return m_JsonConverters;
        }
    }
}
