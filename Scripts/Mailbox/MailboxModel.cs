using System;
using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GrandmaGreen.Mail
{
    using Timer = TimeLayer.TimeLayer;

    [Serializable]
    public struct Letter
    {
        public System.DateTime TimeSent;
        public string Subject;
        public string Heading;
        public string Body;
        public string Signature;
        public ushort[] Attachments;

        public Letter(System.DateTime time, string subject, string heading,
            string body, string signature, ushort[] attachments)
        {
            TimeSent = time;
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

    [CreateAssetMenu(menuName = "GrandmaGreen/Mail/MailboxModel")]
    public class MailboxModel : ScriptableObject
    {
        [ShowInInspector]
        public SortedList<System.DateTime, Letter> letters;
        [ShowInInspector]
        public SortedList<System.DateTime, Letter> letterQueue;
        Timer timer;

        public bool Empty()
        {
            return letters.Count == 0;
        }

        public void SendLetterNow(string subject, string heading, string body,
            string signature, ushort[] attachments = default(ushort[]))
        {
            var time = System.DateTime.Now;
            letters.Add(time, new Letter(time, subject, heading, body, signature, attachments));
        }

        public void SendLetterNow(Letter letter)
        {
            var time = System.DateTime.Now;
            letters.Add(time, letter);
        }

        public void SendLetterAt(System.DateTime time, string subject, string heading, string body,
            string signature, ushort[] attachments = default(ushort[]))
        {
            Debug.Log("Sending letter at " + time);
            letterQueue.Add(time, new Letter(time, subject, heading, body, signature, attachments));
        }

        public void SendLetterAt(System.DateTime time, Letter letter)
        {
            letterQueue.Add(time, letter);
        }

        public List<Letter> GetLetters()
        {
            return new List<Letter>(letters.Values);
        }

        public List<Letter> GetLetterQueue()
        {
            return new List<Letter>(letterQueue.Values);
	    }

        public Letter GetLetterByIndex(int index)
        {
            return letters.Values[index];
        }

        public Letter GetLetterByDateTime(System.DateTime time)
        {
            return letters[time];
        }

        public void RemoveLetter(int index)
        {
            letters.RemoveAt(index);
        }

        private bool QueueEmpty()
        {
            return letterQueue.Count == 0;
        }

        private System.DateTime NextSendTime()
        {
            return letterQueue.Values[0].TimeSent;
        }

        public void Start()
        {
            letters = new SortedList<System.DateTime, Letter>();
            letterQueue = new SortedList<System.DateTime, Letter>();
        }

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

        public override string ToString()
        {
            string str = "";
            foreach (var letter in GetLetters())
            {
                str += letter;
            }
            return str;
        }
    }
}