using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// 攻撃に関する情報を持っているクラス
  /// </summary>
  public class AttackRequest
  {
    /// <summary>
    /// 攻撃者の名前
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 攻撃者の座標
    /// </summary>
    public Vector2Int Coord { get; set; } = Vector2Int.zero;

    /// <summary>
    /// 攻撃力
    /// </summary>
    public int Pow { get; set; } = 0;

    /// <summary>
    /// 攻撃対象範囲
    /// </summary>
    public List<Vector2Int> Area { get; set; } = new List<Vector2Int>();
  }
}