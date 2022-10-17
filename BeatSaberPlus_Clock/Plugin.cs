﻿using IPA;

namespace BeatSaberPlus_Clock
{
    /// <summary>
    /// Main plugin class
    /// </summary>
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static string AssemblyName = "BeatSaberPlus_Clock";

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// </summary>
        /// <param name="p_Logger">Logger instance</param>
        [Init]
        public Plugin(IPA.Logging.Logger p_Logger)
        {
            /// Setup logger
            Logger.Instance = new CP_SDK.Logging.IPALogger(p_Logger);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// On BeatSaberPlus enable
        /// </summary>
        [OnEnable]
        public void OnEnable()
        {

        }
        /// <summary>
        /// On BeatSaberPlus disable
        /// </summary>
        [OnDisable]
        public void OnDisable()
        {

        }
    }
}
