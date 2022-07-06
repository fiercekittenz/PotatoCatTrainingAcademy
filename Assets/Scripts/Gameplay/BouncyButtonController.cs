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
      public int BounceAmount = 7;

      [HideInInspector]
      protected SpriteRenderer mSpriteRenderer;

      [HideInInspector]
      protected Collider2D mCollider2d;

      [HideInInspector]
      protected AudioSource mAudioSource;

      private DateTime mTimeStomped;
      private Bounds mBounds => mCollider2d.bounds;
      private bool mIsDown = false;

      private void Awake()
      {
         mSpriteRenderer = GetComponent<SpriteRenderer>();
         mCollider2d = GetComponent<Collider2D>();
         mAudioSource = GetComponent<AudioSource>();
      }

      private void OnCollisionEnter2D(Collision2D collision)
      {
         PlayerComponent player = collision.gameObject.GetComponent<PlayerComponent>();
         if (player != null && player.Bounds.center.y >= mBounds.max.y)
         {
            // The player has stomped this button.
            mTimeStomped = DateTime.Now;
            mSpriteRenderer.sprite = DownStateSprite;
            mIsDown = true;

            player.Bounce(BounceAmount);

            // Play a sound!
            mAudioSource.PlayOneShot(mAudioSource.clip);
         }
      }

      private void FixedUpdate()
      {
         if (mIsDown)
         {
            double millisecondsDown = DateTime.Now.Subtract(mTimeStomped).TotalMilliseconds;
            if (millisecondsDown >= MillisecondsToStayDown)
            {
               mIsDown = false;
               mSpriteRenderer.sprite = UpStateSprite;
            }
         }
      }
   }
}
