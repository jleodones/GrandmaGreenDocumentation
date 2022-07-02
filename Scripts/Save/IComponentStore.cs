using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.SaveSystem
{
    /// <summary>
    /// The IComponentStore is a wrapper interface for ComponentStores. The purpose of the ComponentStore
    /// is to store data at the start of a scene—-when data gets loaded--and when gameplay changes any sort
    /// of persistent data.
    ///
    /// Requests for the data stored in the IComponentStore object are handled through IObjectSavers.
    /// </summary>

    public interface IComponentStore
    {
        
    }
}