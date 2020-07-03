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

      ToCreatingStagePhase();
    }

    //-------------------------------------------------------------------------
    // ダンジョン生成フェーズ

    private void ToCreatingStagePhase()
    {
      System.Action start = () => {
        // ダンジョン生成
        DungeonManager.Instance.CreateStage();

        // マップチップを生成
        DungeonManager.Instance.CreateMapChips();

        // プレイヤーを生成
        PlayerManager.Instance.CreatePlayer(DungeonManager.Instance.PlayerCoord);

        // カメラ設定
        CameraManager.Instance.SetDungeonMode(PlayerManager.Instance.PlayerObject);

        // 入力待ちフェーズへ
        ToWaitingForInputPhase();      
      };

      SetFunc(start, null);
    }

    //-------------------------------------------------------------------------
    // 入力待ちフェーズ

    private void ToWaitingForInputPhase()
    {
      SetFunc(null, UpdateWaitingForInput);
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
      ToMovingPlayerPhase(direction);
      return false;
    }

    //-------------------------------------------------------------------------
    // プレイヤー移動待ちフェーズ
    
    private void ToMovingPlayerPhase(Direction8 direction)
    {
      System.Action start = () => {
        PlayerManager.Instance.MovePlayer(direction);
      };

      System.Func<bool> update = () => {
        // プレイヤーがIdleになるまで待つ
        if (!PlayerManager.Instance.Player.IsIdle) return true;

        ToWaitingForInputPhase();
        return false;
      };

      SetFunc(start, update);
    }
  
  }

}
