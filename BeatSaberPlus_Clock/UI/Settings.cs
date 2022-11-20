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
using CP_SDK.Unity;

namespace BeatSaberPlus_Clock.UI
{
    /// <summary>
    /// Clock settings view controllers
    /// </summary>
    internal class Settings : BeatSaberPlus.SDK.UI.ResourceViewController<Settings>
    {
        /// Tabs
        [UIObject("Tabs")] private readonly GameObject m_Tabs = null;

        [UIObject("TabSelector")] GameObject m_TabSelector = null;

        [UIObject("TabProfiles")] readonly GameObject m_Tab_Profiles = null;
        [UIObject("TabGeneral")]  readonly GameObject m_Tab_General  = null;
        [UIObject("TabFonts")]    readonly GameObject m_Tab_Font     = null;
        [UIObject("TabFormat")]   readonly GameObject m_Tab_Format   = null;
        [UIObject("TabColor")]    readonly GameObject m_Tab_Color    = null;
        [UIObject("TabPosition")] readonly GameObject m_Tab_Position = null;

        ////////////////////////////////////////////////////////////////////////////
        /// Profiles


        [UIObject("ProfileListFrame_Background")]   readonly GameObject m_ProfileListBackground = null;
        [UIObject("ProfilesList")]                  readonly GameObject m_ProfilesListView      = null;
        [UIObject("ImportProfileFrame")]            readonly GameObject m_ImportProfileFrame    = null;
        [UIObject("ImportProfileFrame_Background")] readonly GameObject m_ImportProfileFrameBackground = null;
        [UIObject("ImportFrameButtonsTransform")]   readonly GameObject m_ImportFrameButtonsTransform  = null;

        [UIComponent("ProfilesUpButton")]            readonly Button m_ProfilesUpButton   = null;
        [UIComponent("ProfilesDownButton")]          readonly Button m_ProfilesDownButton = null;
        [UIComponent("ProfilesManagementButtons")]   readonly HorizontalLayoutGroup m_ProfilesManagementBox = null;
        [UIComponent("ImportProfileFrame_DropDown")] readonly DropDownListSetting m_ImportProfileFrame_DropDown = null;

        BeatSaberPlus.SDK.UI.DataSource.SimpleTextList m_EventsList = null;

        CustomKeyboard m_ProfileRenameKeyboard = null;
        CustomKeyboard m_ProfileCreateKeyboard = null;

        Button m_PrimaryNewProfileButton    = null;
        Button m_PrimaryRenameProfileButton = null;
        Button m_PrimaryDeleteProfileButton = null;
        Button m_ExportButton               = null;
        Button m_ImportButton               = null;

        Button m_ConfirmImport = null;
        Button m_CancelImport  = null;

        private int m_CurrentProfilePage = 0;
        private int m_PageCount = 1;

        private string m_SelectedImportProfile = string.Empty;

        [UIValue("ImportProfileFrame_DropDownOptions")]
        List<object> m_ImportProfileFrame_DropDownOptions = new List<object>();

        ////////////////////////////////////////////////////////////////////////////
        /// General Settings

        [UIComponent("BoolSeparateDayHours")] ToggleSetting m_BoolSeparateDayHours = null;
        [UIComponent("BoolAmPm")]             ToggleSetting m_BoolAmPm = null;

        ////////////////////////////////////////////////////////////////////////////
        /// Font managing

        [UIComponent("SliderFontSize")]  readonly SliderSetting       m_Slider_FontSize = null;
        [UIComponent("FontDropdown")]    readonly DropDownListSetting m_FontDropdown = null;
        [UIObject("FontsRefreshLayout")] readonly GameObject          m_FontRefreshObject = null;

        Button m_FontRefreshButton = null;
        [UIValue("FontValue")] private string FontValue
        {
            get => string.Empty;
            set { }
        }
        [UIValue("Fonts")] private List<object> m_Fonts = new List<object>()
        {
            "1"
        };

        [UIComponent("BoolFontBold")]       private readonly ToggleSetting m_BoolFontBold       = null;
        [UIComponent("BoolFontItalic")]     private readonly ToggleSetting m_BoolFontItalic     = null;
        [UIComponent("BoolFontUnderlined")] private readonly ToggleSetting m_BoolFontUnderlined = null;

        ////////////////////////////////////////////////////////////////////////////
        /// Clock format

