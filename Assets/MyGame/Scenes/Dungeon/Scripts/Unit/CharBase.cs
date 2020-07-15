using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// キャラクターのベースクラス
  /// </summary>
  public abstract class CharBase: IAttackable
  {
    //-------------------------------------------------------------------------
    // コンストラクタ
    public CharBase()
    {

    }

    //-------------------------------------------------------------------------
    // Member

    //-------------------------------------------------------------------------
    // Properity

    /// <summary>
    /// キャラクターチップ
    /// </summary>
    protected CharChip Chip { get; set; } = null;

    /// <summary>
    /// ステータス
    /// </summary>
    public Status Status { get; protected set; } = null;

    /// <summary>
    /// 座標
    /// </summary>
    public Vector2Int Coord { get; set; } = Vector2Int.zero;

    /// <summary>
    /// アイドル状態です
    /// </summary>
    virtual public bool IsIdle
    {
      get
      {
        if (Chip == null)
        {
          return true;
        }

        return Chip.IsIdle;
      }
    }

    //-------------------------------------------------------------------------
    // Method

    /// <summary>
    /// 攻撃をする
    /// </summary>
    virtual public void Attack(IAttackable target)
    {
      target.AcceptAttack(this);
    }

    /// <summary>
    /// 攻撃を受ける
    /// </summary>
    virtual public void AcceptAttack(IAttackable attacker)
    {
      // 攻撃を受ける
      Status.AcceptAttack(attacker.Status);

      // 攻撃してきた奴の方を向く
      Chip.Direction = Direction.LookAt(Coord, attacker.Coord);
    }

  }
}