using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon {
  
  public class Player
  {
    enum Mode
    {
      Think,
      Move,
      Attack,
      WaitMyTurn,
    }

    //-------------------------------------------------------------------------
    // 主要メンバー

    /// <summary>
    /// プレイヤーチップ
    /// </summary>
    private PlayerChip chip;

    /// <summary>
    /// ステートマシン
    /// </summary>
    private StateMachine<Mode> state;

    /// <summary>
    /// プレイヤーの座標
    /// </summary>
    private Vector2Int coord = Vector2Int.zero;

    //-------------------------------------------------------------------------
    // 主要メソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Player(Vector2Int coord)
    {
      this.coord = coord;

      this.state = new StateMachine<Mode>();

      this.chip  = MapChipFactory.Instance.CreatePlayerChip();
      this.chip.transform.position = Util.GetPositionBy(coord);

      // Stateを作成
      this.state.Add(Mode.WaitMyTurn, null, WaitMyTurnUpdate);
      this.state.Add(Mode.Think, null, ThinkUpdate);
      this.state.Add(Mode.Move, MoveEnter, MoveUpdate);
      this.state.Add(Mode.Attack, AttackEnter, AttackUpdate, AttackExit);

      // カメラがプレイヤーを追従するように設定
      CameraManager.Instance.SetTrackingMode(this.chip.gameObject);
    }

    /// <summary>
    /// プレイヤーの動作開始
    /// </summary>
    public void Start()
    {
      CameraManager.Instance.SetTrackingMode(this.chip.gameObject);
      this.state.SetState(Mode.Think);
    }

    /// <summary>
    /// プレイヤーの更新
    /// </summary>
    public void Update()
    {
      this.state.Update();
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Destroy()
    {
      MapChipFactory.Instance.Release(this.chip);
      CameraManager.Instance.SetFreeMode();

      this.chip = null;
      this.state = null;
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
      if (InputManager.Instance.Attack())
      {
        SetAttackState();
        return;
      }

      // 方向キーを取得
      var direction = InputManager.Instance.GetDirectionKey();

      // 方向キーの入力がなければ継続
      if (direction.IsNeutral) return;

      // プレイヤーの方向を更新
      this.chip.Direction = direction;

      // プレイヤーが移動可能
      var isMovable = ChecksPlayerMovable(direction);

      // プレイヤー移動待ちフェーズへ
      if (isMovable)
      {
        var nextCoord = Util.GetCoord(this.coord, direction);
        SetMoveState(nextCoord);
      }
    }

    //-------------------------------------------------------------------------
    // 移動

    /// <summary>
    /// Move状態を設定する
    /// </summary>
    /// <param name="toCoord">移動先の座標</param>
    private void SetMoveState(Vector2Int toCoord)
    {
      this.coord = toCoord;
      var pos = Util.GetPositionBy(toCoord);
      this.chip.Move(Define.SEC_PER_TURN, pos);
      this.state.SetState(Mode.Move);
    }

    /// <summary>
    /// プレイヤー移動時にDungeonManagerのプレイヤー座標を更新する。
    /// </summary>
    private void MoveEnter()
    {
      DungeonManager.Instance.UpdatePlayerCoord(this.coord);
    }

    /// <summary>
    /// プレイヤーの移動が完了するのを待つ
    /// </summary>
    private void MoveUpdate() 
    {
      if (this.chip.IsIdle)
      {
        this.state.SetState(Mode.Think);
      }
    }

    //-------------------------------------------------------------------------
    // 攻撃

    private void SetAttackState()
    {
      this.chip.Attack(Define.SEC_PER_TURN, 1f);
      this.state.SetState(Mode.Attack);
    }

    /// <summary>
    /// 攻撃モーションでカメラがプレイヤーを追従しないように、カメラをロックする
    /// </summary>
    private void AttackEnter() 
    { 
      CameraManager.Instance.Lock();
    }

    /// <summary>
    /// プレイヤーの攻撃完了を待つ
    /// </summary>
    private void AttackUpdate() 
    {
      if (this.chip.IsIdle)
      {
        this.state.SetState(Mode.Think);
      }
    }

    /// <summary>
    /// 攻撃モーションが終わったらカメラのロックを解除する
    /// </summary>
    private void AttackExit() 
    { 
      CameraManager.Instance.Unlock();
    }

    //-------------------------------------------------------------------------
    // 順番待ち
    private void WaitMyTurnUpdate() { }

    // Memo
    // Thinkingは入力内容から行動を決めるモード
    // 移動だったらMoveモードへ
    // 攻撃だったらAttackモードへ

    // Moveはプレイヤーを移動させるモード
    // まず移動できるかどうかを判断する。
    // 移動できない場合はプレイヤーの方向だけ更新して、再びThinkingモードへ戻る
    // 移動できる場合は移動し、移動が完了したらターンエンド通知をだしWaitモードへ
    // ※ターンエンドに関する機能はまだないので考えないといけない
    // DungeonManagerがプレイヤーフェーズかどうかをもっておいて
    // ターンエンド時にDungeonManagerに通知するとか？
    // ターン数とかはDungeonManagerではなくてシーンとかで管理したい気もする

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


