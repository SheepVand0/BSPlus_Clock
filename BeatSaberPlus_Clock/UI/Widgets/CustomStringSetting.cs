using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BeatSaberPlus_Clock.UI.Widgets
{
    internal class CustomStringSetting : CustomUIComponent
    {
        [UIObject("EditButtonTransform")] GameObject            m_EditButtonTransform = null;
        [UIComponent("HorizontalLayout")] HorizontalLayoutGroup m_HorizontalLayout    = null;
        [UIComponent("TextValuePreview")] TextMeshProUGUI       m_PreviewText         = null;

        Button m_EditButton = null;

        CustomKeyboard m_Keyboard = null;

        internal event Action<string> OnChange;

        internal int StringSettingMaxCharacters { get; private set; }
        internal string Text { get; set; }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Setup the setting
        /// </summary>
        /// <param name="p_InitialValue">Default string value</param>
        /// <param name="p_MaxCharacter">The numbers of characters show</param>
        /// <param name="p_RecreateNewKeyboard">Create another keyboard or set a reference</param>
        /// <param name="p_Keyboard">The keyboard reference if no create</param>
        /// <exception cref="Exception"></exception>
        internal void Setup(string p_InitialValue, int p_MaxCharacter, bool p_RecreateNewKeyboard, CustomKeyboard p_Keyboard = null)
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

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Open string setting keyboard
        /// </summary>
        internal void OpenKeyboard()
        {
            m_Keyboard.Open(Text);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Apply setting value
        /// </summary>
        /// <param name="p_Value"></param>
        internal void ApplyValue(string p_Value)
        {
            Text = p_Value;
            m_PreviewText.text = ClockUtils.CutString(p_Value, StringSettingMaxCharacters);
            OnChange?.Invoke(Text);
        }
    }
}
