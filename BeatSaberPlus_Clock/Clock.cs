using BeatSaberMarkupLanguage;
using BeatSaberPlus_Clock.UI;
using CP_SDK.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BeatSaberPlus_Clock
{
    /// <summary>
    /// Online instance
    /// </summary>
    public class Clock : BeatSaberPlus.SDK.BSPModuleBase<Clock>
    {
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
        public override string Description => "A clock";
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

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal const string CLOCK_EXPORT_FOLDER        = "./UserData/BeatSaberPlus/Clock/Export/";

        internal const string CLOCK_IMPORT_FOLDER        = "./UserData/BeatSaberPlus/Clock/Import/";

        internal const string CLOCK_FONTS_FOLDER         =  "./UserData/BeatSaberPlus/Clock/Fonts";

        internal const float  CLOCK_FONT_SIZE_MULTIPLIER = 0.34f;

        internal const int    EVENT_PER_PAGES            = 10;

        internal static BeatSaberPlus.SDK.Game.Logic.SceneType m_MovementMode = BeatSaberPlus.SDK.Game.Logic.SceneType.Menu;

        internal static event Action e_OnConfigLoaded;

        internal static event Action e_OnSettingEdited;

        internal static event Action e_OnFontsLoaded;

        internal static List<TMPro.TMP_FontAsset> m_AvailableFonts = new List<TMPro.TMP_FontAsset>();

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
            e_OnConfigLoaded += OnConfigLoaded;
            Clock.InvokeOnConfigLoaded();
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
        /// <summary>
        /// Function called when a config was loaded
        /// </summary>
        private void OnConfigLoaded()
        {
            MTCoroutineStarter.Start(LoadFonts());
            if (ClockFloatingScreen.Instance == null)
                ClockFloatingScreen.CreateClock();
            else
                ClockFloatingScreen.Instance.ApplySettings();
        }
        #endregion
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        #region Memeber Functions
        /// <summary>
        /// Function to get all fonts in clock fonts folder
        /// </summary>
        internal static IEnumerator LoadFonts()
        {
            yield return new WaitUntil(() => CP_SDK.Unity.FontManager.IsInitialized);

            m_AvailableFonts.Clear();

            if (CP_SDK.Unity.FontManager.TryGetTMPFontByFamily("Arial", out var l_Arial))       m_AvailableFonts.Add(l_Arial);
            if (CP_SDK.Unity.FontManager.TryGetTMPFontByFamily("Segoe UI", out var l_SegoeUI))  m_AvailableFonts.Add(l_SegoeUI);

            if (!System.IO.Directory.Exists(CLOCK_FONTS_FOLDER))
                System.IO.Directory.CreateDirectory(CLOCK_FONTS_FOLDER);

            var l_FontsFiles = System.IO.Directory.GetFiles(Clock.CLOCK_FONTS_FOLDER, "*.ttf");
            for (int l_I = 0; l_I < l_FontsFiles.Length; l_I++)
            {
                if (CP_SDK.Unity.FontManager.AddFontFile(l_FontsFiles[l_I], out var l_FamilyName)
                    && CP_SDK.Unity.FontManager.TryGetTMPFontByFamily(l_FamilyName, out var l_Font))
                    m_AvailableFonts.Add(l_Font);
            }

            var l_ActiveFont = CConfig.Instance.GetActiveConfig().FontName;
            if (!m_AvailableFonts.Any(x => x.name == l_ActiveFont) && m_AvailableFonts.Count > 0)
            {
                ClockFloatingScreen.ClockFont = m_AvailableFonts[0];
                CConfig.Instance.GetActiveConfig().FontName = m_AvailableFonts[0].name;
            }

            e_OnFontsLoaded?.Invoke();
        }

        /// <summary>
        /// Apply font to the clock text
        /// </summary>
        internal static void ApplyFont()
        {
            for (int l_i = 0;l_i < m_AvailableFonts.Count;l_i++)
            {
                if (m_AvailableFonts[l_i].name != CConfig.Instance.GetActiveConfig().FontName) continue;

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
    }
}
