using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PotatoCat.Core
{
   public class InfoBoxController : MonoBehaviour
   {
      public string Information;
      public GameObject InformationCanvas;
      public TextMeshProUGUI InformationTextField;
      public AudioClip DisplaySound;

      private AudioSource mAudioSource;

      private void Awake()
      {
         mAudioSource = GetComponent<AudioSource>();
      }

      private void OnTriggerEnter2D(Collider2D collision)
      {
         PlayerComponent player = collision.gameObject.GetComponent<PlayerComponent>();
         if (player != null)
         {
            mAudioSource.PlayOneShot(DisplaySound);
            player.ControlEnabled = false;
            InformationTextField.text = Information;
            InformationCanvas.SetActive(true);
            Destroy(gameObject);
         }
      }
   }
}
