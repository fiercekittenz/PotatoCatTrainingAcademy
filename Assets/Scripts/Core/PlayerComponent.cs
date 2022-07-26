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
   public ProjectileUserComponent ProjectileUserComponent { get; private set; }
   public Animator Animator { get; private set; }
   public Bounds Bounds => Collider2d.bounds;

   public bool ControlEnabled
   {
      get { return mControlEnabled; }
      set
      {
         mControlEnabled = value;
      }
   }

   //
   // Private Properties
   //

   private bool mControlEnabled = true;
   private bool mStopJump;
   private bool mJump;
   private Vector2 mMove;
   private SpriteRenderer mSpriteRenderer;

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
      ProjectileUserComponent = GetComponent<ProjectileUserComponent>();
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

         if (Input.GetKey(KeyCode.E))
         {
            ProjectileUserComponent.Fire(true);
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
         ProjectileUserComponent.FireDirection = ProjectileUserComponent.FireDirectionValues.Left;
      }
      else if (mMove.x < -0.01f)
      { 
         mSpriteRenderer.flipX = true;
         ProjectileUserComponent.FireDirection = ProjectileUserComponent.FireDirectionValues.Right;
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
