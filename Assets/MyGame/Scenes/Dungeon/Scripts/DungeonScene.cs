using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon {

  /// <summary>
  /// ダンジョンシーン
  /// </summary>
  public class DungeonScene : SceneBase
  {
    /// <summary>
    /// シーンの流れを定義
    /// </summary>
    public enum Phase
    {
      Load,
      CreateStage,
      PlayerThink,
      Move,
      PlayerAttackStart,
      PlayerAttackEnd,
      EnemyAttackStart,
      EnemyAttackEnd,
    }

    //-------------------------------------------------------------------------
    // メンバー

    /// <summary>
    /// ステートマシン
    /// </summary>
    private StateMachine<Phase> state = new StateMachine<Phase>();

    //-------------------------------------------------------------------------
    // 主要メソッド

    /// <summary>
    /// 開始処理
    /// </summary>
    protected override void Start()
    {
      var system = new GameObject("System");

      SingletonManager.Instance
        .Setup(nameof(DungeonManager), system)
        .Setup(nameof(MapChipFactory), system)
        .Setup(nameof(PlayerManager) , system)
        .Setup(nameof(FieldManager)  , system)
        .Setup(nameof(EnemyManager)  , system);

      this.state.Add(Phase.Load, LoadEnter, LoadUpdate, LoadExit);
      this.state.Add(Phase.CreateStage, CreateStageEnter);
      this.state.Add(Phase.PlayerThink, null, PlayerThinkUpdate);
      this.state.Add(Phase.Move, MoveEnter, MoveUpdate);
      this.state.Add(Phase.PlayerAttackStart, PlayerAttackStartEnter, PlayerAttackStartUpdate, PlayerAttackStartExit);
      this.state.Add(Phase.PlayerAttackEnd  , PlayerAttackEndEnter, PlayerAttackEndUpdate, PlayerAttackEndExit);
      this.state.Add(Phase.EnemyAttackStart, EnemyAttackStartEnter, EnemyAttackStartUpdate);
      this.state.Add(Phase.EnemyAttackEnd, EnemyAttackEndEnter, EnemyAttackEndUpdate);

      this.state.SetState(Phase.Load);

#if _DEBUG
      DebugMenuManager.Instance.RegisterMenu( DebugMenu.Page.Dungeon, DrawDebugMenu, nameof(DungeonScene) );
#endif
    }

    protected override void Update()
    {
      this.state.Update();
    }

    //-------------------------------------------------------------------------
    // ロード
    private void LoadEnter()
    {
      //ResourceManager.Instance.LoadAsync(ResourceManager.DungeionLabel); 
    }

    private void LoadUpdate()
    {
      //if (ResourceManager.Instance.IsLoading)
      //{
      //  return;
      //}

      this.state.SetState(Phase.CreateStage);
    }

    private void LoadExit()
    {
    }

    //-------------------------------------------------------------------------
    // ダンジョン生成フェーズ

    private void CreateStageEnter()
    {
      // ダンジョン生成
      DungeonManager.Instance.CreateStage();

      // マップチップを生成
      FieldManager.Instance.CreateFields();

      // プレイヤーを生成
      PlayerManager.Instance.CreatePlayer(DungeonManager.Instance.PlayerCoord);

      // 敵を生成
      EnemyManager.Instance.CreateEnemies();

      // カメラをダンジョン設定にする
      CameraManager.Instance.SetDungeonSettings();
      CameraManager.Instance.SetTrackingMode(PlayerManager.Instance.PlayerObject);

      // 入力待ちフェーズへ
      this.state.SetState(Phase.PlayerThink);

    }


    //-------------------------------------------------------------------------
    // プレイヤー思考フェーズ

    private void PlayerThinkUpdate()
    {
      // プレイヤーの行動を監視
      var behavior = PlayerManager.Instance.MonitorPlayerThoughs();

      switch(behavior)
      {
        // 移動：このケースに来た時
        // ダンジョン情報は既にプレイヤーが移動した後の状態になっている。
        case Player.Behavior.Move:
        {
          // 敵に行動を考えるように命じる
          EnemyManager.Instance.OrderToThink();

          // 移動フェーズへ
          this.state.SetState(Phase.Move);
          break;
        }

        // 通常攻撃
        case Player.Behavior.Attack:
        {
          // 敵に行動を考えるように命じる
          EnemyManager.Instance.OrderToThink();

          // プレイヤー攻撃開始フェーズへ
          this.state.SetState(Phase.PlayerAttackStart);
          break;
        }
      }
    }

    //-------------------------------------------------------------------------
    // 移動フェーズ

    private void MoveEnter()
    {
      // プレイヤーと敵に動けと命じる
      PlayerManager.Instance.OrderToMove();
      EnemyManager.Instance.OrderToMove();
    }

    private void MoveUpdate()
    {
      // 動いてるプレイヤーと敵がいる間は待機
      if (PlayerManager.Instance.HasnActivePlayer) return;
      if (EnemyManager.Instance.HasActiveEnemy) return;

      // 動いてるやつらがいなくなったら次のフェーズへ
      this.state.SetState(Phase.EnemyAttackStart);
    }

    //-------------------------------------------------------------------------
    // プレイヤーの攻撃開始フェーズ

    private void PlayerAttackStartEnter()
    {
      // 攻撃対象となる座標一覧をください
      var targets = PlayerManager.Instance.AttackTargets;

      // プレイヤーのアタッカーとしての情報下さい
      var attacker = PlayerManager.Instance.Attacker;

      // 敵に攻撃を加える
      EnemyManager.Instance.AttackEnemies(attacker, targets);

      // プレイヤーは攻撃を、敵は痛がる動きをしてください
      PlayerManager.Instance.OrderToAttack();
      EnemyManager.Instance.OrderToOuch();

      // プレイヤーの動きに合わせてカメラが動くとガクガクするので
      // プレイヤー攻撃中はカメラが動かないようにロック(Exitと解除するのを忘れずに)
      CameraManager.Instance.Lock();
    }

    private void PlayerAttackStartUpdate()
    {
      if (PlayerManager.Instance.HasnActivePlayer) return;
      if (EnemyManager.Instance.HasActiveEnemy) return;

      this.state.SetState(Phase.PlayerAttackEnd);
    }

    private void PlayerAttackStartExit()
    {
      CameraManager.Instance.Unlock();
    }

    //-------------------------------------------------------------------------
    // プレイヤーの攻撃終了フェーズ

    private void PlayerAttackEndEnter()
    {
      // 死んだ敵は消滅して下さい
      EnemyManager.Instance.OrderToVanish();
    }

    private void PlayerAttackEndUpdate()
    {
      if (EnemyManager.Instance.HasActiveEnemy) return;
      this.state.SetState(Phase.Move);
    }

    private void PlayerAttackEndExit()
    {
      // 死んだ敵を破棄します
      EnemyManager.Instance.DestoryDeadEnemies();
    }

    //-------------------------------------------------------------------------
    // 敵の攻撃開始フェーズ
    // 敵の攻撃は一体ずつ処理していく。

    private void EnemyAttackStartEnter()
    {
      // 敵に攻撃の動きをするように命じるとともに、攻撃した敵の情報を取得
      IAttackable attacker = EnemyManager.Instance.OrderToAttack();

      // プレイヤーに対して攻撃を行う
      PlayerManager.Instance.AttackPlayer(attacker);
    }

    private void EnemyAttackStartUpdate()
    {
      // 動いてる敵がいなくなったら攻撃終了フェーズへ
      if (EnemyManager.Instance.HasActiveEnemy) return;

      this.state.SetState(Phase.EnemyAttackEnd);
    }

    //-------------------------------------------------------------------------
    // 敵の攻撃終了フェーズ

    private void EnemyAttackEndEnter()
    {
      // プレイヤーに痛がるよう命じる
      PlayerManager.Instance.OrderToOuch();
    }

    private void EnemyAttackEndUpdate()
    {
      // プレイヤーが痛がっている間は待機
      if (PlayerManager.Instance.HasnActivePlayer) return;

      // まだ攻撃をする敵が残っている場合は敵の攻撃開始フェーズへ
      if (EnemyManager.Instance.HasAttacker)
      {
        this.state.SetState(Phase.EnemyAttackStart);
        return;
      }

      this.state.SetState(Phase.PlayerThink);
    }

#if _DEBUG
    public void DrawDebugMenu(DebugMenu.MenuWindow menuWindow)
    {
      if (GUILayout.Button("Create"))
      {
        this.state.SetState(Phase.CreateStage);
      }
    }
#endif

  }
}
