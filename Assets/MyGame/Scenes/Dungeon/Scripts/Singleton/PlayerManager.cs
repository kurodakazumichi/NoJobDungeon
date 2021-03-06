﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// プレイヤーマネージャー
  /// </summary>
  public class PlayerManager : SingletonMonobehaviour<PlayerManager>, IDebuggeable
  {
    //-------------------------------------------------------------------------
    // Member

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

    /// <summary>
    /// Activeな(何かしら活動している)プレイヤーがいるかどうか
    /// </summary>
    /// <returns></returns>
    public bool HasnActivePlayer => ((this.player != null && !this.player.IsIdle));

    /// <summary>
    /// アクター
    /// </summary>
    public IActionable Actor => (this.player);

    /// <summary>
    /// プレイヤーは死んでいます
    /// </summary>
    public bool IsPlayerDead => (this.player.Status.IsDead);

    //-------------------------------------------------------------------------
    // 基本的なメソッド

    /// <summary>
    /// プレイヤーを作成し、指定された座標に置く
    /// 既にプレイヤーが存在していたらリセットする
    /// </summary>
    public void CreatePlayer(Vector2Int coord)
    {
      if (this.player == null) {
        this.player = new Player(coord);
      } else {
        this.player.Reset(coord);
      }
    }

    /// <summary>
    /// プレイヤーの思考
    /// </summary>
    public Player.Behavior Think()
    {
      if (this.player == null) {
        return Player.Behavior.Thinking;
      }

      return this.player.Think();
    }

    /// <summary>
    /// プレイヤーの更新
    /// </summary>
    public void UpdatePlayer()
    {
      this.player.Update();
    }

    //-------------------------------------------------------------------------
    // シーンのフェーズ変化時にコールされるメソッド群

    public void OnSceneMoveEnter()
    {
      this.player.OnSceneMoveEnter();
    }

    public void OnSceneMoveExit()
    {
      this.player.OnSceneMoveExit();
    }

    public void OnSceneActionEnter()
    {
      this.player.OnSceneActionEnter();
    }

    public void OnSceneActionExit()
    {
      this.player.OnSceneActionExit();
    }

    public void OnSceneTurnEndEnter()
    {
      if (this.player != null) this.player.OnSceneTurnEndEnter();
    }

    //-------------------------------------------------------------------------
    // 探す
    public List<IActionable> Find(List<Vector2Int> coords)
    {
      List<IActionable> found = new List<IActionable>();

      foreach(var coord in coords) {
        if (this.player.Coord.Equals(coord)) {
          found.Add(this.player);
        }
      }

      return found;
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ
    void IDebuggeable.Draw(MyDebug.Window window)
    {
      this.player.DrawDebug();
    }

    private void OnGUI()
    {
      if (this.player != null)
      {
        this.player.OnGUI();
      }
    }
#endif

  }

}
