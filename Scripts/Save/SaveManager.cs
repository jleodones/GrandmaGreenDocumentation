using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

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
                // TriggerSave();
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
    }   
}