using PotatoCat.Core;
using PotatoCat.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CottonSmilesController : BaseEnemy
{
   //
   // Public Properties (Editor)
   //

   public int HealthAmountVanTrigger = 3;
   public int SecondsOfVanTime = 5;

   //
   // Public Properties
   //

   public ProjectileUserComponent ProjectileUserComponentRef { get; private set; }
   public Animator Animator { get; private set; }

   //
   // Public Enums
   //

   public enum State
   {
      Default,
      Van
   }

   //
   // Private Members
   //

   private State mState = State.Default;
   private DateTime mVanModeStarted;
   private static string skAnimParam_IsVanMode = "IsVanMode";

   /// <summary>
   /// Handle initialization.
   /// </summary>
   protected override void Awake()
   {
      base.Awake();

      ProjectileUserComponentRef = GetComponent<ProjectileUserComponent>();
      Animator = GetComponent<Animator>();

      if (HealthComponentRef != null)
      {
         HealthComponentRef.OnHealthChanged += HealthComponentRef_OnHealthChanged;
      }
   }

   /// <summary>
   /// Handle the death of the boss.
   /// </summary>
   public override void HandleDeath(bool fromJump = false)
   {
      base.HandleDeath(fromJump);

      Collider2d.enabled = false;
      Destroy(gameObject);
   }

   /// <summary>
   /// Process the frame.
   /// </summary>
   protected void FixedUpdate()
   {
      switch (mState)
      {
         case State.Default:
            {
            }
            break;

         case State.Van:
            {
               double timeInVan = DateTime.Now.Subtract(mVanModeStarted).TotalSeconds;
               if (timeInVan >= SecondsOfVanTime)
               {
                  SetVanMode(false);
               }
            }
            break;
      }
   }

   private void SetVanMode(bool vanModeOn)
   {
      if (vanModeOn)
      {
         mState = State.Van;
         ProjectileUserComponentRef.IsAutomaticFireOn = false;
         mVanModeStarted = DateTime.Now;
         CanTakeDamage = false; // No damage during van mode.
         Animator.SetBool(skAnimParam_IsVanMode, true);
      }
      else
      {
         mState = State.Default;
         ProjectileUserComponentRef.IsAutomaticFireOn = true;
         CanTakeDamage = true;
         Animator.SetBool(skAnimParam_IsVanMode, false);
      }
   }

   /// <summary>
   /// Handle the event where Cotton's health is adjusted.
   /// </summary>
   private void HealthComponentRef_OnHealthChanged(object sender, System.EventArgs e)
   {
      if (sender is HealthComponent healthComponent)
      {
         // Bounce the player.
         ProjectileUserComponentRef.PlayerComponentRef.Bounce(3);

         if (healthComponent.CurrentHealth > 0)
         {
            // We only do 1 damage per attack as the player, so it's safe to do an equality check here.
            if (healthComponent.CurrentHealth == HealthAmountVanTrigger)
            {
               SetVanMode(true);
            }
            else
            {
               SetVanMode(false);
            }
         }
      }
   }
}
