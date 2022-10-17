using BeatSaberMarkupLanguage.Attributes;
using BeatSaberPlus.SDK.UI;
using TMPro;

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
            m_ClockText.font = ClockFloatingScreen.ClockFont;
            m_ClockText.fontSize = CConfig.Instance.GetActiveConfig().FontSize;
        }

        internal void ApplyTime(int p_Hours, int p_Minutes, int p_Seconds)
        {
            var l_Config    = CConfig.Instance.GetActiveConfig();
            var l_Text      = string.Empty;

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

            m_ClockText.text = l_Text;

            ClockFloatingScreen.Instance.SetScale(m_ClockText.text.Length * (l_Config.FontSize) * 0.5f);
        }
    }
}
