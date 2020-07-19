using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon {

  /// <summary>
  /// ダンジョンシーン
  /// </summary>
  public class DungeonScene : SceneBase, IDebuggeable
  {
    /// <summary>
    /// シーンの流れを定義
    /// </summary>
    public enum Phase
    {
      Idle,
      Load,
      CreateStage,
      PlayerThink,
      PlayerAction,
      Move,
      EnemyAction,
      TurnEnd,
      GameOver,
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
        .Setup(nameof(ActionManager), system)
        .Setup(nameof(HUD), system);
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    protected override void Start()
    {
      this.state.Add(Phase.Idle);
      this.state.Add(Phase.Load, LoadEnter, LoadUpdate, LoadExit);
      this.state.Add(Phase.CreateStage, CreateStageEnter, CreateStageUpdate);
      this.state.Add(Phase.PlayerThink, null, PlayerThinkUpdate);
      this.state.Add(Phase.PlayerAction, PlayerActionEnter, PlayerActionUpdate, PlayerActionExit);
      this.state.Add(Phase.Move, MoveEnter, MoveUpdate, MoveExit);
      this.state.Add(Phase.EnemyAction, EnemyActionEnter, EnemyActionUpdate, EnemyActionExit);
      this.state.Add(Phase.TurnEnd, TurnEndEnter, TurnEndUpdate);
      this.state.Add(Phase.GameOver, GameOverEnter, GameOverUpdate);
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
    }

    private void CreateStageUpdate()
    {
      // 入力待ちフェーズへ
      this.state.SetState(Phase.PlayerThink);
    }


    //-------------------------------------------------------------------------
    // プレイヤー思考フェーズ

    private void PlayerThinkUpdate()
    {
      // プレイヤーの行動を監視
      var behavior = PlayerManager.Instance.Think();

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

        // 行動
        case Player.Behavior.Action:
        {
          // プレイヤー行動フェーズへ
          this.state.SetState(Phase.PlayerAction);
          break;
        }
      }
    }

    //-------------------------------------------------------------------------
    // 移動フェーズ

    private void MoveEnter()
    {
      PlayerManager.Instance.OnSceneMoveEnter();
      EnemyManager.Instance.OnSceneMoveEnter();
    }

    private void MoveUpdate()
    {
      PlayerManager.Instance.UpdatePlayer();
      EnemyManager.Instance.UpdateEnemies();

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

      // 動いてるやつらがいなくなったら次のフェーズへ
      this.state.SetState(Phase.EnemyAction);
    }

    private void MoveExit()
    {
      PlayerManager.Instance.OnSceneMoveExit();
      EnemyManager.Instance.OnSceneMoveExit();
      
      // 踏破情報更新
      DungeonManager.Instance.UpdateClearFlags();

      // ミニマップ更新
      HUD.Instance.UpdateMinimap();
    }

    //-------------------------------------------------------------------------
    // プレイヤーの行動フェーズ

    private void PlayerActionEnter()
    {
      PlayerManager.Instance.OnSceneActionEnter();
    }

    private void PlayerActionUpdate()
    {
      UpdateSingletonsWhenAction();

      if (!ActionManager.Instance.IsIdle) return;

      // プレイヤーが死亡
      if (PlayerManager.Instance.IsPlayerDead)
      {
        this.state.SetState(Phase.GameOver);
        return;
      }

      this.state.SetState(Phase.Move);
    }

    private void PlayerActionExit()
    {
      EnemyManager.Instance.DestoryDeadEnemies();

      // 敵に行動を考えるように命じる
      EnemyManager.Instance.Think();

      PlayerManager.Instance.OnSceneActionExit();
    }

    //-------------------------------------------------------------------------
    // 敵の行動フェーズ

    private void EnemyActionEnter()
    {
      EnemyManager.Instance.OnSceneActionEnter();
    }

    private void EnemyActionUpdate()
    {
      UpdateSingletonsWhenAction();

      // Action実行中は待機
      if (!ActionManager.Instance.IsIdle) return;

      // プレイヤーが死亡
      if (PlayerManager.Instance.IsPlayerDead)
      {
        this.state.SetState(Phase.GameOver);
        return;
      }

      // Actorが残っている場合は再びEnemyActionへ
      if (EnemyManager.Instance.HasActor)
      {
        this.state.SetState(Phase.EnemyAction);
        return;
      }

      this.state.SetState(Phase.TurnEnd);
    }

    private void EnemyActionExit()
    {
      EnemyManager.Instance.DestoryDeadEnemies();
      EnemyManager.Instance.OnSceneActionExit();
    }

    //-------------------------------------------------------------------------
    // ターンエンド
    private void TurnEndEnter()
    {
      PlayerManager.Instance.OnSceneTurnEndEnter();
      EnemyManager.Instance.OnSceneTurnEndEnter();
    }

    private void TurnEndUpdate()
    {
      this.state.SetState(Phase.PlayerThink);
    }

    //-------------------------------------------------------------------------
    // ゲームオーバー
    
    private void GameOverEnter()
    {
      Debug.Log("プレイヤー死んじゃった");
      CameraManager.Instance.SetFreeMode();
      UnityEngine.SceneManagement.SceneManager.LoadScene("MyGame/Scenes/Title/TitleScene");
    }

    private void GameOverUpdate()
    {
      this.state.SetState(Phase.Idle);
    }

    //-------------------------------------------------------------------------
    // その他

    /// <summary>
    /// Player、EnemyのAction中にUpdateする必要のあるシングルトンのUpdateを呼ぶ
    /// </summary>
    private void UpdateSingletonsWhenAction()
    {
      PlayerManager.Instance.UpdatePlayer();
      EnemyManager.Instance.UpdateEnemies();
      ActionManager.Instance.UpdateAction();
    }


#if _DEBUG
    void IDebuggeable.Draw(MyDebug.Window window)
    {
      GUILayout.Label("Dungeon Scene State");
      GUILayout.Label($"State:{this.state.StateKey}");

      GUILayout.Label("Dungeon Scene Functions");

      if (GUILayout.Button("Remake"))
      {
        this.state.SetState(Phase.CreateStage);
      }
    }
#endif

  }
}
