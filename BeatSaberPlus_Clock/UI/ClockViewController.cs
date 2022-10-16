using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using System.Collections.Generic;
using BeatSaberPlus.SDK.UI;
using TMPro;
using System.Reflection;
using IPA.Utilities;
using UnityEngine;

namespace BeatSaberPlus_Clock.UI
{
    internal class ClockViewController : ViewController<ClockViewController>
    {
        [UIComponent("ClockText")] private TextMeshProUGUI m_ClockText = null;

        protected override string GetViewContentDescription()
        {
            return "<bg><vertical><text id='ClockText' text='Clock'/></vertical></bg>";
        }

        internal void ApplySettings(CConfig.ClockConfig p_Config)
        {
            if (m_ClockText == null) return;

            m_ClockText.enableVertexGradient = p_Config.UseGradient;
            if (p_Config.UseGradient == true)
            {
                if (p_Config.UseFourColorsGradient)
                {
                    m_ClockText.colorGradient =
                        new VertexGradient(
                        p_Config.ClockGradientColor1,
                        p_Config.ClockGradientColor2,
                        p_Config.ClockGradientColor3,
                        p_Config.ClockGradientColor4);
                }
                else
                {
                    m_ClockText.colorGradient =
                        new VertexGradient(
                        p_Config.ClockGradientColor1,
                        p_Config.ClockGradientColor2,
                        p_Config.ClockGradientColor1,
                        p_Config.ClockGradientColor2);
                }
            }
            else
            {
                m_ClockText.color = p_Config.ClockColor;
            }

            /*TMP_FontAsset l_Font = TMP_FontAsset.CreateFontAsset(ClockFloatingScreen.ClockFont);
            m_ClockText.font = l_Font;
            m_ClockText.fontSharedMaterial = l_Font.material;
            m_ClockText.UpdateFontAsset();
            m_ClockText.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, 1);*/
            m_ClockText.fontSize = CConfig.Instance.GetActiveConfig().FontSize;
            //m_ClockText.UpdateFontAsset();
        }

        internal void ApplyTime(int p_Hours, int p_Minutes, int p_Seconds)
        {
            List<string> l_Format = CConfig.Instance.GetActiveConfig().FormatOrder;
            m_ClockText.text = string.Empty;
            for (int l_i = 0; l_i < l_Format.Count; l_i++)
            {
                string l_FormatPart = l_Format[l_i];
                switch (l_FormatPart)
                {
                    case "hh":
                        int l_Hours = 0;
                        l_Hours = (CConfig.Instance.GetActiveConfig().SeparateDayHours || CConfig.Instance.GetActiveConfig().BoolAmPm) ? (p_Hours > 12) ? p_Hours - 12 : p_Hours : p_Hours;
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
                    m_ClockText.text += CConfig.Instance.GetActiveConfig().Separator;
            }
            if (CConfig.Instance.GetActiveConfig().BoolAmPm)
                m_ClockText.text += $" {(p_Hours > 12 ? "PM" : "AM")}";
            ClockFloatingScreen.Instance.SetScale(m_ClockText.text.Length * (CConfig.Instance.GetActiveConfig().FontSize) * 0.5f);
        }
    }
}
