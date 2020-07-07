﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame 
{
  /// <summary>
  /// PlayerChipのReadOnly用インターフェース
  /// </summary>
  public interface IReadOnlyPlayerChip : IReadOnlyDeprecatedCharChipBase {

  }

  public class DeprecatedPlayerChip : DeprecatedCharChipBase, IReadOnlyPlayerChip
  {
    /// <summary>
    /// プレイヤー用のリソースをロード
    /// </summary>
    /// <returns></returns>
    protected override Sprite[] LoadBaseSprites()
    {
      //return Resources.LoadAll<Sprite>("player");
      return ResourceManager.Instance.GetResources<Sprite>("player");
    }
  }
}
