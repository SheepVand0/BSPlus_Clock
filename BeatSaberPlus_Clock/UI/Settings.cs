using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BeatSaberMarkupLanguage.Components;
using BeatSaberPlus_Clock.UI.Components;
using BeatSaberPlus_Clock;
using System.Collections.Generic;
using System.Linq;
using IPA.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BeatSaberPlus_Clock.UI
{
    /// <summary>
    /// Clock settings view controllers
    /// </summary>
    internal class Settings : BeatSaberPlus.SDK.UI.ResourceViewController<Settings>
    {
        const float MIN_FONT_SIZE = 0.1f;
        const float MAX_FONT_SIZE = 300.0f;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #region UIComponents
        #region Tabs
        [UIObject("Tabs")] GameObject m_Tabs = null;

        [UIObject("TabSelector")] GameObject m_TabSelector = null;

        [UIObject("TabProfiles")] GameObject m_Tab_Profiles = null;
        [UIObject("TabGeneral")]  GameObject m_Tab_General  = null;
        [UIObject("TabFormat")]   GameObject m_Tab_Format   = null;
        [UIObject("TabStyle")]    GameObject m_Tab_Style    = null;
        [UIObject("TabPosition")] GameObject m_Tab_Position = null;
        #endregion

        #region Profiles
        [UIObject("ProfileListFrame_Background")] GameObject m_ProfileListBackground = null;
        [UIObject("ProfilesList")] GameObject m_ProfilesListView = null;
        [UIObject("ImportProfileFrame")] GameObject m_ImportProfileFrame = null;
        [UIObject("ImportProfileFrame_Background")] GameObject m_ImportProfileFrameBackground = null;
        [UIObject("ImportFrameButtonsTransform")] GameObject m_ImportFrameButtonsTransform = null;

        [UIComponent("ProfilesUpButton")] Button m_ProfilesUpButton                = null;
        [UIComponent("ProfilesDownButton")] Button m_ProfilesDownButton            = null;
        [UIComponent("ProfilesManagementButtons")] HorizontalLayoutGroup m_ProfilesManagementBox = null;
        [UIComponent("ImportProfileFrame_DropDown")] DropDownListSetting m_ImportProfileFrame_DropDown = null;

        BeatSaberPlus.SDK.UI.DataSource.SimpleTextList m_EventsList = null;

        CustomKeyboard m_ProfileRenameKeyboard = null;
        CustomKeyboard m_ProfileCreateKeyboard = null;

        Button m_PrimaryNewProfileButton = null;
        Button m_PrimaryRenameProfileButton = null;
        Button m_PrimaryDeleteProfileButton = null;
        Button m_ExportButton = null;
        Button m_ImportButton = null;

        Button m_ConfirmImport = null;
        Button m_CancelImport = null;

        private int m_CurrentProfilePage = 0;
        private int m_PageCount = 1;

        [UIValue("ImportProfileFrame_DropDownOptions")] List<object> m_ImportProfileFrame_DropDownOptions = new List<object>();

        string m_SelectedImportProfile = string.Empty;
        #endregion

        #region General
        [UIComponent("BoolSeparateDayHours")] ToggleSetting       m_BoolSeparateDayHours = null;
        [UIComponent("BoolAmPm")]             ToggleSetting       m_BoolAmPm             = null;
        [UIComponent("SliderFontSize")]       SliderSetting       m_Slider_FontSize      = null;
        [UIComponent("FontDropdown")]         DropDownListSetting m_FontDropdown         = null;

        [UIValue("FontValue")] private string       FontValue { get => string.Empty; set { } }
        [UIValue("Fonts")]     private List<object> m_Fonts   = new List<object>() { "1" };
        #endregion

        #region Format
        [UIObject("FormatTransform")] GameObject m_FormatListTransform = null;

        Button               m_DocButton = null;
        CustomStringSetting  m_StringElementsSeparator = null;
        CustomFormatCellList m_FormatSettingsList = null;

        [UIObject("FormatElementsSeparatorTransform")] private GameObject FormatElementsSeparatorTransform = null;
        #endregion

        #region Style
        [UIComponent("BoolUseClockGradient")]           ToggleSetting m_Bool_UseClockGradient      = null;
        [UIComponent("BoolUseFourClockGradientColors")] ToggleSetting m_Bool_UseFourGradientColors = null;
        [UIComponent("BoolClockColor")]                 ColorSetting  m_Color_Clock                = null;
        [UIComponent("ColorClock1")]                    ColorSetting  m_Color_Clock1               = null;
        [UIComponent("ColorClock2")]                    ColorSetting  m_Color_Clock2               = null;
        [UIComponent("ColorClock3")]                    ColorSetting  m_Color_Clock3               = null;
        [UIComponent("ColorClock4")]                    ColorSetting  m_Color_Clock4               = null;
        #endregion

        #region Postition
        [UIComponent("EnableClockGrabbing")] ToggleSetting       m_EnableClockGrabbing = null;
        [UIComponent("ClockMovementMode")]   DropDownListSetting m_ClockMovementMode   = null;
        [UIComponent("EnableAnchors")]       ToggleSetting       m_EnableAnchors       = null;

        [UIValue("ClockMovementModeList")] private List<object> m_ClockMovementChoices = new List<object> { "Menu", "Game" };
        [UIValue("MovementMode")] private string DropdownMovementMode { get => "Menu"; set { } }
        #endregion
        #endregion

        #region UIControl Properties
        private HMUI.TextSegmentedControl m_TabSelector_Control = null;
        #endregion

        #region Properties
        float m_FontPercent = 0f;
        #endregion
        /// <summary>
        /// On view creation
        /// </summary>
        protected override sealed void OnViewCreation()
        {
            var l_Event                      = new BeatSaberMarkupLanguage.Parser.BSMLAction(this, this.GetType().GetMethod(nameof(Settings.OnSettingChanged), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
            var l_FontSizeEvent              = new BeatSaberMarkupLanguage.Parser.BSMLAction(this, this.GetType().GetMethod(nameof(Settings.OnFontSizeChanged), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
            var l_FontDropdownEvent          = new BeatSaberMarkupLanguage.Parser.BSMLAction(this, this.GetType().GetMethod(nameof(Settings.OnFontSelected), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
            var l_ClockMovementDropdownEvent = new BeatSaberMarkupLanguage.Parser.BSMLAction(this, this.GetType().GetMethod(nameof(Settings.OnMovementModeSelected), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
            var l_ClockImportDropdownEvent   = new BeatSaberMarkupLanguage.Parser.BSMLAction(this, this.GetType().GetMethod(nameof(Settings.OnImportProfileSelected), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

            #region Tabs
            m_TabSelector_Control = BeatSaberPlus.SDK.UI.TextSegmentedControl.Create(m_TabSelector.transform as RectTransform, false);
            m_TabSelector_Control.SetTexts(new string[] { "Profiles", "General", "Format", "Style", "Position"/*, "Anchors"*/ });
            m_TabSelector_Control.ReloadData();
            m_TabSelector_Control.didSelectCellEvent += OnTabSelected;

            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_Tab_Profiles, 0.5f);
            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_Tab_General,  0.5f);
            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_Tab_Format,   0.5f);
            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_Tab_Style,    0.5f);
            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_Tab_Position, 0.5f);
            #endregion

            #region Profiles
            m_PrimaryNewProfileButton    = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_ProfilesManagementBox.transform, "NEW",    CreateProfile, p_PreferedWidth: 25);
            m_PrimaryDeleteProfileButton = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_ProfilesManagementBox.transform, "DELETE", DeleteProfile, p_PreferedWidth: 25);
            m_PrimaryRenameProfileButton = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_ProfilesManagementBox.transform, "RENAME", RenameProfile, p_PreferedWidth: 25);
            m_ExportButton               = BeatSaberPlus.SDK.UI.Button.Create(m_ProfilesManagementBox.transform, "EXPORT", ExportCurrentProfile, p_PreferedWidth: 25);
            m_ImportButton               = BeatSaberPlus.SDK.UI.Button.Create(m_ProfilesManagementBox.transform, "IMPORT", OpenImportFrame, p_PreferedWidth: 25);

            m_ProfilesUpButton.onClick.AddListener(ProfilePageUp);
            m_ProfilesDownButton.onClick.AddListener(ProfilePageDown);

            m_CancelImport = BeatSaberPlus.SDK.UI.Button.Create(m_ImportFrameButtonsTransform.transform, "Cancel", CloseImportFrame, p_PreferedWidth: 25);
            m_ConfirmImport = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_ImportFrameButtonsTransform.transform, "Import", ImportProfile, p_PreferedWidth: 25);
            BeatSaberPlus.SDK.UI.DropDownListSetting.Setup(m_ImportProfileFrame_DropDown, l_ClockImportDropdownEvent, true);

            m_ProfilesUpButton.transform.localScale = Vector3.one * 0.5f;
            m_ProfilesDownButton.transform.localScale = Vector3.one * 0.5f;

            var l_BSMLTableView = m_ProfilesListView.GetComponentInChildren<BSMLTableView>();
            l_BSMLTableView.SetDataSource(null, false);
            GameObject.DestroyImmediate(m_ProfilesListView.GetComponentInChildren<CustomListTableData>());
            m_EventsList = l_BSMLTableView.gameObject.AddComponent<BeatSaberPlus.SDK.UI.DataSource.SimpleTextList>();
            m_EventsList.TableViewInstance = l_BSMLTableView;
            m_EventsList.CellSizeValue = 4.8f;
            l_BSMLTableView.didSelectCellWithIdxEvent += OnProfileSelected;
            l_BSMLTableView.SetDataSource(m_EventsList, false);

            RefreshProfilesList();
            #endregion

            #region General
            m_StringElementsSeparator = CustomUIComponent.Create<CustomStringSetting>(FormatElementsSeparatorTransform.transform, true, (p_Item) =>
            {
                p_Item.Setup(Clock.m_ClockConfig.Separator, 32, true);
                p_Item.OnChange += OnSeparatorChange;
            });
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolSeparateDayHours, l_Event, Clock.m_ClockConfig.SeparateDayHours, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolAmPm,             l_Event, Clock.m_ClockConfig.BoolAmPm, true);

            BeatSaberPlus.SDK.UI.DropDownListSetting.Setup(m_FontDropdown, l_Event, true);
            m_FontPercent = ((Clock.m_ClockConfig.FontSize / 10) * 100 / 300) / Clock.CLOCK_FONT_SIZE_MULTIPLIER;
            BeatSaberPlus.SDK.UI.SliderSetting.Setup(m_Slider_FontSize, l_FontSizeEvent, BeatSaberPlus.SDK.UI.BSMLSettingFormartter.Percentage, m_FontPercent, true);
            //UpdateFontsList();
            #endregion

            #region Format
            m_FormatSettingsList = CustomUIComponent.Create<CustomFormatCellList>(m_FormatListTransform.transform, true);
            #endregion

            #region Style
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_Bool_UseClockGradient,      l_Event, Clock.m_ClockConfig.UseGradient,           true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_Bool_UseFourGradientColors, l_Event, Clock.m_ClockConfig.UseFourColorsGradient, true);

            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock,  l_Event, Clock.m_ClockConfig.ClockColor.ToUnityColor(),          true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock1, l_Event, Clock.m_ClockConfig.ClockGradientColor1.ToUnityColor(), true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock2, l_Event, Clock.m_ClockConfig.ClockGradientColor2.ToUnityColor(), true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock3, l_Event, Clock.m_ClockConfig.ClockGradientColor3.ToUnityColor(), true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock4, l_Event, Clock.m_ClockConfig.ClockGradientColor4.ToUnityColor(), true);

            m_Color_Clock2.interactable = Clock.m_ClockConfig.UseGradient;
            m_Color_Clock3.interactable = Clock.m_ClockConfig.UseFourColorsGradient;
            m_Color_Clock4.interactable = Clock.m_ClockConfig.UseFourColorsGradient;
            #endregion

            #region Position
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_EnableClockGrabbing, l_Event, Clock.m_ClockConfig.EnableClockGrabbing, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_EnableAnchors, l_Event, Clock.m_ClockConfig.EnableAnchors, true);
            BeatSaberPlus.SDK.UI.DropDownListSetting.Setup(m_ClockMovementMode, l_ClockMovementDropdownEvent, true);
            #endregion

            Clock.e_OnConfigLoaded += OnConfidLoaded;

            OnTabSelected(null, 0);
        }

        /// <summary>
        /// On view deactivation
        /// </summary>
        protected sealed override void OnViewDeactivation()
        {
            Clock.Instance.SaveConfig();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #region Config managment
        private void OnConfidLoaded()
        {
            m_EnableClockGrabbing.Value = Clock.m_ClockConfig.EnableClockGrabbing;
            m_EnableAnchors.Value = Clock.m_ClockConfig.EnableAnchors;
            m_BoolSeparateDayHours.Value = Clock.m_ClockConfig.SeparateDayHours;
            m_BoolAmPm.Value = Clock.m_ClockConfig.BoolAmPm;
            m_StringElementsSeparator.SetValue(Clock.m_ClockConfig.Separator);
            m_FormatSettingsList.LoadFromConfig();
            m_Slider_FontSize.Value = ((Clock.m_ClockConfig.FontSize / 10) * 100 / 300) / Clock.CLOCK_FONT_SIZE_MULTIPLIER;
            m_Bool_UseClockGradient.Value = Clock.m_ClockConfig.UseGradient;
            m_Bool_UseFourGradientColors.Value = Clock.m_ClockConfig.UseFourColorsGradient;
            m_Color_Clock.CurrentColor = Clock.m_ClockConfig.ClockColor.ToUnityColor();
            m_Color_Clock1.CurrentColor = Clock.m_ClockConfig.ClockGradientColor1.ToUnityColor();
            m_Color_Clock2.CurrentColor = Clock.m_ClockConfig.ClockGradientColor2.ToUnityColor();
            m_Color_Clock3.CurrentColor = Clock.m_ClockConfig.ClockGradientColor3.ToUnityColor();
            m_Color_Clock4.CurrentColor = Clock.m_ClockConfig.ClockGradientColor4.ToUnityColor();

            ClockFloatingScreen.Instance.SetClockPositionByScene(BeatSaberPlus.SDK.Game.Logic.SceneType.Menu);

            m_EnableClockGrabbing.ApplyValue();
            m_EnableAnchors.ApplyValue();
            m_BoolSeparateDayHours.ApplyValue();
            m_BoolAmPm.ApplyValue();
            m_Slider_FontSize.ApplyValue();
            m_Bool_UseClockGradient.ApplyValue();
            m_Bool_UseFourGradientColors.ApplyValue();
            m_Color_Clock.ApplyValue();
            m_Color_Clock1.ApplyValue();
            m_Color_Clock2.ApplyValue();
            m_Color_Clock3.ApplyValue();
            m_Color_Clock4.ApplyValue();

            ClockFloatingScreen.Instance.ApplySettings();

            RefreshProfilesList();
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #region Profiles
        private void OnProfileSelected(HMUI.TableView p_TableView, int p_Index)
        {
            CConfig.Instance.SelectedProfileIndex = p_Index;
            Clock.InvokeOnConfigLoaded();
        }

        internal void RefreshProfilesList()
        {
            m_EventsList.TableViewInstance.ClearSelection();
            m_EventsList.Data.Clear();

            if (CConfig.Instance.Profiles.Count >= Clock.EVENT_PER_PAGES)
            {
                m_PageCount = (int)System.Math.Floor((float)(CConfig.Instance.Profiles.Count / Clock.EVENT_PER_PAGES));
                if (m_CurrentProfilePage > m_PageCount)
                    m_CurrentProfilePage = m_PageCount;

                m_ProfilesUpButton.interactable = !(m_CurrentProfilePage == 0);
                m_ProfilesDownButton.interactable = !(m_CurrentProfilePage == m_PageCount);
            } else
            {
                m_ProfilesUpButton.interactable = false;
                m_ProfilesDownButton.interactable = false;
                m_CurrentProfilePage = 0;
                m_PageCount = 1;
            }

            for (int l_i = 0; l_i < CConfig.Instance.Profiles.Count - (m_CurrentProfilePage * Clock.EVENT_PER_PAGES); l_i++)
            {
                m_EventsList.Data.Add((CConfig.Instance.Profiles[l_i+(m_CurrentProfilePage*Clock.EVENT_PER_PAGES)].ProfileName, null));
            }

            m_EventsList.TableViewInstance.ReloadData();
            if (CConfig.Instance.SelectedProfileIndex > m_EventsList.Data.Count)
                m_EventsList.TableViewInstance.SelectCellWithIdx(0);
            else
                m_EventsList.TableViewInstance.SelectCellWithIdx(CConfig.Instance.SelectedProfileIndex);
        }

        private void ProfilePageUp()
        {
            m_CurrentProfilePage -= 1;
            RefreshProfilesList();
        }

        private void ProfilePageDown()
        {
            m_CurrentProfilePage += 1;
            RefreshProfilesList();
        }

        private void CreateProfile()
        {
            if (m_ProfileCreateKeyboard == null)
            {
                m_ProfileCreateKeyboard = CustomUIComponent.Create<CustomKeyboard>(m_ProfilesManagementBox.transform, true, (p_Item) =>
                {
                    p_Item.OnKeyboardEnterPressed += (p_OldValue, p_NewValue) =>
                    {
                        if (p_NewValue == string.Empty) { ShowMessageModal("Configs Names Can't be empty"); return; }
                        CConfig.Instance.Profiles.Add(new CConfig.ClockConfig(p_NewValue));
                        CConfig.Instance.SelectedProfileIndex = CConfig.Instance.Profiles.Count - 1;
                        Clock.InvokeOnConfigLoaded();
                    };
                });
            }

            m_ProfileCreateKeyboard.Open(string.Empty);
        }

        private void DeleteProfile()
        {
            if (CConfig.Instance.Profiles.Count == 1) { ShowMessageModal("You can't delete a config when you only own one"); return; }

            CConfig.Instance.Profiles.RemoveAt(CConfig.Instance.SelectedProfileIndex);

            CConfig.Instance.SelectedProfileIndex -= 1;
            Clock.InvokeOnConfigLoaded();
            RefreshProfilesList();
        }

        private void RenameProfile()
        {
            if (m_ProfileRenameKeyboard == null)
            {
                m_ProfileRenameKeyboard = CustomUIComponent.Create<CustomKeyboard>(m_ProfilesManagementBox.transform, true, (p_Item) =>
                {
                    p_Item.OnKeyboardEnterPressed += (p_CurrentName, p_NewName) =>
                    {
                        if (p_NewName == string.Empty) { ShowMessageModal("Configs Names Can't be empty"); return; }
                        CConfig.Instance.Profiles[CConfig.Instance.SelectedProfileIndex].ProfileName = p_NewName;
                        Clock.InvokeOnConfigLoaded();
                    };
                });
            }

            m_ProfileRenameKeyboard.Open(CConfig.Instance.Profiles[CConfig.Instance.SelectedProfileIndex].ProfileName);
        }

        private void ExportCurrentProfile()
        {
            try
            {
                if (!System.IO.Directory.Exists(Clock.CLOCK_EXPORT_FOLDER))
                    System.IO.Directory.CreateDirectory(Clock.CLOCK_EXPORT_FOLDER);

                JsonSerializerSettings l_Settings = new JsonSerializerSettings();
                string l_SerializedConfig = JsonConvert.SerializeObject(Clock.m_ClockConfig, Formatting.Indented, l_Settings);
                string l_FileName = $"{CP_SDK.Misc.Time.UnixTimeNow()}_{Clock.m_ClockConfig.ProfileName}.bspclock";
                l_FileName = string.Concat(l_FileName.Split(System.IO.Path.GetInvalidFileNameChars()));

                System.IO.File.WriteAllText($"{Clock.CLOCK_EXPORT_FOLDER}{l_FileName}", l_SerializedConfig);
                ShowMessageModal($"Succefully exported {CConfig.Instance.Profiles[CConfig.Instance.SelectedProfileIndex].ProfileName}");
            } catch (System.Exception l_E)
            {
                Logger.Instance.Error(l_E);
            }
        }

        private void OpenImportFrame()
        {
            m_Tabs.gameObject.SetActive(false);
            m_ImportProfileFrame.gameObject.SetActive(true);

            if (!System.IO.Directory.Exists(Clock.CLOCK_IMPORT_FOLDER))
                System.IO.Directory.CreateDirectory(Clock.CLOCK_IMPORT_FOLDER);

            var l_Files = new List<object>();
            foreach (var l_File in System.IO.Directory.GetFiles(Clock.CLOCK_IMPORT_FOLDER, "*.bspclock"))
                l_Files.Add(System.IO.Path.GetFileNameWithoutExtension(l_File));

            if (l_Files.Count == 0) { ShowMessageModal("No config to import was found"); CloseImportFrame(); return; }

            m_ImportProfileFrame_DropDownOptions = l_Files;
            m_ImportProfileFrame_DropDown.values = l_Files;
            m_ImportProfileFrame_DropDown.UpdateChoices();
        }

        private void CloseImportFrame()
        {
            m_Tabs.gameObject.SetActive(true);
            m_ImportProfileFrame.gameObject.SetActive(false);
        }

        private void ImportProfile()
        {
            try
            {
                string l_FileName = $"{Clock.CLOCK_IMPORT_FOLDER}{m_SelectedImportProfile}.bspclock";
                if (!System.IO.File.Exists(l_FileName)) { ShowMessageModal("File not found"); return; }
                string l_NewConfigName = string.Empty;
                CConfig.ClockConfig l_NewConfig = JsonConvert.DeserializeObject<CConfig.ClockConfig>(System.IO.File.ReadAllText(l_FileName));
                CConfig.Instance.Profiles.Add(l_NewConfig);
                l_NewConfig.ProfileName = $"{l_NewConfig.ProfileName} (Imported)";
                ShowMessageModal($"Succefully Imported Config {l_NewConfig.ProfileName}");
                CloseImportFrame();
                RefreshProfilesList();
            } catch (System.Exception l_E)
            {
                ShowMessageModal("On error ocurred on profile import");
                Logger.Instance.Error(l_E);
            }
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On Tab Selected
        /// </summary>
        /// <param name="p_Tab"></param>
        /// <param name="p_Index"></param>

        private void OnTabSelected(HMUI.SegmentedControl p_Tab, int p_Index)
        {
            m_Tab_Profiles.SetActive(p_Index == 0);
            m_Tab_General.SetActive(p_Index == 1);
            m_Tab_Format.SetActive(p_Index == 2);
            m_Tab_Style.SetActive(p_Index == 3);
            m_Tab_Position.SetActive(p_Index == 4);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #region Settings Change
        private void OnSettingChanged(object p_Value)
        {
            Clock.m_ClockConfig.EnableClockGrabbing   = m_EnableClockGrabbing.Value;
            Clock.m_ClockConfig.EnableAnchors         = m_EnableAnchors.Value;
            Clock.m_ClockConfig.SeparateDayHours      = m_BoolSeparateDayHours.Value;
            Clock.m_ClockConfig.BoolAmPm              = m_BoolAmPm.Value;
            Clock.m_ClockConfig.Separator             = m_StringElementsSeparator.Text;
            Clock.m_ClockConfig.UseGradient           = m_Bool_UseClockGradient.Value;
            Clock.m_ClockConfig.UseFourColorsGradient = m_Bool_UseFourGradientColors.Value;
            Clock.m_ClockConfig.ClockColor            = SerializableColor.ToSerializableColor(m_Color_Clock.CurrentColor);
            Clock.m_ClockConfig.ClockGradientColor1   = SerializableColor.ToSerializableColor(m_Color_Clock1.CurrentColor);
            Clock.m_ClockConfig.ClockGradientColor2   = SerializableColor.ToSerializableColor(m_Color_Clock2.CurrentColor);
            Clock.m_ClockConfig.ClockGradientColor3   = SerializableColor.ToSerializableColor(m_Color_Clock3.CurrentColor);
            Clock.m_ClockConfig.ClockGradientColor4   = SerializableColor.ToSerializableColor(m_Color_Clock4.CurrentColor);

            m_Color_Clock1.interactable = Clock.m_ClockConfig.UseGradient;
            m_Color_Clock2.interactable = Clock.m_ClockConfig.UseGradient;
            m_Color_Clock3.interactable = Clock.m_ClockConfig.UseFourColorsGradient && Clock.m_ClockConfig.UseGradient;
            m_Color_Clock4.interactable = Clock.m_ClockConfig.UseFourColorsGradient && Clock.m_ClockConfig.UseGradient;

            Clock.Instance.SaveConfig();

            //ClockFloatingScreen.Instance.ApplySettings();
        }

        private void OnFontSizeChanged(object p_Value)
        {
            Clock.m_ClockConfig.FontSize = Clock.CLOCK_FONT_SIZE_MULTIPLIER * (((float)p_Value * 10) * 300 / 100);
            Clock.InvokeOnSettingChanged();
            Clock.Instance.SaveConfig();
        }

        private void OnFontSelected(object p_Value)
        {
            Clock.m_ClockConfig.FontName = "Arial";
        }

        private static void DisableObjects(List<string> p_ObjectsToDisable)
        {
            try {
                foreach (var l_Current in p_ObjectsToDisable)
                    GameObject.Find(l_Current).SetActive(false);
            } catch (System.Exception l_E)
            {
                Logger.Instance.Error(l_E);
            }
        }

        private static void EnableObjects(List<string> p_Objects)
        {
            try
            {
                foreach (var l_Current in p_Objects)
                {
                    GameObject.Find(l_Current).SetActive(true);
                }
            } catch (System.Exception l_E)
            {
                Logger.Instance.Error(l_E);
            }
        }

        private void OnMovementModeSelected(object p_Value)
        {
            /*MenuEnvironmentManager l_MenuEnvironmentManager = Resources.FindObjectsOfTypeAll<MenuEnvironmentManager>().FirstOrDefault();
            GameScenesManager l_Manager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();
            var l_MenuTransitionsHelper = Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().FirstOrDefault();
            var l_StandardLevelScenesTransitionSetupData = l_MenuTransitionsHelper.GetField<StandardLevelScenesTransitionSetupDataSO, MenuTransitionsHelper>("_standardLevelScenesTransitionSetupData");
            var l_StandardGameplaySceneInfo = l_StandardLevelScenesTransitionSetupData.GetField<SceneInfo, StandardLevelScenesTransitionSetupDataSO>("_standardGameplaySceneInfo");
            var l_GameCoreSceneInfo = l_StandardLevelScenesTransitionSetupData.GetField<SceneInfo, StandardLevelScenesTransitionSetupDataSO>("_gameCoreSceneInfo");
            PlayerData l_PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().FirstOrDefault().playerData;
            */
            EnvironmentSceneSetup l_SceneSetup = Resources.FindObjectsOfTypeAll<EnvironmentSceneSetup>().FirstOrDefault();
            GameScenesManager l_GameScenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();
            MenuEnvironmentManager l_MenuEnvironmentManager = Resources.FindObjectsOfTypeAll<MenuEnvironmentManager>().FirstOrDefault();
            MenuTransitionsHelper l_MenuTransitionsHelper = Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().FirstOrDefault();
            PlayerData l_PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;

            switch ((string)p_Value)
            {
                case "Game":
                    Clock.m_MovementMode = ClockMovementMode.Menu;
                    ClockFloatingScreen.Instance.SetClockPositionByScene(BeatSaberPlus.SDK.Game.Logic.SceneType.Menu);

                    /*l_GameScenesManager.GetField<HashSet<string>, GameScenesManager>("_neverUnloadScenes").Add("MenuCore");
                    StandardLevelScenesTransitionSetupDataSO l_standardLevelScenesTransitionSetupData = l_MenuTransitionsHelper.GetField<StandardLevelScenesTransitionSetupDataSO, MenuTransitionsHelper>("_standardLevelScenesTransitionSetupData");

                    LevelCollectionTableView l_Levels = Resources.FindObjectsOfTypeAll<LevelCollectionTableView>().FirstOrDefault();

                    StandardLevelDetailViewController l_StandardLevelDetailViewController = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();
                    IReadOnlyList<IPreviewBeatmapLevel> l_previewBeatmapLevels = Resources.FindObjectsOfTypeAll<LevelCollectionTableView>().FirstOrDefault().GetField<IReadOnlyList<IPreviewBeatmapLevel>, LevelCollectionTableView>("_previewBeatmapLevels");

                    l_standardLevelScenesTransitionSetupData.Init("Standard", l_StandardLevelDetailViewController.selectedDifficultyBeatmap, l_previewBeatmapLevels[0],l_PlayerData.overrideEnvironmentSettings,
                        l_PlayerData.colorSchemesSettings.GetSelectedColorScheme(), l_PlayerData.gameplayModifiers, l_PlayerData.playerSpecificSettings, l_PlayerData.practiceSettings, "Menu", true, true);

                    Transform l_Gameplay = GameObject.Find("StandardGameplay").transform;

                    l_MenuEnvironmentManager.transform.root.gameObject.SetActive(true);

                    l_Gameplay.gameObject.SetActive(false);
                    EnableObjects(new List<string> { "EventSystem", "ControllerLeft", "ControllerRight" });*/
                    break;
                case "Menu":
                    Clock.m_MovementMode = ClockMovementMode.Game;
                    ClockFloatingScreen.Instance.SetClockPositionByScene(BeatSaberPlus.SDK.Game.Logic.SceneType.Playing);

                    /*l_GameScenesManager.PopScenes(0.25f, null, (_) =>
                    {
                        l_GameScenesManager.GetField<HashSet<string>, GameScenesManager>("_neverUnloadScenes").Remove("MenuCore");
                        l_MenuEnvironmentManager.ShowEnvironmentType(MenuEnvironmentManager.MenuEnvironmentType.Default);
                    });*/
                    break;
                default:
                    Logger.Instance.Error("Not Valid movement selected");
                    break;
            }
        }

        private void OnImportProfileSelected(object p_Value)
        {
            m_SelectedImportProfile = (string)p_Value;
        }

        private void OnSeparatorChange(string p_Value)
        {
            Clock.m_ClockConfig.Separator = p_Value;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #region Font
        internal void UpdateFontsList()
        {
            List<object> l_NewFonts = new List<object>();
            foreach (Font l_Current in Clock.m_AvailableFonts)
                l_NewFonts.Add(l_Current.name);
            m_Fonts = l_NewFonts;
            m_FontDropdown.dropdown.ReloadData();
            SelectFont(Clock.m_ClockConfig.FontName, true);
        }

        internal void SelectFont(string p_Name, bool p_ApplyOnDropdown)
        {
            foreach (var l_Current in Clock.m_AvailableFonts)
                if (p_Name == l_Current.name)
                {
                    Clock.m_ClockConfig.FontName = p_Name;
                    if (p_ApplyOnDropdown) { m_FontDropdown.Value = p_Name; m_FontDropdown.ApplyValue(); }
                    return;
                }
            Clock.m_ClockConfig.FontName = Clock.m_AvailableFonts[0].name;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal string ClockMovementModeToString(ClockMovementMode p_MovementMode)
        {
            switch (p_MovementMode)
            {
                case ClockMovementMode.Menu: return "Menu";
                case ClockMovementMode.Game: return "Game";
                default: return "Menu";
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
