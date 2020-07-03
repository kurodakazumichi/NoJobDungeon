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

      SetPhaseToCreatingStage();
    }

    //-------------------------------------------------------------------------
    // ダンジョン生成フェーズ

    private void SetPhaseToCreatingStage()
    {
      SetFunc(UpdateCreateStage);
    }

    private bool UpdateCreateStage()
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
      SetPhaseToWaitingForInput();
      return false;
    }

    //-------------------------------------------------------------------------
    // 入力待ちフェーズ

    private void SetPhaseToWaitingForInput()
    {
      SetFunc(UpdateWaitingForInput);
    }

    private bool UpdateWaitingForInput()
    {
      // 方向キーを取得
      var direction = InputManager.Instance.GetDirectionKey();

      // 方向キーの入力がなければ継続
      if (direction == Direction8.Neutral) return true;

      // 方向キーの入力があった場合
      PlayerManager.Instance.SetPlayerDirection(direction);

      // 指定した方向にプレイヤーが動けるかどうかをチェック
      if (!PlayerManager.Instance.ChecksPlayerMovable(direction)) return true;

      // 入力された方向にプレイヤーを動かす
      SetPhaseToMovingPlayer(direction);
      return false;
    }

    //-------------------------------------------------------------------------
    // プレイヤー移動待ちフェーズ
    
    private void SetPhaseToMovingPlayer(Direction8 direction)
    {
      PlayerManager.Instance.MovePlayer(direction);
      SetFunc(UpdateMovingPlayer);
    }

    private bool UpdateMovingPlayer()
    {
      // プレイヤーがIdleになるまで待つ
      if (!PlayerManager.Instance.Player.IsIdle) return true;

      SetPhaseToWaitingForInput();
      return false;
    }
    
  }

}
