using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberPlus.SDK.Game;
using HMUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BeatSaberPlus_Clock.UI
{
    internal class ClockFloatingScreen : MonoBehaviour
    {
        const float DAY_DURATION = 24 * 60 * 60;
        const float HOUR = 60 * 60;
        const float MINUTE = 60;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private FloatingScreen FloatingScreenObject;
        private float m_DayTime;
        private float m_LastUpdate;
        private int m_LastUpdateSecond = -1;
        private ClockViewController m_ClockViewController = null;

        internal static TMPro.TMP_FontAsset ClockFont = Resources.FindObjectsOfTypeAll<TMPro.TMP_FontAsset>().FirstOrDefault();

        internal static readonly List<Components.Anchor.Settings> m_Anchors = new List<Components.Anchor.Settings>() {
            new Components.Anchor.Settings("Anchor_1", new Vector3( 0.00f, 2.70f, 3.87f), new Vector3(-10,   0, 0), Components.Anchor.DEFAULT_RADIUS),
            new Components.Anchor.Settings("Anchor_2", new Vector3( 0.00f, 0.40f, 3.85f), new Vector3( 20,   0, 0), Components.Anchor.DEFAULT_RADIUS),
            new Components.Anchor.Settings("Anchor_3", new Vector3( 4.28f, 1.43f, 3.80f), new Vector3(  0,  60, 0), Components.Anchor.DEFAULT_RADIUS),
            new Components.Anchor.Settings("Anchor_4", new Vector3(-4.28f, 1.43f, 3.80f), new Vector3(  0, -60, 0), Components.Anchor.DEFAULT_RADIUS)
        };

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////


        internal static ClockFloatingScreen Instance = null;

        /// <summary>
        /// Used to create clock, only one can be created
        /// </summary>
        /// <returns></returns>
        internal static ClockFloatingScreen CreateClock()
        {
            if (Instance != null) { Logger.Instance.Error("An instance of the clock already exist not creating"); return null; }
            ClockFloatingScreen l_Clock = new GameObject("BeatSaberPlusClockFloatingSceen").AddComponent<ClockFloatingScreen>();
            return l_Clock;
        }

        /// <summary>
        /// Destroy clock
        /// </summary>
        internal static void DestroyClock()
        {
            Instance.Destroy();
            Instance = null;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Called when Clock was created
        /// </summary>
        private void Awake()
        {
            if (Instance != null) { Logger.Instance.Error("An instance of the clock already exist not creating"); return; }

            Instance = this;

            m_DayTime    = (float)DateTime.Now.TimeOfDay.TotalSeconds;
            m_LastUpdate = Time.realtimeSinceStartup;

            m_ClockViewController = BeatSaberUI.CreateViewController<ClockViewController>();

            FloatingScreenObject                 = FloatingScreen.CreateFloatingScreen(new Vector2(50, 10), true, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            FloatingScreenObject.ShowHandle      = false;
            FloatingScreenObject.HighlightHandle = true;
            FloatingScreenObject.HandleSide      = FloatingScreen.Side.Right;

            FloatingScreenObject.SetRootViewController(m_ClockViewController, ViewController.AnimationType.None);

            FloatingScreenObject.HandleGrabbed      += OnClockGrab;
            FloatingScreenObject.HandleReleased     += OnClockRelease;
            CP_SDK.ChatPlexSDK.OnGenericSceneChange += OnGenericSceneChange;
            Clock.e_OnSettingEdited                 += ApplySettings;

            ApplySettings();

            SetClockPositionByScene(Logic.SceneType.Menu);

            GameObject.DontDestroyOnLoad(this);
            GameObject.DontDestroyOnLoad(FloatingScreenObject);
            GameObject.DontDestroyOnLoad(m_ClockViewController);

            StartCoroutine(ManualLateUpdate());
        }
        /// <summary>
        /// Destroy Clock (Called by DestroyClock())
        /// </summary>
        private void Destroy()
        {
            GameObject.DestroyImmediate(FloatingScreenObject.gameObject);

            FloatingScreenObject.HandleGrabbed      -= OnClockGrab;
            FloatingScreenObject.HandleReleased     -= OnClockRelease;
            CP_SDK.ChatPlexSDK.OnGenericSceneChange -= OnGenericSceneChange;
            Clock.e_OnSettingEdited                 -= ApplySettings;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Event called when User grab the clock with the pointer
        /// </summary>
        /// <param name="p_Sender"></param>
        /// <param name="p_EventArgs"></param>
        private void OnClockGrab(object p_Sender, FloatingScreenHandleEventArgs p_EventArgs)
        {
            if (!CConfig.Instance.GetActiveConfig().EnableAnchors) return;

            foreach (var l_Current in m_Anchors)
                Components.Anchor.CreateAnchor(l_Current.Position, l_Current.RotationEuler, l_Current.Radius);
        }

        /// <summary>
        /// Called when user release the pointer
        /// </summary>
        /// <param name="p_Sender"></param>
        /// <param name="p_EventArgs"></param>
        private void OnClockRelease(object p_Sender, FloatingScreenHandleEventArgs p_EventArgs)
        {
            Vector3 l_NewPosition = p_EventArgs.Position;
            Vector3 l_NewRotation = p_EventArgs.Rotation.eulerAngles;

            ///Check if the clock is in a radius of a anchor
            if (CConfig.Instance.GetActiveConfig().EnableAnchors)
            {
                BeatSaberPlus_Clock.Components.Anchor[] l_Anchors = Resources.FindObjectsOfTypeAll<BeatSaberPlus_Clock.Components.Anchor>();
                foreach (var l_Current in l_Anchors)
                {
                    if (ClockUtils.Vector3Distance(l_Current.transform.position, p_EventArgs.Position) > l_Current.m_Radius) continue;

                    l_NewPosition = l_Current.transform.localPosition;
                    l_NewRotation = l_Current.transform.localRotation.eulerAngles;
                    FloatingScreenObject.transform.localPosition = l_NewPosition;
                    FloatingScreenObject.transform.localRotation = Quaternion.Euler(l_NewRotation);
                }
            }

            BeatSaberPlus_Clock.Components.Anchor.DestroysAnchors();

            ///Apply positions
            switch (Clock.m_MovementMode)
            {
                case Logic.SceneType.Menu:
                    CConfig.Instance.GetActiveConfig().MenuClockPosition = l_NewPosition;
                    CConfig.Instance.GetActiveConfig().MenuClockRotationEuler = l_NewRotation;
                    break;
                case Logic.SceneType.Playing:
                    CConfig.Instance.GetActiveConfig().GameClockPosition = l_NewPosition;
                    CConfig.Instance.GetActiveConfig().GameClockRotationEuler = l_NewRotation;
                    break;
                default: return;
            }

            Clock.Instance.SaveConfig();
        }
        /// <summary>
        /// Change clock position by scene
        /// </summary>
        /// <param name="p_Obj"></param>
        private void OnGenericSceneChange(CP_SDK.ChatPlexSDK.EGenericScene p_Obj)
        {
            m_DayTime = (float)DateTime.Now.TimeOfDay.TotalSeconds;
            m_LastUpdate = Time.realtimeSinceStartup;

            switch (p_Obj)
            {
                case CP_SDK.ChatPlexSDK.EGenericScene.Menu:
                    SetClockPositionByScene(Logic.SceneType.Menu);
                    break;
                case CP_SDK.ChatPlexSDK.EGenericScene.Playing:
                    SetClockPositionByScene(Logic.SceneType.Playing);
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set the position (In config) of the clock by selecting a scene
        /// </summary>
        /// <param name="p_SceneType"></param>
        internal void SetClockPositionByScene(Logic.SceneType p_SceneType)
        {
            switch (p_SceneType)
            {
                case Logic.SceneType.Menu:
                    FloatingScreenObject.transform.localPosition = CConfig.Instance.GetActiveConfig().MenuClockPosition;
                    FloatingScreenObject.transform.localRotation = Quaternion.Euler(CConfig.Instance.GetActiveConfig().MenuClockRotationEuler);
                    break;
                case Logic.SceneType.Playing:
                    FloatingScreenObject.transform.localPosition = CConfig.Instance.GetActiveConfig().GameClockPosition;
                    FloatingScreenObject.transform.localRotation = Quaternion.Euler(CConfig.Instance.GetActiveConfig().GameClockRotationEuler);
                    break;
                default: return;
            }
        }

        /// <summary>
        /// Change screen scale
        /// </summary>
        /// <param name="p_Width"></param>
        /// <param name="p_Height"></param>
        internal void SetScale(float p_Width, float p_Height)
        {
            FloatingScreenObject.ScreenSize = new Vector2(p_Width, p_Height);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Apply settings on screen and view controller
        /// </summary>
        internal void ApplySettings()
        {
            FloatingScreenObject.ShowHandle = CConfig.Instance.GetActiveConfig().EnableClockGrabbing;
            m_ClockViewController.ApplySettings(CConfig.Instance.GetActiveConfig());
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Update the clock every 0.1s
        /// </summary>
        /// <returns></returns>
        private IEnumerator ManualLateUpdate()
        {
            yield return new WaitForSeconds(0.1f);

            float l_TimeSinceStartup = Time.realtimeSinceStartup;
            m_DayTime += (l_TimeSinceStartup - m_LastUpdate);
            m_DayTime %= DAY_DURATION;
            m_LastUpdate = l_TimeSinceStartup;

            int l_Hours = (int)(m_DayTime / HOUR);
            int l_Minutes = (int)((m_DayTime - (l_Hours * HOUR)) / MINUTE);
            int l_Seconds = (int)((m_DayTime - ((l_Hours * HOUR) + (l_Minutes * MINUTE))));

            if (l_Seconds == m_LastUpdateSecond)
            {
                StartCoroutine(ManualLateUpdate());
                yield break;
            }

            m_LastUpdateSecond = l_Seconds;
            m_ClockViewController.ApplyTime(l_Hours, l_Minutes, l_Seconds);

            StartCoroutine(ManualLateUpdate());
        }
    }
}
