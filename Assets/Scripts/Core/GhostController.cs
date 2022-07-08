using PotatoCat.Events;
using PotatoCat.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Core
{
   public class GhostController : BaseEnemy
   {
      public ProjectileUserComponent ProjectileUserComponentRef { get; private set; }
      public Animator Animator { get; private set; }

      private DateTime mTimeLastBooed = DateTime.Now;

      protected override void Awake()
      {
         base.Awake();

         ProjectileUserComponentRef = GetComponent<ProjectileUserComponent>();
         Animator = GetComponent<Animator>();
      }

      public override void HandleDeath(bool fromJump = false)
      {
         base.HandleDeath(fromJump);

         Collider2d.enabled = false;

         var ev = Simulation.Schedule<GhostDeath>();
         ev.Animator = Animator;
         ev.Player = PlayerComponentRef;
         ev.Ghost = gameObject;
         ev.FromJump = fromJump;
      }

      private void FixedUpdate()
      {
         if (HealthComponentRef.CurrentHealth > 0)
         { 
            double msSinceLastBooed = DateTime.Now.Subtract(mTimeLastBooed).TotalMilliseconds;
            if (msSinceLastBooed >= ProjectileUserComponentRef.MillisecondsBetweenProjectiles)
            {
               ProjectileUserComponentRef.Fire();
            }
         }
      }
   }
}