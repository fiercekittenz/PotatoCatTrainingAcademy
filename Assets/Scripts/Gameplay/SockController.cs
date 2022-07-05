using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Gameplay
{
   public class SockController : MonoBehaviour
   {
      public int SockValue = 1;
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
            GameSimulation.Instance.AddSocks(SockValue);
            GameSimulation.Instance.AudioSource.PlayOneShot(mAudioSource.clip);
            Destroy(gameObject, 0.1f);
         }
      }
   }
}