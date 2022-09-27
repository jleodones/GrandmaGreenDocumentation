using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Newtonsoft.Json;

namespace GrandmaGreen.SaveSystem
{
    /// <summary>
    /// The IObjectSaver interface is a wrapper for ObjectSaver Scriptable Objects, which holds a list of IComponentSavers. IObjectSavers are instantiated and registered by SaveControllers upon awakening in a scene.
    ///
    /// Importantly, IObjectSavers handle data requests from game interfaces. Other game interfaces request the information, and IObjectSavers retrieve the data from IObjectStores to pass it along. Similarly, IObjectSavers take in new data from game interfaces and updates the IObjectStore appropriately.
    /// </summary>
    public interface IObjectSaver
    {
        /// <summary>
        /// The component store in question.
        /// </summary>
        List<IComponentStore> componentStores { get; set; }
    }
    
    /// <summary>
    /// The ObjectSaver interfaces with game systems directly when new data is added, changed, or removed as player's navigate the game.
    /// It is a ScriptableObject that is the core of the save system, and was designed to be editor friendly. They store pure data.
    /// </summary>
    [CreateAssetMenu(menuName = "Save System/Object Saver", fileName = "ObjectSaver")]
    public class ObjectSaver : ScriptableObject, IObjectSaver
    {
        /// <summary>
        /// Identifier number used to identify the ObjectSaver during saving and loading operations. As the hash depends on the asset's file name,
        /// no two ObjectSavers should be named alike.
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        public string ID;
        
        /// <summary>
        /// The component stores in question.
        /// </summary>
        [ShowInInspector]
        public List<IComponentStore> componentStores { get; set; }
        
        /// <summary>
        /// The reference to the game's SaveController, which it communicates with to present internal updates.
        /// </summary>
        [ShowInInspector]
        [JsonIgnore]
        public SaveController saveController;

        /// <summary>
        /// Initializes member variables upon enabling.
        /// </summary>
        public void OnEnable()
        {
            ID = this.ToString().GetHashCode().ToString();
            componentStores = new List<IComponentStore>();
        }
        
        /// <summary>
        /// Used to set the data with information loaded from the save file. It also sets the newly instantiated SaveController.
        /// </summary>
        public void Set(SaveController newSaveController, ObjectSaver os)
        {
            saveController = newSaveController;

            if (os != null)
            {
                componentStores = os.componentStores;
            }
        }
        
        /// <summary>
        /// Creates a given ComponentStore for each ObjectSaver ahead of time. This is done through the Unity Editor.
        /// Utilizes reflection. The Type must be a C# data type (ie. strings, ints, structs) or it will get rejected.
        /// </summary>
        [Button("Create Component Store")]
        public void CreateNewStore(Type myType)
        {
            Type generic = typeof(ComponentStore<>);
            Type concrete = generic.MakeGenericType(myType);

            componentStores.Add((IComponentStore)Activator.CreateInstance(concrete));
        }

        public void CreateNewStore<T>() where T : struct
        {
            componentStores.Add(new ComponentStore<T>());
        }

        /// <summary>
        /// Adds a component to the appropriate ComponentStore. Stored internally, marked for update, then saved by the SaveController.
        /// Wrapper for the corresponding function in the ComponentStore. This interfaces directly with game systems.
        /// </summary>
        /// <param name="index">
        /// Optional index parameter to insert a component at the given index.
        /// </param>
        public bool AddComponent<T>(int index, T component) where T : struct
        {
            // Iterates through component stores to find component store of appropriate type.
            foreach (IComponentStore componentStore in componentStores)
            {
                if (componentStore.GetType() == typeof(T))
                {
                    // Once found, it adds the new component.
                    return ((ComponentStore<T>)componentStore).AddComponent(index, component);
                }
            }

            return false;
        }

        /// <summary>
        /// Removes a component from the appropriate ComponentStore. Stored internally, then saved by the SaveController.
        /// Wrapper for the corresponding function in the ComponentStore. This interfaces directly with game systems.
        /// </summary>
        /// <param name="index">
        /// Index for moments where the index of the object is known. -1 signifies that it should use an equality based comparison to
        /// find the component (no index available).
        /// </param>
        public bool RemoveComponent<T>(int index, T component) where T : struct
        {
            // Iterates through component stores to find component store of appropriate type.
            foreach (IComponentStore componentStore in componentStores)
            {
                if (componentStore.GetType() == typeof(T))
                {
                    // Once found, it removes the component.
                    // Test this, make sure it actually removes the right component.
                    return ((ComponentStore<T>)componentStore).RemoveComponent(index, component);
                }
            }

            return false;
        }
        
        /// <summary>
        /// Updates the value of the given component.
        /// </summary>
        /// <param name="index">
        /// Used to find the index of the component. If -1 is given, it searches for the component based on equality search (no index available).
        /// </param>
        /// <param name="component">
        /// The value to update it with.
        /// </param>
        public bool UpdateValue<T>(int index, T component) where T : struct
        {
            // Iterates through component stores to find component store of appropriate type.
            foreach (IComponentStore componentStore in componentStores)
            {
                if (componentStore.GetType() == typeof(T))
                {
                    // Once found, it updates the component.
                    return ((ComponentStore<T>)componentStore).UpdateValue(index, component);
                }
            }

            return false;
        }

        
        public bool RequestData<T>(int index, ref T component) where T : struct
        {
            // Iterates through component stores to find component store of appropriate type.
            foreach (IComponentStore componentStore in componentStores)
            {
                if (componentStore.GetType() == typeof(T))
                {
                    // Once found, it retrieves the data for the component; since it's passed by reference, the information
                    // is stored in the component.
                    return ((ComponentStore<T>)componentStore).RequestData(index, ref component);
                }
            }

            return false;
        }
    }
}
