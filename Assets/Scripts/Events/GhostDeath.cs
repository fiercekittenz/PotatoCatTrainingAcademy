using PotatoCat.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Events
{
   public class GhostDeath : Simulation.Event<GhostDeath>
   {
      public Animator Animator;
      public PlayerComponent Player;
      public GameObject Ghost;
      public bool FromJump;

      public override void Execute()
      {
         if (Player != null)
         {
            if (FromJump)
            {
               Player.Bounce(6);
            }
         }

         Animator.SetBool("IsDying", true);
         Object.Destroy(Ghost, 0.833f);
      }
   }
}
