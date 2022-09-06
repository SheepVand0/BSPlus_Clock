﻿using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BeatSaberPlus_Clock.UI.Components
{
    internal class CustomStringSetting : CustomUIComponent
    {
        public override string GetResourceName()
        {
            return $"{Plugin.AssemblyName}.UI.Components.Views.{GetType().Name}.bsml";
        }

        [UIObject("EditButtonTransform")] GameObject            m_EditButtonTransform = null;
        [UIComponent("HorizontalLayout")] HorizontalLayoutGroup m_HorizontalLayout    = null;
        [UIComponent("TextValuePreview")] TextMeshProUGUI       m_PreviewText         = null;

        Button m_EditButton = null;

        CustomKeyboard m_Keyboard = null;

        public event Action<string> OnChange;

        public int StringSettingMaxCharacters { get; private set; }
        public string Text { get; set; }

        public void SetValue(string p_Value)
        {
            Text = p_Value;
            m_PreviewText.text = p_Value;
        }

        public void Setup(string p_InitialValue, int p_MaxCharacter, bool p_RecreateNewKeyboard, CustomKeyboard p_Keyboard = null)
        {
            BeatSaberPlus.SDK.UI.Backgroundable.SetOpacity(m_HorizontalLayout.gameObject, 0.4f);

            m_EditButton = BeatSaberPlus.SDK.UI.Button.CreatePrimary(m_EditButtonTransform.transform, "📝", OpenKeyboard, "Edit text");

            if (p_RecreateNewKeyboard)
                m_Keyboard = Create<CustomKeyboard>(this.transform, true);
            else if (p_Keyboard != null)
                m_Keyboard = p_Keyboard;
            else
                throw new Exception("Keyboard has to be non null to do a reference");

            m_Keyboard.OnKeyboardEnterPressed += (p_OldValue, p_Value) => { if (p_OldValue == Text) ApplyValue(p_Value); };

            StringSettingMaxCharacters = p_MaxCharacter;
            Text = p_InitialValue;
            m_PreviewText.text = Text;
        }

        public void OpenKeyboard()
        {
            m_Keyboard.Open(Text);
        }

        public void ApplyValue(string p_Value)
        {
            Text = p_Value;
            m_PreviewText.text = ClockUtils.CutString(p_Value, StringSettingMaxCharacters);
            OnChange?.Invoke(Text);
        }
    }
}
