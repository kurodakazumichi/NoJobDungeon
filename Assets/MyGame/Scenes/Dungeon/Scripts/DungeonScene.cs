using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon {

  /// <summary>
  /// ダンジョンシーン
  /// </summary>
  public class DungeonScene : SceneBase
  {
    public enum Phase
    {
      Load,
      CreateStage,
      PlayerThink,
      Move,
      PlayerAttackStart,
      PlayerAttackEnd,
    }

    private StateMachine<Phase> state;

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

      this.state = new StateMachine<Phase>();

      this.state.Add(Phase.Load, LoadEnter, LoadUpdate, LoadExit);
      this.state.Add(Phase.CreateStage, CreateStageEnter);
      this.state.Add(Phase.PlayerThink, null, PlayerThinkUpdate);
      this.state.Add(Phase.Move, MoveEnter, MoveUpdate);
      this.state.Add(Phase.PlayerAttackStart, PlayerAttackStartEnter, PlayerAttackStartUpdate, PlayerAttackStartExit);
      this.state.Add(Phase.PlayerAttackEnd  , PlayerAttackEndEnter, PlayerAttackEndUpdate);

      this.state.SetState(Phase.Load);
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
          // 敵に移動について考えるように命じる
          EnemyManager.Instance.OrderToThinkAboutMoving();

          // 移動フェーズへ
          this.state.SetState(Phase.Move);
          break;
        }

        // 通常攻撃
        case Player.Behavior.Attack:
        {
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
      if (PlayerManager.Instance.HasOnMovePlayer) return;
      if (EnemyManager.Instance.HasOnMoveEnemy) return;

      // 動いてるやつらがいなくなったら次のフェーズへ
      // TODO: 本来は敵の攻撃フェーズへ遷移
      this.state.SetState(Phase.PlayerThink);
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
      if (PlayerManager.Instance.HasOnMovePlayer) return;
      if (EnemyManager.Instance.HasOnMoveEnemy) return;

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

    }

    private void PlayerAttackEndUpdate()
    {
      this.state.SetState(Phase.PlayerThink);
    }

#if UNITY_EDITOR
    [SerializeField]
    private bool showDebug = false;

    private void OnGUI()
    {
      if (!this.showDebug) return;
      if (GUI.Button(new Rect(10, 10, 100, 20), "Create"))
      {
        this.state.SetState(Phase.CreateStage);
      }
    }
#endif
  }
}
