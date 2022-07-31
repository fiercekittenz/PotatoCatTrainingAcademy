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
      float mSpeed = 1.0f;
      float mPingPong = 0;
      float mDuration;
      float mStartTime;

      public Mover(Transform objectTransform, Vector2 startPosition, Vector2 endPosition, float speed)
      {
         mObjectTransform = objectTransform;
         mStartPosition = startPosition;
         mEndPosition = endPosition;         
         mStartTime = Time.time;
         Speed = speed;
      }

      public float Speed
      {
         get { return mSpeed; }
         set
         {
            mSpeed = value;
            mDuration = (mEndPosition - mStartPosition).magnitude / value;
         }
      }

      public Vector2 Position
      {
         get
         {
            mPingPong = Mathf.InverseLerp(0, mDuration, Mathf.PingPong(Time.time - mStartTime, mDuration));
            return Vector2.Lerp(mStartPosition, mEndPosition, mPingPong);
         }
      }
   }
}
