using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GrandmaGreen.SaveSystem
{
    /// <summary>
    /// The SaveManager class is in charge of high level saving. It is the only part of the save system that lives in the game.
    /// Upon awakening, it instantiates a new SaveController. It also tracks the auto save intervals, then pings the SaveController at
    /// appropriate moments. The SaveManager also stores an internal list of every ObjectSaver relevant to the whole game.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        [SerializeField]
        private float m_internalTime = 0.0f;

        [SerializeField]
        private float m_autoSaveBuffer = 10.0f;

        [ShowInInspector]
        private SaveController m_saveController;

        [SerializeField]
        private List<ObjectSaver> m_objectSavers = new List<ObjectSaver>();

        public void Awake()
        {
            m_saveController = new SaveController(m_objectSavers);

            EventManager.instance.EVENT_DELETE_SAVE += TriggerDeleteAllData;
        }

        public void OnDestroy()
        {
            EventManager.instance.EVENT_DELETE_SAVE -= TriggerDeleteAllData;
        }

        /// <summary>
        /// On Update, the save manager performs a check to see if the game must be saved.
        /// The game is auto saved after certain predetermined time intervals.
        /// </summary>
        public void Update()
        {
            m_internalTime += Time.deltaTime;
            
            if(m_internalTime >= m_autoSaveBuffer)
            {
                TriggerSave();
                m_internalTime = 0.0f;
            }
        }
        
        /// <summary>
        /// Triggers the SaveController AutoSave function.
        /// </summary>
        [Button(ButtonSizes.Medium)]
        public void TriggerSave()
        {
            m_saveController.AutoSave();
        }


        /// <summary>
        /// Wipe the player's saved data.
        /// </summary>
        [Button(ButtonSizes.Medium)]
        public void TriggerDeleteAllData()
        {
            m_saveController ??= new SaveController();
            m_saveController.DeleteAllData();

            // Refresh.
            m_saveController = new SaveController(m_objectSavers);
        }
        
        /// <summary>
        /// Editor only—creates a new default save file out of any developer adjustments. Run from the prefab.
        /// </summary>
        [Button(ButtonSizes.Medium)]
        public void TriggerAddNewComponentToDefault(ObjectSaver objectSaver)
        {
            m_objectSavers.Add(objectSaver);
            
            // Creates new save controller with the object savers. This automatically loads the save loaders based on the save file we already have.
            m_saveController = new SaveController(m_objectSavers);
            
            m_saveController.CreateNewDefaultSaveFile();
        }
    }   
}