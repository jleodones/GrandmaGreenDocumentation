using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen
{
    public class UIDisplayBase : MonoBehaviour
    {
        public VisualElement m_rootVisualElement;

        private Dictionary<string, List<Action>> m_callbackDictionary;
        
        private void Awake()
        {
            m_rootVisualElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("rootVisualElement");
            m_callbackDictionary = new Dictionary<string, List<Action>>();
            
            RegisterButtonCallback("exitButton", CloseUI);

            Load();
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<string, List<Action>> entry in m_callbackDictionary)
            {
                foreach (Action action in entry.Value)
                {
                    DeregisterButtonCallback(entry.Key, action);
                }
            }

            Unload();
        }

        public virtual void Load()
        {
        }

        public virtual void Unload()
        {
        }

        public virtual void OpenUI()
        {
            m_rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public virtual void CloseUI()
        {
            m_rootVisualElement.style.display = DisplayStyle.None;
        }

        public void RegisterButtonCallback(string buttonName, Action callbackFunction)
        {
            Button button;
            try
            { 
                button = m_rootVisualElement.Q<Button>(buttonName);
                button.clicked += callbackFunction;
            }
            catch (NullReferenceException e)
            {
                return;
            }

            // If this button does not already exist in the callback dictionary, add it with a new list entry.
            if (!m_callbackDictionary.ContainsKey(buttonName))
            {
                m_callbackDictionary.Add(buttonName, new List<Action>() { callbackFunction });
            }
            // Else, add the callback to the list of callbacks.
            else
            {
                m_callbackDictionary[buttonName].Add(callbackFunction);
            }
        }

        public void RegisterButtonCallbackWithClose(string buttonName, Action callbackFunction)
        {
            Button button;
            try
            { 
                button = m_rootVisualElement.Q<Button>(buttonName);
                button.clicked += CloseUI;
                button.clicked += callbackFunction;
            }
            catch (NullReferenceException e)
            {
                return;
            }
            
            // If this button does not already exist in the callback dictionary, add it with a new list entry.
            if (!m_callbackDictionary.ContainsKey(buttonName))
            {
                m_callbackDictionary.Add(buttonName, new List<Action>() { callbackFunction });
                m_callbackDictionary[buttonName].Add(CloseUI);
            }
            // Else, add the callback to the list of callbacks.
            else
            {
                m_callbackDictionary[buttonName].Add(callbackFunction);
            }
        }

        private void DeregisterButtonCallback(string buttonName, Action callbackFunction)
        {
            m_rootVisualElement.Q<Button>(buttonName).clicked -= callbackFunction;
        }
    }
}
