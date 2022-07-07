using PotatoCat.Core;
using PotatoCat.Events;
using PotatoCat.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSimulation : MonoBehaviour
{
   //
   // Game Object References
   //

   public GameObject PlayerReference;

   //
   // GUI References
   //

   public GameObject InGameMenu;
   public GameObject InformationBoxPanel;
   public TextMeshProUGUI PraisesText;
   public TextMeshProUGUI SocksText;
   public Cinemachine.CinemachineVirtualCamera VirtualCamera;

   //
   // Static Instance to the Game's Simulation (this)
   //

   public static GameSimulation Instance { get; private set; }

   //
   // Publicly Accessible Components/Variables
   //

   public bool InGameMenuIsOpen { get; set; } = false;
   public AudioSource AudioSource { get; private set; }

   //
   // Static Values
   //

   public static float skDefaultVolume = 0.1f;

   //
   // Private Variables
   //

   private PlayerComponent mPlayerComponentRef { get; set; }

   /// <summary>
   /// First method called when the simulation is created.
   /// </summary>
   void Awake()
   {
      Instance = this;

      AudioSource = GetComponent<AudioSource>();
      mPlayerComponentRef = PlayerReference.GetComponent<PlayerComponent>();

      // Default start the volume at 25% - think of the ears!
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

   public void AddSocks(int amount)
   {
      int currentValue = Int32.Parse(SocksText.text);
      SocksText.text = $"{currentValue + amount}";
   }

   public void AddPraises(int amount)
   {
      int currentValue = Int32.Parse(PraisesText.text);
      PraisesText.text = $"{currentValue + amount}";
   }

   public void CloseInformationPanel()
   {      
      InformationBoxPanel.SetActive(false);
      mPlayerComponentRef.ControlEnabled = true;
   }
}
