using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
        [OdinSerialize]
        List<IComponentStore> componentStores { get; set; }

        // Queue of component stores to be passed on during auto save.
        List<IComponentStore> toBeSaved { get; set; }
    }

    [CreateAssetMenu(menuName = "Save System/Object Saver", fileName = "ObjectSaver")]
    public class ObjectSaver : ScriptableObject, IObjectSaver
    {
        // Identifier number for file names.
        [ReadOnly]
        [ShowInInspector]
        public string ID;

        // The component stores in question.
        [ShowInInspector]
        public List<IComponentStore> componentStores { get; set; }

        // List of component stores to be passed on during auto save.
        [ShowInInspector]
        public List<IComponentStore> toBeSaved { get; set; }

        // Save controller within the game.
        [ShowInInspector]
        public SaveController saveController;

        // Constructor initializes member variables upon creation.
        public void OnEnable()
        {
            ID = this.ToString().GetHashCode().ToString();
            componentStores = new List<IComponentStore>();
            toBeSaved = new List<IComponentStore>();
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

        public void AddComponent<T>(T component) where T : struct
        {
            // Iterates through component stores to find component store of appropriate type.
            foreach(IComponentStore componentStore in componentStores)
            {
                if(componentStore.GetType() == typeof(T))
                {
                    // Once found, it adds the new component.
                    ((ComponentStore<T>) componentStore).AddComponent(component);

                    // Then it adds it to its own "to be saved" list.
                    toBeSaved.Add(componentStore);

                    // Finally, it adds itself to the save controller's "to be saved" list.
                    saveController.toBeSaved.Add(this);
                    break;
                }
            }
        }
    }
}
