using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GrandmaGreen.SaveSystem
{
    /// <summary>
    /// The JSONSaveLoader was designed specifically to serialize and deserialize JSON files ONLY.
    /// This is used largely for internal testing and readable save files, but can be converted to BSON as needed.
    /// </summary>
    public class JSONSaveLoader : ISaveLoader
    {
        private string m_baseFilePath = Application.persistentDataPath + "/save.grandma";

        // private string m_baseFilePath = Application.dataPath + "/_GrandmaGreen/Scripts/Save/FirstPlayableSave.grandma";

        [SerializeField]
        [ReadOnly]
        private SaveController m_saveController;

        [ShowInInspector] private List<ObjectSaver> m_objectSavers;
        
        // Archived below -- these were initially used to concurrently keep track of data for future scene loads without making
        // further calls to JSON deserializers, but it felt like a waste of space. In this case, the optimization wasn't really
        // worth it...But I am concerned about data corruption.

        // [ShowInInspector] private List<ObjectSaver> m_loadedList;

        // [ReadOnly] public JArray jsonDataArray;

        public JSONSaveLoader(SaveController saveController)
        {
            m_saveController = saveController;

            if (!File.Exists(m_baseFilePath)) // If the file doesn't exist, create a new one.
            {
                using (StreamWriter file = File.CreateText(m_baseFilePath))
                {
                    // TODO: Replace this with an actual default save file.
                    TextAsset f = Resources.Load("FirstPlayableSave") as TextAsset;
                    
                    file.Write(f.text);
                }
            }
        }

        public void LoadAllData(ref List<ObjectSaver> objectSavers)
        {
            m_objectSavers = objectSavers;
            List<ObjectSaver> loadedList;
            
            using (StreamReader file = File.OpenText(m_baseFilePath))
            {
                JsonSerializer serializer = new JsonSerializer()
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                loadedList = (List<ObjectSaver>)serializer.Deserialize(file, typeof(List<ObjectSaver>));
            }

            // Iterates through every ObjectSaver in the save controller list and instantiates it based on its loaded data.
            foreach(ObjectSaver os in objectSavers)
            {
                os.Set(m_saveController, loadedList.Find(obj => obj.ID == os.ID));
            }
        }

        // This should only be saving to one save file.
        [Button()]
        public void SaveAllData()
        {
            // Serializes and write to file.
            using (StreamWriter file = new StreamWriter(m_baseFilePath))
            {
                JsonSerializer serializer = new JsonSerializer()
                {
                    Formatting = Formatting.Indented,
                    TypeNameHandling = TypeNameHandling.Auto
                };
                serializer.Serialize(file, m_objectSavers);
            }
        }
    }
}