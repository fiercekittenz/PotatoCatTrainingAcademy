using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PotatoCat.UI
{
   public class InGameMenuController : MonoBehaviour
   {
      public string MainMenuSceneName;
      public GameObject GameSimulationObject;

      public void CloseMenu()
      {
         if (GameSimulationObject != null)
         {
            GameSimulation controller = GameSimulationObject.GetComponent<GameSimulation>();
            if (controller != null)
            {
               controller.InGameMenuIsOpen = false;
            }
         }

         gameObject.SetActive(false);
      }

      public void ReturnToMainMenu()
      {
         if (!string.IsNullOrEmpty(MainMenuSceneName))
         {
            SceneManager.LoadScene(MainMenuSceneName);
         }
      }
   }
}