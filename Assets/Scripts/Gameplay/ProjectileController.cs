using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Gameplay
{
   public class ProjectileController : MonoBehaviour
   {
      public int SecondsUntilDeath = 3;
      public float Velocity = 1.0f;
      public float SpinVelocity = -500;
      public AudioClip CollisionAudio;
      public GameObject BoomEffectPrefab;

      public Direction FireDirection { get; set; } = Direction.Right;

      public enum Direction
      {
         Left,
         Right
      }

      private SpriteRenderer mSpriteRenderer;

      public static float skProjectileScale = 0.2967267f;

      // Start is called before the first frame update
      protected void Start()
      {
         mSpriteRenderer = GetComponent<SpriteRenderer>();
         if (mSpriteRenderer != null && FireDirection == Direction.Right)
         {
            mSpriteRenderer.flipX = true;
         }
      }

      protected void OnCollisionEnter2D(Collision2D collision)
      {
         bool isEnemy = collision.gameObject.tag.Equals("enemy", StringComparison.OrdinalIgnoreCase);
         bool isCollisionTerrain = collision.gameObject.tag.Equals("levelcollision", StringComparison.OrdinalIgnoreCase);

         if (isEnemy || isCollisionTerrain)
         {
            HandleCollisionEffects();
         }
      }

      protected void FixedUpdate()
      {
         float xPosition = transform.position.x;
         if (FireDirection == Direction.Left)
         {
            xPosition += (Velocity * Time.deltaTime);
         }
         else
         {
            xPosition -= (Velocity * Time.deltaTime);
         }

         transform.Rotate(Vector3.forward * SpinVelocity * Time.deltaTime);
         transform.position = new Vector3(xPosition,
                                          transform.position.y,
                                          transform.position.z);
      }

      private void HandleCollisionEffects()
      {
         GameObject boom = Instantiate(BoomEffectPrefab);
         boom.transform.position = gameObject.transform.position;
         
         SpriteRenderer spriteRenderer = boom.GetComponent<SpriteRenderer>();
         if (spriteRenderer != null)
         {
            spriteRenderer.sortingLayerName = "Foreground";
            spriteRenderer.sortingOrder = 999;
         }

         GameSimulation.Instance.AudioSource.PlayOneShot(CollisionAudio);
         Destroy(gameObject);
      }
   }
}