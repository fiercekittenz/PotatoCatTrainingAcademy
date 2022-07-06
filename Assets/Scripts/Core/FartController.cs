using PotatoCat.Core;
using PotatoCat.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FartController : MonoBehaviour
{
   public AudioClip DeathSound;

   [HideInInspector]
   private Animator mAnimator;

   [HideInInspector]
   public Collider2D Collider2d;
   public Bounds Bounds => Collider2d.bounds;

   private void Awake()
   {
      mAnimator = GetComponent<Animator>();
      Collider2d = GetComponent<Collider2D>();
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
         }
      }
   }
}
