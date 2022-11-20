using UnityEngine;
using System;
using System.Threading.Tasks;

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
        /// <param name="p_CharCount"></param>
        /// <returns></returns>
        internal static string CutString(string p_Base, int p_CharCount)
        {
            if (p_Base.Length > p_CharCount)
                p_Base = $"{p_Base.Substring(0, p_CharCount)}...";
            return p_Base;
        }
    }

    internal class WaitUtils
    {
        public static async Task<Task> Wait(Func<bool> p_Func, int p_ToleranceMs, int p_DelayAfter = 0, int p_MaxTryCount = 0, int p_CodeLine = -1)
        {
            int l_TryCount = 0;
            await Task.Run(async () =>
            {
                bool l_ShouldTryCount = p_MaxTryCount > 0;

                do
                {
                    try
                    {
                        if (p_Func.Invoke()) return;

                        if (l_ShouldTryCount)
                            l_TryCount += 1;
                        await Task.Delay(p_ToleranceMs);
                    }
                    catch (Exception l_E)
                    {
                        Logger.Instance.Error(l_E, nameof(WaitUtils), nameof(Wait));
                        if (p_CodeLine != -1)
                            Logger.Instance.Error(new Exception($"At line {p_CodeLine}"), nameof(WaitUtils), nameof(Wait));
                    }
                } while (p_Func.Invoke() == false && (!l_ShouldTryCount || l_TryCount < p_MaxTryCount));
            });
            if (p_DelayAfter != 0)
                await Task.Delay(p_DelayAfter);
            return Task.CompletedTask;
        }
    }
}
