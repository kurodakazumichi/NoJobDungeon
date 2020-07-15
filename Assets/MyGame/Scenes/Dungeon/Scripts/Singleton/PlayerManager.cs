using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{

  /// <summary>
  /// プレイヤーマネージャー
  /// </summary>
  public class PlayerManager : SingletonMonobehaviour<PlayerManager>
  {
    //-------------------------------------------------------------------------
    // メンバ変数

    /// <summary>
    /// プレイヤー
    /// </summary>
    private Player player = null;

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// プレイヤーチップのオブジェクト
    /// </summary>
    public GameObject PlayerObject => ((this.player != null)? this.player.PlayerObject:null);

    /// <summary>
    /// Activeな(何かしら活動している)プレイヤーがいるかどうか
    /// </summary>
    /// <returns></returns>
    public bool HasnActivePlayer => ((this.player != null && !this.player.IsIdle));

    /// <summary>
    /// 通常攻撃の対象となる座標リスト
    /// </summary>
    public List<Vector2Int> AttackTargets => (this.player.GetAttackTargets());

    /// <summary>
    /// プレイヤーのアタッカーとしての能力
    /// </summary>
    public IAttackable Attacker => (this.player);

    /// <summary>
    /// プレイヤーは死んでいます
    /// </summary>
    public bool IsPlayerDead => (this.player.Status.IsDead);

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// プレイヤーを作成し、指定された座標に置く
    /// 既にプレイヤーが存在していたらリセットする
    /// </summary>
    public void CreatePlayer(Vector2Int coord)
    {
      if (this.player == null)
      {
        this.player = new Player(coord);
      }

      else
      {
        this.player.Reset(coord);
      }
    }

    /// <summary>
    /// １ターンに１度だけ呼ぶ更新処理
    /// </summary>
    public void UpdatePlayer()
    {
      if (this.player != null) this.player.Update();
    }

    /// <summary>
    /// プレイヤーの思考を監視する
    /// </summary>
    public Player.Behavior MonitorPlayerThoughs()
    {
      if (this.player == null)
      {
        return Player.Behavior.Thinking;
      }

      return this.player.Think();
    }

    /// <summary>
    /// プレイヤーに移動するように指示をだす
    /// </summary>
    public void OrderToMove()
    {
      this.player.DoMoveMotion();
    }

    /// <summary>
    /// プレイヤーに攻撃するように指示を出す
    /// </summary>
    public void OrderToAttack()
    {
      this.player.DoAttackMotion();
    }


    /// <summary>
    /// プレイヤーが攻撃を受けていたら「いてぇっ！」って演出するように指示を出す
    /// </summary>
    public void OrderToOuch()
    {
      this.player.DoOuchMotion();
    }

    /// <summary>
    /// 攻撃をうける
    /// </summary>
    public void AttackPlayer(IAttackable attacker)
    {
      if (attacker != null)
      {
        this.player.AcceptAttack(attacker);
      }
    }

    //-------------------------------------------------------------------------
    // Protected Method

    protected override void Awake()
    {
      base.Awake();
#if _DEBUG
      DebugMenuManager.Instance.RegisterMenu(DebugMenu.Page.Player, DrawDebugMenu, nameof(PlayerManager));
#endif
    }

#if _DEBUG
    private void DrawDebugMenu(DebugMenu.MenuWindow menuWindow)
    {
      this.player.DrawDebugMenu();
    }

    private void OnGUI()
    {
      if (this.player != null)
      {
        this.player.OnGUI();
      }
    }
#endif

  }

}
