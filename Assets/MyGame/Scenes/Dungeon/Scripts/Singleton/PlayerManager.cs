using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{

  /// <summary>
  /// プレイヤーマネージャー
  /// </summary>
  public class PlayerManager : SingletonMonobehaviour<PlayerManager>
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// プレイヤー
    /// </summary>
    private Player player = null;

    //-------------------------------------------------------------------------
    // Public

    /// <summary>
    /// プレイヤーを作成し、指定された座標に置く
    /// </summary>
    public void CreatePlayer(Vector2Int coord)
    {
      if (this.player == null)
      {
        this.player = new Player(coord);
      }
    }

#if UNITY_EDITOR

    [SerializeField]
    private bool showDebug = false;

    private void OnGUI()
    {
      if (!showDebug) return;

      if (this.player == null && GUILayout.Button("Create Player"))
      {
        CreatePlayer(Vector2Int.zero);
      }

      if (this.player != null)
      {
        Vector2Int coord = this.player.Coord;
        GUILayout.Label($"Player Coord:({coord.x}, {coord.y})");

        GUILayout.Label($"Player Behavior:{this.player.Think()}");
      }
    }

#endif

  }

}
