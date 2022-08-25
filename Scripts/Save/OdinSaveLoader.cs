using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.IO;
using System.Security.Cryptography;

namespace GrandmaGreen.SaveSystem
{
    [System.Serializable]
    public class OdinSaveLoader : ISaveLoader
    {
        [ShowInInspector]
        [ReadOnly]
        private string m_baseFilePath { get; set; }

        [ShowInInspector]
        private SaveController m_saveController;

        [ShowInInspector]
        List<IComponentStore> deserializedComponentStores;

        public OdinSaveLoader(SaveController saveController)
        {
            // First sets the file path.
            m_baseFilePath = Application.persistentDataPath + "/Data";

            // Sets the save controller.
            m_saveController = saveController;
        }

        [Button()]
        public void LoadAllData()
        {
            // Iterates through every object saver in the save controller list and instantiates it based on its data.
            foreach(ObjectSaver os in m_saveController.objectSavers)
            {
                LoadData(os);
            }
        }

        [Button()]
        public void LoadData(ObjectSaver os)
        {
            string path = m_baseFilePath + "/" + os.ID + ".grandma";

            // First checks if the save file exists in the file path.
            if (!File.Exists(path))
            {
                return;
            }

            // If it does exist, it grabs the bytes from the save file and loads them up.
            byte[] bytes = File.ReadAllBytes(path);

            // This particular SaveLoader is looking to deserialize the saved component stores.
            os.componentStores = SerializationUtility.DeserializeValue<List<IComponentStore>>(bytes, DataFormat.JSON);

            // Lastly, adds save controller to object saver.
            os.saveController = m_saveController;
        }

        public void SaveAllData()
        {
            // Retrieves the object savers that need to be saved and write to file.
            foreach(ObjectSaver os in m_saveController.toBeSaved)
            {
                SaveData(os);
            }
        }

        [Button()]
        public void SaveData(ObjectSaver os)
        {
            string path = m_baseFilePath + "/" + os.ID + ".grandma";
            // FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            byte[] bytes = SerializationUtility.SerializeValue(os.componentStores, DataFormat.JSON);
            File.WriteAllBytes(path, bytes);
        }
    }
}