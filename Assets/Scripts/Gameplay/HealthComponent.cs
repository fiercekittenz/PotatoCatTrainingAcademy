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
      public AudioClip HealingSound;
      public AudioClip DamagedSound;

      [HideInInspector]
      private int mCurrentHealth = 0;

      /// <summary>
      /// Deals damage to the game object. Returns if the object should be considered dead or not.
      /// </summary>
      /// <param name="amount">The amount of damage to deal.</param>
      /// <returns>If the game object is dead after taking damage.</returns>
      public bool TakeDamage(int amount)
      {
         mCurrentHealth -= amount;
         if (mCurrentHealth < 0)
         {
            if (DeathSound != null)
            { 
               GameSimulation.Instance.AudioSource.PlayOneShot(DeathSound);
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
         mCurrentHealth = Math.Clamp(mCurrentHealth + amount, 0, MaxHealth);

         if (HealingSound != null)
         { 
            GameSimulation.Instance.AudioSource.PlayOneShot(HealingSound);
         }
      }

      public void Reset()
      {
         mCurrentHealth = MaxHealth;
      }

      private void Awake()
      {
         mCurrentHealth = MaxHealth;
      }
   }
}