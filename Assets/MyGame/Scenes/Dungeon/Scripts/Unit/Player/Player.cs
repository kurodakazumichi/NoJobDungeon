using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon {
  
  public class Player
  {
    /// <summary>
    /// プレイヤーの行動一覧
    /// </summary>
    public enum Behavior
    {
      Thinking, // 考え中
      Move,     // 移動
      Attack1,  // 通常攻撃
      Attack2,  // 遠距離攻撃
      Dash,     // ダッシュ
      Menu,     // メニューを開く
    }

    //-------------------------------------------------------------------------
    // メンバー

    /// <summary>
    /// プレイヤーチップ
    /// </summary>
    private PlayerChip chip;

    /// <summary>
    /// プレイヤーの座標
    /// </summary>
    private Vector2Int coord = Vector2Int.zero;

    //-------------------------------------------------------------------------
    // Public

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Player(Vector2Int coord)
    {
      this.coord = coord;

      this.chip  = MapChipFactory.Instance.CreatePlayerChip();
      this.chip.transform.position = Util.GetPositionBy(coord);
    }

    /// <summary>
    /// プレイヤーの座標
    /// </summary>
    public Vector2Int Coord => (this.coord);

    /// <summary>
    /// プレイヤーの思考処理、といっても処理の実態は入力内容から行動を決定する事になる
    /// </summary>
    public Behavior Think()
    {
       return Behavior.Thinking;
    }

    //-------------------------------------------------------------------------
    // 思考

    /// <summary>
    /// 入力を調べて移動や攻撃の場合はそれぞれのステートへ遷移
    /// 方向転換は状態遷移しないなど入力＝状態遷移ではない
    /// </summary>
    private void ThinkUpdate() 
    {
      // 攻撃の入力
      if (InputManager.Instance.RB2.IsDown)
      {
        return;
      }

      // 方向キーを取得
      var direction = InputManager.Instance.DirectionKey;

      // 方向キーの入力がなければ継続
      if (direction.IsNeutral) return;

      // プレイヤーの方向を更新
      this.chip.Direction = direction;

      // プレイヤーが移動可能
      var isMovable = ChecksPlayerMovable(direction);

      // プレイヤー移動待ちフェーズへ
      //if (isMovable)
      //{
      //  var nextCoord = Util.GetCoord(this.coord, direction);
      //  SetMoveState(nextCoord);
      //}
    }

    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// 指定した方向にプレイヤーが動けるかを確認
    /// </summary>
    public bool ChecksPlayerMovable(Direction direction)
    {
      DungeonManager DM = DungeonManager.Instance;

      var coord = this.coord;

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
  }

}


