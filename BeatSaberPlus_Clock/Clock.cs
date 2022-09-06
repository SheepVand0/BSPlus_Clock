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

        internal static event Action e_SettingEdited;

        internal static CConfig.ClockConfig m_ClockConfig;

        internal static List<Font> m_AvailableFonts = new List<Font>();

        internal static ClockMovementMode m_MovementMode = ClockMovementMode.Menu;

        internal const string CLOCK_EXPORT_FOLDER = "./UserData/BeatSaberPlus/Clock/Export/";

        internal const string CLOCK_IMPORT_FOLDER = "./UserData/BeatSaberPlus/Clock/Import/";

        internal const float CLOCK_FONT_SIZE_MULTIPLIER = 0.34f;

        internal const int EVENT_PER_PAGES = 10;

        internal static int m_SelectedProfileIndex = 0;

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
            ClockFloatingScreen.Destroy();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        #region Events Invoking
        internal static void InvokeOnSettingChanged()
        {
            e_SettingEdited?.Invoke();
        }

        internal static void InvokeOnConfigLoaded()
        {
            try
            {
                m_ClockConfig = CConfig.Instance.Profiles[CConfig.Instance.SelectedProfileIndex];
            } catch (Exception l_E)
            {
                Logger.Instance.Error(l_E);
            }
            e_OnConfigLoaded?.Invoke();
            Instance.OnConfigLoaded();
        }
        #endregion

        #region Events
        private void OnConfigLoaded()
        {
            Logger.Instance.Info("On Config loaded event called");
            if (ClockFloatingScreen.Instance == null)
                ClockFloatingScreen.CreateClock();
            else
                ClockFloatingScreen.Instance.ApplySettings();
            //LoadFonts();
        }
        #endregion
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #region Memeber Functions
        private void LoadFonts()
        {
            AssetBundle l_Bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream($"{Plugin.AssemblyName}.Bundle.fontassetbundle"));
            m_AvailableFonts.Clear();
            Font[] l_TempFonts = l_Bundle.LoadAllAssets<Font>();
            m_AvailableFonts.AddRange(l_TempFonts);
        }

        /// <summary>
        /// Save current Profile
        /// </summary>
        internal void SaveConfig()
        {
            for (int l_i = 0; l_i < CConfig.Instance.Profiles.Count; l_i++) {
                if (m_ClockConfig.ProfileName == CConfig.Instance.Profiles[l_i].ProfileName)
                    CConfig.Instance.Profiles[l_i] = m_ClockConfig;
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

        /// <summary>
        /// Settings view
        /// </summary>
        private UI.Settings m_SettingsView = null;
    }
    internal enum ClockMovementMode
    {
        Menu,
        Game
    }
}
