using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Utility
{
   public static class MathHelpers
   {
      /// <summary>
      /// Takes the provided positions and averages them.
      /// </summary>
      public static Vector3 GetMeanVector(List<Vector3> positions)
      {
         if (positions.Count == 0)
         {
            return Vector3.zero;
         }

         Vector3 meanVector = Vector3.zero;

         foreach (Vector3 pos in positions)
         {
            meanVector += pos;
         }

         return (meanVector / positions.Count);
      }
   }
}