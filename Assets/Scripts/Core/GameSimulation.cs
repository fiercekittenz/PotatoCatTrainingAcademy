using PotatoCat.Core;
using PotatoCat.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSimulation : MonoBehaviour
{
   public Cinemachine.CinemachineVirtualCamera VirtualCamera;

   public GameObject InGameMenu;

   public static GameSimulation Instance { get; private set; }

   public static float skDefaultVolume = 0.25f;

   public bool InGameMenuIsOpen { get; set; } = false;

   // Start is called before the first frame update
   void Start()
   {
      Instance = this;

      // Default start the volume at 50% - think of the ears!
      float currentVolume = PlayerPrefs.GetFloat("Volume");
      if (currentVolume == 0)
      {
         PlayerPrefs.SetFloat("Volume", skDefaultVolume);
         PlayerPrefs.Save();
      }

      AudioListener.volume = PlayerPrefs.GetFloat("Volume");
   }

   void OnEnable()
   {
      Instance = this;
   }

   void OnDisable()
   {
      if (Instance == this) Instance = null;
   }

   private void Update()
   {
      if (Instance == this) Simulation.Tick();

      if (Input.GetKey(KeyCode.Escape) && InGameMenu != null && !InGameMenuIsOpen)
      {
         // If we use this, we have to move all game objects in the scene to a
         // single parent object with the DontDestroy component.
         //SceneManager.LoadScene("MainMenuScene");

         InGameMenu.SetActive(true);
         InGameMenuIsOpen = true;
      }
   }
}
