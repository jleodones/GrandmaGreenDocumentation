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
        private string m_baseFilePath = Application.persistentDataPath + "/Data/save.grandma";

        [SerializeField]
        [ReadOnly]
        private SaveController m_saveController;

        [ReadOnly]
        public JArray jsonDataArray;

        public JSONSaveLoader(SaveController saveController)
        {
            m_saveController = saveController;

            if(!File.Exists(m_baseFilePath)) // If the file doesn't exist, create a new one.
            {
                // TODO: Replace this empty file with a file prefab that can be loaded for new games.
                using (var myFile = File.CreateText(m_baseFilePath))
                {
                    using (JsonTextWriter writer = new JsonTextWriter(myFile))
                    {
                        new JArray().WriteTo(writer);
                    }
                }
            }

            // Load all data in file path once, making it easy to update and adjust.
            jsonDataArray = JArray.Parse(File.ReadAllText(m_baseFilePath));
        }

        public void LoadAllData(List<ObjectSaver> objectSavers)
        {
            // Iterates through every ObjectSaver in the save controller list and instantiates it based on its data.
            foreach(ObjectSaver os in objectSavers)
            {
                LoadData(os);
            }
        }   

        public void LoadData(ObjectSaver os)
        {
            // This gives the ObjectSaver the appropriate SaveController (the one that just got instantiated).
            os.saveController = m_saveController;
            
            // Load the given ObjectSaver based off of the deserialized data.
            var osWrapper = jsonDataArray.Children<JObject>()
                .FirstOrDefault(o => o["ID"].ToString() == os.ID);

            // Converts the JArray data into an ObjectSaver and sets it.
            os = new JObject(osWrapper).ToObject<ObjectSaver>();
        }

        // This should only be saving to one save file.
        public void SaveAllData()
        {
            // Retrieves the object savers that need to be saved and adjusts it in the JSON data array living in local memory.
            foreach(ObjectSaver os in m_saveController.toBeSaved)
            {
                SaveData(os);
            }

            // Afterwards, save the modified data array to file.
            // NOTE: This saves the whole file, as JSONs cannot be overwritten in individual parts.
            File.WriteAllText(m_baseFilePath, jsonDataArray.ToString(Formatting.Indented));
        }

        [Button()]
        public void SaveData(ObjectSaver os)
        {
            // Find the specific os in the loaded data.
            var osWrapper = jsonDataArray.Children<JObject>()
                    .FirstOrDefault(o => o["ID"].ToString() == os.ID);

            if (osWrapper == null) // If it didn't find anything, add it to the data.
            {
                jsonDataArray.Add(JToken.FromObject(os));
            }
            else // If it found something already present in the array, it overwrites it with the updated ObjectSaver.
            {
                jsonDataArray.Children<JObject>()
                    .FirstOrDefault(o => o["ID"].ToString() == os.ID).Replace(JToken.FromObject(os));
            }
        }
    }
}
