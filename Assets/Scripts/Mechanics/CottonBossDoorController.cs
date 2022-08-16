using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Mechanics
{
   public class CottonBossDoorController : MonoBehaviour
   {
      //
      // Public Editor Properties
      //

      public CottonSmilesController CottonSmilesControllerRef;

      //
      // Public Properties
      //

      public DoorState State { get; private set; } = DoorState.Open;
      public enum DoorState
      {
         Open,
         WaitingForBattle,
         ClosedForBattle
      }

      //
      // Private Members
      //

      private DateTime mTimeInteractionStarted;
      private static int skSecondsToStartBattle = 3;

      /// <summary>
      /// Handle when the player fires the trigger through collision events.
      /// </summary>
      public void OnTriggerEnter2D(Collider2D collision)
      {
         if (State == DoorState.Open &&
             collision.tag.Equals("player", System.StringComparison.OrdinalIgnoreCase))
         {
            State = DoorState.WaitingForBattle;
            mTimeInteractionStarted = DateTime.Now;
         }
      }

      /// <summary>
      /// Handle the state of the door on the update frame.
      /// </summary>
      protected void Update()
      {
         if (State == DoorState.WaitingForBattle)
         { 
            double secondsSinceEntered = DateTime.Now.Subtract(mTimeInteractionStarted).TotalSeconds;
            if (secondsSinceEntered >= skSecondsToStartBattle)
            {
               State = DoorState.ClosedForBattle;
               CottonSmilesControllerRef.ProjectileUserComponentRef.IsAutomaticFireOn = true;
            }
         }
      }
   }
}
