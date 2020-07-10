using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public class PlayerChip : CharChipBase
  {
    protected override void Start()
    {
      base.Start();
      this.spriteRenderer.sortingOrder = SpriteSortingOrder.Player;
    }
  }
}