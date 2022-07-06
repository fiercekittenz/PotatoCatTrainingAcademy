using PotatoCat.Core;
using PotatoCat.Events;
using PotatoCat.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FartController : MonoBehaviour
{
   // Editor Properties
   public AudioClip DeathSound;
   public bool MovesBetweenPoints;
   public Vector2 StartPosition;
   public Vector2 EndPosition;
   public float MaxPathingSpeed = 7;

   [HideInInspector]
   private Animator mAnimator;
   public Collider2D Collider2d;
   public Bounds Bounds => Collider2d.bounds;

   private Mover mMover;
   private bool mMovingTowardPointB = true;

   private void Awake()
   {
      mAnimator = GetComponent<Animator>();
      Collider2d = GetComponent<Collider2D>();
   }

   public Mover CreateMover(float speed = 1) => new Mover(gameObject.transform, StartPosition, EndPosition, speed);

   void Reset()
   {
      StartPosition = Vector3.left;
      EndPosition = Vector3.right;
   }

   private void OnCollisionEnter2D(Collision2D collision)
   {
      if (collision.gameObject.tag.Equals("projectiledamage", StringComparison.OrdinalIgnoreCase))
      {
         var ev = Simulation.Schedule<FartDeath>();
         ev.Animator = mAnimator;
         ev.Fart = gameObject;
      }
      else if (collision.gameObject.tag.Equals("player", StringComparison.OrdinalIgnoreCase))
      {
         var playerComponent = collision.gameObject.GetComponent<PlayerComponent>();
         if (playerComponent != null)
         {
            bool willHurtEnemy = playerComponent.Bounds.center.y >= Bounds.max.y;
            if (willHurtEnemy)
            {
               var ev = Simulation.Schedule<FartDeath>();
               ev.Player = playerComponent;
               ev.Animator = mAnimator;
               ev.Fart = gameObject;
            }
            else
            {
               Simulation.Schedule<PlayerDeath>().Player = playerComponent;
            }
         }
      }
   }

   private void FixedUpdate()
   {
      if (MovesBetweenPoints)
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
