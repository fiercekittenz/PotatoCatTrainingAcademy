using PotatoCat.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Gameplay
{
   public class ProjectileController : MonoBehaviour
   {
      public int MillisecondsUntilDeath = 3000;
      public float Velocity = 1.0f;
      public float SpinVelocity = -500;
      public int DamageDealt = 1;
      public AudioClip CollisionAudio;
      public GameObject BoomEffectPrefab;
      public GameObject SummonerRef { get; set; } = null;
      public Vector2 TargetLocation { get; set; } = Vector2.zero;
      public bool ReadyToMove { get; set; } = false;

      private Direction mFireDirection = Direction.Right;
      public Direction FireDirection
      {
         get { return mFireDirection; }
         set
         {
            mFireDirection = value;
            if (mSpriteRenderer != null && FireDirection == Direction.Right)
            {
               mSpriteRenderer.flipX = true;
            }
         }
      }

      public enum Direction
      {
         Left,
         Right
      }

      private SpriteRenderer mSpriteRenderer;
      private DateTime mFiredTime;

      protected void Awake()
      {
         mFiredTime = DateTime.Now;
         mSpriteRenderer = GetComponent<SpriteRenderer>();
      }

      protected void OnCollisionEnter2D(Collision2D collision)
      {
         bool isPlayer = collision.gameObject.tag.Equals("player", StringComparison.OrdinalIgnoreCase);
         bool isEnemy = collision.gameObject.tag.Equals("enemy", StringComparison.OrdinalIgnoreCase);
         bool isCollisionTerrain = collision.gameObject.tag.Equals("levelcollision", StringComparison.OrdinalIgnoreCase);

         if (collision.gameObject == SummonerRef || 
             (isEnemy && !SummonerRef.tag.Equals("player", StringComparison.OrdinalIgnoreCase)))
         {
            // Can't hit yourself or other enemies!
            return;
         }

         if (isPlayer || isEnemy || isCollisionTerrain)
         {
            HandleCollisionEffects();
         }

         if (isEnemy)
         {
            BaseEnemy enemyComponent = collision.gameObject.GetComponent<BaseEnemy>();
            if (enemyComponent != null)
            {
               enemyComponent.HealthComponentRef.TakeDamage(DamageDealt);
            }
         }
         else if (isPlayer)
         {
            PlayerComponent player = collision.gameObject.GetComponent<PlayerComponent>();
            player.HealthComponent.TakeDamage(DamageDealt);
         }
      }

      protected void FixedUpdate()
      {
         if (!ReadyToMove)
         {
            return;
         }

         // Handle a timed self-destruct.
         double msSinceFired = DateTime.Now.Subtract(mFiredTime).TotalMilliseconds;
         if (msSinceFired >= MillisecondsUntilDeath)
         {
            Destroy(gameObject);
         }
         else
         {
            if (SpinVelocity != 0)
            {
               transform.Rotate(Vector3.forward * SpinVelocity * Time.deltaTime);
            }
         }
      }

      private void HandleCollisionEffects()
      {
         if (BoomEffectPrefab != null)
         {
            GameObject boom = Instantiate(BoomEffectPrefab);
            boom.transform.position = gameObject.transform.position;

            SpriteRenderer spriteRenderer = boom.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
               spriteRenderer.sortingLayerName = "Foreground";
               spriteRenderer.sortingOrder = 999;
            }
         }

         if (CollisionAudio != null)
         {
            GameSimulation.Instance.AudioSource.PlayOneShot(CollisionAudio);
         }

         Destroy(gameObject);
      }
   }
}