        [UIObject("FormatElementsSeparatorTransform")] private readonly GameObject FormatElementsSeparatorTransform = null;
        [UIObject("FormatTransform")]                  private readonly GameObject m_FormatListTransform            = null;

        Button m_DocButton = null;

        private CustomStringSetting m_StringElementsSeparator = null;
        private CustomFormatCellList m_FormatSettingsList = null;

        ////////////////////////////////////////////////////////////////////////////
        /// Clock colors

        [UIComponent("BoolUseClockGradient")]           readonly ToggleSetting m_Bool_UseClockGradient      = null;
        [UIComponent("BoolUseFourClockGradientColors")] readonly ToggleSetting m_Bool_UseFourGradientColors = null;
        [UIComponent("BoolClockColor")] readonly ColorSetting m_Color_Clock  = null;
        [UIComponent("ColorClock1")]    readonly ColorSetting m_Color_Clock1 = null;
        [UIComponent("ColorClock2")]    readonly ColorSetting m_Color_Clock2 = null;
        [UIComponent("ColorClock3")]    readonly ColorSetting m_Color_Clock3 = null;
        [UIComponent("ColorClock4")]    readonly ColorSetting m_Color_Clock4 = null;

        ////////////////////////////////////////////////////////////////////////////
        /// Clock position

        [UIComponent("EnableClockGrabbing")] readonly ToggleSetting       m_EnableClockGrabbing = null;
        [UIComponent("ClockMovementMode")]   readonly DropDownListSetting m_ClockMovementMode   = null;
        [UIComponent("EnableAnchors")]       readonly ToggleSetting       m_EnableAnchors       = null;

