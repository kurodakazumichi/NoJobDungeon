using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// キャラクターのベースクラス
  /// </summary>
  public abstract class CharBase
  {
    //-------------------------------------------------------------------------
    // コンストラクタ
    public CharBase()
    {

    }

    //-------------------------------------------------------------------------
    // Member

    //-------------------------------------------------------------------------
    // Properity

    /// <summary>
    /// キャラクターチップ
    /// </summary>
    protected CharChip Chip { get; set; } = null;

    /// <summary>
    /// 座標
    /// </summary>
    public Vector2Int Coord { get; set; } = Vector2Int.zero;

  }
}