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
      public PlayerComponent PlayerComponentRef;
      public bool IsAutomaticFireOn = false;
      public bool ShootAtPlayer = false;
      public GameObject ProjectilePrefab;
      public AudioClip Shooting;
      public FireDirectionValues FireDirection = FireDirectionValues.Right;
      public int MillisecondsBetweenProjectiles = 500;
      public float ProjectileDistanceFromEntity = 1.0f;
      public float ProjectileVelocity = 1.0f;
      public float DistanceToTravel = 10.0f;

      private AudioSource AudioSourceRef { get; set; }
      private SpriteRenderer SpriteRendererRef { get; set; }
      private DateTime mLastTimeProjectileFired;

      public enum FireDirectionValues
      {
         Left,
         Right
      }

      void Awake()
      {
         AudioSourceRef = gameObject.GetComponent<AudioSource>();
         SpriteRendererRef = gameObject.GetComponent<SpriteRenderer>();
      }

      public void Fire(bool forced = false)
      {
         if (forced || IsAutomaticFireOn)
         {
            double msSinceLastProjectile = DateTime.Now.Subtract(mLastTimeProjectileFired).TotalMilliseconds;
            if (msSinceLastProjectile >= MillisecondsBetweenProjectiles)
            {
               // Play the shooting sound.
               if (AudioSourceRef != null)
               {
                  AudioSourceRef.PlayOneShot(Shooting);
               }

               // Flag the time the projectile was fired and create it.
               var projectile = Instantiate(ProjectilePrefab);

               // Set the sorting order so it flies topmost to all things except the entity.
               SpriteRenderer projectileRenderer = projectile.GetComponent<SpriteRenderer>();
               projectileRenderer.sortingLayerName = "Foreground";
               projectileRenderer.sortingOrder = 999;

               // Set the position for its starting point based on the entity firing.
               // Note: The manipulation of X is backwards from what you'd think. Subtraction to move it left,
               //       addition to move it right. This is beyond bizarre, but I'm just letting it go.
               float xPosition = transform.position.x - (SpriteRendererRef.bounds.size.x / 2) - ProjectileDistanceFromEntity;
               float yPosition = SpriteRendererRef.bounds.center.y;
               float zPosition = transform.position.z;

               ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
               Debug.Assert(projectileController != null);

               // Dynamically set the firing position if the intended target is the player as this goes against the
               // scripted AI version of just shooting in a configured direction.
               if (ShootAtPlayer && PlayerComponentRef != null)
               {
                  if (PlayerComponentRef.Bounds.center.x < transform.position.x)
                  { 
                     projectileController.FireDirection = ProjectileController.Direction.Right;
                     FireDirection = FireDirectionValues.Right;
                  }
                  else
                  {
                     projectileController.FireDirection = ProjectileController.Direction.Left;
                     FireDirection = FireDirectionValues.Left;
                  }
               }

               // Adjust the starting X position if firing to the left.
               if (FireDirection == FireDirectionValues.Left)
               {
                  xPosition = transform.position.x + (SpriteRendererRef.bounds.size.x / 2) + ProjectileDistanceFromEntity;

                  // The entity should control if it is facing left or right as it could be set via user input (player)
                  // or AI (game-controlled entity).
                  projectileController.FireDirection = ProjectileController.Direction.Left;
               }

               // Place the projectile in its starting position and set the summoning game object reference.
               Vector3 placement = new Vector3(xPosition, yPosition, zPosition);
               projectile.transform.position = placement;
               projectileController.SummonerRef = gameObject;

               // Determine the target location. This can be the player, or it will default to a position N units away
               // on the X axis.
               Vector3 targetLocation;
               if (ShootAtPlayer && PlayerComponentRef != null)
               {
                  targetLocation = new Vector3(PlayerComponentRef.Bounds.center.x, PlayerComponentRef.Bounds.center.y, PlayerComponentRef.Bounds.center.z);
               }
               else
               {
                  if (FireDirection == FireDirectionValues.Left)
                  {
                     targetLocation = new Vector3(xPosition + DistanceToTravel, yPosition, zPosition);
                  }
                  else
                  {
                     targetLocation = new Vector3(xPosition - DistanceToTravel, yPosition, zPosition);
                  }
               }

               // Get the difference between the target location and the starting location of the projectile.
               Vector3 difference = targetLocation - projectile.transform.position;

               // Determine the rotation based on the difference.
               float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

               // Get the distance and determine the direction, then apply the rotation to the
               // projectile so that it faces where it's firing.
               float distance = difference.magnitude;
               Vector2 direction = difference / distance;
               direction.Normalize();
               projectile.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

               // Set the velocity of the rigid body component based on the direction and configured speed.
               // The physics system will handle the actual movement updates for render.
               projectile.GetComponent<Rigidbody2D>().velocity = direction * ProjectileVelocity;

               // Let it move AFTER setting all of the values!
               mLastTimeProjectileFired = DateTime.Now;
               projectileController.ReadyToMove = true;
            }
         }
      }

      private void FixedUpdate()
      {
         if (IsAutomaticFireOn)
         {
            double msSinceLastFired = DateTime.Now.Subtract(mLastTimeProjectileFired).TotalMilliseconds;
            if (msSinceLastFired >= MillisecondsBetweenProjectiles)
            {
               Fire();
            }
         }
      }
   }
}