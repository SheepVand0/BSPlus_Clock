using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using BeatSaberPlus.SDK.UI;
using TMPro;
using System.Reflection;

namespace BeatSaberPlus_Clock.UI
{
    internal class ClockViewController : ViewController<ClockViewController>
    {
        [UIComponent("ClockText")] private TextMeshProUGUI m_ClockText = null;

        protected override string GetViewContentDescription()
        {
            return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), $"{Plugin.AssemblyName}.UI.Views.ClockViewController.bsml");
        }

        protected sealed override void OnViewActivation()
        {
            Logger.Instance.Info("Clock view controller inited");
            Logger.Instance.Info($"{m_ClockText is null}");
        }

        public void ApplySettings(CConfig.ClockConfig p_Config)
        {
            if (m_ClockText == null) return;

            m_ClockText.enableVertexGradient = p_Config.UseGradient;
            if (p_Config.UseGradient == true)
            {
                if (p_Config.UseFourColorsGradient)
                    m_ClockText.colorGradient =
                        new VertexGradient(
                        p_Config.ClockGradientColor1.ToUnityColor(),
                        p_Config.ClockGradientColor2.ToUnityColor(),
                        p_Config.ClockGradientColor3.ToUnityColor(),
                        p_Config.ClockGradientColor4.ToUnityColor());
                else
                    m_ClockText.colorGradient =
                        new VertexGradient(
                        p_Config.ClockGradientColor1.ToUnityColor(),
                        p_Config.ClockGradientColor2.ToUnityColor(),
                        p_Config.ClockGradientColor1.ToUnityColor(),
                        p_Config.ClockGradientColor2.ToUnityColor());
            }
            else
            {
                m_ClockText.color = p_Config.ClockColor.ToUnityColor();
            }

            m_ClockText.fontSize = Clock.m_ClockConfig.FontSize;
        }

        public void ApplyTime(int p_Hours, int p_Minutes, int p_Seconds)
        {
            List<string> l_Format = Clock.m_ClockConfig.FormatOrder;
            m_ClockText.text = string.Empty;
            for (int l_i = 0; l_i < l_Format.Count;l_i++)
            {
                string l_FormatPart = l_Format[l_i];
                switch (l_FormatPart)
                {
                    case "hh":
                        int l_Hours = 0;
                        l_Hours = (Clock.m_ClockConfig.SeparateDayHours || Clock.m_ClockConfig.BoolAmPm) ? (p_Hours > 12) ? p_Hours - 12 : p_Hours : p_Hours;
                        m_ClockText.text += l_Hours.ToString("00");
                        break;
                    case "mn":
                        m_ClockText.text += p_Minutes.ToString("00");
                        break;
                    case "ss":
                        m_ClockText.text += p_Seconds.ToString("00");
                        break;
                    default: m_ClockText.text += l_FormatPart; break;
                }
                if (l_i != l_Format.Count - 1)
                    m_ClockText.text += Clock.m_ClockConfig.Separator;
            }
            if (Clock.m_ClockConfig.BoolAmPm)
                m_ClockText.text += $" {(p_Hours > 12 ? "PM" : "AM")}";
            ClockFloatingScreen.Instance.SetScale(m_ClockText.text.Length*(Clock.m_ClockConfig.FontSize)*0.5f);
        }
    }
}
