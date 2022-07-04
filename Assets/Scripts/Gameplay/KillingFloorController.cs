using PotatoCat.Core;
using PotatoCat.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KillingFloorController : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D collision)
   {
      var player = collision.gameObject.GetComponent<PlayerComponent>();
      if (player != null)
      {
         Simulation.Schedule<PlayerDeath>().Player = player;
      }
   }
}
