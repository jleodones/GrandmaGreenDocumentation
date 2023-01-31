using UnityEngine;
using System.Collections;

namespace GrandmaGreen.Mail
{
    public class MailboxController : MonoBehaviour
    {
        public MailboxModel mailbox;

        void Start()
        {
            mailbox.Start();
        }

        void Update()
        {
            if (mailbox.CheckMailQueue())
            {
                Debug.Log("Letter Received at " + System.DateTime.Now);
                Debug.Log(mailbox);
            }
        }

        [ContextMenu("TestSendLetterNow")]
        public void DebugSendLetterNow()
        {
            Debug.Log("--- Sending Letter");
            mailbox.SendLetterNow("Hello World!", "Heading", "Lorem ipsum", "From Grandma");
            Debug.Log(mailbox);
	    }

        [ContextMenu("TestSendLetterIn5s")]
        public void DebugSendLetterIn5s()
        {
            var time = System.DateTime.Now.AddSeconds(5);
            Debug.Log("--- Sending letter at " + time);
            mailbox.SendLetterAt(time,
                "Hello World in 5 seconds!",
                "We'll wait for 5 seconds",
                "5 seconds have passed...",
                "From Grandma"); 
	    }

        [ContextMenu("TestOldLettersArrive")]
        public void TestOldLettersArrive()
        {
            var time = System.DateTime.Now.AddSeconds(-5);
            Debug.Log("--- Sending 5 Letters at " + time);
            for (int i = 0; i < 5; i++) {
                double randMs = UnityEngine.Random.Range(0, 1000);
                var time2 = time.AddMilliseconds(randMs);
                mailbox.SendLetterAt(time2,
                    "Hello world 5 seconds ago!",
                    "We already waited 5 seconds",
                    "-5 seconds have passed...",
                    "From Grandma");
            }
        }

        [ContextMenu("GetLetters")]
        public void GetLetters()
        {
            Debug.Log(mailbox);
        }
    }
}
