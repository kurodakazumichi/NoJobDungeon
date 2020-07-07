using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame 
{
  /// <summary>
  /// PlayerChipのReadOnly用インターフェース
  /// </summary>
  public interface IReadOnlyPlayerChip : IReadOnlyCharChipBase {

  }

  public class PlayerChip : CharChipBase, IReadOnlyPlayerChip
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
