using PotatoCat.Core;
using PotatoCat.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CottonSmilesController : BaseEnemy
{
   public ProjectileUserComponent ProjectileUserComponentRef { get; private set; }
   public Animator Animator { get; private set; }

   protected override void Awake()
   {
      base.Awake();

      ProjectileUserComponentRef = GetComponent<ProjectileUserComponent>();
      Animator = GetComponent<Animator>();
   }

}
