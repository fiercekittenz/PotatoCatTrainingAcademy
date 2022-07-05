using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedSelfDestruct : MonoBehaviour
{
   public int MillisecondsToSelfDestruct = 1000;

   private DateTime mTimeCreated;

   private void Start()
   {
      mTimeCreated = DateTime.Now;
   }

   private void Update()
   {
      double msAlive = DateTime.Now.Subtract(mTimeCreated).TotalMilliseconds;
      if (msAlive >= MillisecondsToSelfDestruct)
      {
         Destroy(gameObject);
      }
   }
}
