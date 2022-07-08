using PotatoCat.Core;
using PotatoCat.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Gameplay
{
   public class HealthComponent : MonoBehaviour
   {
      public int MaxHealth = 1;
      public AudioClip DeathSound;
      public AudioClip DamagedSound;

      [HideInInspector]
      public int CurrentHealth { get; private set; } = 0;
      private HeartMeterComponent HeartMeterComponent { get; set; }
      private PlayerComponent PlayerComponent { get; set; }
      private BaseEnemy BaseEnemyComponent { get; set; }

      /// <summary>
      /// Deals damage to the game object. Returns if the object should be considered dead or not.
      /// </summary>
      /// <param name="amount">The amount of damage to deal.</param>
      /// <param name="fromJump">If the damage is coming a jump above.</param>
      /// <returns>If the game object is dead after taking damage.</returns>
      public bool TakeDamage(int amount, bool fromJump = false)
      {
         CurrentHealth = Math.Clamp(CurrentHealth - amount, 0, MaxHealth);
         UpdateHeartMeter();

         if (CurrentHealth <= 0)
         {
            if (DeathSound != null)
            { 
               GameSimulation.Instance.AudioSource.PlayOneShot(DeathSound);
            }

            if (PlayerComponent != null)
            { 
               Simulation.Schedule<PlayerDeath>().Player = PlayerComponent;
            }
            else if (BaseEnemyComponent != null)
            {
               BaseEnemyComponent.HandleDeath(fromJump);
            }

            return true;
         }
         else
         {
            if (DamagedSound != null)
            { 
               GameSimulation.Instance.AudioSource.PlayOneShot(DamagedSound);
            }
         }

         return false;
      }

      public void Heal(int amount)
      {
         CurrentHealth = Math.Clamp(CurrentHealth + amount, 0, MaxHealth);
         UpdateHeartMeter();

         // Do not play a sound. Each item may make the player say or do
         // something special on a heal bonus.
      }

      private void UpdateHeartMeter()
      {
         if (HeartMeterComponent != null)
         {
            HeartMeterComponent.UpdateDisplay(CurrentHealth);
         }
      }

      public void Reset()
      {
         CurrentHealth = MaxHealth;
         UpdateHeartMeter();
      }

      private void Awake()
      {
         CurrentHealth = MaxHealth;

         HeartMeterComponent = gameObject.GetComponentInParent<HeartMeterComponent>();
         PlayerComponent = gameObject.GetComponentInParent<PlayerComponent>();
         BaseEnemyComponent = gameObject.GetComponentInParent<BaseEnemy>();

         UpdateHeartMeter();
      }
   }
}