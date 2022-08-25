using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace GrandmaGreen.SaveSystem
{
    /// <summary>
    /// The IComponentStore is a wrapper interface for ComponentStores. The purpose of the ComponentStore is to store data at the start of a scene—when data gets loaded–and when gameplay changes any sort of persistent data.
    ///
    /// Requests for the data stored in the IComponentStore object are handled through IObjectSavers.
    /// </summary>

    public interface IComponentStore
    {
        // Returns component store type.
        Type GetType();
    }

    public class ComponentStore<T> : IComponentStore where T : struct
    {
        [OdinSerialize]
        public List<T> components { get; set; }

        private Type m_type;

        public ComponentStore()
        {
            components = new List<T>();
        }

        public bool AddComponent(T component)
        {
            try
            {
                components.Add(component);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool RemoveComponent<T>(T _component) where T : struct
        {
            return true;
        }

        public bool UpdateValue<T>(int index, T _component) where T : struct
        {
            return true;
        }

        public bool RequestData<T>(int index, ref T _component) where T : struct
        {
            return true;
        }

        public Type GetType()
        {
            return typeof(T);
        }
    }
}