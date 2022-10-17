using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using HMUI;

namespace BeatSaberPlus_Clock.UI.Widgets
{
    internal class CustomKeyboard : CustomUIComponent
    {
        internal override string GetResourceName()
        {
            return $"{Plugin.AssemblyName}.UI.Components.Views.{GetType().Name}.bsml";
        }

        [UIComponent("KeyboardModal")] ModalKeyboard m_ModalKeyboard = null;
        [UIValue("InputKeyboardValue")] private string m_KeyboardValue { get => string.Empty; set { EnterPressed(value); } }

        internal event Action<string, string> OnKeyboardEnterPressed;

        internal string m_OldValue;

        internal void Open(string p_OpenText)
        {
            m_ModalKeyboard.modalView.Show(true, true);
            m_ModalKeyboard.SetText(p_OpenText);
            m_OldValue = p_OpenText;
        }

        private void EnterPressed(string p_Value)
        {
            OnKeyboardEnterPressed?.Invoke(m_OldValue, p_Value);
            m_OldValue = p_Value;
        }
    }
}
