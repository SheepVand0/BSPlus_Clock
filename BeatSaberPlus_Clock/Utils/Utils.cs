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
        /// <summary>
        /// Calculate distance between two vector3 points
        /// </summary>
        /// <param name="p_Point1"></param>
        /// <param name="p_Point2"></param>
        /// <returns></returns>
        internal static float Vector3Distance(Vector3 p_Point1, Vector3 p_Point2)
        {
            return (float)Math.Abs(Math.Sqrt(Math.Pow((p_Point2.x - p_Point1.x), 2) + Math.Pow((p_Point2.y - p_Point1.y), 2) + Math.Pow((p_Point2.z - p_Point1.z), 2)));
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// If the p_Base value has more than p_CharacterCount, it will be cutted and ... will be added
        /// </summary>
        /// <param name="p_Base"></param>
        /// <param name="p_CharactedCount"></param>
        /// <returns></returns>
        internal static string CutString(string p_Base, int p_CharactedCount)
        {
            if (p_Base.Length > p_CharactedCount)
                p_Base = $"{p_Base.Substring(0, p_CharactedCount)}...";
            return p_Base;
        }
    }
}
