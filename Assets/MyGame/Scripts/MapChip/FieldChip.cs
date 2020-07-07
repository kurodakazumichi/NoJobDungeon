using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame {

  /// <summary>
  /// 床や壁などのフィールド系のチップ
  /// </summary>
  public class FieldChip : MapChipBase
  {
    override protected void Awake()
    {
      base.Awake();
    }

    protected override void Start()
    {
      this.spriteRenderer.sortingOrder = SpriteSortingOrder.BackGround;
    }
  }

}
