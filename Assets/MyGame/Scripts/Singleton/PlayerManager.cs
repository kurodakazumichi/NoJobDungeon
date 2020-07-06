using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Unit.Dungeon;

namespace MyGame.Singleton {

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
    // プレイヤー生成

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
    /// Player.StartのWrapper
    /// </summary>
    public void StartPlayer()
    {
      if (this.player != null)
      {
        this.player.Start();
      }
    }

    /// <summary>
    /// プレイヤーの更新
    /// </summary>
    public void UpdatePlayer()
    {
      if (this.player != null)
      {
        this.player.Update();
      }
    }
  }

}
