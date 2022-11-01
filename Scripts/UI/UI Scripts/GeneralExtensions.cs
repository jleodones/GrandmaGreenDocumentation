using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace GrandmaGreen
{
   public static class GeneralExtensions
    {
        /// <summary>
        /// Returns the current vector with a new X
        /// </summary>
        public static Vector2 WithNewX(this Vector2 vector, float newX) => new Vector2(newX, vector.y);
    }
}
