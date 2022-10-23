using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using BeatSaberPlus.SDK.UI;
using System.Diagnostics;
using UnityEngine;

namespace BeatSaberPlus_Clock.UI
{
    internal class LeftSettings : ResourceViewController<LeftSettings>
    {
        private static readonly string s_InformationsStr = "<line-height=125%><b><u>Clock</u></b>"
            + "\n" + "<i><color=#CCCCCCFF>Allows you to spawn a clock in the game !</color></i>"
            + "\n" + "<i><color=#CCCCCCFF>Made with love by SheepVand#3030 & HardCPP#1985</color></i>"
            + "\n" + "<i><color=#CCCCCCFF>To know how to use the mod click the documentation button below 🙂</color></i>"
            + "\n"
            + "\n"
            + "\n"
            + "\n"
            + "\n"
            + "\n";

        [UIComponent("Informations")] HMUI.TextPageScrollView m_TextPageScrollView = null;

        [UIObject("Background")] GameObject m_Background = null;

        protected override void OnViewCreation()
        {
            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_Background.gameObject, 0.5f);
            m_TextPageScrollView.SetText(s_InformationsStr);
            m_TextPageScrollView.UpdateVerticalScrollIndicator(0);
        }

        [UIAction("click-documentation-btn-pressed")]
        private void OnDocumentationClicked()
        {
            ShowMessageModal("URL opened in your browser");
            Process.Start("https://youtu.be/dQw4w9WgXcQ?mute=1");
        }
    }
}
