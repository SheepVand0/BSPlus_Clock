﻿using UnityEngine;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage;
using System.Reflection;
using System;
using CP_SDK.Unity;

namespace BeatSaberPlus_Clock.UI
{
    internal abstract class CustomUIComponent : MonoBehaviour
    {
        internal GameObject m_ParentGameObject;
        private BSMLParserParams m_ParserParams = null;

        internal static TItem Create<TItem>(Transform p_Parent, bool p_UnderParent, Action<TItem> p_PostCreate = null) where TItem : CustomUIComponent
        {
            GameObject l_ParentGameObject = (p_UnderParent) ? p_Parent.transform.gameObject : p_Parent.transform.parent.gameObject;
            TItem l_Item = l_ParentGameObject.AddComponent<TItem>();
            l_Item.m_ParentGameObject = l_ParentGameObject;
            string l_Resource = string.Empty;
            if ((l_Resource = l_Item.GetResourceName()) != string.Empty) l_Item.Parse(l_Resource, l_Item);
            l_Item.PostCreate();
            p_PostCreate?.Invoke(l_Item);
            return l_Item;
        }

        internal void Parse(string p_ResourceName, object p_Host)
        {
            try
            {
                m_ParserParams = BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), p_ResourceName), m_ParentGameObject, p_Host);
            }
            catch (Exception l_E)
            {
                Logger.Instance.Error(l_E);
            }
        }

        internal abstract string GetResourceName();

        protected virtual void PostCreate(){}
    }
}
