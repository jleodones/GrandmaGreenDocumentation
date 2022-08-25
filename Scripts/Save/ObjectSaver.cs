using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Runtime.CompilerServices;
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
        
        /// <summary>
        /// Adds a component to the appropriate ComponentStore. Stored internally, marked for update, then saved by the SaveController.
        /// This interfaces directly with game systems.
        /// </summary>
        public void AddComponent<T>(T component) where T : struct
        {
            // Iterates through component stores to find component store of appropriate type.
            foreach(IComponentStore componentStore in componentStores)
            {
                if(componentStore.GetType() == typeof(T))
                {
                    // Once found, it adds the new component.
                    ((ComponentStore<T>) componentStore).AddComponent(component);
                    
                    // Finally, it adds itself to the save controller's "to be saved" list.
                    saveController.toBeSaved.Add(this);
                    break;
                }
            }
        }
    }
}
