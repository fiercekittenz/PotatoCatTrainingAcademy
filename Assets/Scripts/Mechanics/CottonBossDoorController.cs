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

      public bool IsDoorPassable { get; private set; } = true;

      /// <summary>
      /// Handle when the player fires the trigger through collision events.
      /// </summary>
      public void OnTriggerEnter2D(Collider2D collision)
      {
         if (collision.tag.Equals("player", System.StringComparison.OrdinalIgnoreCase))
         {
            CottonSmilesControllerRef.ProjectileUserComponentRef.IsAutomaticFireOn = true;
         }
      }
   }
}
