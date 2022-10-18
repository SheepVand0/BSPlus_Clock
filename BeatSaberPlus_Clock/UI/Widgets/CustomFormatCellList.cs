using BeatSaberMarkupLanguage.Attributes;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage;
using System.Reflection;

namespace BeatSaberPlus_Clock.UI.Widgets
{
    internal class CustomFormatCellList : CustomUIComponent
    {
        public override string GetResourceDescription()
        {
            return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), $"{Plugin.AssemblyName}.UI.Widgets.CustomFormatCellList.bsml");
        }

        [UIObject("AddButtonTransform")] GameObject m_ButtonParent = null;
        [UIComponent("FormatSettingList")] CustomCellListTableData m_FormatSettingsList = null;

        [UIValue("FormatSettings")] List<object> m_ListContent = new List<object>();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal CustomKeyboard m_Keyboard = null;

        private Button m_AddButton = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override void PostCreate()
        {
            m_Keyboard = Create<CustomKeyboard>(transform, true);
            m_AddButton = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_ButtonParent.transform, "Add", AddFormat, "Add Another thing to format", 20, 10);
            LoadFromConfig();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

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
