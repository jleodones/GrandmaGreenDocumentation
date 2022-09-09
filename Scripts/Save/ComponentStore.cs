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
        /// <summary>
        /// Returns the type of the ComponentStore.
        /// </summary>
        Type GetType();
    }

    /// <summary>
    /// The ComponentStore is a data structure written in pure C#. See IComponentStore.
    /// </summary>
    public class ComponentStore<T> : IComponentStore where T : struct
    {
        /// <summary>
        /// The components in question.
        /// </summary>
        [ShowInInspector]
        public List<T> components { get; set; }

        /// <summary>
        /// Constructor initializes member variables and does nothing else.
        /// </summary>
        public ComponentStore()
        {
            components = new List<T>();
        }
        
        /// <summary>
        /// Adds a component to its internal storage list. Returns true if successful, false otherwise.
        /// </summary>
        public bool AddComponent(int index, T component)
        {
            try
            {
                if (index == -1)
                {
                    components.Add(component);
                }
                else
                {
                    components.Insert(index, component);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Removes a component from its internal storage list. Returns true if successful, false otherwise.
        /// </summary>
        public bool RemoveComponent(int index, T component)
        {
            try
            {
                if (index == -1)
                {
                    components.RemoveAt(index);
                }
                else
                {
                    components.Remove(component);
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Updates a given value in its internal storage list based on a provided index. Returns true if successful, false otherwise.
        /// </summary>
        public bool UpdateValue(int index, T component)
        {
            try
            {
                components[index] = component;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Receives data requests from its internal storage list based on a provided index and outputs it through a reference.
        /// Returns true if successful, false otherwise.
        /// </summary>
        public bool RequestData(int index, ref T component)
        {
            try
            {
                component = components[index];
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Returns the type of components it stores.
        /// </summary>
        public Type GetType()
        {
            return typeof(T);
        }
    }
}