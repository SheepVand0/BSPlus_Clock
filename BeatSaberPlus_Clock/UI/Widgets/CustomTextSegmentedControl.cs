using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberPlus_Clock.UI.Widgets
{
    internal struct Tab
    {
        public string Name { get; set; }
        public GameObject Object { get; set; }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p_Name">Tab name</param>
        /// <param name="p_Object">Tab GameObject reference</param>
        internal Tab(string p_Name, GameObject p_Object)
        {
            Name = p_Name;
            Object = p_Object;
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    internal class CustomTextSegmentedControl
    {
        private readonly List<GameObject> m_Tabs = new List<GameObject>();

        private HMUI.TextSegmentedControl m_TabManager;

        public event Action<SegmentedControl, int> e_OnCellSelectedEvent;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create tab
        /// </summary>
        /// <param name="p_Parent">Where the tab will be created</param>
        /// <param name="p_HideCellBackground">Hide the background of the tabs cells</param>
        /// <param name="p_Tabs">List of tabs</param>
        internal CustomTextSegmentedControl(RectTransform p_Parent, bool p_HideCellBackground, List<Tab> p_Tabs)
        {
            List<string> l_TabTexts = new List<string>();

            foreach (Tab l_Index in p_Tabs)
                l_TabTexts.Add(l_Index.Name);
            foreach (Tab l_Index in p_Tabs)
                m_Tabs.Add(l_Index.Object);

            m_TabManager = BeatSaberPlus.SDK.UI.TextSegmentedControl.Create(p_Parent, p_HideCellBackground, l_TabTexts.ToArray());
            m_TabManager.ReloadData();

            m_TabManager.didSelectCellEvent += (p_SegmentedControl, p_Index) => {
                if (m_Tabs.Count == 0) return;
                e_OnCellSelectedEvent?.Invoke(p_SegmentedControl, p_Index);
                for (int l_i = 0; l_i < m_TabManager.cells.Count; l_i++)
                {
                    if (m_Tabs.ElementAt(l_i) != null)
                    {
                        if (l_i == p_Index)
                            m_Tabs.ElementAt(l_i).gameObject.SetActive(true);
                        else
                            m_Tabs.ElementAt(l_i).gameObject.SetActive(false);
                    }
                }
            };

            foreach (var l_Current in m_Tabs)
                l_Current.gameObject.SetActive(false);

            if (m_Tabs.ElementAt(0) != null) m_Tabs.ElementAt(0).gameObject.SetActive(true);
        }
    }
}
