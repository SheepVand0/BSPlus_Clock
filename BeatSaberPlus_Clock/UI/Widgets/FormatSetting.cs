using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace BeatSaberPlus_Clock.UI.Widgets
{
    internal class FormatSetting
    {

        [UIComponent("HorizontalLayout")] private readonly HorizontalLayoutGroup m_HorizontalLayout = null;

        CustomStringSetting m_StringFormatSettingValue = null;
        CustomFormatCellList m_ParentList = null;

        Button m_ButtonDown = null;
        Button m_ButtonUp = null;

        [UIValue("FormatSettingValue")] private string SelectedFormat { get => string.Empty; set => ApplyFormatSettingOnconfig(value); }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal string ValueName { get; set; }

        private int ValueIndex { get; set; }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Consturctor
        /// </summary>
        /// <param name="p_ParentList">Parent list</param>
        /// <param name="p_ValueName">Default value</param>
        /// <param name="p_Index">Index in parent list</param>
        internal FormatSetting(CustomFormatCellList p_ParentList, string p_ValueName, int p_Index)
        {
            m_ParentList = p_ParentList;
            ValueName = p_ValueName;
            ValueIndex = p_Index;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

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

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check if Up and down buttons can to be pressed
        /// </summary>
        internal void UpdateUpAndDownButtons()
        {
            m_ButtonDown.interactable = ValueIndex != CConfig.Instance.GetActiveConfig().FormatOrder.Count - 1;
            m_ButtonUp.interactable = ValueIndex != 0;
        }

        /// <summary>
        /// Move Format up in list
        /// </summary>
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

        /// <summary>
        /// Move format down in list
        /// </summary>
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

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Change format in config
        /// </summary>
        /// <param name="p_Value"></param>
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

        /// <summary>
        /// Remove current format
        /// </summary>
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
}
