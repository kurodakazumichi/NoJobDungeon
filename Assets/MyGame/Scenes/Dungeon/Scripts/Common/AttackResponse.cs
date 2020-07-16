using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// 攻撃後の情報を持っているクラス
  /// </summary>
  public class AttackResponse
  {
    /// <summary>
    /// 攻撃された相手の名前
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 攻撃を受けたフラグ
    /// </summary>
    public bool IsAccepted { get;  set; } = false;

    /// <summary>
    /// 攻撃が当たったフラグ
    /// </summary>
    public bool IsHit { get; set; } = false;

    /// <summary>
    /// 実際に与えたダメージ
    /// </summary>
    public int Damage { get; set; } = 0;

    /// <summary>
    /// ダメージがあるか
    /// </summary>
    public bool HasDamage => (0 < Damage);

    /// <summary>
    /// 値をコピーする
    /// </summary>
    public void Copy(AttackResponse res)
    {
      Name       = res.Name;
      IsAccepted = res.IsAccepted;
      IsHit      = res.IsHit;
      Damage     = res.Damage;
    }

    /// <summary>
    /// リセット
    /// </summary>
    public void Reset()
    {
      Name       = "";
      IsAccepted = false;
      IsHit      = false;
      Damage     = 0;
    }
  }
}