using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.SaveSystem
{
    /// <summary>
    /// The IObjectSaver interface is a wrapper for ObjectSaver Scriptable Objects, which holds
    /// a list of IComponentSavers. IObjectSavers are instantiated and registered by SaveControllers
    /// upon awakening in a scene.
    ///
    /// Importantly, IObjectSavers handle data requests from game interfaces. Other game interfaces
    /// request the information, and IObjectSavers retrieve the data from IObjectStores to pass it along.
    /// Similarly, IObjectSavers take in new data from game interfaces and updates the IObjectStore
    /// appropriately.
    /// </summary>
    
    public interface IObjectSaver
    {

    }
}
