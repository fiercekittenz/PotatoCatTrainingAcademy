using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PotatoCat.Gameplay
{
   public class HeartMeterComponent : MonoBehaviour
   {
      public Image[] HeartContainers;
      public Sprite FullHeartSprite;
      public Sprite EmptyHeartSprite;
      public int FullHeartHealthValue = 1;

      public void UpdateDisplay(int currentHealth)
      {
         // Determine how much health each point of health is worth based on the value of
         // each heart container at our disposal. 
         //
         // Note: Never just assume that the value is going to be the same number of available
         //       containers. This could easily be represented by a progress bar for larger
         //       health pools to allow for fine-tuned boss encounters!
         double healthPointValue = (HeartContainers.Length * FullHeartHealthValue) / HeartContainers.Length;

         // How many hearts should be fully lit up based on the player's current health?
         // Round down, because we don't have partial fills.
         double healthValueByHearts = Math.Floor(currentHealth * healthPointValue);

         // Set the heart displays.
         int heartsSet = 0;
         foreach (var heart in HeartContainers)
         {
            if (heartsSet < healthValueByHearts)
            {
               heart.sprite = FullHeartSprite;
               ++heartsSet;
            }
            else
            {
               heart.sprite = EmptyHeartSprite;
            }
         }
      }
   }
}