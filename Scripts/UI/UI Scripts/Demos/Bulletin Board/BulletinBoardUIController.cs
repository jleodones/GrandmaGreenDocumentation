using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using UnityEngine;

namespace GrandmaGreen.UI.BulletinBoard
{
    public class BulletinBoardUIController
    {
        public Contest currentBigContest;
        public BulletinDataStore possibleBulletins;
        //private int currContestIndex = 0;
        private BulletinBoardUIDisplay m_parent;
        
        public BulletinBoardUIController(BulletinBoardUIDisplay parent)
        {
            m_parent = parent;

            // Setting up default contest.
            // TODO: Set this up for randomization later.
            possibleBulletins = m_parent.b_dataStore;
            currentBigContest.targetPlant = new Plant((ushort) PlantId.Tulip, "Tulip", new Genotype("AaBb"));
            currentBigContest.rewardMoney = 200;
        }

        // TODO: Make this more advanced. Currently only evaluates based on plant type.
        public bool EvaluatePlant(Plant submissionPlant)
        {
            EventManager.instance.HandleEVENT_SUBMIT_PLANT(submissionPlant.itemID, submissionPlant.plantGenotype);
            // Check if they're the same plant lol.
            return possibleBulletins.bulletinBoardOptions[0].WinState;
            /*if (submissionPlant.itemID == currentBigContest.targetPlant.itemID)
            {
                return true;
            }
            else
            {
                return false;
            }*/
        }
    }
}
