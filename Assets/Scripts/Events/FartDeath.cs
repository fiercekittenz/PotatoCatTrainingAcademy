using PotatoCat.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PotatoCat.Events
{
   public class FartDeath : Simulation.Event<FartDeath>
   {
      public Animator Animator;
      public GameObject Fart;

      public override void Execute()
      {
         Animator.SetBool("IsDying", true);
         Object.Destroy(Fart, 0.833f);
      }
   }
}
