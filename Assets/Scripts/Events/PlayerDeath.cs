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
         AudioSource source = Player.GetComponent<AudioSource>();
         if (source != null && Player.Death != null)
         {
            source.PlayOneShot(Player.Death);
         }

         Vector3 spawnPointPosition = Player.SpawnLocation.transform.position;
         Player.transform.position = spawnPointPosition;
      }
   }
}
