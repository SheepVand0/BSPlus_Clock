using BeatSaberMarkupLanguage.Attributes;
using BeatSaberPlus.SDK.UI;
using System.Linq;
using TMPro;

namespace BeatSaberPlus_Clock.UI
{
    internal class ClockViewController : ViewController<ClockViewController>
    {
        [UIComponent("ClockText")] private TextMeshProUGUI m_ClockText = null;

        string m_Rich0 = string.Empty;
        string m_Rich1 = string.Empty;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override string GetViewContentDescription()
        {
            return "<bg><vertical><text id='ClockText' text='Clock' rich-text='true'/></vertical></bg>";
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

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

            m_Rich0 = string.Empty;
            m_Rich1 = string.Empty;

            if (p_Config.FontBold)
            {
                m_Rich0 += "<b>";
                m_Rich1 += "</b>";
            }

            if (p_Config.FontItalic)
            {
                m_Rich0 += "<i>";
                m_Rich1 += "</i>";
            }

            if (p_Config.FontUnderlined)
            {
                m_Rich0 += "<u>";
                m_Rich1 += "</u>";
            }

            m_ClockText.font = ClockFloatingScreen.ClockFont;
            m_ClockText.fontSize = p_Config.FontSize;

            var l_Text = string.Empty;
            for (int l_i = 0; l_i < p_Config.FormatOrder.Count;l_i++)
            {
                if (l_i != 0) l_Text += p_Config.Separator;

                l_Text += p_Config.FormatOrder.ElementAt(l_i);
            }

            ClockFloatingScreen.Instance.SetScale(l_Text.Length * (p_Config.FontSize) * 0.8f, p_Config.FontSize * 2f);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal void ApplyTime(int p_Hours, int p_Minutes, int p_Seconds)
        {
            var l_Config    = CConfig.Instance.GetActiveConfig();
            var l_Text      = string.Empty;

            l_Text += m_Rich0;

            for (int l_I = 0; l_I < l_Config.FormatOrder.Count; ++l_I)
            {
                if (l_I != 0)
                    l_Text += l_Config.Separator;

                var l_FormatPart = l_Config.FormatOrder[l_I];
                switch (l_FormatPart)
                {
                    case "hh":
                        int l_Hours = 0;
                        l_Hours = (l_Config.SeparateDayHours || l_Config.ShowAmPm) ? (p_Hours > 12) ? p_Hours - 12 : p_Hours : p_Hours;
                        l_Text += l_Hours.ToString("00");
                        break;

                    case "mn":
                        l_Text += p_Minutes.ToString("00");
                        break;

                    case "ss":
                        l_Text += p_Seconds.ToString("00");
                        break;

                    default:
                        l_Text += l_FormatPart;
                        break;
                }
            }

            if (l_Config.ShowAmPm)
                l_Text += $" {(p_Hours > 12 ? "PM" : "AM")}";

            l_Text += m_Rich1;

            m_ClockText.text = l_Text;
        }
    }
}
