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

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// プレイヤーを作成し、指定された座標に置く
    /// </summary>
    public void CreatePlayer(Vector2Int coord)
    {
      if (this.player == null)
      {
        this.player = new Player(coord);
      }
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
      this.player.Move();
    }

    /// <summary>
    /// プレイヤーに攻撃するように指示を出す
    /// </summary>
    public void OrderToAttack()
    {
      this.player.Attack();
    }

#if UNITY_EDITOR

    [SerializeField]
    private bool showDebug = false;

    private void OnGUI()
    {
      if (!showDebug) return;

      this.player.OnGUI();
    }

#endif

  }

}
