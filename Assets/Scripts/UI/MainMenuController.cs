using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PotatoCat.UI
{
   public class MainMenuController : MonoBehaviour
   {
      public string GameSceneName;
      public AudioSource MainMenuAudio;

      public void Awake()
      {
         if (MainMenuAudio != null)
         {
            MainMenuAudio.volume = 0.1f;
            AudioListener.volume = 0.1f;
         }
      }

      public void StartGame()
      {
         if (!string.IsNullOrEmpty(GameSceneName))
         { 
            SceneManager.LoadScene(GameSceneName);
         }
      }

      public void QuitGame()
      {
         Application.Quit();
      }
   }
}