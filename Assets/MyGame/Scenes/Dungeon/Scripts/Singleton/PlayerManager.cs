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
    /// プレイヤーが攻撃しようと狙っている座標
    /// </summary>
    public List<Vector2Int> AttackArea => (this.player.AttackRequest.Area);

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
    /// 移動モーションを行う
    /// </summary>
    public void DoMoveMotion()
    {
      this.player.DoMoveMotion();
    }

    /// <summary>
    /// 攻撃モーションを行う
    /// </summary>
    public void DoAttackMotion()
    {
      this.player.DoAttackMotion();
    }


    /// <summary>
    /// 「いてぇっ！」ってモーションを行う
    /// </summary>
    public void DoOuchMotion()
    {
      this.player.DoOuchMotion();
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    public void Attack(List<IAttackable> targets)
    {
      foreach(var target in targets)
      {
        this.player.Attack(target);
      }
    }

    public void AttackEndEnter()
    {
      if (this.player != null)
      {
        this.player.OnAttackEndEnter();
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
