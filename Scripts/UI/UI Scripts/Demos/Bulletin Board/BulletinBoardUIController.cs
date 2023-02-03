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
            currentBigContest.targetPlant = new Plant((ushort) PlantId.Tulip, "Tulip", new Genotype("AaBb"));
            currentBigContest.rewardMoney = 200;
        }

        // TODO: Make this more advanced. Currently only evaluates based on plant type.
        public bool EvaluatePlant(Plant submissionPlant)
        {
            // Check if they're the same plant lol.
            if (submissionPlant.itemID == currentBigContest.targetPlant.itemID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
