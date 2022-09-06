using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using HMUI;

namespace BeatSaberPlus_Clock.UI.Components
{
    internal class CustomKeyboard : CustomUIComponent
    {
        public override string GetResourceName()
        {
            return $"{Plugin.AssemblyName}.UI.Components.Views.{GetType().Name}.bsml";
        }

        [UIComponent("KeyboardModal")] ModalKeyboard m_ModalKeyboard = null;
        [UIValue("InputKeyboardValue")] private string m_KeyboardValue { get => string.Empty; set { EnterPressed(value); } }

        public event Action<string, string> OnKeyboardEnterPressed;

        public string m_OldValue;

        public void Open(string p_OpenText)
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
