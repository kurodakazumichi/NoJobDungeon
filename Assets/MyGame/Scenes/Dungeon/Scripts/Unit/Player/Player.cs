﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon 
{
  /// <summary>
  /// ダンジョン内のプレイヤーに該当するクラス
  /// プレイヤーに関するパラメータやダンジョン内での行動判断ロジックを持つ。
  /// またプレイヤーチップの制御を行う。
  /// </summary>
  public class Player : CharBase
  {
    /// <summary>
    /// プレイヤーの行動一覧
    /// </summary>
    public enum Behavior
    {
      Thinking,  // 考え中
      Move,      // 移動
      Action,    // 行動
      SubAttack, // 遠距離攻撃
      Dash,      // ダッシュ
      Menu,      // メニューを開く
    }

    /// <summary>
    /// アクション
    /// </summary>
    public enum ActionPhase
    {
      Idle,
      Move,
      AttackStart,
      Damage,
    }

    //-------------------------------------------------------------------------
    // メンバー

    /// <summary>
    /// ステートマシン
    /// </summary>
    private StateMachine<ActionPhase> state = new StateMachine<ActionPhase>();

    /// <summary>
    /// ダッシュの方向
    /// </summary>
    private Direction dashDirection = new Direction();

    /// <summary>
    /// 敵の座標を入れるための配列とその配列を指すIndex
    /// 周囲に敵がいた時に、ワンボタンで敵の方を向く処理で使う。
    /// </summary>
    private List<Vector2Int> aroundEnemies = new List<Vector2Int>();
    private int aroundEnemiesIndex = 0;

    /// <summary>
    /// 投げるアイテム
    /// </summary>
    private FieldItem item = null;

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// Player Chipのゲームオブジェクト
    /// </summary>
    public GameObject PlayerObject => (Chip.gameObject);

    /// <summary>
    /// アイドル状態です
    /// </summary>
    public override bool IsIdle => (state.StateKey == ActionPhase.Idle);

    //-------------------------------------------------------------------------
    // 基本的なメソッド

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Player(Vector2Int coord)
    {
      Chip = MapChipFactory.Instance.CreatePlayerChip();

      Reset(coord);

      Status.Props props = new Status.Props("無職", 10, 10, 2);
      Status = new Status(props);

      this.item = null;

      this.state.Add(ActionPhase.Idle);
      this.state.Add(ActionPhase.Move, MoveEnter, MoveUpdate, MoveExit);
      this.state.Add(ActionPhase.AttackStart, AttackStartEnter, AttackStartUpdate, AttackStartExit);
      this.state.Add(ActionPhase.Damage, DamageEnter, DamageUpdate);
    }

    /// <summary>
    /// リセット
    /// </summary>
    public void Reset(Vector2Int coord)
    {
      Coord = coord;
      Chip.transform.position = Util.GetPositionBy(coord);
      Chip.Direction = Direction.down;
    }

    /// <summary>
    /// プレイヤーの更新処理
    /// </summary>
    public void Update()
    {
      this.state.Update();
    }

    //-------------------------------------------------------------------------
    // 思考(入力検知)

    /// <summary>
    /// 入力内容からプレイヤーがどんな行動をするかを決める処理
    /// </summary>
    public Behavior Think()
    {
      var behaviour = DecideBehaviour();

      if (behaviour != Behavior.Thinking)
      {
        this.aroundEnemies.Clear();
        this.aroundEnemiesIndex = 0;
      }

      return behaviour;
    }

    private Behavior DecideBehaviour()
    {
      var direction = InputManager.Instance.DirectionKey;

      // 方向キー入力があったらプレイヤーの向きを更新
      if (!direction.IsNeutral)
      {
        Chip.Direction = direction;
      }

      // 周囲に敵がいれば敵の方を向く
      if (InputManager.Instance.RB4.IsUp)
      {
        if (this.aroundEnemies.Count == 0)
        {
          // 周囲１マスを見る
          DungeonManager.Instance.LookAround(Coord, 1, (x, y, tile) =>
          {
            if (tile.IsEnemy)
            {
              this.aroundEnemies.Add(new Vector2Int(x, y));
            }
          });
        }

        // 周囲に敵がいたらそっちを向く
        if (this.aroundEnemies.Count != 0)
        {
          var pos = this.aroundEnemies[this.aroundEnemiesIndex % this.aroundEnemies.Count];

          Chip.Direction = Direction.LookAt(Coord, pos);
          this.aroundEnemiesIndex++;
        }
      }

      // ダッシュしたい場合はダッシュできるかどうかをチェック
      // TODO: ダッシュチェック未実装
      if (IsWantToDash())
      {
        this.dashDirection = direction;
        return Behavior.Dash;
      }

      // 移動したい場合は移動できるかどうかをチェックしてDungeon情報を更新
      if (IsWantToMove() && CanMoveTo(direction))
      {
        // 移動先の座標を算出
        var next = Coord + direction.ToVector(false);

        // 座標の更新
        DungeonManager.Instance.UpdatePlayerCoord(Coord, next);
        Coord = next;

        return Behavior.Move;
      }

      // 通常攻撃(RB2)
      if (InputManager.Instance.RB2.IsHold)
      {
        // 攻撃要求を設定
        this.actionRequest.Name = Status.Name;
        this.actionRequest.Pow = Status.Pow;
        this.actionRequest.Coord = Coord;
        this.actionRequest.Area = GetAttackCoords();

        return Behavior.Action;
      }

      // 遠距離攻撃(R)
      if (InputManager.Instance.R.IsDown)
      {
        var (isHit, pos) = SearchAttackTarget(Coord, Chip.Direction);

        this.item = ItemManager.Instance.CreateItemByAlias(Master.Item.Alias.Shuriken);
        this.item.SetCoord(Coord);
        this.item.SetThrow(pos);

        return Behavior.Action;
      }

      // メニュー(RB1)
      if (InputManager.Instance.RB1.IsDown)
      {
        return Behavior.Menu;
      }

      return Behavior.Thinking;
    }

    //-------------------------------------------------------------------------
    // Sceneから呼ばれるコールバック

    public override void OnSceneMoveEnter()
    {
      this.state.SetState(ActionPhase.Move);
      Status.UseEnergy();
    }

    public override void OnSceneActionEnter()
    {
      // アイテムを投げる
      if (this.item != null) {
        this.item.StartAction();
        this.item = null;
      } 
      
      else {
        StartAction();
      }

      Status.UseEnergy();
    }

    public override void OnSceneTurnEndEnter()
    {
      base.OnSceneTurnEndEnter();

      // 体力を微量回復
      Status.AddHP(Status.MaxHP * 0.015f);
    }

    //-------------------------------------------------------------------------
    // IActionableの実装

    /// <summary>
    /// ActionManagerにActorとTargetを登録して開始
    /// </summary>
    public override void StartAction()
    {
      var targets = EnemyManager.Instance.FindTarget(ActionRequest.Area);

      var AM = ActionManager.Instance;
      AM.SetActor(this);
      AM.AddTargets(targets);
      AM.StartAction();
    }

    public override void OnActionStartWhenActor()
    {
      this.state.SetState(ActionPhase.AttackStart);
    }

    public override void OnActionWhenTarget(IActionable actor)
    {
      this.state.SetState(ActionPhase.Damage);
    }

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

    private void MoveExit()
    {
      // 足元にアイテムがあるかチェック
      var item = ItemManager.Instance.Find(DungeonManager.Instance.PlayerCoord);
      if (item != null)
      {
        Debug.Log($"{item.Status.Name}の上に乗った。");
      }
    }

    //-------------------------------------------------------------------------
    // 通常攻撃 開始

    private void AttackStartEnter()
    {
      CameraManager.Instance.Lock();
      Chip.Attack(Define.SEC_PER_TURN, 0.4f);
    }

    private void AttackStartUpdate()
    {
      if (!Chip.IsIdle) return;
      this.state.SetState(ActionPhase.Idle);
    }

    private void AttackStartExit()
    {
      CameraManager.Instance.Unlock();
    }

    //-------------------------------------------------------------------------
    // ダメージ
    private void DamageEnter()
    {
      // 攻撃を受けていなければ痛がらない
      if (!actionResponse.IsAccepted) return;

      // ダメージがある場合は痛がる
      if (actionResponse.HasDamage)
      {
        Chip.Ouch(Define.SEC_PER_TURN * 2);
      }

      // ダメージがなければ待機
      else
      {
        Chip.Wait(Define.SEC_PER_TURN * 2);
      }
    }

    private void DamageUpdate()
    {
      if (Chip.IsIdle)
      {
        this.state.SetState(ActionPhase.Idle);
      }
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

    /// <summary>
    /// ダッシュしたがっている
    /// </summary>
    /// <returns></returns>
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

    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// 通常攻撃をした場合、攻撃対象となる座標リストを返す。
    /// </summary>
    public List<Vector2Int> GetAttackCoords()
    {
      var area = new List<Vector2Int>();

#if true
      // 前方三方向攻撃
      var dirs = Direction.Get3Way(Chip.Direction);

      foreach (var dir in dirs)
      {
        if (CanAttackTo(dir) == false) continue;

        area.Add(Coord + dir.ToVector(false));
      }
#else
      // 前方攻撃
      if (CanAttackTo(Chip.Direction))
      {
        area.Add(Coord + Chip.Direction.ToVector(false));
      }
#endif
      return area;
    }

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ

    public void DrawDebug()
    {
      GUILayout.Label($"Current Coord: ({this.Coord})");
      GUILayout.Label($"Dash Direction: ({this.dashDirection.value})");

      Status.DrawDebug();
      actionRequest.DrawDebug();
      actionResponse.DrawDebug();
    }

    //-------------------------------------------------------------------------
    // デバッグ用体力ゲージ表示(一時的なもの)
    private Texture2D tex1 = new Texture2D(1, 1);
    private Texture2D tex2 = new Texture2D(1, 1);

    public void OnGUI()
    {
      this.tex1.SetPixel(1, 1, new Color(1, 1, 1, 0.2f));
      this.tex2.SetPixel(1, 1, new Color(0, 1, 1, 0.5f));
      this.tex2.Apply();

      GUIStyle style1 = new GUIStyle();
      GUIStyle style2 = new GUIStyle();
      style1.normal.background = this.tex1;
      style2.normal.background = this.tex2;

      var width = 200 * Status.RateHP;

      GUI.Box(new Rect(10, 10, 200, 20), "", style1);
      GUI.Box(new Rect(10, 10, width, 20), "", style2);

      GUIStyle text = new GUIStyle();
      text.normal.textColor = Color.black;
      GUI.Label(new Rect(10, 30, 100, 20), $"HP:{Status.HP}/{Status.MaxHP}", text);
    }
#endif
  }

}


