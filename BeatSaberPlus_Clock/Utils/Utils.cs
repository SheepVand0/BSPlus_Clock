using System.Linq;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Components.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace BeatSaberPlus_Clock
{
    internal static class ClockUtils
    {
        public static void Setup(this StringSetting p_Setting, BSMLAction p_Action, string p_Value, bool p_RemoveLabel)
        {
            p_Setting.gameObject.SetActive(value: false);
            p_Setting.Text = p_Value;
            p_Setting.ApplyValue();
            if (p_Action != null)
            {
                p_Setting.onChange = p_Action;
            }

            if (p_RemoveLabel)
            {
                UnityEngine.Object.Destroy(p_Setting.gameObject.GetComponentsInChildren<TextMeshProUGUI>().ElementAt(0).transform.gameObject);
                RectTransform obj = p_Setting.gameObject.transform.GetChild(1) as RectTransform;
                obj.anchorMin = Vector2.zero;
                obj.anchorMax = Vector2.one;
                obj.sizeDelta = Vector2.one;
                p_Setting.gameObject.GetComponent<LayoutElement>().preferredWidth = -1f;
            }

            p_Setting.gameObject.SetActive(value: true);
        }

        public static float Vector3Distance(Vector3 p_Point1, Vector3 p_Point2)
        {
            return (float)Math.Abs(Math.Sqrt(Math.Pow((p_Point2.x - p_Point1.x), 2) + Math.Pow((p_Point2.y - p_Point1.y), 2) + Math.Pow((p_Point2.z - p_Point1.z), 2)));
        }

        public static string CutString(string p_Base, int p_CharactedCount)
        {
            if (p_Base.Length > p_CharactedCount)
            {
                string l_NewString = string.Empty;
                for (int l_i = 0; l_i < p_Base.Length;l_i++)
                {
                    if (l_i < p_CharactedCount)
                        l_NewString += p_Base[l_i];
                    else
                        return l_NewString += "...";
                }
            }
            return p_Base;
        }
    }
}
