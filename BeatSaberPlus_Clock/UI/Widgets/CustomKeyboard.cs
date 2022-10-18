using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using HMUI;
using System.Reflection;

namespace BeatSaberPlus_Clock.UI.Widgets
{
    internal class CustomKeyboard : CustomUIComponent
    {
        [UIComponent("KeyboardModal")] ModalKeyboard m_ModalKeyboard = null;
        [UIValue("InputKeyboardValue")] private string m_KeyboardValue { get => string.Empty; set { EnterPressed(value); } }

        internal event Action<string, string> OnKeyboardEnterPressed;

        internal string m_OldValue;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Open keyboard
        /// </summary>
        /// <param name="p_OpenText">The text that will appear when opening the keyboard</param>
        internal void Open(string p_OpenText)
        {
            m_ModalKeyboard.modalView.Show(true, true);
            m_ModalKeyboard.SetText(p_OpenText);
            m_OldValue = p_OpenText;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Event called when the key Enter was pressed on the virtual keyboard
        /// </summary>
        /// <param name="p_Value"></param>
        private void EnterPressed(string p_Value)
        {
            OnKeyboardEnterPressed?.Invoke(m_OldValue, p_Value);
            m_OldValue = p_Value;
        }
    }
}
