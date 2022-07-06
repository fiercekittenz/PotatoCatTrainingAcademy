using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Gameplay
{
   public class BouncyButtonController : MonoBehaviour
   {
      public Sprite UpStateSprite;
      public Sprite DownStateSprite;
      public int MillisecondsToStayDown = 500;

      [HideInInspector]
      protected SpriteRenderer mSpriteRenderer;
      protected Collider2D mCollider2d;

      private DateTime mTimeStomped;
      private Bounds mBounds => mCollider2d.bounds;

      private void Awake()
      {
         mSpriteRenderer = GetComponent<SpriteRenderer>();
         mCollider2d = GetComponent<Collider2D>();
      }

      private void OnTriggerEnter2D(Collider2D collision)
      {
         PlayerComponent player = collision.gameObject.GetComponent<PlayerComponent>();
         if (player != null && player.Bounds.center.y >= mBounds.max.y)
         {
            // The player has stomped this button.
         }
      }

      private void FixedUpdate()
      {

      }
   }
}
