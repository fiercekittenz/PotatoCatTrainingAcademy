using PotatoCat.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Core
{
   public class BaseEnemy : MonoBehaviour
   {
      //
      // Editor Properties
      //

      public bool AllowStompDamageFromPlayer = true;

      //
      // Public Properties
      //

      [HideInInspector]
      public Collider2D Collider2d { get; set; }
      public Bounds Bounds => Collider2d.bounds;
      public HealthComponent HealthComponentRef { get; protected set; }
      public PlayerComponent PlayerComponentRef { get; protected set; }
      public bool CanTakeDamage { get; set; } = true;

      protected virtual void Awake()
      {
         Collider2d = GetComponent<Collider2D>();
         HealthComponentRef = GetComponent<HealthComponent>();
      }

      public virtual void HandleDeath(bool fromJump = false)
      {
      }

      protected void OnCollisionEnter2D(Collision2D collision)
      {
         if (HealthComponentRef.CurrentHealth <= 0)
         {
            // Do not handle anymore collision while dying.
            return;
         }

         //
         // React to being stomped on by the player. If the player missed, do damage.
         // 

         if (collision.gameObject.tag.Equals("player", StringComparison.OrdinalIgnoreCase))
         {
            var playerComponent = collision.gameObject.GetComponent<PlayerComponent>();
            if (CanTakeDamage && playerComponent != null)
            {
               bool willHurtEnemy = playerComponent.Bounds.center.y >= Bounds.max.y;
               if (willHurtEnemy)
               {
                  // Only deal 1 damage when stomping. Record the player component for reference
                  // if the enemy handles damage by bouncing the player.
                  PlayerComponentRef = playerComponent;
                  HealthComponentRef.TakeDamage(1, true);
               }
               else
               {
                  // Only take 1 damage when the player collides.
                  // Note: For the future, this could be configured on the BaseEnemy.
                  playerComponent.HealthComponent.TakeDamage(1);
               }
            }
         }
      }
   }
}