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
   // Public Properties (privately set)
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
                  mState = State.Default;
                  Animator.SetBool(skAnimParam_IsVanMode, false);
               }
            }
            break;
      }
   }

   /// <summary>
   /// Handle the event where Cotton's health is adjusted.
   /// </summary>
   private void HealthComponentRef_OnHealthChanged(object sender, System.EventArgs e)
   {
      if (sender is HealthComponent healthComponent)
      {
         // We only do 1 damage per attack as the player, so it's safe to do an equality check here.
         if (healthComponent.CurrentHealth == HealthAmountVanTrigger)
         {
            mState = State.Van;
            mVanModeStarted = DateTime.Now;
            Animator.SetBool(skAnimParam_IsVanMode, true);
         }
         else
         {
            mState = State.Default;
            Animator.SetBool(skAnimParam_IsVanMode, false);
         }
      }
   }
}
