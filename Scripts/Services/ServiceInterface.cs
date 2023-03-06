using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GrandmaGreen
{
    public interface IServicer<T>
    {
        protected static T[] s_activeServices { get; set; }

        void RegisterService(T service, int index);

        void DeregisterService(T service, int index);

        T PrimaryService => s_activeServices[0];

        T this[int i] { get => s_activeServices[i]; }
    }


    public interface IServiceUser<U, T> where T : IServicer<U>
    {
        public static T Servicer { get; private set; }

        public static void SetServicer(T servicer)
        {
            Servicer = servicer;
        }

        public static void ClearServicer()
        {
            Servicer = default(T);
        }

    }

    
}
