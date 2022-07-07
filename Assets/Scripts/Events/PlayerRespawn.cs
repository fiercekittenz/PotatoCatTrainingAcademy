using PotatoCat.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Events
{
   public class PlayerRespawn : Simulation.Event<PlayerRespawn>
   {
      public PlayerComponent Player { get; set; }

      public override void Execute()
      {
         if (Player.Animator != null)
         {
            Player.Animator.SetBool("IsDead", false);
         }

         Player.HealthComponent.Reset();

         Vector3 spawnPointPosition = Player.SpawnLocation.transform.position;
         Player.transform.position = spawnPointPosition;
         Player.ControlEnabled = true;
      }
   }
}