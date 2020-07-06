using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.MapChip {

  /// <summary>
  /// ChipBaseのReadOnly用インターフェース
  /// </summary>
  public interface IReadOnlyChipBase {
    Vector2Int Coord { get; }
  }

  /// <summary>
  /// マップチップのベースクラス
  /// </summary>
  public class ChipBase : MyMonoBehaviour, IReadOnlyChipBase
  {
    protected SpriteRenderer spriteRenderer;

    /// <summary>
    /// マップチップの存在する座標
    /// </summary>
    protected  Vector2Int coord;

    public Vector2Int Coord { 
      get { return this.coord; }
      set { this.coord = value; }
    }

    /// <summary>
    /// SpriteRendererをアタッチする
    /// </summary>
    protected override void Awake()
    {
      this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
    }
  }
}