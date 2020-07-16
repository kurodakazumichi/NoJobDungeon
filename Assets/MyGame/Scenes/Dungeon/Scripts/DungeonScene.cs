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
      TurnEnd,
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
    /// 起動処理
    /// </summary>
    protected override void Awake()
    {
      base.Awake();

      var system = new GameObject("System");

      SingletonManager.Instance
        .Setup(nameof(DungeonManager), system)
        .Setup(nameof(MapChipFactory), system)
        .Setup(nameof(PlayerManager), system)
        .Setup(nameof(FieldManager), system)
        .Setup(nameof(EnemyManager), system)
        .Setup(nameof(ItemManager), system)
        .Setup(nameof(HUD), system);

#if _DEBUG
      DebugMenuManager.Instance.RegisterMenu(DebugMenu.Page.Dungeon, DrawDebugMenu, nameof(DungeonScene));
#endif
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    protected override void Start()
    {
      this.state.Add(Phase.Load, LoadEnter, LoadUpdate, LoadExit);
      this.state.Add(Phase.CreateStage, CreateStageEnter);
      this.state.Add(Phase.PlayerThink, null, PlayerThinkUpdate);
      this.state.Add(Phase.Move, MoveEnter, MoveUpdate);
      this.state.Add(Phase.PlayerAttackStart, PlayerAttackStartEnter, PlayerAttackStartUpdate, PlayerAttackStartExit);
      this.state.Add(Phase.PlayerAttackEnd  , PlayerAttackEndEnter, PlayerAttackEndUpdate, PlayerAttackEndExit);
      this.state.Add(Phase.EnemyAttackStart, EnemyAttackStartEnter, EnemyAttackStartUpdate);
      this.state.Add(Phase.EnemyAttackEnd, EnemyAttackEndEnter, EnemyAttackEndUpdate);
      this.state.Add(Phase.TurnEnd, TurnEndEnter, TurnEndUpdate);

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

      // アイテムを生成
      ItemManager.Instance.CreateItems();

      // 敵を生成
      EnemyManager.Instance.CreateEnemies();

      // プレイヤーを生成
      PlayerManager.Instance.CreatePlayer(DungeonManager.Instance.PlayerCoord);

      // カメラをダンジョン設定にする
      CameraManager.Instance.SetDungeonSettings();
      CameraManager.Instance.SetTrackingMode(PlayerManager.Instance.PlayerObject);

      // マップの踏破情報を更新
      DungeonManager.Instance.UpdateClearFlags();

      // HUD生成
      HUD.Instance.Setup();

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
          EnemyManager.Instance.Think();

          // 移動フェーズへ
          this.state.SetState(Phase.Move);
          break;
        }

        // 通常攻撃
        case Player.Behavior.Attack:
        {
          // 敵に行動を考えるように命じる
          EnemyManager.Instance.Think();

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
      PlayerManager.Instance.DoMoveMotion();
      EnemyManager.Instance.DoMoveMotion();

      // 踏破情報更新
      DungeonManager.Instance.UpdateClearFlags();

      // ミニマップ更新
      HUD.Instance.UpdateMinimap();
    }

    private void MoveUpdate()
    {
      // 動いてるプレイヤーと敵がいる間は待機
      if (PlayerManager.Instance.HasnActivePlayer) return;
      if (EnemyManager.Instance.HasActiveEnemy) return;

      // プレイヤーがゴールにたどり着いてたら次のフロアへ
      if (DungeonManager.Instance.CanGoNextFloor)
      {
        DungeonManager.Instance.upFloor();
        this.state.SetState(Phase.CreateStage);
        return;
      }

      // 足元にアイテムがあるかチェック
      var item = ItemManager.Instance.Find(DungeonManager.Instance.PlayerCoord);
      if (item != null)
      {
        Debug.Log($"{item.Name}の上に乗った。");
      }

      // 動いてるやつらがいなくなったら次のフェーズへ
      this.state.SetState(Phase.EnemyAttackStart);
    }

    //-------------------------------------------------------------------------
    // プレイヤーの攻撃開始フェーズ

    private void PlayerAttackStartEnter()
    {
      // 攻撃対象を取得
      var area     = PlayerManager.Instance.AttackArea;
      var targets = EnemyManager.Instance.FindTarget(area);

      PlayerManager.Instance.Attack(targets);
      
      // プレイヤーは攻撃を、敵は痛がる動きをしてください
      PlayerManager.Instance.DoAttackMotion();
      EnemyManager.Instance.DoOuchMotion();

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
      EnemyManager.Instance.DoVanishMotion();
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
      // 敵の攻撃
      EnemyManager.Instance.Attack(PlayerManager.Instance.Attacker);
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
      PlayerManager.Instance.DoOuchMotion();
    }

    private void EnemyAttackEndUpdate()
    {
      // プレイヤーが痛がっている間は待機
      if (PlayerManager.Instance.HasnActivePlayer) return;

      // プレイヤーが死んじゃったらタイトル画面へ
      if (PlayerManager.Instance.IsPlayerDead)
      {
        Debug.Log("プレイヤー死んじゃった");
        CameraManager.Instance.SetFreeMode();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MyGame/Scenes/Title/TitleScene");
      }

      // まだ攻撃をする敵が残っている場合は敵の攻撃開始フェーズへ
      if (EnemyManager.Instance.HasAttacker)
      {
        this.state.SetState(Phase.EnemyAttackStart);
        return;
      }

      this.state.SetState(Phase.TurnEnd);
    }

    //-------------------------------------------------------------------------
    // ターンエンド
    private void TurnEndEnter()
    {
      PlayerManager.Instance.UpdatePlayer();
    }

    private void TurnEndUpdate()
    {
      this.state.SetState(Phase.PlayerThink);
    }

#if _DEBUG
    public void DrawDebugMenu(DebugMenu.MenuWindow menuWindow)
    {
      GUILayout.Label("Dungeon Scene Functions");

      if (GUILayout.Button("Remake"))
      {
        this.state.SetState(Phase.CreateStage);
      }
    }
#endif

  }
}
