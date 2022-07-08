using PotatoCat.Core;
using PotatoCat.Events;
using PotatoCat.Gameplay;
using PotatoCat.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FartController : BaseEnemy
{
   //
   // Editor Properties
   //

   public bool MovesBetweenPoints;
   public Vector2 StartPosition;
   public Vector2 EndPosition;
   public float MaxPathingSpeed = 7;

   //
   // Private Properties
   //

   private Mover mMover;
   private Animator mAnimator { get; set; }

   protected override void Awake()
   {
      base.Awake();

      mAnimator = GetComponent<Animator>();
   }

   public Mover CreateMover(float speed = 1) => new Mover(gameObject.transform, StartPosition, EndPosition, speed);

   void Reset()
   {
      StartPosition = Vector3.left;
      EndPosition = Vector3.right;
   }

   public override void HandleDeath(bool fromJump = false)
   {
      base.HandleDeath(fromJump);

      Collider2d.enabled = false;

      var ev = Simulation.Schedule<FartDeath>();
      ev.Animator = mAnimator;
      ev.Player = PlayerComponentRef;
      ev.Fart = gameObject;
      ev.FromJump = fromJump;
   }

   private void FixedUpdate()
   {
      if (MovesBetweenPoints && HealthComponentRef.CurrentHealth > 0)
      {
         if (mMover == null)
         {
            mMover = CreateMover(MaxPathingSpeed * 0.5f);
         }

         gameObject.transform.localPosition = new Vector3(mMover.Position.x, 
                                                          mMover.Position.y, 
                                                          gameObject.transform.localPosition.z);
      }
   }
}
