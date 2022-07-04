using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Core
{
   public class DontDestroy : MonoBehaviour
   {
      [HideInInspector]
      public string ObjectId;

      private void Awake()
      {
         ObjectId = $"{name}{transform.position.ToString()}{transform.eulerAngles.ToString()}";
      }

      void Start()
      {
         foreach(var obj in Object.FindObjectsOfType<DontDestroy>())
         {
            if (obj != this && obj.ObjectId == ObjectId)
            {
               Destroy(gameObject);
            }
         }

         DontDestroyOnLoad(gameObject);
      }
   }
}