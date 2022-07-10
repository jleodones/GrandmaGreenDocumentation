using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GrandmaGreen.SaveSystem
{
    /// <summary>
    /// The IComponentStore is a wrapper interface for ComponentStores. The purpose of the ComponentStore is to store data at the start of a scene—when data gets loaded–and when gameplay changes any sort of persistent data.
    ///
    /// Requests for the data stored in the IComponentStore object are handled through IObjectSavers.
    /// </summary>

    public struct Component<T> : IComparable<Component<T>> where T : struct
    {
        private int m_index { get; set; }
        private T m_struct { get; set; }

        public int CompareTo(Component<T> other)
        {
            return m_index.CompareTo(other.m_index);
        }
    }

    public interface IComponentStore
    {
        // Human readable name for differentiating component stores.
        string name { get; set; }

        // Generic overrideable function for adding new pairs to the store.
        bool AddComponent<T>(Component<T> component) where T : struct;

        // //Generic overrideable function for removing components from the store.
        bool RemoveComponent<T>(Component<T> component) where T : struct;

        // // Generic overrideable update function.
        bool UpdateValue<T>(Component<T> component) where T : struct;

        // // Generic overrideable find function.
        bool RequestData<T>(Component<T> component) where T : struct;
    }

    public class ComponentStore<T> : IComponentStore where T : struct
    {
        [ShowInInspector]
        public string name { get; set; }

        [ShowInInspector]
        public List<Component<T>> components { get; set; }

        public ComponentStore()
        {
            components = new List<Component<T>>();
        }

        public bool AddComponent<T>(Component<T> component) where T : struct
        {
            return true;
        }

        public bool RemoveComponent<T>(Component<T> component) where T : struct
        {
            return true;
        }

        public bool UpdateValue<T>(Component<T> component) where T : struct
        {
            return true;
        }

        public bool RequestData<T>(Component<T> component) where T : struct
        {
            return true;
        }
    }
}