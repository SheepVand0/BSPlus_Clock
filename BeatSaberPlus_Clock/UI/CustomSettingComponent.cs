using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberPlus_Clock.UI
{
    internal class CustomSettingComponent : CustomUIComponent
    {
        public override string GetResourceName()
        {
            return string.Empty;
        }

        internal object m_ObjectValue;

        public CustomSettingComponent Bind(ref object p_Value)
        {
            m_ObjectValue = p_Value;
            return this;
        }

    }
}
