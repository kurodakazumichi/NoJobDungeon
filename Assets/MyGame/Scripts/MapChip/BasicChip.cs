using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// 基本的なマップチップのクラス
  /// 1. Sprite１枚とSortingOrderが設定できる
  /// </summary>
  public class BasicChip : MapChipBase
  {
    override public void Reset()
    {
      base.Reset();

      Sprite  = null;
      Sorting = 0;
    }
  }
}