using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PotatoCat.UI
{
   public class MainMenuController : MonoBehaviour
   {
      public string GameSceneName;

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