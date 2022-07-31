using PotatoCat.Core;
using PotatoCat.Gameplay;
using PotatoCat.Mechanics;
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
   public Vector2 LeftMostPosition;
   public Vector2 RightMostPosition;

   //
   // Public Properties
   //

   public ProjectileUserComponent ProjectileUserComponentRef { get; private set; }
   public Animator Animator { get; private set; }
   public SpriteRenderer SpriteRendererRef { get; private set; }

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
   private Mover mMover;
   private static string skAnimParam_IsVanMode = "IsVanMode";

   /// <summary>
   /// Handle initialization.
   /// </summary>
   protected override void Awake()
   {
      base.Awake();

      ProjectileUserComponentRef = GetComponent<ProjectileUserComponent>();
      Animator = GetComponent<Animator>();
      SpriteRendererRef = GetComponent<SpriteRenderer>();

      if (HealthComponentRef != null)
      {
         HealthComponentRef.OnHealthChanged += HealthComponentRef_OnHealthChanged;
      }

      mMover = new Mover(gameObject.transform, RightMostPosition, LeftMostPosition, 1.0f);
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
               mMover.Speed = 0.5f;
            }
            break;

         case State.Van:
            {
               mMover.Speed = 1.5f;
               double timeInVan = DateTime.Now.Subtract(mVanModeStarted).TotalSeconds;
               if (timeInVan >= SecondsOfVanTime)
               {
                  SetVanMode(false);
               }
            }
            break;
      }

      if (mMover != null)
      {
         // We only care about left to right movement on the x axis.
         float currentX = gameObject.transform.localPosition.x;

         float newX = mMover.Position.x;
         float newY = mMover.Position.y;

         if (mState == State.Van)
         {
            // Only in van mode does the sprite renderer need to be flipped, because
            // when shooting, it will face the player's position.
            if (currentX > newX)
            {
               SpriteRendererRef.flipX = true;
            }
            else
            {
               SpriteRendererRef.flipX = false;
            }
         }

         gameObject.transform.localPosition = new Vector3(newX,
                                                          newY,
                                                          gameObject.transform.localPosition.z);
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
