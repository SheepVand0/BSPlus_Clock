using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using UnityEngine;
using TMPro;
using System;
using BeatSaberPlus.SDK.Game;
using HMUI;
using System.Collections.Generic;
using System.Linq;

namespace BeatSaberPlus_Clock.UI
{
    internal class ClockFloatingScreen : MonoBehaviour
    {
        const float DAY_DURATION = 24 * 60 * 60;
        const float HOUR = 60 * 60;
        const float MINUTE = 60;

        #region Properties
        private FloatingScreen FloatingScreenObject;
        private float m_DayTime;
        private float m_LastUpdate;
        private int m_LastUpdateSecond = -1;
        private ClockViewController m_ClockViewController = null;

        public readonly List<Anchor> m_Anchors = new List<Anchor>() { new Anchor("Anchor_1", new Vector3(0, 2.8f, 3.85f), new Vector3(-14, 0, 0), GameObjects.Anchor.DEFAULT_RADIUS), new Anchor("Anchor_2", new Vector3(0, 0.4f, 3.85f), new Vector3(20, 0, 0), GameObjects.Anchor.DEFAULT_RADIUS) };

        internal static Font ClockFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        #endregion

        #region Static
        public static ClockFloatingScreen Instance;

        public static ClockFloatingScreen CreateClock()
        {
            if (Instance != null) { Logger.Instance.Error("An instance of the clock already exist not creating"); return null; }
            ClockFloatingScreen l_Clock = new GameObject("BeatSaberPlusClockFloatingSceen").AddComponent<ClockFloatingScreen>();
            return l_Clock;
        }

        public static void DestroyClock()
        {
            GameObject.DestroyImmediate(Instance.FloatingScreenObject.gameObject);
            Instance = null;
        }
        #endregion

        #region Init
        private void Awake()
        {
            if (Instance != null) { Logger.Instance.Error("An instance of the clock already exist not creating"); return; }

            m_DayTime = (float)DateTime.Now.TimeOfDay.TotalSeconds;
            m_LastUpdate = Time.realtimeSinceStartup;

            m_ClockViewController = BeatSaberUI.CreateViewController<ClockViewController>();

            FloatingScreenObject = FloatingScreen.CreateFloatingScreen(new Vector2(50, 10), true, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            FloatingScreenObject.HighlightHandle = true;
            FloatingScreenObject.HandleSide = FloatingScreen.Side.Right;
            FloatingScreenObject.ShowHandle = false;
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

            Instance = this;
        }
        private void Destroy()
        {
            GameObject.DestroyImmediate(FloatingScreenObject.gameObject);

            FloatingScreenObject.HandleGrabbed -= OnClockGrab;
            FloatingScreenObject.HandleReleased -= OnClockRelease;
            CP_SDK.ChatPlexSDK.OnGenericSceneChange -= OnGenericSceneChange;
            Clock.e_OnSettingEdited -= ApplySettings;
        }
        private void OnClockGrab(object p_Sender, FloatingScreenHandleEventArgs p_EventArgs)
        {
            if (!CConfig.Instance.GetActiveConfig().EnableAnchors) return;

            foreach (var l_Current in m_Anchors)
                GameObjects.Anchor.CreateAnchor(l_Current.AnchorPosition, l_Current.AnchorRotationEuler, l_Current.AnchorRadius);
        }

        private void OnClockRelease(object p_Sender, FloatingScreenHandleEventArgs p_EventArgs)
        {
            Vector3 l_NewPosition = p_EventArgs.Position;
            Vector3 l_NewRotation = p_EventArgs.Rotation.eulerAngles;

            if (CConfig.Instance.GetActiveConfig().EnableAnchors)
            {
                GameObjects.Anchor[] l_Anchors = Resources.FindObjectsOfTypeAll<GameObjects.Anchor>();
                foreach (var l_Current in l_Anchors)
                {
                    if (ClockUtils.Vector3Distance(l_Current.transform.position, p_EventArgs.Position) > l_Current.m_Radius) continue;

                    l_NewPosition = l_Current.transform.localPosition;
                    l_NewRotation = l_Current.transform.localRotation.eulerAngles;
                    FloatingScreenObject.transform.localPosition = l_NewPosition;
                    FloatingScreenObject.transform.localRotation = Quaternion.Euler(l_NewRotation);

                }
            }

            GameObjects.Anchor.DestroysAnchors();

            switch (Clock.m_MovementMode)
            {
                case BeatSaberPlus.SDK.Game.Logic.SceneType.Menu:
                    CConfig.Instance.GetActiveConfig().MenuClockPosition = l_NewPosition;
                    CConfig.Instance.GetActiveConfig().MenuClockRotationEuler = l_NewRotation;
                    break;
                case BeatSaberPlus.SDK.Game.Logic.SceneType.Playing:
                    CConfig.Instance.GetActiveConfig().GameClockPosition = l_NewPosition;
                    CConfig.Instance.GetActiveConfig().GameClockRotationEuler = l_NewRotation;
                    break;
                default: return;
            }

            Clock.Instance.SaveConfig();
        }

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
        #endregion

        #region Transform
        public void SetClockPositionByScene(Logic.SceneType p_SceneType)
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

        public void SetScale(float p_Width)
        {
            FloatingScreenObject.ScreenSize = new Vector2(p_Width, FloatingScreenObject.ScreenSize.y);
        }
        #endregion

        #region Style
        public void ApplySettings()
        {
            FloatingScreenObject.ShowHandle = CConfig.Instance.GetActiveConfig().EnableClockGrabbing;
            m_ClockViewController.ApplySettings(CConfig.Instance.GetActiveConfig());
        }
        #endregion

        #region Clock
        public void Update()
        {
            m_DayTime += (Time.realtimeSinceStartup - m_LastUpdate);
            m_DayTime %= DAY_DURATION;
            m_LastUpdate = Time.realtimeSinceStartup;

            int l_Hours = (int)(m_DayTime / HOUR);
            int l_Minutes = (int)((m_DayTime - (l_Hours * HOUR)) / MINUTE);
            int l_Seconds = (int)((m_DayTime - ((l_Hours * HOUR) + (l_Minutes * MINUTE))));

            if (l_Seconds == m_LastUpdateSecond)
                return;

            m_LastUpdateSecond = l_Seconds;
            m_ClockViewController.ApplyTime(l_Hours, l_Minutes, l_Seconds);
        }
        #endregion
    }
}
