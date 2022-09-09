using BeatSaberMarkupLanguage;
using UnityEngine;
using UnityEngine.SceneManagement;
using BeatSaberPlus_Clock.UI;
using System;
using UnityEditor;
using UnityEngine.UI;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace BeatSaberPlus_Clock
{
    /// <summary>
    /// Online instance
    /// </summary>
    internal class Clock : BeatSaberPlus.SDK.BSPModuleBase<Clock>
    {
        internal const string CLOCK_EXPORT_FOLDER        = "./UserData/BeatSaberPlus/Clock/Export/";

        internal const string CLOCK_IMPORT_FOLDER        = "./UserData/BeatSaberPlus/Clock/Import/";

        internal const string CLOCK_FONTS_FOLDER         =  "./UserData/BeatSaberPlus/Clock/Fonts";

        internal const float  CLOCK_FONT_SIZE_MULTIPLIER = 0.34f;

        internal const int    EVENT_PER_PAGES            = 10;

        /// <summary>
        /// Module type
        /// </summary>
        public override CP_SDK.EIModuleBaseType Type => CP_SDK.EIModuleBaseType.Integrated;
        /// <summary>
        /// Name of the Module
        /// </summary>
        public override string Name => "Clock";
        /// <summary>
        /// Description of the Module
        /// </summary>
        public override string Description => "A Clock";
        /// <summary>
        /// Is the Module using chat features
        /// </summary>
        public override bool UseChatFeatures => false;
        /// <summary>
        /// Is enabled
        /// </summary>
        public override bool IsEnabled { get => CConfig.Instance.Enabled; set { CConfig.Instance.Enabled = value; CConfig.Instance.Save(); } }
        /// <summary>
        /// Activation kind
        /// </summary>
        public override CP_SDK.EIModuleBaseActivationType ActivationType => CP_SDK.EIModuleBaseActivationType.OnMenuSceneLoaded;

        internal static event Action e_OnConfigLoaded;

        internal static event Action e_OnSettingEdited;

        internal static event Action e_OnFontsLoaded;

        internal static List<Font> m_AvailableFonts = new List<Font>();

        internal static BeatSaberPlus.SDK.Game.Logic.SceneType m_MovementMode = BeatSaberPlus.SDK.Game.Logic.SceneType.Menu;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Settings view
        /// </summary>
        private UI.Settings m_SettingsView = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Enable the Module
        /// </summary>
        protected override void OnEnable()
        {
            Clock.InvokeOnConfigLoaded();
            e_OnConfigLoaded += OnConfigLoaded;
        }

        /// <summary>
        /// Disable the Module
        /// </summary>
        protected override void OnDisable()
        {
            e_OnConfigLoaded -= OnConfigLoaded;
            ClockFloatingScreen.DestroyClock();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        #region Events Invoking
        internal static void InvokeOnSettingChanged()
        {
            e_OnSettingEdited?.Invoke();
        }

        internal static void InvokeOnConfigLoaded()
        {
            e_OnConfigLoaded?.Invoke();
            Instance.OnConfigLoaded();
        }
        #endregion

        #region Events
        private void OnConfigLoaded()
        {
            if (ClockFloatingScreen.Instance == null)
                ClockFloatingScreen.CreateClock();
            else
                ClockFloatingScreen.Instance.ApplySettings();
            LoadFonts();
        }
        #endregion
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        #region Memeber Functions
        internal static void LoadFonts()
        {
            m_AvailableFonts.Clear();
            m_AvailableFonts.Add(Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font);

            if (!System.IO.Directory.Exists(CLOCK_FONTS_FOLDER))
                System.IO.Directory.CreateDirectory(CLOCK_FONTS_FOLDER);

            var l_FontsFiles = System.IO.Directory.GetFiles(Clock.CLOCK_FONTS_FOLDER, "*.ttf");

            List<Font> l_Fonts = new List<Font>();

            for (int l_i = 0;l_i < l_FontsFiles.Length;l_i++)
            {
                m_AvailableFonts.Add(CP_SDK.Unity.FontManager.AddFontFile(l_FontsFiles[l_i]));

                if (System.IO.Path.GetFileNameWithoutExtension(l_FontsFiles[l_i]) != CConfig.Instance.GetActiveConfig().FontName)
                    continue;

                ClockFloatingScreen.ClockFont = m_AvailableFonts[l_i];
            }

            if (m_AvailableFonts.Count == 1)
            {
                ClockFloatingScreen.ClockFont = m_AvailableFonts[0];
                CConfig.Instance.GetActiveConfig().FontName = "Arial";
            }

            e_OnFontsLoaded?.Invoke();
        }

        internal static void ApplyFont()
        {
            for (int l_i = 0;l_i < m_AvailableFonts.Count;l_i++)
            {
                if (m_AvailableFonts[l_i].name != CConfig.Instance.GetActiveConfig().FontName)
                    continue;

                ClockFloatingScreen.ClockFont = m_AvailableFonts[l_i];
                ClockFloatingScreen.Instance.ApplySettings();
            }
        }

        /// <summary>
        /// Save current Profile
        /// </summary>
        internal void SaveConfig()
        {
            for (int l_i = 0; l_i < CConfig.Instance.Profiles.Count; l_i++) {
                if (CConfig.Instance.GetActiveConfig().ProfileName != CConfig.Instance.Profiles[l_i].ProfileName)
                    continue;

                CConfig.Instance.Profiles[l_i] = CConfig.Instance.GetActiveConfig();
            }
            CConfig.Instance.Save();
            InvokeOnSettingChanged();
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get Module settings UI
        /// </summary>
        protected override (HMUI.ViewController, HMUI.ViewController, HMUI.ViewController) GetSettingsUIImplementation()
        {
            /// Create view if needed
            if (m_SettingsView == null)
                m_SettingsView = BeatSaberUI.CreateViewController<UI.Settings>();

            /// Change main view
            return (m_SettingsView, null, null);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
