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
      this.player.transform.position = DungeonManager.Instance.GetPositionBy(coord);
    }

    //-------------------------------------------------------------------------
    // 移動関連の処理

    /// <summary>
    /// 指定方向にプレイヤーが移動できるかチェックし、移動できる場合は移動する。
    /// 移動できなかった場合はfalse、移動した場合はtureを返す。
    /// </summary>
    public bool CheckAndMovePlayer(Direction direction)
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
    public bool ChecksPlayerMovable(Direction direction)
    {
      DungeonManager DM = DungeonManager.Instance;

      var coord = this.player.Coord;

      IReadOnlyTile curr = DM.GetTile(coord);
      IReadOnlyTile next = DM.GetTile(coord, direction);

      // 上下左右だったら進行方向のタイルが障害物でなければ進める
      if (direction.IsStraight || direction.IsNeutral)
      {
        return !next.IsObstacle;
      }

      // 斜め入力の場合は入力された方向によって周囲の壁の情報を見て判断
      IReadOnlyTile tile1 = (direction.hasLeft)
        ? DM.GetTile(coord, Direction.left)
        : DM.GetTile(coord, Direction.right);

      IReadOnlyTile tile2 = (direction.hasUp)
        ? DM.GetTile(coord, Direction.up)
        : DM.GetTile(coord, Direction.down);

      // 斜め入力時は周囲に壁があったら進めない
      if (tile1.IsWall || tile2.IsWall)
      {
        return false;
      }

      // 斜め入力時に進行方向をふさぐように敵がいたら進めない
      if (tile1.IsEnemy && tile2.IsEnemy)
      {
        return false;
      }

      // その他のケースはタイルが障害物でなければ進める
      return !next.IsObstacle;
    }

    /// <summary>
    /// 指定方向にプレイヤーを動かす
    /// </summary>
    public void MovePlayer(Direction direction)
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
    public void SetPlayerDirection(Direction direction)
    {
      this.player.Direction = direction;
    }

    /// <summary>
    /// 指定した座標に配置する
    /// </summary>
    /// <param name="pos"></param>
    public void SetPlayerPosition(Vector3 pos)
    {
      if (!this.player) return;

      this.player.transform.position = pos;
    }

    //-------------------------------------------------------------------------
    // 攻撃関連

    public void SetAttack()
    {
      this.player.Attack(0.15f);
    }
  }

}
