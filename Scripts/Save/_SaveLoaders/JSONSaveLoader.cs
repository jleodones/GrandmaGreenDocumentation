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
        
        [SerializeField]
        [ReadOnly]
        private SaveController m_saveController;

        [ShowInInspector] private List<ObjectSaver> m_objectSavers;

        public JSONSaveLoader(SaveController saveController)
        {
            m_saveController = saveController;

            if (!File.Exists(m_baseFilePath)) // If the file doesn't exist, create a new one.
            {
                using (StreamWriter file = File.CreateText(m_baseFilePath))
                {
                    TextAsset f = Resources.Load("DefaultSaveFile") as TextAsset;
                    
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

        /// <summary>
        /// Fully deletes current save file.
        /// </summary>
        public void DeleteAllData()
        {
            if (File.Exists(m_baseFilePath)) // If the save file exists, delete it.
            {
                File.Delete(m_baseFilePath);
            }
        }

        public void CreateDefaultSaveFile()
        {
            // Loads from relative path—editor only.
            string defaultSavePath = "Assets/_GrandmaGreen/_Modules/_App/Resources/DefaultSaveFile.txt";

            // Serializes and write to file.
            using (StreamWriter file = new StreamWriter(defaultSavePath))
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
