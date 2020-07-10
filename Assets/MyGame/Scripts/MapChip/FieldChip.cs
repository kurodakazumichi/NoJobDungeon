using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame {

  /// <summary>
  /// 床や壁などのフィールド系のチップ
  /// </summary>
  public class FieldChip : MapChipBase
  {
    public void SetSprite(Sprite sprite)
    {
      Debug.Log(sprite);
      this.spriteRenderer.sprite = sprite;
    }

    protected override void Start()
    {
      this.spriteRenderer.sortingOrder = SpriteSortingOrder.BackGround;
    }
  }

}
