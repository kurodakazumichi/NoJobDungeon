using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// キャラクターのベースクラス
  /// </summary>
  public abstract class CharBase: IAttackable
  {
    //-------------------------------------------------------------------------
    // コンストラクタ
    public CharBase()
    {

    }

    //-------------------------------------------------------------------------
    // Member

    //-------------------------------------------------------------------------
    // Properity

    /// <summary>
    /// キャラクターチップ
    /// </summary>
    protected CharChip Chip { get; set; } = null;

    /// <summary>
    /// ステータス
    /// </summary>
    public Status Status { get; protected set; } = null;

    /// <summary>
    /// 座標
    /// </summary>
    public Vector2Int Coord { get; set; } = Vector2Int.zero;

    /// <summary>
    /// アイドル状態です
    /// </summary>
    virtual public bool IsIdle
    {
      get
      {
        if (Chip == null)
        {
          return true;
        }

        return Chip.IsIdle;
      }
    }

    //-------------------------------------------------------------------------
    // Method

    /// <summary>
    /// 攻撃をする
    /// </summary>
    virtual public void Attack(IAttackable target)
    {
      target.AcceptAttack(this);
    }

    /// <summary>
    /// 攻撃を受ける
    /// </summary>
    virtual public void AcceptAttack(IAttackable attacker)
    {
      // 攻撃を受ける
      Status.AcceptAttack(attacker.Status);

      // 攻撃してきた奴の方を向く
      Chip.Direction = Direction.LookAt(Coord, attacker.Coord);
    }

    /// <summary>
    /// 指定方向が障害物かどうか
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected bool IsObstacle(Direction direction)
    {
      IReadOnlyTile next = DungeonManager.Instance.GetTile(Coord, direction);
      return next.IsObstacle;
    }

    /// <summary>
    /// 方向の両脇に位置するタイル情報を返す。
    /// このメソッドは方向が斜めの場合にのみ対応する特殊なメソッド
    /// 例えば左下の場合は左と下の２種類のタイル情報が得られる。
    /// </summary>
    protected (IReadOnlyTile, IReadOnlyTile) GetTilesByDiagonal(Direction direction)
    {
      DungeonManager DM = DungeonManager.Instance;

      // 斜め方向以外の場合はnullを返す
      if (direction.IsStraight || direction.IsNeutral)
      {
        return (null, null);
      }

      // 斜め方向の場合はその両サイドに該当するタイル情報を返す
      IReadOnlyTile tile1 = (direction.hasLeft)
        ? DM.GetTile(Coord, Direction.left)
        : DM.GetTile(Coord, Direction.right);

      IReadOnlyTile tile2 = (direction.hasUp)
        ? DM.GetTile(Coord, Direction.up)
        : DM.GetTile(Coord, Direction.down);

      return (tile1, tile2);
    }

    /// <summary>
    /// 指定した方向に攻撃できる
    /// </summary>
    protected bool CanAttackTo(Direction direction)
    {
      // 上下左右の攻撃は基本OK
      if (direction.Unified.IsStraight)
      {
        return true;
      }

      // 斜め攻撃の場合は周囲の情報を見て判断
      var (tile1, tile2) = GetTilesByDiagonal(direction);

      if (tile1.IsWall || tile2.IsWall)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// 指定した方向に移動できるかを確認
    /// </summary>
    protected bool CanMoveTo(Direction direction)
    {
      // 次の座標が障害物だったら進めない
      if (IsObstacle(direction))
      {
        return false;
      }

      // 上下左右の場合は進める
      if (direction.Unified.IsStraight) {
        return true;
      } 

      // 斜め方向からタイル情報を２つ取得する
      var (tile1, tile2) = GetTilesByDiagonal(direction);

      // 斜め移動時は周囲に壁があったら進めない
      if (tile1.IsWall || tile2.IsWall)
      {
        return false;
      }

      // 進行方向をふさぐように敵がいたら進めない
      if (tile1.IsEnemy && tile2.IsEnemy)
      {
        return false;
      }

      // その他のケースはタイルが障害物でなければ進める
      return true;
    }

  }
}