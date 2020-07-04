using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapChip;
using Dungeon;

namespace Singleton {

  /// <summary>
  /// プレイヤーマネージャー
  /// </summary>
  public class PlayerManager : SingletonMonobehaviour<PlayerManager>
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// プレイヤーマップチップ
    /// </summary>
    private PlayerChip player = null;

    //-------------------------------------------------------------------------
    // 主要なプロパティ

    public IReadOnlyPlayerChip Player
    {
      get { return this.player; }
    }

    /// <summary>
    /// プレイヤーのゲームオブジェクト
    /// </summary>
    public GameObject PlayerObject
    {
      get
      {
        if (this.player == null) return null;
        return this.player.gameObject;
      }
    }

    //-------------------------------------------------------------------------
    // プレイヤー生成

    /// <summary>
    /// プレイヤーを作成し、指定された座標に置く
    /// </summary>
    public void CreatePlayer(Vector2Int coord)
    {
      if (!this.player)
      {
        this.player = MapChipFactory.Instance.CreatePlayerChip();
      }

      this.player.Coord = coord;
      this.player.transform.position = DungeonManager.Instance.GetPositionFromCoord(coord);
    }

    //-------------------------------------------------------------------------
    // 移動関連の処理

    /// <summary>
    /// 指定方向にプレイヤーが移動できるかチェックし、移動できる場合は移動する。
    /// 移動できなかった場合はfalse、移動した場合はtureを返す。
    /// </summary>
    public bool CheckAndMovePlayer(Direction8 direction)
    {
      // Playerの移動に関わらず方向は更新する
      SetPlayerDirection(direction);

      // 移動できない
      if (!ChecksPlayerMovable(direction)) return false;

      // プレイヤーを動かす
      MovePlayer(direction);
      return true;
    }

    /// <summary>
    /// 指定した方向にプレイヤーが動けるかを確認
    /// </summary>
    public bool ChecksPlayerMovable(Direction8 direction)
    {
      IReadOnlyTile tile = DungeonManager.Instance.GetTile(this.player.Coord, direction);

      return !tile.IsObstacle;
    }

    /// <summary>
    /// 指定方向にプレイヤーを動かす
    /// </summary>
    public void MovePlayer(Direction8 direction)
    {
      SetPlayerDirection(direction);

      var coord = DungeonManager.Instance.GetCoord(this.player.Coord, direction);
      this.player.Move(0.15f, coord);

      DungeonManager.Instance.UpdatePlayerCoord(coord);
    }

    /// <summary>
    /// プレイヤーの方向をセットする
    /// </summary>
    /// <param name="direction"></param>
    public void SetPlayerDirection(Direction8 direction)
    {
      this.player.Direction = direction;
    }

    public void SetPlayerPosition(Vector3 pos)
    {
      if (!this.player) return;

      this.player.transform.position = pos;
    }


  }

}
