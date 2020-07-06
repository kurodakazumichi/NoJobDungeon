using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Singleton;

namespace MyGame.Scene {

  /// <summary>
  /// ダンジョンシーン
  /// </summary>
  public class DungeonScene : SceneBase
  {
    public enum Phase
    {
      Load,
      CreatingStage,
      PlayingStage,
    }

    private StateMachine<Phase> state;

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
      var system = new GameObject("System");

      SingletonManager.Instance
        .Setup(nameof(DungeonManager), system)
        .Setup(nameof(MapChipFactory), system)
        .Setup(nameof(PlayerManager) , system)
        .Setup(nameof(FieldManager)  , system);

      this.state = new StateMachine<Phase>();

      this.state.Add(Phase.Load, LoadEnter, LoadUpdate, LoadExit);
      this.state.Add(Phase.CreatingStage, CreateStageEnter);
      this.state.Add(Phase.PlayingStage, PlayingStageEnter, PlayingStageUpdate);

      this.state.SetState(Phase.Load);
    }

    private void Update()
    {
      this.state.Update();
    }

    //-------------------------------------------------------------------------
    // ロード
    private void LoadEnter()
    {
      ResourceManager.Instance.LoadAsync(ResourceManager.DungeionLabel); 
    }

    private void LoadUpdate()
    {
      if (ResourceManager.Instance.IsLoading)
      {
        return;
      }

      this.state.SetState(Phase.CreatingStage);
    }

    private void LoadExit()
    {
      FieldManager.Instance.ReserveFields();
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

      // カメラをダンジョン設定にする
      CameraManager.Instance.SetDungeonSettings();

      // 入力待ちフェーズへ
      this.state.SetState(Phase.PlayingStage);

    }

    //-------------------------------------------------------------------------
    // ステージプレイ中
    private void PlayingStageEnter()
    {
      PlayerManager.Instance.StartPlayer();
    }

    private void PlayingStageUpdate ()
    {
      PlayerManager.Instance.UpdatePlayer();
    }

#if UNITY_EDITOR
    [SerializeField]
    private bool showDebug = false;

    private void OnGUI()
    {
      if (!this.showDebug) return;
      if (GUI.Button(new Rect(10, 10, 100, 20), "Create"))
      {
        this.state.SetState(Phase.CreatingStage);
      }
    }
#endif
  }
}
