using PotatoCat.Core;
using PotatoCat.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : KinematicObject
{
   //
   // Editor Exposed Properties
   //

   public GameObject SpawnLocation;
   public GameObject ProjectilePrefab;
   public AudioClip Shooting;
   public int MillisecondsBetweenProjectiles = 500;
   public float MaxSpeed = 7;
   public float JumpTakeOffSpeed = 7;

   //
   // Public Properties (Not in the editor)
   //

   [HideInInspector]
   public JumpState CurrentJumpState = JumpState.Grounded;
   public Collider2D Collider2d { get; private set; }
   public AudioSource AudioSource { get; private set; }
   public HealthComponent HealthComponent { get; private set; }
   public Animator Animator { get; private set; }
   public bool ControlEnabled { get; set; } = true;
   public Bounds Bounds => Collider2d.bounds;

   //
   // Private Properties
   //

   private bool mStopJump;
   private bool mJump;
   private Vector2 mMove;
   private SpriteRenderer mSpriteRenderer;
   private DateTime mLastTimeProjectileFired;

   // 
   // Static Variables
   //

   public static float skJumpModifier = 1.5f;
   public static float skJumpDeceleration = 0.5f;

   /// <summary>
   /// Instantiation of the player component and referenced components.
   /// </summary>
   private void Awake()
   {
      mSpriteRenderer = GetComponent<SpriteRenderer>();
      Animator = GetComponent<Animator>();
      AudioSource = GetComponent<AudioSource>();
      Collider2d = GetComponent<Collider2D>();
      HealthComponent = GetComponent<HealthComponent>();
   }

   protected override void Update()
   {
      if (ControlEnabled)
      {
         //
         // Handle Motion
         //

         mMove.x = Input.GetAxis("Horizontal");
         if (CurrentJumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
         {
            CurrentJumpState = JumpState.PrepareToJump;
         }
         else if (Input.GetButtonUp("Jump"))
         {
            mStopJump = true;
         }

         //
         // Handle Projectile
         //

         double msSinceLastProjectile = DateTime.Now.Subtract(mLastTimeProjectileFired).TotalMilliseconds;
         if (Input.GetKey(KeyCode.E) && msSinceLastProjectile >= MillisecondsBetweenProjectiles)
         {
            AudioSource.PlayOneShot(Shooting);

            mLastTimeProjectileFired = DateTime.Now;
            var projectile = Instantiate(ProjectilePrefab);
            Vector3 placement = new Vector3(transform.position.x + 0.2f, transform.position.y + 0.1f, transform.position.z);
            projectile.transform.position = placement;

            if (!mSpriteRenderer.flipX)
            {
               ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
               if (projectileController != null)
               { 
                  projectileController.FireDirection = ProjectileController.Direction.Left;
               }
            }
         }
      }
      else
      {
         Animator.SetBool("Walking", false);
         mMove.x = 0;
      }

      UpdateJumpState();
      base.Update();
   }

   void UpdateJumpState()
   {
      mJump = false;
      switch (CurrentJumpState)
      {
         case JumpState.PrepareToJump:
            CurrentJumpState = JumpState.Jumping;
            mJump = true;
            mStopJump = false;
            break;
         case JumpState.Jumping:
            if (!IsGrounded)
            {
               CurrentJumpState = JumpState.InFlight;
            }
            break;
         case JumpState.InFlight:
            if (IsGrounded)
            {
               CurrentJumpState = JumpState.Landed;
            }
            break;
         case JumpState.Landed:
            CurrentJumpState = JumpState.Grounded;
            break;
      }
   }

   protected override void ComputeVelocity()
   {
      if (mJump && IsGrounded)
      {
         velocity.y = JumpTakeOffSpeed * skJumpModifier;
         mJump = false;
      }
      else if (mStopJump)
      {
         mStopJump = false;
         if (velocity.y > 0)
         {
            velocity.y = velocity.y * skJumpDeceleration;
         }
      }

      if (mMove.x > 0.01f)
      {
         mSpriteRenderer.flipX = false;
      }
      else if (mMove.x < -0.01f)
      { 
         mSpriteRenderer.flipX = true;
      }

      Animator.SetBool("grounded", IsGrounded);
      Animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / MaxSpeed);

      targetVelocity = mMove * MaxSpeed;

      if (targetVelocity.x == 0.0f)
      {
         Animator.SetBool("Walking", false);
      }
      else
      {
         Animator.SetBool("Walking", true);
      }
   }

   public enum JumpState
   {
      Grounded,
      PrepareToJump,
      Jumping,
      InFlight,
      Landed
   }
}
