using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Net;
using GrandmaGreen.SaveSystem;

namespace GrandmaGreen.TimeLayer
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Time/Time Layer Clock")]
    public class TimeLayerClock : ObjectSaver
    {
        [SerializeField] bool m_CheckPreviousSessionDateTime = false;
        [SerializeField] System.DateTime m_previousSessionEndDateTime;
        [SerializeField] System.DateTime m_currentSessionInitalDataTime;
        [SerializeField] List<TimeLayer> timeLayers;

        public void SetClock(bool checkInternetTime = true)
        {
            m_currentSessionInitalDataTime = checkInternetTime ? InternetDateTime() : System.DateTime.Now;

            Debug.Log("Session Inital Time set to: " + m_currentSessionInitalDataTime);

            if (m_CheckPreviousSessionDateTime)
                LoadPreviousSessionEndTime();

        }

        public void LoadPreviousSessionEndTime()
        {
            m_previousSessionEndDateTime = default;

            if (!RequestData<System.DateTime>(0, ref m_previousSessionEndDateTime))
            {
                m_previousSessionEndDateTime = m_currentSessionInitalDataTime;
                CreateNewStore<System.DateTime>();
                AddComponent<System.DateTime>(0, m_previousSessionEndDateTime);
            }


            Debug.Log("Prev Session End Time loaded as:" + m_previousSessionEndDateTime);

            double deltaTime = m_currentSessionInitalDataTime.Subtract(m_previousSessionEndDateTime).TotalSeconds;

            TickClock(deltaTime);
        }

        public void TickClock(double deltaTime)
        {
            foreach (TimeLayer timeLayer in timeLayers)
            {
                timeLayer.Tick(deltaTime);
            }
        }



        public void SaveCurrentDateTime()
        {
            m_previousSessionEndDateTime = InternetDateTime();

            if (!UpdateValue<System.DateTime>(0, m_previousSessionEndDateTime))
                Debug.Log("Could not save Session End Time");
            else
                Debug.Log("Session End Time saved as: " + m_previousSessionEndDateTime);
        }

        public static System.DateTime InternetDateTime()
        {
            try
            {
                Debug.Log("---Attempting to fetch Internet Time---");
                using (var response =
                    WebRequest.Create("http://www.google.com").GetResponse())
                    //string todaysDates =  response.Headers["date"];
                    return System.DateTime.ParseExact(response.Headers["date"],
                        "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture.DateTimeFormat,
                        DateTimeStyles.AssumeUniversal);
            }
            catch (WebException)
            {
                Debug.Log("---Failed to fetch Internet Time, falling back to System Time---");
                return System.DateTime.Now; //In case something goes wrong. 
            }
        }

    }


}
