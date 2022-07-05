using PotatoCat.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Events
{
   public class FartDeath : Simulation.Event<FartDeath>
   {
      public Animator Animator;
      public GameObject Fart;
      public PlayerComponent Player;

      public override void Execute()
      {
         if (Player != null)
         { 
            Player.Bounce(5);

            // Only play this death sound if the player caused the damage, not the projectile. The 
            // projectile has its own death sound that will collide with this one.
            AudioClip deathSound = Fart.GetComponent<FartController>().DeathSound;
            if (deathSound != null)
            {
               GameSimulation.Instance.AudioSource.PlayOneShot(deathSound);
            }
         }

         Animator.SetBool("IsDying", true);
         Object.Destroy(Fart, 0.833f);
      }
   }
}
