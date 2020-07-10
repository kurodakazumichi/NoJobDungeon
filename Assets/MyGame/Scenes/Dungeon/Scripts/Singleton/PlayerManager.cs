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
    // Public Properity

    /// <summary>
    /// プレイヤーチップのオブジェクト
    /// </summary>
    public GameObject PlayerObject => ((this.player != null)? this.player.PlayerObject:null);

    //-------------------------------------------------------------------------
    // Public Method

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

    /// <summary>
    /// プレイヤーの思考を監視する
    /// </summary>
    /// <returns></returns>
    public Player.Behavior monitorPlayerThoughs()
    {
      if (this.player == null)
      {
        return Player.Behavior.Thinking;
      }

      return this.player.Think();
    }

#if UNITY_EDITOR

    [SerializeField]
    private bool showDebug = false;

    private void OnGUI()
    {
      if (!showDebug) return;

      this.player.OnGUI();
    }

#endif

  }

}
