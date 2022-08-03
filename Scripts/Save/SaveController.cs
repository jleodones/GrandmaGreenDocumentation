using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GrandmaGreen.SaveSystem
{
    public interface ISaveLoader
    {
        // Call this to load the data upon startup.
        void LoadAllData();

        // Call this to save data during auto save or upon game pause.
        void SaveAllData();
    }

    public class SaveController
    {
        [SerializeField]
        public List<ObjectSaver> objectSavers { get; private set; }

        [ShowInInspector]
        public List<ObjectSaver> toBeSaved = new List<ObjectSaver>();

        [SerializeField]
        private List<ISaveLoader> m_saveLoaders = new List<ISaveLoader>();

        public SaveController(List<ObjectSaver> initialObjectSaverList)
        {
            //Sets the object saver list.
            objectSavers = initialObjectSaverList;

            // Immediately adds the necessary save loaders.
            m_saveLoaders.Add(new OdinSaveLoader(this));

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
                saveLoader.LoadAllData();
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
        }
    }
}