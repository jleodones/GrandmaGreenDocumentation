using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GrandmaGreen.SaveSystem
{
    /// <summary>
    /// The ISaveLoader interface is a wrapper interface used for all SaveLoaders.
    /// </summary>
    public interface ISaveLoader
    {
        /// <summary>
        /// Call this to load all relevant scene data.
        /// </summary>
        void LoadAllData(ref List<ObjectSaver> objectSavers);
        
        /// <summary>
        /// Call this to save data during auto save or upon game pause.
        /// </summary>
        void SaveAllData();

        /// <summary>
        /// Call this to delete all currently saved data, ie. delete any files.
        /// </summary>
        void DeleteAllData();

        /// <summary>
        /// Recreates save file given new object savers.
        /// </summary>
        void CreateDefaultSaveFile();
    }
    
    /// <summary>
    /// The SaveController is a purely scripted C# class that gets reinstantiated everytime the game is loaded.
    /// The SaveController interfaces directly with SaveLoaders of various types as necessary.
    /// When the SaveManager calls for an AutoSave, the SaveController pings each SaveLoader to tell it to save the relevant data.
    /// </summary>
    public class SaveController
    {
        // List of ObjectSavers. Private, gets set upon instantiation.
        [SerializeField] private List<ObjectSaver> m_objectSavers;
        
        // List of SaveLoaders.
        // Ideally, the SaveController should be able to support multiple different types of saving and loading at once.
        [SerializeField]
        private List<ISaveLoader> m_saveLoaders = new List<ISaveLoader>();

        /// <summary>
        /// Every time an ObjectSaver gets "dirtied," ie. a change is made to its contents, it adds itself to the
        /// toBeSaved list. This queues it up for saving at the next call.
        /// </summary>
        [SerializeField]
        public List<ObjectSaver> toBeSaved = new List<ObjectSaver>();

        /// <summary>
        /// Initializes with a list of object savers, which sets the internal list of object savers.
        /// This list changes itself as the scenes load.
        /// </summary>
        public SaveController(List<ObjectSaver> initialObjectSaverList)
        {
            //Sets the object saver list.
            m_objectSavers = initialObjectSaverList;

            // Immediately adds the necessary save loaders.
            m_saveLoaders.Add(new JSONSaveLoader(this));

            // Reads from file.
            // For now, this is a dummy read from file function.
            OnTriggerLoad();
        }

        /// <summary>
        /// Called as necessary, primarily upon loading a new scene. This method runs through each save loader and triggers the appropriate loading call.
        /// TODO: Implement loading data so that it varies from scene to scene, ie. the Save Controller knows which data to pull and which data not to pull.
        /// </summary>
        private void OnTriggerLoad()
        {
            foreach(ISaveLoader saveLoader in m_saveLoaders)
            {
                saveLoader.LoadAllData(ref m_objectSavers);
            }
        }

        /// <summary>
        /// Called as necessary, primarily on an interval (determined by the Save Manager) or when the game is paused.
        /// </summary>
        public void AutoSave()
        {
            foreach(ISaveLoader saveLoader in m_saveLoaders)
            {
                saveLoader.SaveAllData();
            }
            toBeSaved.Clear();
        }

        /// <summary>
        /// Deletes all data in all save loaders.
        /// </summary>
        public void DeleteAllData()
        {
            foreach (ISaveLoader saveLoader in m_saveLoaders)
            {
                saveLoader.DeleteAllData();
            }
        }
        
        /// <summary>
        /// Creates new default save file for each save loader.
        /// </summary>
        public void CreateNewDefaultSaveFile()
        {
            foreach (ISaveLoader saveLoader in m_saveLoaders)
            {
                saveLoader.CreateDefaultSaveFile();
            }
        }
    }
}