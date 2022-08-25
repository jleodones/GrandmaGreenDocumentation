using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace GrandmaGreen.SaveSystem
{
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

        // On update, the save manager performs a check to see if the game must be saved.
        // The game is auto saved after certain time intervals.
        public void Update()
        {
            m_internalTime += Time.deltaTime;
            
            if(m_internalTime >= m_autoSaveBuffer)
            {
                // TriggerSave();
                m_internalTime = 0.0f;
            }
        }

        [Button(ButtonSizes.Medium)]
        public void TriggerSave()
        {
            m_saveController.AutoSave();
        }
    }   
}