using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public class PlayerChip : CharChipBase
  {
    protected override void Awake()
    {
      base.Awake();

      var sprites = Resources.LoadAll<Sprite>("Textures/CharChip/Nico");
      this.SetSprite(sprites);
    }

    protected override void Start()
    {
      base.Start();
      this.spriteRenderer.sortingOrder = SpriteSortingOrder.Player;
    }
  }
}