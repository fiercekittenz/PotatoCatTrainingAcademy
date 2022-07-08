using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Gameplay
{
   public class DoleWhipController : MonoBehaviour
   {
      public int HealValue = 1;
      private AudioSource mAudioSource;

      private void Awake()
      {
         mAudioSource = GetComponent<AudioSource>();
      }

      private void OnTriggerEnter2D(Collider2D collision)
      {
         var player = collision.gameObject.GetComponent<PlayerComponent>();
         if (player != null)
         {
            player.HealthComponent.Heal(HealValue);
            GameSimulation.Instance.AudioSource.PlayOneShot(mAudioSource.clip);
            Destroy(gameObject, 0.1f);
         }
      }
   }
}