using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

namespace Scene {

  /// <summary>
  /// ダンジョンシーン
  /// </summary>
  public class DungeonScene : SceneBase
  {
    public enum Phase
    {
      Load,
      CreatingStage,
      WaitingForInput,
      MovingPlayer,
    }

    private StateMachine<Phase> state;

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
      var system = new GameObject("System");

      SingletonManager.Instance
        .Setup("DungeonManager", system)
        .Setup("MapChipFactory", system)
        .Setup("PlayerManager" , system);

      this.state = new StateMachine<Phase>();

      this.state.Add(Phase.Load, null, LoadUpdate);
      this.state.Add(Phase.CreatingStage, CreateStageEnter);
      this.state.Add(Phase.WaitingForInput, null, WaitingForInputUpdate);
      this.state.Add(Phase.MovingPlayer, null, MovingPlayerUpdate);

      this.state.SetState(Phase.Load);
    }

    private void Update()
    {
      this.state.Update();
    }

    //-------------------------------------------------------------------------
    // ロード
    private void LoadUpdate()
    {
      this.state.SetState(Phase.CreatingStage);
    }

    //-------------------------------------------------------------------------
    // ダンジョン生成フェーズ

    private void CreateStageEnter()
    {
      // ダンジョン生成
      DungeonManager.Instance.CreateStage();

      // マップチップを生成
      DungeonManager.Instance.CreateMapChips();

      // プレイヤーを生成
      PlayerManager.Instance.CreatePlayer(DungeonManager.Instance.PlayerCoord);

      // カメラ設定
      CameraManager.Instance.SetDungeonMode(PlayerManager.Instance.PlayerObject);

      // 入力待ちフェーズへ
      this.state.SetState(Phase.WaitingForInput);

    }

    //-------------------------------------------------------------------------
    // 入力待ちフェーズ

    private void WaitingForInputUpdate()
    {
      // 方向キーを取得
      var direction = InputManager.Instance.GetDirectionKey();

      // 方向キーの入力がなければ継続
      if (direction == Direction8.Neutral) return;

      // プレイヤーの移動を試す
      var isMoved = PlayerManager.Instance.CheckAndMovePlayer(direction);

      // プレイヤー移動待ちフェーズへ
      if (isMoved) 
      {
        this.state.SetState(Phase.MovingPlayer);
      }
    }

    //-------------------------------------------------------------------------
    // プレイヤー移動待ちフェーズ
    
    private void MovingPlayerUpdate()
    {
      // プレイヤーがIdleになるまで待つ
      if (!PlayerManager.Instance.Player.IsIdle) return;

      this.state.SetState(Phase.WaitingForInput);
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
