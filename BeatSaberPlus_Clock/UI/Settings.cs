using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BeatSaberMarkupLanguage.Components;
using BeatSaberPlus_Clock.UI.Widgets;
using BeatSaberPlus_Clock;
using System.Collections.Generic;
using System.Linq;
using IPA.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using BeatSaberMarkupLanguage;

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
        [UIObject("Tabs")] private readonly GameObject m_Tabs = null;

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

        string m_SelectedImportProfile = string.Empty;

        [UIValue("ImportProfileFrame_DropDownOptions")] List<object> m_ImportProfileFrame_DropDownOptions = new List<object>();
        #endregion

        #region General
        [UIComponent("BoolSeparateDayHours")] ToggleSetting       m_BoolSeparateDayHours = null;
        [UIComponent("BoolAmPm")]             ToggleSetting       m_BoolAmPm             = null;
        [UIComponent("SliderFontSize")]       SliderSetting       m_Slider_FontSize      = null;
        [UIComponent("FontDropdown")]         DropDownListSetting m_FontDropdown         = null;
        [UIObject("FontsRefreshLayout")]      GameObject          m_FontRefreshObject    = null;

        Button m_FontRefreshButton = null;
        [UIValue("FontValue")] private string       FontValue { get => string.Empty; set { } }
        [UIValue("Fonts")]     private List<object> m_Fonts   = new List<object>() { "1" };
        #endregion

        #region Format
        [UIObject("FormatElementsSeparatorTransform")] private GameObject FormatElementsSeparatorTransform = null;
        [UIObject("FormatTransform")] GameObject m_FormatListTransform = null;

        Button               m_DocButton = null;
        CustomStringSetting  m_StringElementsSeparator = null;
        CustomFormatCellList m_FormatSettingsList = null;
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
                p_Item.Setup(CConfig.Instance.GetActiveConfig().Separator, 32, true);
                p_Item.OnChange += OnSeparatorChange;
            });
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolSeparateDayHours, l_Event, CConfig.Instance.GetActiveConfig().SeparateDayHours, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolAmPm,             l_Event, CConfig.Instance.GetActiveConfig().BoolAmPm, true);

            BeatSaberPlus.SDK.UI.DropDownListSetting.Setup(m_FontDropdown, l_FontDropdownEvent, true);
            Clock.e_OnFontsLoaded += OnFontsLoaded;
            OnFontsLoaded();
            m_FontRefreshButton = BeatSaberPlus.SDK.UI.Button.Create(m_FontRefreshObject.transform, "Refresh fonts", () => { Clock.LoadFonts(); });
            m_FontPercent = ((CConfig.Instance.GetActiveConfig().FontSize / 10) * 100 / 300) / Clock.CLOCK_FONT_SIZE_MULTIPLIER;
            BeatSaberPlus.SDK.UI.SliderSetting.Setup(m_Slider_FontSize, l_FontSizeEvent, BeatSaberPlus.SDK.UI.BSMLSettingFormartter.Percentage, m_FontPercent, true);
            //UpdateFontsList();
            #endregion

            #region Format
            m_FormatSettingsList = CustomUIComponent.Create<CustomFormatCellList>(m_FormatListTransform.transform, true);
            #endregion

            #region Style
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_Bool_UseClockGradient,      l_Event, CConfig.Instance.GetActiveConfig().UseGradient,           true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_Bool_UseFourGradientColors, l_Event, CConfig.Instance.GetActiveConfig().UseFourColorsGradient, true);

            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock,  l_Event, CConfig.Instance.GetActiveConfig().ClockColor,          true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock1, l_Event, CConfig.Instance.GetActiveConfig().ClockGradientColor1, true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock2, l_Event, CConfig.Instance.GetActiveConfig().ClockGradientColor2, true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock3, l_Event, CConfig.Instance.GetActiveConfig().ClockGradientColor3, true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock4, l_Event, CConfig.Instance.GetActiveConfig().ClockGradientColor4, true);

            m_Color_Clock2.interactable = CConfig.Instance.GetActiveConfig().UseGradient;
            m_Color_Clock3.interactable = CConfig.Instance.GetActiveConfig().UseFourColorsGradient;
            m_Color_Clock4.interactable = CConfig.Instance.GetActiveConfig().UseFourColorsGradient;
            #endregion

            #region Position
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_EnableClockGrabbing, l_Event, CConfig.Instance.GetActiveConfig().EnableClockGrabbing, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_EnableAnchors, l_Event, CConfig.Instance.GetActiveConfig().EnableAnchors, true);
            BeatSaberPlus.SDK.UI.DropDownListSetting.Setup(m_ClockMovementMode, l_ClockMovementDropdownEvent, true);
            #endregion

            Clock.e_OnConfigLoaded += OnConfidLoaded;

            OnTabSelected(null, 0);
        }

        private void OnFontsLoaded()
        {
            m_FontDropdown.values.Clear();
            foreach (var l_Current in Clock.m_AvailableFonts)
                m_FontDropdown.values.Add(l_Current.name);
            m_FontDropdown.UpdateChoices();
            m_FontDropdown.Value = CConfig.Instance.GetActiveConfig().FontName;
            m_FontDropdown.ApplyValue();
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
            m_EnableClockGrabbing.Value = CConfig.Instance.GetActiveConfig().EnableClockGrabbing;
            m_EnableAnchors.Value = CConfig.Instance.GetActiveConfig().EnableAnchors;
            m_BoolSeparateDayHours.Value = CConfig.Instance.GetActiveConfig().SeparateDayHours;
            m_BoolAmPm.Value = CConfig.Instance.GetActiveConfig().BoolAmPm;
            m_StringElementsSeparator.SetValue(CConfig.Instance.GetActiveConfig().Separator);
            m_FormatSettingsList.LoadFromConfig();
            m_Slider_FontSize.Value = ((CConfig.Instance.GetActiveConfig().FontSize / 10) * 100 / 300) / Clock.CLOCK_FONT_SIZE_MULTIPLIER;
            m_Bool_UseClockGradient.Value = CConfig.Instance.GetActiveConfig().UseGradient;
            m_Bool_UseFourGradientColors.Value = CConfig.Instance.GetActiveConfig().UseFourColorsGradient;
            m_Color_Clock.CurrentColor = CConfig.Instance.GetActiveConfig().ClockColor;
            m_Color_Clock1.CurrentColor = CConfig.Instance.GetActiveConfig().ClockGradientColor1;
            m_Color_Clock2.CurrentColor = CConfig.Instance.GetActiveConfig().ClockGradientColor2;
            m_Color_Clock3.CurrentColor = CConfig.Instance.GetActiveConfig().ClockGradientColor3;
            m_Color_Clock4.CurrentColor = CConfig.Instance.GetActiveConfig().ClockGradientColor4;

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
            CConfig.Instance.Save();
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
                        CConfig.Instance.Save();
                        Clock.InvokeOnConfigLoaded();
                    };
                });
            }

            m_ProfileCreateKeyboard.Open(string.Empty);
        }

        private void DeleteProfile()
        {
            if (CConfig.Instance.Profiles.Count == 1) { ShowMessageModal("You can't delete a config when you only own one"); return; }

            string l_Name = CConfig.Instance.GetActiveConfig().ProfileName;

            CConfig.Instance.Profiles.RemoveAt(CConfig.Instance.SelectedProfileIndex);

            CConfig.Instance.SelectedProfileIndex -= 1;
            Clock.InvokeOnConfigLoaded();
            CConfig.Instance.Save();
            RefreshProfilesList();

            ShowMessageModal($"Succefully deleted profile : {l_Name}");
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
                        CConfig.Instance.Save();
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

                string l_SerializedConfig = JsonConvert.SerializeObject(CConfig.Instance.GetActiveConfig(), CConfig.Instance.GetConverters().ToArray());

                string l_FileName = $"{CP_SDK.Misc.Time.UnixTimeNow()}_{CConfig.Instance.GetActiveConfig().ProfileName}.bspclock";
                l_FileName = string.Concat(l_FileName.Split(System.IO.Path.GetInvalidFileNameChars()));

                System.IO.File.WriteAllText($"{Clock.CLOCK_EXPORT_FOLDER}{l_FileName}", l_SerializedConfig);
                ShowMessageModal($"Succefully exported {CConfig.Instance.Profiles[CConfig.Instance.SelectedProfileIndex].ProfileName}");
            } catch (System.Exception l_E)
            {
                ShowMessageModal($"Error on exporting : {l_E.Message}");
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
                CConfig.ClockConfig l_NewConfig = JsonConvert.DeserializeObject<CConfig.ClockConfig>(System.IO.File.ReadAllText(l_FileName), CConfig.Instance.GetConverters().ToArray());
                CConfig.Instance.Profiles.Add(l_NewConfig);
                l_NewConfig.ProfileName = $"{l_NewConfig.ProfileName} (Imported)";
                ShowMessageModal($"Succefully Imported Config {l_NewConfig.ProfileName}");
                CloseImportFrame();
                RefreshProfilesList();
            } catch (System.Exception l_E)
            {
                ShowMessageModal($"Error on import : {l_E.Message}");
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
            var l_Profile = CConfig.Instance.GetActiveConfig();

            l_Profile.EnableClockGrabbing   = m_EnableClockGrabbing.Value;
            l_Profile.EnableAnchors         = m_EnableAnchors.Value;
            l_Profile.SeparateDayHours      = m_BoolSeparateDayHours.Value;
            l_Profile.BoolAmPm              = m_BoolAmPm.Value;
            l_Profile.Separator             = m_StringElementsSeparator.Text;
            l_Profile.UseGradient           = m_Bool_UseClockGradient.Value;
            l_Profile.UseFourColorsGradient = m_Bool_UseFourGradientColors.Value;
            l_Profile.ClockColor            = m_Color_Clock.CurrentColor;
            l_Profile.ClockGradientColor1   = m_Color_Clock1.CurrentColor;
            l_Profile.ClockGradientColor2   = m_Color_Clock2.CurrentColor;
            l_Profile.ClockGradientColor3   = m_Color_Clock3.CurrentColor;
            l_Profile.ClockGradientColor4   = m_Color_Clock4.CurrentColor;

            m_Color_Clock1.interactable = CConfig.Instance.GetActiveConfig().UseGradient;
            m_Color_Clock2.interactable = CConfig.Instance.GetActiveConfig().UseGradient;
            m_Color_Clock3.interactable = CConfig.Instance.GetActiveConfig().UseFourColorsGradient && CConfig.Instance.GetActiveConfig().UseGradient;
            m_Color_Clock4.interactable = CConfig.Instance.GetActiveConfig().UseFourColorsGradient && CConfig.Instance.GetActiveConfig().UseGradient;

            Clock.Instance.SaveConfig();

            //ClockFloatingScreen.Instance.ApplySettings();
        }

        private void OnFontSizeChanged(object p_Value)
        {
            CConfig.Instance.GetActiveConfig().FontSize = Clock.CLOCK_FONT_SIZE_MULTIPLIER * (((float)p_Value * 10) * 300 / 100);
            Clock.InvokeOnSettingChanged();
            Clock.Instance.SaveConfig();
        }

        private void OnFontSelected(object p_Value)
        {
            CConfig.Instance.GetActiveConfig().FontName = (string)p_Value;
            CConfig.Instance.Save();
            Clock.ApplyFont();
        }

        private void OnMovementModeSelected(object p_Value)
        {
            var l_MenuTransitionsHelper = Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().FirstOrDefault();
            var l_StandardLevelScenesTransitionSetupData = l_MenuTransitionsHelper.GetField<StandardLevelScenesTransitionSetupDataSO, MenuTransitionsHelper>("_standardLevelScenesTransitionSetupData");
            var l_StandardGameplaySceneInfo = l_StandardLevelScenesTransitionSetupData.GetField<SceneInfo, StandardLevelScenesTransitionSetupDataSO>("_standardGameplaySceneInfo");
            var l_GameCoreSceneInfo = l_StandardLevelScenesTransitionSetupData.GetField<SceneInfo, StandardLevelScenesTransitionSetupDataSO>("_gameCoreSceneInfo");
            PlayerData l_PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().FirstOrDefault().playerData;

            Dictionary<EnvironmentTypeSO, EnvironmentInfoSO> l_Data = l_PlayerData.overrideEnvironmentSettings.GetField<Dictionary<EnvironmentTypeSO, EnvironmentInfoSO>, OverrideEnvironmentSettings>("_data");

            switch ((string)p_Value)
            {
                case "Game":
                    if (LoadEnvironment() == false) { m_ClockMovementMode.Value = "Menu"; m_ClockMovementMode.ApplyValue(); return; }

                    Clock.m_MovementMode = BeatSaberPlus.SDK.Game.Logic.SceneType.Playing;
                    ClockFloatingScreen.Instance.SetClockPositionByScene(BeatSaberPlus.SDK.Game.Logic.SceneType.Playing);
                    break;
                case "Menu":
                    UnloadEnvironment();

                    Clock.m_MovementMode = BeatSaberPlus.SDK.Game.Logic.SceneType.Menu;
                    ClockFloatingScreen.Instance.SetClockPositionByScene(BeatSaberPlus.SDK.Game.Logic.SceneType.Menu);
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
            CConfig.Instance.GetActiveConfig().Separator = p_Value;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #region Font
        internal void UpdateFontsList()
        {
            List<object> l_NewFonts = new List<object>();
            foreach (var l_Current in Clock.m_AvailableFonts)
                l_NewFonts.Add(l_Current.name);
            m_Fonts = l_NewFonts;
            m_FontDropdown.dropdown.ReloadData();
            SelectFont(CConfig.Instance.GetActiveConfig().FontName, true);
        }

        internal void SelectFont(string p_Name, bool p_ApplyOnDropdown)
        {
            foreach (var l_Current in Clock.m_AvailableFonts)
                if (p_Name == l_Current.name)
                {
                    CConfig.Instance.GetActiveConfig().FontName = p_Name;
                    if (p_ApplyOnDropdown) { m_FontDropdown.Value = p_Name; m_FontDropdown.ApplyValue(); }
                    return;
                }
            CConfig.Instance.GetActiveConfig().FontName = Clock.m_AvailableFonts[0].name;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private bool LoadEnvironment(Action p_Callback = null)
        {

            //var l_MapSelectionViewController = Resources.FindObjectsOfTypeAll<LevelSelectionNavigationController>().First();
            StandardLevelDetailViewController l_DetailViewController = null;

            l_DetailViewController = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().First();

            if (l_DetailViewController.beatmapLevel == null || l_DetailViewController.selectedDifficultyBeatmap == null) { ShowMessageModal("Please go to solo menu before using this functionnality"); return false; }

            var l_GameSceneManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();
            var l_MainSettings = Resources.FindObjectsOfTypeAll<SettingsFlowCoordinator>().FirstOrDefault();

            l_GameSceneManager.MarkSceneAsPersistent("MenuCore");

//            var l_TutorialSceneSetup = Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().FirstOrDefault().GetField<TutorialScenesTransitionSetupDataSO, MenuTransitionsHelper>("_tutorialScenesTransitionSetupData");
            var l_TutorialSceneSetup = Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().FirstOrDefault().GetField<StandardLevelScenesTransitionSetupDataSO, MenuTransitionsHelper>("_standardLevelScenesTransitionSetupData");

            var l_PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;

            l_TutorialSceneSetup.Init("Standard", l_DetailViewController.selectedDifficultyBeatmap, l_DetailViewController.beatmapLevel, l_PlayerData.overrideEnvironmentSettings,
                l_PlayerData.colorSchemesSettings.GetSelectedColorScheme(), l_PlayerData.gameplayModifiers, l_PlayerData.playerSpecificSettings, l_PlayerData.practiceSettings, "No");

            var l_MenuEnvironmentManager = Resources.FindObjectsOfTypeAll<MenuEnvironmentManager>().FirstOrDefault();
            l_MenuEnvironmentManager.ShowEnvironmentType(MenuEnvironmentManager.MenuEnvironmentType.None);

            l_GameSceneManager.PushScenes(l_TutorialSceneSetup, 0.25f, null, (_) =>
            {
                //Resources.FindObjectsOfTypeAll<BeatSaberPlus.UI.MainViewFlowCoordinator>().FirstOrDefault().SetField("showBackButton", false);

                Transform l_Gameplay = GameObject.Find("StandardGameplay").transform;

                l_MenuEnvironmentManager.transform.root.gameObject.SetActive(true);

                Resources.FindObjectsOfTypeAll<MenuShockwave>().FirstOrDefault().gameObject.SetActive(false);

                Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().FirstOrDefault().CrossfadeToDefault();

                GameObject.Find("NarrowGameHUD").gameObject.SetActive(false);

                var l_CountersPlus = IPA.Loader.PluginManager.GetPluginFromId("Counters+");
                if (l_CountersPlus != null)
                    GameObject.Find("Counters+ | Main Canvas");

                if (!Environment.GetCommandLineArgs().Any(p_Arg => p_Arg.ToLowerInvariant() == "fpfc"))
                {
                    GameObject.Find("EventSystem").gameObject.SetActive(true);
                    GameObject.Find("ControllerLeft").gameObject.SetActive(true);
                    GameObject.Find("ControllerRight").gameObject.SetActive(true);
                } else
                {

                    foreach (Transform l_Child in l_Gameplay)
                        l_Child.gameObject.SetActive(false);
                }

                p_Callback?.Invoke();
            });

            return true;
        }

        private void UnloadEnvironment()
        {
            GameScenesManager l_ScenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();
            l_ScenesManager.PopScenes(0.25f, null, (_) =>
            {
                //Resources.FindObjectsOfTypeAll<BeatSaberPlus.UI.MainViewFlowCoordinator>().FirstOrDefault().SetField("showBackButton", true);
                HashSet<string> l_Scenes = l_ScenesManager.GetField<HashSet<string>, GameScenesManager>("_neverUnloadScenes");
                l_Scenes.Remove("MenuCore");
                l_ScenesManager.SetField("_neverUnloadScenes", l_Scenes);
                Resources.FindObjectsOfTypeAll<FadeInOutController>().FirstOrDefault().FadeIn();
                Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().FirstOrDefault().CrossfadeToDefault();
                Resources.FindObjectsOfTypeAll<MenuEnvironmentManager>().First().ShowEnvironmentType(MenuEnvironmentManager.MenuEnvironmentType.Default);
            });
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
