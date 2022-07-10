using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;

namespace GrandmaGreen.SaveSystem
{
    /// <summary>
    /// The IObjectSaver interface is a wrapper for ObjectSaver Scriptable Objects, which holds a list of IComponentSavers. IObjectSavers are instantiated and registered by SaveControllers upon awakening in a scene.
    ///
    /// Importantly, IObjectSavers handle data requests from game interfaces. Other game interfaces request the information, and IObjectSavers retrieve the data from IObjectStores to pass it along. Similarly, IObjectSavers take in new data from game interfaces and updates the IObjectStore appropriately.
    /// </summary>

    public interface IObjectSaver
    {
        // The component store in question.
        List<IComponentStore> componentStores { get; set; }

        // Queue of component stores to be passed on during auto save.
        Queue<IComponentStore> toBeSaved { get; set; }

        // List of events mapped to corresponding methods.
        Dictionary<Delegate, UnityEvent> eventMap { get; set; }

        // Once a system pings the object saver, it finds it in the map and triggers the event.
        bool TriggerEvent(ref Delegate myDelegate)
        {
            try
            {
                eventMap[myDelegate]?.Invoke();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }

    [CreateAssetMenu(menuName = "Save System/Object Saver", fileName = "ObjectSaver")]
    public class ObjectSaver : SerializedScriptableObject, IObjectSaver
    {
        // The component store in question.
        [ShowInInspector]
        public List<IComponentStore> componentStores { get; set; }

        // Queue of component stores to be passed on during auto save.
        [ShowInInspector]
        public Queue<IComponentStore> toBeSaved { get; set; }

        // List of events mapped to corresponding methods.
        [ShowInInspector]
        public Dictionary<Delegate, UnityEvent> eventMap { get; set; }

        // Constructor initializes member variables upon creation.
        public ObjectSaver()
        {
            componentStores = new List<IComponentStore>();
            toBeSaved = new Queue<IComponentStore>();
            eventMap = new Dictionary<Delegate, UnityEvent>();
        }

        // Create the appropriate stores for each type of object saver ahead of time.
        // This is done through reflection.
        [Button("Create Component Store")]
        public void CreateNewStore(Type myType)
        {
            Type generic = typeof(ComponentStore<>);
            Type concrete = generic.MakeGenericType(myType);

            componentStores.Add((IComponentStore)Activator.CreateInstance(concrete));
        }
    }
}
