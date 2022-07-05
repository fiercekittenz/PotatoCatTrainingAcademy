using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicObject : MonoBehaviour
{
   /// <summary>
   /// The minimum normal (dot product) considered suitable for the entity sit on.
   /// </summary>
   public float minGroundNormalY = .65f;

   /// <summary>
   /// A custom gravity coefficient applied to this entity.
   /// </summary>
   public float gravityModifier = 1f;

   /// <summary>
   /// The current velocity of the entity.
   /// </summary>
   public Vector2 velocity;

   /// <summary>
   /// Is the entity currently sitting on a surface?
   /// </summary>
   /// <value></value>
   public bool IsGrounded { get; private set; }

   protected Vector2 targetVelocity;
   protected Vector2 groundNormal;
   protected Rigidbody2D body;
   protected ContactFilter2D contactFilter;
   protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

   protected const float minMoveDistance = 0.001f;
   protected const float shellRadius = 0.01f;


   /// <summary>
   /// Bounce the object's vertical velocity.
   /// </summary>
   /// <param name="value"></param>
   public void Bounce(float value)
   {
      velocity.y = value;
   }

   /// <summary>
   /// Bounce the objects velocity in a direction.
   /// </summary>
   /// <param name="dir"></param>
   public void Bounce(Vector2 dir)
   {
      velocity.y = dir.y;
      velocity.x = dir.x;
   }

   /// <summary>
   /// Teleport to some position.
   /// </summary>
   /// <param name="position"></param>
   public void Teleport(Vector3 position)
   {
      body.position = position;
      velocity *= 0;
      body.velocity *= 0;
   }

   protected virtual void OnEnable()
   {
      body = GetComponent<Rigidbody2D>();
      body.isKinematic = true;
   }

   protected virtual void OnDisable()
   {
      body.isKinematic = false;
   }

   protected virtual void Start()
   {
      contactFilter.useTriggers = false;
      contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
      contactFilter.useLayerMask = true;
   }

   protected virtual void Update()
   {
      targetVelocity = Vector2.zero;
      ComputeVelocity();
   }

   protected virtual void ComputeVelocity()
   {

   }

   protected virtual void FixedUpdate()
   {
      //if already falling, fall faster than the jump speed, otherwise use normal gravity.
      if (velocity.y < 0)
         velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
      else
         velocity += Physics2D.gravity * Time.deltaTime;

      velocity.x = targetVelocity.x;

      IsGrounded = false;

      var deltaPosition = velocity * Time.deltaTime;

      var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

      var move = moveAlongGround * deltaPosition.x;

      PerformMovement(move, deltaPosition.y, false);

      move = Vector2.up * deltaPosition.y;

      PerformMovement(move, deltaPosition.y, true);

   }

   void PerformMovement(Vector2 move, float yDelta, bool yMovement)
   {
      var distance = move.magnitude;

      if (distance > minMoveDistance)
      {
         // Check if we hit anything in current direction of travel
         var count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
         for (var i = 0; i < count; i++)
         {
            if (hitBuffer[i].collider != null && hitBuffer[i].collider.usedByEffector && yMovement && yDelta > 0)
            {
               // We're colliding with an object that has a passthrough effector. Ignore.
               continue;
            }

            var currentNormal = hitBuffer[i].normal;

            // Is this surface flat enough to land on?
            if (currentNormal.y > minGroundNormalY)
            {
               IsGrounded = true;

               // If moving up, change the groundNormal to new surface normal.
               if (yMovement)
               {
                  groundNormal = currentNormal;
                  currentNormal.x = 0;
               }
            }
            if (IsGrounded)
            {
               // How much of our velocity aligns with surface normal?
               var projection = Vector2.Dot(velocity, currentNormal);
               if (projection < 0)
               {
                  // Slower velocity if moving against the normal (up a hill).
                  velocity = velocity - projection * currentNormal;
               }
            }
            else
            {
               // We are airborne, but hit something, so cancel vertical up and horizontal velocity.
               velocity.x *= 0;
               velocity.y = Mathf.Min(velocity.y, 0);
            }

            // Remove shellDistance from actual move distance.
            var modifiedDistance = hitBuffer[i].distance - shellRadius;
            distance = modifiedDistance < distance ? modifiedDistance : distance;
         }
      }
      body.position = body.position + move.normalized * distance;
   }
}
