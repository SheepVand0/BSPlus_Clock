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
        private bool m_Parsed = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Used to create UIComponents
        /// </summary>
        /// <typeparam name="TItem">Type of the component (Instance of CustomUIComponent)</typeparam>
        /// <param name="p_Parent">Where the component will be placed</param>
        /// <param name="p_UnderParent">Is the component next or under this parent ? (In Hierarchy)</param>
        /// <param name="p_PostCreate">Callback called when item has finished creation</param>
        /// <returns></returns>
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
                    Logger.Instance.Error(l_Exception, nameof(BeatSaberPlus_Clock.UI.CustomUIComponent), nameof(Create));
                }
            }

            MTCoroutineStarter.Start(_PostCreate(l_Item));
            p_PostCreate?.Invoke(l_Item);

            return l_Item;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Coroutine called to prevent BSML delay when Parse
        /// </summary>
        /// <param name="p_Item"></param>
        /// <returns></returns>
        private static IEnumerator _PostCreate(CustomUIComponent p_Item)
        {
            yield return new WaitForSeconds(0.4f);

            p_Item.PostCreate();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Get content for parsing
        /// </summary>
        /// <returns></returns>
        protected virtual string GetResourceDescription() { return string.Empty; }
        /// <summary>
        /// Function called when Item had finished creation (Overridable)
        /// </summary>
        protected virtual void PostCreate(){}
    }
}
