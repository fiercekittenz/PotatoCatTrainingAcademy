using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PotatoCat.Core;

namespace PotatoCat.Events
{
   public class PlayerDeath : Simulation.Event<PlayerDeath>
   {
      public PlayerComponent Player { get; set; }

      public override void Execute()
      {
         if (Player != null)
         {
            Player.ControlEnabled = false;

            if (Player.AudioSource != null &&
                Player.HealthComponent != null &&
                Player.HealthComponent.DeathSound != null)
            {
               Player.AudioSource.PlayOneShot(Player.HealthComponent.DeathSound);
            }

            if (Player.Animator != null)
            {
               Player.Animator.SetBool("IsDead", true);
            }

            Simulation.Schedule<PlayerRespawn>(2.0f).Player = Player;
         }
      }
   }
}
