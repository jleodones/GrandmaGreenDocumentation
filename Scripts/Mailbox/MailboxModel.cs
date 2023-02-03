using System;
using UnityEngine;
using System.Collections.Generic;
using GrandmaGreen.SaveSystem;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace GrandmaGreen.Mail
{
    using Timer = TimeLayer.TimeLayer;

    [Serializable]
    public struct Letter
    {
        public long TimeSent;
        public string Subject;
        public string Heading;
        public string Body;
        public string Signature;
        public ushort[] Attachments;

        public Letter(System.DateTime time, string subject, string heading,
            string body, string signature, ushort[] attachments)
        {
            TimeSent = time.ToBinary();
            Subject = subject;
            Heading = heading;
            Body = body;
            Signature = signature;
            Attachments = attachments;
        }

        public override string ToString()
	    {
            return "Letter " + TimeSent + "(Click to Expand)"
                + "\nSubject: " + Subject
                + "\nHeading: " + Heading
                + "\nBody: " + Body
                + "\nSignature: " + Signature
                + "\nAttachments: " + Attachments
                + "\n";
	    }
    }
    
    public struct QueuedLetter
    {
        public long time;
        public Letter letter;
        
        public QueuedLetter(System.DateTime time, string subject, string heading,
            string body, string signature, ushort[] attachments)
        {
            this.time = time.ToBinary();
            letter = new Letter(time, subject, heading, body, signature, attachments);
        }
        
        public QueuedLetter(System.DateTime time, Letter letter)
        {
            this.time = time.ToBinary();
            this.letter = letter;
        }
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Mail/MailboxModel")]
    public class MailboxModel : ObjectSaver
    {
        [JsonIgnore]
        Timer timer;

        public bool Empty()
        {
            return GetLetters().Count == 0;
        }

        public void SendLetterNow(string subject, string heading, string body,
            string signature, ushort[] attachments = default(ushort[]))
        {
            var time = System.DateTime.Now;
            AddComponent<Letter>(-1, new Letter(time, subject, heading, body, signature, attachments));
        }

        public void SendLetterNow(Letter letter)
        {
            var time = System.DateTime.Now;
            AddComponent<Letter>(-1, letter);
        }

        public void SendLetterAt(System.DateTime time, string subject, string heading, string body,
            string signature, ushort[] attachments = default(ushort[]))
        {
            Debug.Log("Sending letter at " + time);
            AddComponent<QueuedLetter>(-1, new QueuedLetter(time, subject, heading, body, signature, attachments));
        }

        public void SendLetterAt(System.DateTime time, Letter letter)
        {
            AddComponent<QueuedLetter>(-1, new QueuedLetter(time, letter));
        }

        public List<Letter> GetLetters()
        {
            return ((ComponentStore<Letter>)GetComponentStore<Letter>()).components;
        }

        public List<QueuedLetter> GetLetterQueue()
        {
            return ((ComponentStore<QueuedLetter>)GetComponentStore<QueuedLetter>()).components;
	    }

        public Letter GetLetterByIndex(int index)
        {
            return GetLetters()[index];
        }

        /*
        public Letter GetLetterByDateTime(System.DateTime time)
        {
            return letters[time];
        }
        */

        public void RemoveLetter(int index)
        {
            GetLetters().RemoveAt(index);
        }

        private bool QueueEmpty()
        {
            return GetLetterQueue().Count == 0;
        }

        /*
        private System.DateTime NextSendTime()
        {
            return letterQueue.Values[0].TimeSent;
        }
        */
        
        public void Start()
        {
            //letters = new SortedList<System.DateTime, Letter>();
            // letterQueue = new SortedList<System.DateTime, Letter>();
        }

        /*
        public bool CheckMailQueue()
        {
            bool letterRemoved = false;
            while (!QueueEmpty() && NextSendTime() < System.DateTime.Now)
            {
                Letter letter = letterQueue.Values[0];
                letters.Add(letter.TimeSent, letter);
                letterQueue.RemoveAt(0);
                letterRemoved = true;
            }
            return letterRemoved;
        }
        */

        public string MailboxString()
        {
            string str = "";
            if (!Empty())
            {
                foreach (var letter in GetLetters())
                {
                    str += letter;
                }
            }
            return str;
        }
    }
}