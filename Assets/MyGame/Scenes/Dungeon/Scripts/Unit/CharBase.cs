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

    /// <summary>
    /// キャラクターチップ
    /// </summary>
    protected CharChip chip;

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// 座標
    /// </summary>
    public Vector2Int Coord { get; set; }

  }
}