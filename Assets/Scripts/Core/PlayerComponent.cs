using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : KinematicObject
{
   #region Public Editor Properties

   public GameObject SpawnLocation;
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

   bool jump;
   Vector2 move;
   SpriteRenderer spriteRenderer;
   internal Animator animator;

   public static float skJumpModifier = 1.5f;
   public static float skJumpDeceleration = 0.5f;

   public Bounds Bounds => Collider2d.bounds;

   #endregion

   private void Awake()
   {
      spriteRenderer = GetComponent<SpriteRenderer>();
      animator = GetComponent<Animator>();
      AudioSource = GetComponent<AudioSource>();
      Collider2d = GetComponent<Collider2D>();
   }

   protected override void Update()
   {
      if (ControlEnabled)
      {
         move.x = Input.GetAxis("Horizontal");
         if (CurrentJumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
         {
            CurrentJumpState = JumpState.PrepareToJump;
         }
         else if (Input.GetButtonUp("Jump"))
         {
            StopJump = true;
            //Schedule<PlayerStopJump>().player = this;
         }
      }
      else
      {
         move.x = 0;
      }

      UpdateJumpState();
      base.Update();
   }

   void UpdateJumpState()
   {
      jump = false;
      switch (CurrentJumpState)
      {
         case JumpState.PrepareToJump:
            CurrentJumpState = JumpState.Jumping;
            jump = true;
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
      if (jump && IsGrounded)
      {
         velocity.y = JumpTakeOffSpeed * skJumpModifier;
         jump = false;
      }
      else if (StopJump)
      {
         StopJump = false;
         if (velocity.y > 0)
         {
            velocity.y = velocity.y * skJumpDeceleration;
         }
      }

      if (move.x > 0.01f)
      {
         spriteRenderer.flipX = false;
      }
      else if (move.x < -0.01f)
      { 
         spriteRenderer.flipX = true;
      }

      animator.SetBool("grounded", IsGrounded);
      animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / MaxSpeed);

      targetVelocity = move * MaxSpeed;
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
