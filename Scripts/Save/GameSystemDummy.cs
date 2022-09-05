using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using GrandmaGreen;
using GrandmaGreen.Garden;
using GrandmaGreen.SaveSystem;
using System;

namespace GrandmaGreen {
    public class GameSystemDummy : MonoBehaviour
    {
        [SerializeField]
        private List<ObjectSaver> m_objectSavers = new List<ObjectSaver>();

        [Button(ButtonSizes.Medium)]
        public void AddTrait()
        {
            Allele[] arr = {(Allele) 1, (Allele) 0};
            var rand = new System.Random();

            Trait t;
            t.allele1 = arr[rand.Next() % 2];
            t.allele2 = arr[rand.Next() % 2];

            m_objectSavers[0].AddComponent<Trait>(t);
        }

        public int SampleTest()
        {
            return 3;
        }
    }
}
