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

    /// <summary>
    /// 次の座標
    /// </summary>
    private Vector2Int nextCoord = Vector2Int.zero;

    /// <summary>
    /// ダッシュの方向
    /// </summary>
    private Direction dashDirection = new Direction();

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// Player Chipのゲームオブジェクト
    /// </summary>
    public GameObject PlayerObject => (this.chip.gameObject);

    /// <summary>
    /// プレイヤーの座標
    /// </summary>
    public Vector2Int Coord => (this.coord);

    /// <summary>
    /// アイドル状態です
    /// </summary>
    public bool IsIdle => (this.chip.IsIdle);

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
    /// プレイヤーの思考処理、といっても処理の実態は入力内容から行動を決定する事になる
    /// </summary>
    public Behavior Think()
    {
      var direction = InputManager.Instance.DirectionKey;

      // 方向キー入力があったらプレイヤーの向きを更新
      if (!direction.IsNeutral)
      {
        this.chip.Direction = direction;
      }

      // ダッシュしたい場合はダッシュできるかどうかをチェック
      // TODO: ダッシュチェック未実装
      if (IsWantToDash())
      {
        this.dashDirection = direction;
        return Behavior.Dash;
      }

      // 移動したい場合は移動できるかどうかをチェック
      if (IsWantToMove() && CanMoveTo(direction))
      {
        this.nextCoord = this.coord + direction.ToVector(false);
        return Behavior.Move;
      }

      // 通常攻撃(RB2)
      if (InputManager.Instance.RB2.IsHold)
      {
        return Behavior.Attack1;
      }

      // 遠距離攻撃(R)
      if (InputManager.Instance.R.IsHold)
      {
        return Behavior.Attack2;
      }

      // メニュー(RB1)
      if (InputManager.Instance.RB1.IsDown)
      {
        return Behavior.Menu;
      }

       return Behavior.Thinking;
    }

    /// <summary>
    /// 移動
    /// </summary>
    public void Move()
    {
      this.coord = this.nextCoord;
      DungeonManager.Instance.UpdatePlayerCoord(this.coord);
      this.chip.Move(Define.SEC_PER_TURN, Util.GetPositionBy(this.coord));
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    public void Attack()
    {
      this.chip.Attack(Define.SEC_PER_TURN, 1f);
    }

    //-------------------------------------------------------------------------
    // 移動に関するUtil

    /// <summary>
    /// 移動したがっている
    /// </summary>
    private bool IsWantToMove()
    {
      var direction = InputManager.Instance.DirectionKey;

      // 方向キー入力がなければ移動なし
      if (direction.IsNeutral)
      {
        return false;
      }

      // 斜めモードで、斜め入力じゃない場合も移動なし
      if (InputManager.Instance.L.IsHold && !direction.IsDiagonal)
      {
        return false;
      }

      // 方向転換モードだったら移動なし
      if (InputManager.Instance.RB4.IsHold)
      {
        return false;
      }

      return true;
    }

    private bool IsWantToDash()
    {
      var direction = InputManager.Instance.DirectionKey;

      // ダッシュキーが押されてなければダッシュしない
      if (!InputManager.Instance.RB3.IsHold)
      {
        return false;
      }

      // 方向入力がなければダッシュしたくない
      if (direction.IsNeutral)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// 指定した方向にプレイヤーが動けるかを確認
    /// </summary>
    public bool CanMoveTo(Direction direction)
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

#if UNITY_EDITOR

    public void OnGUI()
    {
      GUILayout.BeginArea(new Rect(500, 0, 500, 500));
      {
        GUILayout.Label($"Current Coord: ({this.Coord})");
        GUILayout.Label($"Next    Coord: ({this.nextCoord})");
        GUILayout.Label($"Dash Direction: ({this.dashDirection.value})");
        GUILayout.Label($"Behavior:{this.Think()}");
      }
      GUILayout.EndArea();
    }

#endif
  }

}


