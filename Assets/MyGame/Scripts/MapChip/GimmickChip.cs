using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public class GimmickChip : MapChipBase
  {
    /// <summary>
    /// スプライトを設定する
    /// </summary>
    public void SetSprite(Sprite sprite)
    {
      this.spriteRenderer.sprite = sprite;
    }

    /// <summary>
    /// スプライトの表示順を設定
    /// </summary>
    protected override void Start()
    {
      this.spriteRenderer.sortingOrder = SpriteSortingOrder.Gimmick;
    }
  }

}
