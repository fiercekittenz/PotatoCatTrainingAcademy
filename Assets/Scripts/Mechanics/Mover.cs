using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Mechanics
{
   public class Mover
   {
      Transform mObjectTransform;
      Vector2 mStartPosition;
      Vector2 mEndPosition;
      float p = 0;
      float duration;
      float startTime;

      public Mover(Transform objectTransform, Vector2 startPosition, Vector2 endPosition, float speed)
      {
         mObjectTransform = objectTransform;
         mStartPosition = startPosition;
         mEndPosition = endPosition;

         duration = (mEndPosition - mStartPosition).magnitude / speed;
         startTime = Time.time;
      }

      public Vector2 Position
      {
         get
         {
            p = Mathf.InverseLerp(0, duration, Mathf.PingPong(Time.time - startTime, duration));
            return Vector2.Lerp(mStartPosition, mEndPosition, p);
         }
      }
   }
}
