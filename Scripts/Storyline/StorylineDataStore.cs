using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.SaveSystem;

namespace GrandmaGreen
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Storyline/Data Store")]
    public class StorylineDataStore : ObjectSaver
    {
        public List<Storyline> storylineSet;

        public void Initalize()
        {
            foreach (Storyline storyline in storylineSet)
            {
                storyline.onProgress += UpdateStorylineProgress;
                storyline.onRegress += UpdateStorylineProgress;
                storyline.onCompletion += UpdateStorylineProgress;
            }

            LoadStorylineProgress();
        }

        public void Release()
        {
            foreach (Storyline storyline in storylineSet)
            {
                storyline.onProgress -= UpdateStorylineProgress;
                storyline.onRegress -= UpdateStorylineProgress;
                storyline.onCompletion -= UpdateStorylineProgress;
            }
        }

        public void LoadStorylineProgress()
        {
            uint newProgress = 0;
            foreach (Storyline storyline in storylineSet)
            {
                if(RequestData<uint>(storylineSet.IndexOf(storyline),ref newProgress))
                {
                    storyline.SetProgress(newProgress);
                }
                else
                    AddComponent<uint>(storylineSet.IndexOf(storyline), 0);

                storyline.StartStory();
            }
        }

        public void UpdateStorylineProgress(Storyline storyline)
        {
            UpdateValue<uint>(storylineSet.IndexOf(storyline), storyline.progress);
        }
    }
}
