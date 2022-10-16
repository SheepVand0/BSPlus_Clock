﻿using BeatSaberMarkupLanguage.Attributes;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using BeatSaberMarkupLanguage.Components;

namespace BeatSaberPlus_Clock.UI.Components
{
    internal class FormatSetting
    {
        #region Components
        [UIComponent("HorizontalLayout")] private readonly HorizontalLayoutGroup m_HorizontalLayout = null;

        CustomStringSetting m_StringFormatSettingValue = null;
        CustomFormatCellList m_ParentList = null;

        Button m_ButtonDown = null;
        Button m_ButtonUp = null;
        #endregion

        [UIValue("FormatSettingValue")] private string SelectedFormat { get => string.Empty; set => ApplyFormatSettingOnconfig(value); }

        internal string ValueName { get; set; }

        private int ValueIndex { get; set; }

        internal FormatSetting(CustomFormatCellList p_ParentList, string p_ValueName, int p_Index)
        {
            m_ParentList = p_ParentList;
            ValueName = p_ValueName;
            ValueIndex = p_Index;
        }

        [UIAction("#post-parse")]
        void PostParse()
        {
            m_StringFormatSettingValue = CustomUIComponent.Create<CustomStringSetting>(m_HorizontalLayout.transform, true);
            m_StringFormatSettingValue.Setup(ValueName, 32, false, m_ParentList.m_Keyboard);
            m_StringFormatSettingValue.OnChange += ApplyFormatSettingOnconfig;

            m_ButtonDown = BeatSaberPlus.SDK.UI.Button.Create(m_HorizontalLayout.transform, "🔽", MoveDown);
            m_ButtonUp = BeatSaberPlus.SDK.UI.Button.Create(m_HorizontalLayout.transform, "🔼", MoveUp);
            BeatSaberPlus.SDK.UI.Button.Create(m_HorizontalLayout.transform, "-", RemoveFormatSetting, "Remove current setting");

            UpdateUpAndDownButtons();
        }

        internal void UpdateUpAndDownButtons()
        {
            m_ButtonDown.interactable = ValueIndex != CConfig.Instance.GetActiveConfig().FormatOrder.Count - 1;
            m_ButtonUp.interactable = ValueIndex != 0;
        }

        private void MoveUp()
        {
            for (int l_i = 0; l_i < CConfig.Instance.GetActiveConfig().FormatOrder.Count; l_i++)
            {
                if (CConfig.Instance.GetActiveConfig().FormatOrder[l_i] != ValueName) continue;

                CConfig.Instance.GetActiveConfig().FormatOrder[l_i] = CConfig.Instance.GetActiveConfig().FormatOrder[l_i - 1];
                CConfig.Instance.GetActiveConfig().FormatOrder[l_i - 1] = ValueName;
                break;
            }
            Clock.Instance.SaveConfig();
            m_ParentList.LoadFromConfig();
            UpdateUpAndDownButtons();
        }

        private void MoveDown()
        {
            for (int l_i = 0; l_i < CConfig.Instance.GetActiveConfig().FormatOrder.Count; l_i++)
            {
                if (CConfig.Instance.GetActiveConfig().FormatOrder[l_i] != ValueName) continue;

                CConfig.Instance.GetActiveConfig().FormatOrder[l_i] = CConfig.Instance.GetActiveConfig().FormatOrder[l_i + 1];
                CConfig.Instance.GetActiveConfig().FormatOrder[l_i + 1] = ValueName;
                break;
            }

            Clock.Instance.SaveConfig();
            m_ParentList.LoadFromConfig();
            UpdateUpAndDownButtons();
        }

        private void ApplyFormatSettingOnconfig(string p_Value)
        {
            if (string.IsNullOrEmpty(p_Value)) return;

            for (int l_i = 0; l_i < CConfig.Instance.GetActiveConfig().FormatOrder.Count; l_i++)
                if (CConfig.Instance.GetActiveConfig().FormatOrder[l_i] == ValueName)
                    CConfig.Instance.GetActiveConfig().FormatOrder[l_i] = p_Value;
            ValueName = p_Value;
            Logger.Instance.Info(ValueName);
            Clock.Instance.SaveConfig();
        }

        private void RemoveFormatSetting()
        {
            for (int l_i = 0; l_i < CConfig.Instance.GetActiveConfig().FormatOrder.Count; l_i++)
            {
                if (CConfig.Instance.GetActiveConfig().FormatOrder[l_i] != ValueName) continue;

                CConfig.Instance.GetActiveConfig().FormatOrder.RemoveAt(l_i);
            }
            Clock.Instance.SaveConfig();
            if (m_ParentList != null) m_ParentList.LoadFromConfig();
        }
    }

    internal class CustomFormatCellList : CustomUIComponent
    {
        internal override string GetResourceName()
        {
            return $"{Plugin.AssemblyName}.UI.Components.Views.{GetType().Name}.bsml";
        }

        [UIObject("AddButtonTransform")] GameObject m_ButtonParent = null;
        [UIComponent("FormatSettingList")] CustomCellListTableData m_FormatSettingsList = null;

        [UIValue("FormatSettings")] List<object> m_ListContent = new List<object>();

        internal CustomKeyboard m_Keyboard = null;

        private Button m_AddButton = null;

        [UIAction("#post-parse")]
        private void PostParse()
        {
            m_Keyboard = Create<CustomKeyboard>(transform, true);
            m_AddButton = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_ButtonParent.transform, "Add", AddFormat, "Add Another thing to format", 20, 10);
            LoadFromConfig();
        }

        private void AddFormat()
        {
            CConfig.Instance.GetActiveConfig().FormatOrder.Add("NewValue");
            Clock.Instance.SaveConfig();
            LoadFromConfig();
        }

        internal void LoadFromConfig()
        {
            List<string> l_FormatOrder = CConfig.Instance.GetActiveConfig().FormatOrder;
            m_ListContent.Clear();
            for (int l_i = 0; l_i < l_FormatOrder.Count; l_i++)
                m_ListContent.Add(new FormatSetting(this, l_FormatOrder[l_i], l_i));
            m_FormatSettingsList.tableView.ReloadData();
        }
    }
}
