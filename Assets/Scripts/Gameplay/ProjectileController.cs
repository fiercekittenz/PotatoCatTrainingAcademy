using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
   public int SecondsUntilDeath = 3;
   public float Velocity = 1.0f;

   public Direction FireDirection { get; set; } = Direction.Right;

   public enum Direction
   {
      Left,
      Right
   }

   private SpriteRenderer mSpriteRenderer;
   private DateTime mTimeFired;
   private bool mIsDoneFiring;

   public static float skProjectileScale = 0.2967267f;

   // Start is called before the first frame update
   protected void Start()
   {
      mSpriteRenderer = GetComponent<SpriteRenderer>();
      if (mSpriteRenderer != null && FireDirection == Direction.Right)
      {
         mSpriteRenderer.flipX = true;
      }

      mTimeFired = DateTime.Now;
   }

   // Update is called once per frame
   protected void Update()
   {
      double secondsAlive = DateTime.Now.Subtract(mTimeFired).TotalSeconds;
      if (secondsAlive >= SecondsUntilDeath)
      {
         mIsDoneFiring = true;
         Destroy(gameObject);
      }
   }

   protected void OnCollisionEnter2D(Collision2D collision)
   {
      if (collision.gameObject.tag.Equals("enemy", StringComparison.OrdinalIgnoreCase))
      {
         Debug.Log("collided!");
      }
   }

   protected void FixedUpdate()
   {
      if (!mIsDoneFiring)
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

         transform.position = new Vector3(xPosition,
                                          transform.position.y,
                                          transform.position.z);
      }
   }
}
