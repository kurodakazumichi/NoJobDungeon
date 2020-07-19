using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class Enemy : CharBase
  {
    /// <summary>
    /// 敵生成に必要なパラメータ
    /// </summary>
    public class Props
    {
      public Props(Vector2Int coord, EnemyChipType chipType, Status.Props statusProps)
      {
        Coord = coord;
        ChipType = chipType;
        StatusProps = statusProps;
      }

      public Vector2Int Coord = Vector2Int.zero;
      public EnemyChipType ChipType = default;
      public Status.Props StatusProps = null;
    }

    /// <summary>
    /// 敵の行動一覧
    /// </summary>
    public enum BehaviorType {
      None,
      Move,
      Action,
    }

    /// <summary>
    /// アクション
    /// </summary>
    public enum ActionPhase
    {
      Idle,
      Move,
      Damage,
      Vanish,
    }

    //-------------------------------------------------------------------------
    // メンバー

    /// <summary>
    /// ステートマシン
    /// </summary>
    private StateMachine<ActionPhase> state = new StateMachine<ActionPhase>();

    /// <summary>
    /// 行動
    /// </summary>
    private BehaviorType behavior = BehaviorType.None;

    /// <summary>
    /// DungeonManagerのステージ上の座標
    /// ThinkのタイミングでCoordを更新してしまうと攻撃判定が移動後の座標で行われるため
    /// ThinkのタイミングではstageCoordを更新し実際に動く際にCoordを更新する。
    /// </summary>
    public Vector2Int stageCoord = Vector2Int.zero;

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// 行動
    /// </summary>
    public BehaviorType Behavior => (this.behavior);

    /// <summary>
    /// アイドル状態です
    /// </summary>
    public override bool IsIdle => (state.StateKey == ActionPhase.Idle);

    //-------------------------------------------------------------------------
    // 基本的なメソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Enemy()
    {
      this.state.Add(ActionPhase.Idle);
      this.state.Add(ActionPhase.Move, MoveEnter, MoveUpdate);
      this.state.Add(ActionPhase.Damage, DamageEnter, DamageUpdate);
      this.state.Add(ActionPhase.Vanish, VanishEnter, VanishUpdate, VanishExit);
    }

    /// <summary>
    /// セットアップ
    /// </summary>
    virtual public void Setup(Props props)
    {
      Chip = MapChipFactory.Instance.CreateEnemyChip(props.ChipType);
      Coord = this.stageCoord = props.Coord;

      Chip.transform.position = Util.GetPositionBy(Coord);
      Chip.Direction = Direction.Random();

      Status = new Status(props.StatusProps);
    }

    /// <summary>
    /// 敵の更新処理
    /// </summary>
    public void Update()
    {
      this.state.Update();
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Destory()
    {
      MapChipFactory.Instance.Release(Chip);
      Chip = null;
    }

    //-------------------------------------------------------------------------
    // 思考(AI)

    /// <summary>
    /// AI: どんな行動をするか決定する処理
    /// </summary>
    public void Think()
    {
      // エネルギーがなければ行動しない
      if (!Status.HasEnergy) return;

      // アクションを考え、アクションするならそこで終了
      if (ThinkAction()) return;

      // アクションしない場合は移動を考える
      ThinkMove();
    }

    private bool ThinkAction()
    {
      // 自分の周囲１マスにプレイヤーがいるかどうか
      var player = DungeonManager.Instance.PlayerCoord;
      var v = player - Coord;

      // 周囲１マスにプレイヤーがいる、かつその方向に攻撃可能であれば
      if (Mathf.Abs(v.x) <= 1 && Mathf.Abs(v.y) <= 1 && CanAttackTo(new Direction(v, false)))
      {
        // かつ攻撃できる方向であれば攻撃
        this.behavior = BehaviorType.Action;
        Chip.Direction = new Direction(v, false);

        // 攻撃要求をセット
        ActionRequest.Name = Status.Name;
        ActionRequest.Pow = Status.Pow;
        ActionRequest.Coord = Coord;
        ActionRequest.Area.Add(player);
        return true;
      }

      return false;
    }

    private bool ThinkMove()
    {
      // ランダムで移動量を決める
      var move = new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));

      // 移動方向を生成
      var moveDir = new Direction(move, false);

      // 移動可能であれば移動する
      if (CanMoveTo(moveDir))
      {
        Chip.Direction = moveDir;
        this.stageCoord = Coord + move;
        this.behavior = BehaviorType.Move;

        // ダンジョンの情報を書き換え
        UpdateStageCoord(Coord, this.stageCoord);

        return true;
      }
      return false;
    }

    //-------------------------------------------------------------------------
    // Sceneから呼ばれるコールバック

    public override void OnSceneMoveEnter()
    {
      if (this.behavior != BehaviorType.Move) return;

      Coord = this.stageCoord;
      Chip.Move(Define.SEC_PER_TURN, Util.GetPositionBy(Coord));
      this.state.SetState(ActionPhase.Move);

      this.behavior = BehaviorType.None;
      Status.UseEnergy();
    }

    public override void OnSceneActionEnter()
    {
      if (this.behavior != BehaviorType.Action) return;

      var AM = ActionManager.Instance;
      var PM = PlayerManager.Instance;

      AM.SetActor(this);

      var target = PM.Find(ActionRequest.Area);
      AM.AddTarget(target);
      AM.StartAction();

      this.behavior = BehaviorType.None;
      Status.UseEnergy();
    }

    //-------------------------------------------------------------------------
    // IActionableの実装

    public override void OnActionStartWhenActor()
    {
      DoAttackMotion();
    }

    public override void OnActionWhenTarget(IActionable actor)
    {
      this.state.SetState(ActionPhase.Damage);
    }

    public override void OnActionEndWhenTarget()
    {
      if (Status.IsDead)
      {
        this.state.SetState(ActionPhase.Vanish);
      }
    }

    public override void OnReactionStartWhenActor()
    {
      if (Status.IsDead) return;
      ThinkAction();
      if (this.behavior == BehaviorType.Action)
      {
        Status.UseEnergy();
      }
    }

    public override bool IsReaction => (this.ActionResponse.IsAccepted);

    //-------------------------------------------------------------------------
    // 移動処理

    private void MoveEnter()
    {
      Chip.Move(Define.SEC_PER_TURN, Util.GetPositionBy(Coord));
    }

    private void MoveUpdate()
    {
      if (Chip.IsIdle)
      {
        this.state.SetState(ActionPhase.Idle);
      }
    }

    //-------------------------------------------------------------------------
    // ダメージを受けた
    
    private void DamageEnter()
    {
      DoOuchMotion();
    }

    private void DamageUpdate()
    {
      if (Chip.IsIdle)
      {
        this.state.SetState(ActionPhase.Idle);
      }
    }

    //-------------------------------------------------------------------------
    // 消滅する

    private void VanishEnter()
    {
      DoVanishMotion();
    }

    private void VanishUpdate()
    {
      if (Chip.IsIdle)
      {
        this.state.SetState(ActionPhase.Idle);
      }
    }

    private void VanishExit()
    {
      HUD.Instance.UpdateMinimap();
    }

    //-------------------------------------------------------------------------
    // モーション系

    /// <summary>
    /// このメソッドを呼ぶと敵が動き始める
    /// </summary>
    public void DoMoveMotion()
    {
      if (this.behavior != BehaviorType.Move) return;

      Coord = this.stageCoord;
      Chip.Move(Define.SEC_PER_TURN, Util.GetPositionBy(Coord));
      this.behavior = BehaviorType.None;
    }

    /// <summary>
    /// 攻撃予定の敵がこのメソッドを呼ばれると、攻撃の動きを開始する
    /// </summary>
    public void DoAttackMotion()
    {
      // アタッカーじゃなければ何もしない
      if (this.behavior != BehaviorType.Action) return;

      // 攻撃の動きを開始
      Chip.Attack(Define.SEC_PER_TURN, 1f);
      this.behavior = BehaviorType.None;
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が痛がる
    /// </summary>
    public void DoOuchMotion()
    {
      // 攻撃を受けていなければ痛がらない
      if (!ActionResponse.IsAccepted) return;

      // ダメージがある場合は痛がる
      if (ActionResponse.HasDamage)
      {
        Chip.Ouch(Define.SEC_PER_TURN * 2);
      }
      
      // ダメージがなければ待機
      else
      {
        Chip.Wait(Define.SEC_PER_TURN * 2);
      }
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が消滅する
    /// </summary>
    public void DoVanishMotion()
    {
      // 死んでいなければ消えない
      if (!Status.IsDead) return;

      // マップ上の敵の情報を除去する
      DungeonManager.Instance.RemoveEnemyCoord(stageCoord);

      // 消滅モーション開始
      Chip.Vanish(Define.SEC_PER_TURN);
    }



    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// ステージ座標を更新
    /// </summary>
    private void UpdateStageCoord(Vector2Int from, Vector2Int to)
    {
      DungeonManager.Instance.UpdateEnemyCoord(from, to);
      stageCoord = to;
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ
    public void DrawDebugMenu()
    {
      GUILayout.Label($"Current Coord: ({this.Coord})");
      GUILayout.Label($"Behavior:{this.behavior}" );;
      Status.DrawDebug();
      ActionRequest.DrawDebug();
      ActionResponse.DrawDebug();
      Chip.DrawDebugMenu();
    }
#endif
  }
}