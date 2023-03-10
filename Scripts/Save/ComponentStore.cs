using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using GrandmaGreen.Collections;

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
        public List<T> components;

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
                    components.Remove(component);
                }
                else
                {
                    components.RemoveAt(index);
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
            if (index >= 0)
            {
                try
                {
                    components[index] = component;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {
                {
                    T searchComponent = component;
                    int indexFound = components.FindIndex(comp => searchComponent.Equals(comp));

                    if (indexFound == -1)
                    {
                        AddComponent(-1, component);
                    }
                    else
                    {
                        components[indexFound] = component;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Receives data requests from its internal storage list based on a provided index and outputs it through a reference.
        /// Returns true if successful, false otherwise.
        /// </summary>
        public bool RequestData(int index, ref T component)
        {
            if (index >= 0)
            {
                try
                {
                    component = components[index];
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else // If requesting data by component equals operator.
            {
                {
                    T searchComponent = component;
                    int searchIndex = components.FindIndex(comp => searchComponent.Equals(comp));

                    if (searchIndex == -1)
                    {
                        return false;
                    }
                    else
                    {
                        component = components[searchIndex];
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the type of components it stores.
        /// </summary>
        public new Type GetType()
        {
            return typeof(T);
        }
    }
}