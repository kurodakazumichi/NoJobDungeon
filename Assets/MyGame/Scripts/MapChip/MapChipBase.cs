using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame {

  /// <summary>
  /// ChipBaseのReadOnly用インターフェース
  /// </summary>
  public interface IReadOnlyMapChipBase {
  }

  /// <summary>
  /// マップチップのベースクラス
  /// </summary>
  [RequireComponent(typeof(SpriteRenderer))]
  public class MapChipBase : MyMonoBehaviour, IReadOnlyMapChipBase
  {
    protected SpriteRenderer spriteRenderer;

    /// <summary>
    /// SpriteRendererをアタッチする
    /// </summary>
    protected override void Awake()
    {
      this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
    }
  }
}