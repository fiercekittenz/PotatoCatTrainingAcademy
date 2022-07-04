using PotatoCat.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : KinematicObject
{
   #region Public Editor Properties

   public GameObject SpawnLocation;
   public GameObject ProjectilePrefab;
   public AnimationClip IdleAnimation;
   public AnimationClip WalkingAnimation;
   public AudioClip Damage;
   public AudioClip Death;

   public float MaxSpeed = 7;
   public float JumpTakeOffSpeed = 7;

   public JumpState CurrentJumpState = JumpState.Grounded;
   private bool StopJump;
   public Collider2D Collider2d;
   public AudioSource AudioSource;
   public bool ControlEnabled = true;

   #endregion

   #region Internal Properties

   bool mJump;
   Vector2 mMove;
   SpriteRenderer mSpriteRenderer;
   internal Animator mAnimator;

   private DateTime mLastTimeProjectileFired;
   private static int skSecondsBetweenFiring = 1;

   public static float skJumpModifier = 1.5f;
   public static float skJumpDeceleration = 0.5f;

   public Bounds Bounds => Collider2d.bounds;

   #endregion

   private void Awake()
   {
      mSpriteRenderer = GetComponent<SpriteRenderer>();
      mAnimator = GetComponent<Animator>();
      AudioSource = GetComponent<AudioSource>();
      Collider2d = GetComponent<Collider2D>();
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
            StopJump = true;
         }

         mAnimator.SetFloat("Horizontal", mMove.x);

         //
         // Handle Projectile
         //

         double secondsSinceLastProjectile = DateTime.Now.Subtract(mLastTimeProjectileFired).TotalSeconds;
         if (Input.GetKey(KeyCode.E) && secondsSinceLastProjectile >= skSecondsBetweenFiring)
         {
            mLastTimeProjectileFired = DateTime.Now;
            var projectile = Instantiate(ProjectilePrefab);
            Vector3 placement = new Vector3(transform.position.x + 0.2f, transform.position.y, transform.position.z);
            projectile.transform.position = placement;
         }
      }
      else
      {
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
            StopJump = false;
            break;
         case JumpState.Jumping:
            if (!IsGrounded)
            {
               //Schedule<PlayerJumped>().player = this;
               CurrentJumpState = JumpState.InFlight;
            }
            break;
         case JumpState.InFlight:
            if (IsGrounded)
            {
               //Schedule<PlayerLanded>().player = this;
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
      else if (StopJump)
      {
         StopJump = false;
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

      mAnimator.SetBool("grounded", IsGrounded);
      mAnimator.SetFloat("velocityX", Mathf.Abs(velocity.x) / MaxSpeed);

      targetVelocity = mMove * MaxSpeed;
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
