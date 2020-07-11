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
      var behavior = PlayerManager.Instance.monitorPlayerThoughs();

      switch(behavior)
      {
        case Player.Behavior.Move: 
          this.state.SetState(Phase.Move); 
          // TODO: 敵の移動先を決定する
          break;
      }


    }

    //-------------------------------------------------------------------------
    // 移動フェーズ

    private void MoveEnter()
    {
      PlayerManager.Instance.orderToMove();
    }

    private void MoveUpdate()
    {
      // 移動中
      if (PlayerManager.Instance.hasOnMovePlayer) return;

      // 移動完了
      // TODO: 本来は敵の攻撃フェーズへ遷移
      this.state.SetState(Phase.PlayerThink);
    }

    //-------------------------------------------------------------------------
    // ステージプレイ中
    private void PlayingStageEnter()
    {
      //PlayerManager.Instance.StartPlayer();
      EnemyManager.Instance.StartEnemies();
    }

    private void PlayingStageUpdate ()
    {
      //PlayerManager.Instance.UpdatePlayer();
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
