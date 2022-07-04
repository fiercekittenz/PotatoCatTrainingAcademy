using UnityEngine;

public class WaterDetector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<Rigidbody2D>() != null)
        {
            PlayerComponent player = collider.GetComponent<PlayerComponent>();

            if (player != null)
            {
                transform.parent.GetComponent<SpriteShapeWater>().Splash(transform.position.x, player.velocity.y * collider.GetComponent<Rigidbody2D>().mass / 80f);
            }
            else
            {
                transform.parent.GetComponent<SpriteShapeWater>().Splash(transform.position.x, collider.GetComponent<Rigidbody2D>().velocity.y * collider.GetComponent<Rigidbody2D>().mass / 200f);
            }
        }
    }
}