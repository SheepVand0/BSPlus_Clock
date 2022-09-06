using System.Reflection;

namespace BeatSaberPlus_Clock.UI
{
    internal class ResourceCustomUIComponent : CustomUIComponent
    {
        public sealed override string GetResourceName()
        {
            Logger.Instance.Info(string.Join(".", GetType().Namespace, GetType().Name));
            return string.Join(".", GetType().Namespace, GetType().Name);
        }
    }
}
