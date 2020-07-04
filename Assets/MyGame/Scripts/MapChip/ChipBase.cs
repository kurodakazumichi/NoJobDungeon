using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapChip {
  /// <summary>
  /// マップチップのベースクラス
  /// </summary>
  public class ChipBase : StatefullMonoBehavior
  {
    protected SpriteRenderer spriteRenderer;

    /// <summary>
    /// SpriteRendererをアタッチする
    /// </summary>
    protected virtual void Awake()
    {
      this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
    }
  }
}