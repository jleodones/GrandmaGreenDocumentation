using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using UnityEngine;

namespace GrandmaGreen
{
    public class BulletinBoardUIController
    {
        public Contest currentBigContest;
        private BulletinBoardUIDisplay m_parent;
        
        public BulletinBoardUIController(BulletinBoardUIDisplay parent)
        {
            m_parent = parent;
            
            // Setting up default contest.
            // TODO: Set this up for randomization later.
            currentBigContest.targetPlant = new Plant((ushort) PlantId.Rose, "Rose", new Genotype("AaBb"));
            currentBigContest.rewardMoney = 200;
        }
        
    }
}
