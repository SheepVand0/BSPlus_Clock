using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using CP_SDK.Unity;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace BeatSaberPlus_Clock.UI
{
    internal abstract class CustomUIComponent : MonoBehaviour
    {
        internal GameObject m_ParentGameObject;
        private BSMLParserParams m_ParserParams = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        internal static TItem Create<TItem>(Transform p_Parent, bool p_UnderParent, Action<TItem> p_PostCreate = null) where TItem : CustomUIComponent
        {
            GameObject l_ParentGameObject = (p_UnderParent) ? p_Parent.transform.gameObject : p_Parent.transform.parent.gameObject;
            TItem l_Item = l_ParentGameObject.AddComponent<TItem>();
            l_Item.m_ParentGameObject = l_ParentGameObject;

            var l_Resource = l_Item.GetResourceDescription();
            if (l_Resource == string.Empty)
            {
                try
                {
                    l_Resource = CP_SDK.Misc.Resources.FromPathStr(Assembly.GetAssembly(typeof(TItem)), string.Join(".", typeof(TItem).Namespace, typeof(TItem).Name));
                }
                catch { }
            }

            if (!string.IsNullOrEmpty(l_Resource))
            {
                try
                {
                    l_Item.m_ParserParams = BSMLParser.instance.Parse(l_Resource, l_Item.m_ParentGameObject, l_Item);
                }
                catch (System.Exception l_Exception)
                {
                    Logger.Instance.Error("[BeatSaberPlus_Clock.UI][CustomUIComponent.Create] Error:");
                    Logger.Instance.Error(l_Exception);
                }
            }

            MTCoroutineStarter.Start(_PostCreate(l_Item));
            p_PostCreate?.Invoke(l_Item);

            return l_Item;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private static IEnumerator _PostCreate(CustomUIComponent p_Item)
        {
            yield return new WaitForSeconds(0.4f);

            p_Item.PostCreate();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public virtual string GetResourceDescription() { return string.Empty; }
        protected virtual void PostCreate(){}
    }
}
