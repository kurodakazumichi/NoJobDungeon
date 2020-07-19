using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// 攻撃に関する情報を持っているクラス
  /// </summary>
  public class ActionRequest
  {
    /// <summary>
    /// Actorの名前
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Actorの座標
    /// </summary>
    public Vector2Int Coord { get; set; } = Vector2Int.zero;

    /// <summary>
    /// 攻撃力
    /// </summary>
    public int Pow { get; set; } = 0;

    /// <summary>
    /// 対象範囲
    /// </summary>
    public List<Vector2Int> Area { get; set; } = new List<Vector2Int>();

    /// <summary>
    /// リセット
    /// </summary>
    public void Reset()
    {
      Name  = "";
      Coord = Vector2Int.zero;
      Pow   = 0;
      Area.Clear();
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ

    public void DrawDebug()
    {
      GUILayout.Label("■ ActionRequest");

      using (var scope = new GUILayout.HorizontalScope())
      {
        GUILayout.Label($"Name:{Name}");
        GUILayout.Label($"Coord:{Coord}");
        GUILayout.Label($"Pow:{Pow}");
        GUILayout.Label("Attack Area");
      }
      using (var scope = new GUILayout.HorizontalScope())
      {
        foreach (var coord in Area)
        {
          GUILayout.Label(coord.ToString());
        }
      }
    }
#endif
  }
}