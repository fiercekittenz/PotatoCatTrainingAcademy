using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
   public int SecondsUntilDeath = 3;
   public float Velocity = 1.0f;

   private DateTime mTimeFired;
   private bool mIsDoneFiring;

   public static float skProjectileScale = 0.2967267f;

   // Start is called before the first frame update
   protected void Start()
   {
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

   protected void FixedUpdate()
   {
      if (!mIsDoneFiring)
      {
         transform.position = new Vector3(transform.position.x + (Velocity * Time.deltaTime),
                                          transform.position.y,
                                          transform.position.z);
      }
   }
}