        [UIValue("ClockMovementModeList")] private List<object> m_ClockMovementChoices = new List<object>
        {
            "Menu",
            "Game"
        };
        [UIValue("MovementMode")] private string DropdownMovementMode
        {
            get => "Menu";
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        ////////////////////////////////////////////////////////////////////////////
        /// Others

        private CustomTextSegmentedControl m_TabSelector_Control = null;

        private float m_FontPercent = 0f;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On view creation
        /// </summary>
        protected override sealed void OnViewCreation()
        {
            var l_Event = new BeatSaberMarkupLanguage.Parser.BSMLAction(this, this.GetType().GetMethod(nameof(Settings.OnSettingChanged), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
            var l_FontSizeEvent = new BeatSaberMarkupLanguage.Parser.BSMLAction(this, this.GetType().GetMethod(nameof(Settings.OnFontSizeChanged), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
            var l_FontDropdownEvent = new BeatSaberMarkupLanguage.Parser.BSMLAction(this, this.GetType().GetMethod(nameof(Settings.OnFontSelected), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
            var l_ClockMovementDropdownEvent = new BeatSaberMarkupLanguage.Parser.BSMLAction(this, this.GetType().GetMethod(nameof(Settings.OnMovementModeSelected), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
            var l_ClockImportDropdownEvent = new BeatSaberMarkupLanguage.Parser.BSMLAction(this, this.GetType().GetMethod(nameof(Settings.OnImportProfileSelected), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

            ////////////////////////////////////////////////////////////////////////////
            /// Tabs

            m_TabSelector_Control = new CustomTextSegmentedControl(m_TabSelector.transform as RectTransform, false, new List<Widgets.Tab>
            {
                new Widgets.Tab("Profiles", m_Tab_Profiles.gameObject),
                new Widgets.Tab("General", m_Tab_General.gameObject),
                new Widgets.Tab("Font", m_Tab_Font.gameObject),
                new Widgets.Tab("Format", m_Tab_Format.gameObject),
                new Widgets.Tab("Color", m_Tab_Color.gameObject),
                new Widgets.Tab("Position", m_Tab_Position.gameObject)
            });

            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_Tabs.gameObject, 0.5f);

            ////////////////////////////////////////////////////////////////////////////
            /// Profiles

            var l_Config = CConfig.Instance.GetActiveConfig();

            m_PrimaryNewProfileButton = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_ProfilesManagementBox.transform, "NEW", CreateProfile, p_PreferedWidth: 25);
            m_PrimaryDeleteProfileButton = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_ProfilesManagementBox.transform, "DELETE", DeleteProfile, p_PreferedWidth: 25);
            m_PrimaryRenameProfileButton = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_ProfilesManagementBox.transform, "RENAME", RenameProfile, p_PreferedWidth: 25);
            m_ExportButton = BeatSaberPlus.SDK.UI.Button.Create(m_ProfilesManagementBox.transform, "EXPORT", ExportCurrentProfile, p_PreferedWidth: 25);
            m_ImportButton = BeatSaberPlus.SDK.UI.Button.Create(m_ProfilesManagementBox.transform, "IMPORT", OpenImportFrame, p_PreferedWidth: 25);

            m_ProfilesUpButton.onClick.AddListener(ProfilePageUp);
            m_ProfilesDownButton.onClick.AddListener(ProfilePageDown);

            m_CancelImport = BeatSaberPlus.SDK.UI.Button.Create(m_ImportFrameButtonsTransform.transform, "Cancel", CloseImportFrame, p_PreferedWidth: 25);
            m_ConfirmImport = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_ImportFrameButtonsTransform.transform, "Import", ImportProfile, p_PreferedWidth: 25);

            BeatSaberPlus.SDK.UI.DropDownListSetting.Setup(m_ImportProfileFrame_DropDown, l_ClockImportDropdownEvent, true);

            m_ProfilesUpButton.transform.localScale = Vector3.one * 0.5f;
            m_ProfilesDownButton.transform.localScale = Vector3.one * 0.5f;

            var l_BSMLTableView = m_ProfilesListView.GetComponentInChildren<BSMLTableView>();
            l_BSMLTableView.SetDataSource(null, false);
            DestroyImmediate(m_ProfilesListView.GetComponentInChildren<CustomListTableData>());
            m_EventsList = l_BSMLTableView.gameObject.AddComponent<BeatSaberPlus.SDK.UI.DataSource.SimpleTextList>();
            m_EventsList.TableViewInstance = l_BSMLTableView;
            m_EventsList.CellSizeValue = 4.8f;
            l_BSMLTableView.didSelectCellWithIdxEvent += OnProfileSelected;
            l_BSMLTableView.SetDataSource(m_EventsList, false);

            RefreshProfilesList();

            ////////////////////////////////////////////////////////////////////////////
            /// General settings

            m_StringElementsSeparator = CustomUIComponent.Create<CustomStringSetting>(FormatElementsSeparatorTransform.transform, true, (p_Item) =>
            {
                p_Item.Setup(l_Config.Separator, 32, true);
                p_Item.OnChange += OnSeparatorChange;
            });
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolSeparateDayHours, l_Event, l_Config.SeparateDayHours, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolAmPm, l_Event, l_Config.ShowAmPm, true);
            //UpdateFontsList();

            ////////////////////////////////////////////////////////////////////////////
            /// Font managing

            BeatSaberPlus.SDK.UI.DropDownListSetting.Setup(m_FontDropdown, l_FontDropdownEvent, true);
            Clock.e_OnFontsLoaded += OnFontsLoaded;
            OnFontsLoaded();
            m_FontRefreshButton = BeatSaberPlus.SDK.UI.Button.Create(m_FontRefreshObject.transform, "Refresh fonts",
                () => { MTCoroutineStarter.Start(Clock.LoadFonts()); });

            m_FontPercent = ((l_Config.FontSize / 10) * 100 / 300) / Clock.CLOCK_FONT_SIZE_MULTIPLIER;
            BeatSaberPlus.SDK.UI.SliderSetting.Setup(m_Slider_FontSize, l_FontSizeEvent, BeatSaberPlus.SDK.UI.BSMLSettingFormartter.Percentage, m_FontPercent, true);

            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolFontBold, l_Event, l_Config.FontBold, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolFontItalic, l_Event, l_Config.FontItalic, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_BoolFontUnderlined, l_Event, l_Config.FontUnderlined, true);

            ////////////////////////////////////////////////////////////////////////////
            /// Format

            m_FormatSettingsList = CustomUIComponent.Create<CustomFormatCellList>(m_FormatListTransform.transform, true);

            ////////////////////////////////////////////////////////////////////////////
            /// Colors

            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_Bool_UseClockGradient, l_Event, l_Config.UseGradient, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_Bool_UseFourGradientColors, l_Event, l_Config.UseFourColorsGradient, true);

            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock, l_Event, l_Config.ClockColor, true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock1, l_Event, l_Config.ClockGradientColor1, true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock2, l_Event, l_Config.ClockGradientColor2, true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock3, l_Event, l_Config.ClockGradientColor3, true);
            BeatSaberPlus.SDK.UI.ColorSetting.Setup(m_Color_Clock4, l_Event, l_Config.ClockGradientColor4, true);

            m_Color_Clock2.interactable = l_Config.UseGradient;
            m_Color_Clock3.interactable = l_Config.UseFourColorsGradient;
            m_Color_Clock4.interactable = l_Config.UseFourColorsGradient;

            ////////////////////////////////////////////////////////////////////////////
            /// Position

            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_EnableClockGrabbing, l_Event, l_Config.EnableClockGrabbing, true);
            BeatSaberPlus.SDK.UI.ToggleSetting.Setup(m_EnableAnchors, l_Event, l_Config.EnableAnchors, true);
            BeatSaberPlus.SDK.UI.DropDownListSetting.Setup(m_ClockMovementMode, l_ClockMovementDropdownEvent, true);

            ////////////////////////////////////////////////////////////////////////////
            /// Event

            Clock.e_OnConfigLoaded += OnConfigLoaded;
        }

        /// <summary>
        /// On view deactivation
        /// </summary>
        protected override sealed void OnViewDeactivation()
        {
            Clock.Instance.SaveConfig();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event called when a config is loaded
        /// </summary>
        private void OnConfigLoaded()
        {
            m_EnableClockGrabbing.Value = CConfig.Instance.GetActiveConfig().EnableClockGrabbing;
            m_EnableAnchors.Value = CConfig.Instance.GetActiveConfig().EnableAnchors;
            m_BoolSeparateDayHours.Value = CConfig.Instance.GetActiveConfig().SeparateDayHours;
            m_BoolAmPm.Value = CConfig.Instance.GetActiveConfig().ShowAmPm;
            m_StringElementsSeparator.ApplyValue(CConfig.Instance.GetActiveConfig().Separator);
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

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Event called when a profile is selected
        /// </summary>
        /// <param name="p_TableView"></param>
        /// <param name="p_Index"></param>
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnProfileSelected(HMUI.TableView p_TableView, int p_Index)
        {
            CConfig.Instance.SelectedProfileIndex = p_Index;
            CConfig.Instance.Save();
            Clock.InvokeOnConfigLoaded();
        }

        /// <summary>
        /// Rebuild the list of profiles
        /// </summary>
        private void RefreshProfilesList()
        {
            m_EventsList.TableViewInstance.ClearSelection();
            m_EventsList.Data.Clear();

            if (CConfig.Instance.Profiles.Count >= Clock.EVENT_PER_PAGES)
            {
                // ReSharper disable once PossibleLossOfFraction
                m_PageCount = (int)System.Math.Floor((float)(CConfig.Instance.Profiles.Count / Clock.EVENT_PER_PAGES));
                if (m_CurrentProfilePage > m_PageCount)
                    m_CurrentProfilePage = m_PageCount;

                m_ProfilesUpButton.interactable = !(m_CurrentProfilePage == 0);
                m_ProfilesDownButton.interactable = !(m_CurrentProfilePage == m_PageCount);
            }
            else
            {
                m_ProfilesUpButton.interactable = false;
                m_ProfilesDownButton.interactable = false;
                m_CurrentProfilePage = 0;
                m_PageCount = 1;
            }

            for (int l_i = 0; l_i < CConfig.Instance.Profiles.Count - (m_CurrentProfilePage * Clock.EVENT_PER_PAGES); l_i++)
            {
                m_EventsList.Data.Add((CConfig.Instance.Profiles[l_i + (m_CurrentProfilePage * Clock.EVENT_PER_PAGES)].ProfileName, null));
            }

            m_EventsList.TableViewInstance.ReloadData();
            if (CConfig.Instance.SelectedProfileIndex > m_EventsList.Data.Count)
                m_EventsList.TableViewInstance.SelectCellWithIdx(0);
            else
                m_EventsList.TableViewInstance.SelectCellWithIdx(CConfig.Instance.SelectedProfileIndex);
        }
        /// <summary>
        /// Go one page up on profiles list
        /// </summary>
        private void ProfilePageUp()
        {
            m_CurrentProfilePage -= 1;
            RefreshProfilesList();
        }

        /// <summary>
        /// Go one page down on profiles list
        /// </summary>
        private void ProfilePageDown()
        {
            m_CurrentProfilePage += 1;
            RefreshProfilesList();
        }

        /// <summary>
        /// Create new profile by clicking the button
        /// </summary>
        private void CreateProfile()
        {
            if (m_ProfileCreateKeyboard == null)
            {
                m_ProfileCreateKeyboard = CustomUIComponent.Create<CustomKeyboard>(m_ProfilesManagementBox.transform, true, (p_Item) =>
                {
                    p_Item.OnKeyboardEnterPressed += (p_OldValue, p_NewValue) =>
                    {
                        if (p_NewValue == string.Empty)
                        {
                            ShowMessageModal("Configs Names Can't be empty");
                            return;
                        }
                        CConfig.Instance.Profiles.Add(new CConfig.ClockConfig(p_NewValue));
                        CConfig.Instance.SelectedProfileIndex = CConfig.Instance.Profiles.Count - 1;
                        CConfig.Instance.Save();
                        Clock.InvokeOnConfigLoaded();
                    };
                });
            }

            m_ProfileCreateKeyboard.Open(string.Empty);
        }
        /// <summary>
        /// Delete selected profile by clicking the button
        /// </summary>
        private void DeleteProfile()
        {
            if (CConfig.Instance.Profiles.Count == 1)
            {
                ShowMessageModal("You can't delete a config when you only own one");
                return;
            }

            string l_Name = CConfig.Instance.GetActiveConfig().ProfileName;

            CConfig.Instance.Profiles.RemoveAt(CConfig.Instance.SelectedProfileIndex);

            CConfig.Instance.SelectedProfileIndex -= 1;
            Clock.InvokeOnConfigLoaded();
            CConfig.Instance.Save();
            RefreshProfilesList();

            ShowMessageModal($"Successfully deleted profile : {l_Name}");
        }
        /// <summary>
        /// Rename profile by clicking the button
        /// </summary>
        private void RenameProfile()
        {
            if (m_ProfileRenameKeyboard == null)
            {
                m_ProfileRenameKeyboard = CustomUIComponent.Create<CustomKeyboard>(m_ProfilesManagementBox.transform, true, (p_Item) =>
                {
                    p_Item.OnKeyboardEnterPressed += (p_CurrentName, p_NewName) =>
                    {
                        if (p_NewName == string.Empty)
                        {
                            ShowMessageModal("Configs Names Can't be empty");
                            return;
                        }
                        CConfig.Instance.Profiles[CConfig.Instance.SelectedProfileIndex].ProfileName = p_NewName;
                        CConfig.Instance.Save();
                        Clock.InvokeOnConfigLoaded();
                    };
                });
            }

            m_ProfileRenameKeyboard.Open(CConfig.Instance.Profiles[CConfig.Instance.SelectedProfileIndex].ProfileName);
        }
        /// <summary>
        /// Exporting profile by confirming
        /// </summary>
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
                ShowMessageModal($"Successfully exported {CConfig.Instance.Profiles[CConfig.Instance.SelectedProfileIndex].ProfileName}");
            }
            catch (System.Exception l_E)
            {
                ShowMessageModal($"Error on exporting : {l_E.Message}");
                Logger.Instance.Error(l_E, nameof(Settings), nameof(ExportCurrentProfile));
            }
        }
        /// <summary>
        /// Show import profile frame
        /// </summary>
        private void OpenImportFrame()
        {
            m_Tabs.gameObject.SetActive(false);
            m_ImportProfileFrame.gameObject.SetActive(true);

            if (!System.IO.Directory.Exists(Clock.CLOCK_IMPORT_FOLDER))
                System.IO.Directory.CreateDirectory(Clock.CLOCK_IMPORT_FOLDER);

            List<object> l_Files = new();
            foreach (string l_File in System.IO.Directory.GetFiles(Clock.CLOCK_IMPORT_FOLDER, "*.bspclock"))
                l_Files.Add(System.IO.Path.GetFileNameWithoutExtension(l_File));

            if (l_Files.Count == 0)
            {
                ShowMessageModal("No config to import was found");
                CloseImportFrame();
                return;
            }

            m_ImportProfileFrame_DropDownOptions = l_Files;
            m_ImportProfileFrame_DropDown.values = l_Files;
            m_ImportProfileFrame_DropDown.UpdateChoices();
        }
        /// <summary>
        /// Close import profile frame
        /// </summary>
        private void CloseImportFrame()
        {
            m_Tabs.gameObject.SetActive(true);
            m_ImportProfileFrame.gameObject.SetActive(false);
        }
        /// <summary>
        /// Confirm profile import
        /// </summary>
        private void ImportProfile()
        {
            try
            {
                string l_FileName = $"{Clock.CLOCK_IMPORT_FOLDER}{m_SelectedImportProfile}.bspclock";
                if (!System.IO.File.Exists(l_FileName))
                {
                    ShowMessageModal("File not found");
                    return;
                }
                string l_NewConfigName = string.Empty;
                CConfig.ClockConfig l_NewConfig = JsonConvert.DeserializeObject<CConfig.ClockConfig>(System.IO.File.ReadAllText(l_FileName), CConfig.Instance.GetConverters().ToArray());
                CConfig.Instance.Profiles.Add(l_NewConfig);
                l_NewConfig.ProfileName = $"{l_NewConfig.ProfileName} (Imported)";
                ShowMessageModal($"Successfully Imported Config {l_NewConfig.ProfileName}");
                CloseImportFrame();
                RefreshProfilesList();
            }
            catch (System.Exception l_E)
            {
                ShowMessageModal($"Error on import : {l_E.Message}");
                Logger.Instance.Error(l_E, nameof(Settings), nameof(ImportProfile));
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event called when a setting has been changed
        /// </summary>
        /// <param name="p_Value"></param>
        private void OnSettingChanged(object p_Value)
        {
            var l_Profile = CConfig.Instance.GetActiveConfig();

            l_Profile.EnableClockGrabbing = m_EnableClockGrabbing.Value;
            l_Profile.EnableAnchors = m_EnableAnchors.Value;
            l_Profile.SeparateDayHours = m_BoolSeparateDayHours.Value;
            l_Profile.ShowAmPm = m_BoolAmPm.Value;
            l_Profile.Separator = m_StringElementsSeparator.Text;
            l_Profile.UseGradient = m_Bool_UseClockGradient.Value;
            l_Profile.UseFourColorsGradient = m_Bool_UseFourGradientColors.Value;
            l_Profile.ClockColor = m_Color_Clock.CurrentColor;
            l_Profile.ClockGradientColor1 = m_Color_Clock1.CurrentColor;
            l_Profile.ClockGradientColor2 = m_Color_Clock2.CurrentColor;
            l_Profile.ClockGradientColor3 = m_Color_Clock3.CurrentColor;
            l_Profile.ClockGradientColor4 = m_Color_Clock4.CurrentColor;
            l_Profile.FontBold = m_BoolFontBold.Value;
            l_Profile.FontItalic = m_BoolFontItalic.Value;
            l_Profile.FontUnderlined = m_BoolFontUnderlined.Value;

            m_Color_Clock1.interactable = CConfig.Instance.GetActiveConfig().UseGradient;
            m_Color_Clock2.interactable = CConfig.Instance.GetActiveConfig().UseGradient;
            m_Color_Clock3.interactable = CConfig.Instance.GetActiveConfig().UseFourColorsGradient && CConfig.Instance.GetActiveConfig().UseGradient;
            m_Color_Clock4.interactable = CConfig.Instance.GetActiveConfig().UseFourColorsGradient && CConfig.Instance.GetActiveConfig().UseGradient;

            Clock.Instance.SaveConfig();

            //ClockFloatingScreen.Instance.ApplySettings();
        }
        /// <summary>
        /// Event called when the font size has been changed
        /// </summary>
        /// <param name="p_Value"></param>
        private void OnFontSizeChanged(object p_Value)
        {
            CConfig.Instance.GetActiveConfig().FontSize = Clock.CLOCK_FONT_SIZE_MULTIPLIER * (((float)p_Value * 10) * 300 / 100);
            Clock.InvokeOnSettingChanged();
            Clock.Instance.SaveConfig();
        }
        /// <summary>
        /// Event called when a font has been selected
        /// </summary>
        /// <param name="p_Value"></param>
        private void OnFontSelected(object p_Value)
        {
            CConfig.Instance.GetActiveConfig().FontName = (string)p_Value;
            CConfig.Instance.Save();
            Clock.ApplyFont();
        }
        /// <summary>
        /// Eventy called when movement mode changed
        /// </summary>
        /// <param name="p_Value"></param>
        private void OnMovementModeSelected(object p_Value)
        {
            switch ((string)p_Value)
            {
                case "Game":
                    if (LoadEnvironment() == false)
                    {
                        m_ClockMovementMode.Value = "Menu";
                        m_ClockMovementMode.ApplyValue();
                        return;
                    }

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
        /// <summary>
        /// Event on a profile to import has been selected
        /// </summary>
        /// <param name="p_Value"></param>
        private void OnImportProfileSelected(object p_Value)
        {
            m_SelectedImportProfile = (string)p_Value;
        }
        /// <summary>
        /// Event when the format separator has been changed
        /// </summary>
        /// <param name="p_Value"></param>
        private void OnSeparatorChange(string p_Value)
        {
            CConfig.Instance.GetActiveConfig().Separator = p_Value;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event called when fonts has been loaded
        /// </summary>
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
        /// Refresh dropdown containing list of fonts
        /// </summary>
        internal void UpdateFontsList()
        {
            List<object> l_NewFonts = new List<object>();
            foreach (var l_Current in Clock.m_AvailableFonts)
                l_NewFonts.Add(l_Current.name);
            m_Fonts = l_NewFonts;
            m_FontDropdown.dropdown.ReloadData();
            SelectFont(CConfig.Instance.GetActiveConfig().FontName, true);
        }
        /// <summary>
        /// Select font from dropdown
        /// </summary>
        /// <param name="p_Name">Font name</param>
        /// <param name="p_ApplyOnDropdown">Apply change on dropdown</param>
        internal void SelectFont(string p_Name, bool p_ApplyOnDropdown)
        {
            foreach (var l_Current in Clock.m_AvailableFonts)
                if (p_Name == l_Current.name)
                {
                    CConfig.Instance.GetActiveConfig().FontName = p_Name;
                    if (p_ApplyOnDropdown)
                    {
                        m_FontDropdown.Value = p_Name;
                        m_FontDropdown.ApplyValue();
                    }
                    return;
                }
            CConfig.Instance.GetActiveConfig().FontName = Clock.m_AvailableFonts[0].name;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Load tutorial environment in menu
        /// </summary>
        /// <param name="p_Callback"></param>
        /// <returns></returns>
        private bool LoadEnvironment(Action p_Callback = null)
        {
            var l_GameSceneManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();
            var l_MainSettings = Resources.FindObjectsOfTypeAll<SettingsFlowCoordinator>().FirstOrDefault();
            var l_MenuEnvironmentManager = Resources.FindObjectsOfTypeAll<MenuEnvironmentManager>().FirstOrDefault();

            l_GameSceneManager.MarkSceneAsPersistent("MenuCore");

            var l_TutorialSceneSetup = Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().FirstOrDefault().GetField<TutorialScenesTransitionSetupDataSO, MenuTransitionsHelper>("_tutorialScenesTransitionSetupData");
            var l_PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;

            l_TutorialSceneSetup.Init(l_PlayerData.playerSpecificSettings);

            l_MenuEnvironmentManager.ShowEnvironmentType(MenuEnvironmentManager.MenuEnvironmentType.None);

            l_GameSceneManager.PushScenes(l_TutorialSceneSetup, 0.25f, null, (_) =>
            {
                Transform l_Gameplay = GameObject.Find("TutorialGameplay").transform;

                l_MenuEnvironmentManager.transform.root.gameObject.SetActive(true);

                Resources.FindObjectsOfTypeAll<MenuShockwave>().FirstOrDefault().gameObject.SetActive(false);
                Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().FirstOrDefault().CrossfadeToDefault();

                if (!Environment.GetCommandLineArgs().Any(p_Arg => p_Arg.ToLowerInvariant() == "fpfc"))
                {
                    GameObject.Find("EventSystem").gameObject.SetActive(true);
                    GameObject.Find("ControllerLeft").gameObject.SetActive(true);
                    GameObject.Find("ControllerRight").gameObject.SetActive(true);
                }

                foreach (Transform l_Child in l_Gameplay)
                    l_Child.gameObject.SetActive(false);

                p_Callback?.Invoke();
            });

            return true;
        }
        /// <summary>
        /// Unload tutorial environment from menu
        /// </summary>
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
