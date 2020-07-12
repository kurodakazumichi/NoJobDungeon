﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// ダンジョンマネージャー
  /// </summary>
  public class DungeonManager : SingletonMonobehaviour<DungeonManager>
  {
    //-------------------------------------------------------------------------
    // メンバ変数
    /// <summary>
    /// ダンジョン生成アルゴリズム
    /// </summary>
    private Algorithm algorithm;

    /// <summary>
    /// ダンジョン配列
    /// </summary>
    private Stage stage;

    //-------------------------------------------------------------------------
    // 主要なメソッド

    /// <summary>
    /// メンバの初期化
    /// </summary>
    protected override void Awake()
    {
      base.Awake();
      this.algorithm = new Algorithm();
      this.stage     = new Stage();

#if _DEBUG
      DebugMenuManager.Instance.RegisterMenu(DebugMenu.Page.Dungeon, DrawDebugMenu, nameof(DungeonScene));
#endif
    }

    //-------------------------------------------------------------------------
    // 生成

    /// <summary>
    /// ダンジョンの生成
    /// </summary>
    public void CreateStage()
    {
      this.algorithm.SetConfig(3, 2, 0.5f);
      this.stage.Make(this.algorithm);
    }

    public Vector2Int PlayerCoord
    {
      get { return this.stage.Find(Tiles.Player)[0]; }
    }

    //-------------------------------------------------------------------------
    // 取得

    /// <summary>
    /// ステージのタイル情報を取得する
    /// </summary>
    public IReadOnlyTile GetTile(Vector2Int coord)
    {
      return this.stage.GetTile(coord);
    }

    public IReadOnlyTile GetTile(Vector2Int coord, Direction direction)
    {
      return GetTile(Dungeon.Util.GetCoord(coord, direction));
    }

    /// <summary>
    /// プレイヤーの座標を更新する
    /// </summary>
    public void UpdatePlayerCoord(Vector2Int from, Vector2Int to)
    {
      this.stage.RemoveTileState(from, Tiles.Player);
      this.stage.AddTileState(to, Tiles.Player);
    }

    /// <summary>
    /// 敵の座標を更新する
    /// </summary>
    public void UpdateEnemyCoord(Vector2Int from, Vector2Int to)
    {
      this.stage.RemoveTileState(from, Tiles.Enemy);
      this.stage.AddTileState(to, Tiles.Enemy);
    }

    /// <summary>
    /// 指定した座標から敵の情報を除去する
    /// </summary>
    public void RemoveEnemyCoord(Vector2Int coord)
    {
      this.stage.RemoveTileState(coord, Tiles.Enemy);
    }

    //-------------------------------------------------------------------------
    // その他

    public void Map(System.Action<int, int, IReadOnlyTile> cb)
    {
      this.stage.Map(cb);
    }

#if _DEBUG
    public void DrawDebugMenu(DebugMenu.MenuWindow menuWindow)
    {
      this.stage.DrawDebugMenu(menuWindow);

      if (GUILayout.Button("Algorithm"))
      {
        DebugMenuManager.Instance.OpenWindow(
          DebugMenu.Page.Algorithm,
          this.algorithm.DrawDebugMenu
        );
      }
    }
#endif
  }

}

