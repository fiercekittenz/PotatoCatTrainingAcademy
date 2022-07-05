using PotatoCat.Core;
using PotatoCat.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FartController : MonoBehaviour
{
   private Animator mAnimator;

   private void Awake()
   {
      mAnimator = GetComponent<Animator>();
   }

   private void OnCollisionEnter2D(Collision2D collision)
   {
      if (collision.gameObject.tag.Equals("projectiledamage", StringComparison.OrdinalIgnoreCase))
      {
         var ev = Simulation.Schedule<FartDeath>();
         ev.Animator = mAnimator;
         ev.Fart = gameObject;
      }
   }
}
