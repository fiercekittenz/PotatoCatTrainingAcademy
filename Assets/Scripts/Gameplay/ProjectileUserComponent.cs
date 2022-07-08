using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Gameplay
{
   /// <summary>
   /// Component for an entity that can fire projectiles. Handles
   /// all management of the firing mechanics and generation of
   /// the projectile prefab.
   /// </summary>
   public class ProjectileUserComponent : MonoBehaviour
   {
      public GameObject ProjectilePrefab;
      public AudioClip Shooting;
      public FireDirectionValues FireDirection = FireDirectionValues.Right;
      public int MillisecondsBetweenProjectiles = 500;
      public float ProjectileDistanceFromEntity = 1.0f;

      private AudioSource AudioSourceRef { get; set; }
      private SpriteRenderer SpriteRendererRef { get; set; }
      private DateTime mLastTimeProjectileFired;
      private RectTransform mRectTransform;

      public enum FireDirectionValues
      {
         Left,
         Right
      }

      void Awake()
      {
         AudioSourceRef = gameObject.GetComponent<AudioSource>();
         SpriteRendererRef = gameObject.GetComponent<SpriteRenderer>();

         mRectTransform = gameObject.GetComponent<RectTransform>();
         Debug.Assert(mRectTransform != null);
      }

      public void Fire()
      {
         //
         // Handle Projectile
         //

         double msSinceLastProjectile = DateTime.Now.Subtract(mLastTimeProjectileFired).TotalMilliseconds;
         if (msSinceLastProjectile >= MillisecondsBetweenProjectiles)
         {
            if (AudioSourceRef != null)
            {
               AudioSourceRef.PlayOneShot(Shooting);
            }

            // Flag the time the projectile was fired and create it.
            mLastTimeProjectileFired = DateTime.Now;
            var projectile = Instantiate(ProjectilePrefab);

            // Set the sorting order so it flies topmost to all things except the entity.
            SpriteRenderer projectileRenderer = projectile.GetComponent<SpriteRenderer>();
            projectileRenderer.sortingLayerName = "Foreground";
            projectileRenderer.sortingOrder = 999;

            // Set the position for its starting point based on the entity firing.
            float xPosition = 0.0f;
            float yPosition = 0.0f;
            float zPosition = transform.position.z;

            if (SpriteRendererRef != null)
            {
               // Note: The manipulation of X is backwards from what you'd think. Subtraction to move it left,
               //       addition to move it right. This is beyond bizarre, but I'm just letting it go.

               yPosition = SpriteRendererRef.bounds.center.y;
               xPosition = transform.position.x - (SpriteRendererRef.bounds.size.x / 2) - ProjectileDistanceFromEntity;

               if (FireDirection == FireDirectionValues.Left)
               {
                  xPosition = transform.position.x + (SpriteRendererRef.bounds.size.x / 2) + ProjectileDistanceFromEntity;

                  ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
                  if (projectileController != null)
                  {
                     // The entity should control if it is facing left or right as it could be set via user input (player)
                     // or AI (game-controlled entity).
                     projectileController.FireDirection = ProjectileController.Direction.Left;
                  }
               }
            }

            Vector3 placement = new Vector3(xPosition, yPosition, zPosition);
            projectile.transform.position = placement;
         }
      }
   }
}