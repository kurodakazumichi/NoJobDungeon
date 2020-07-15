﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class Status
  {
    //-------------------------------------------------------------------------
    // Member

    /// <summary>
    /// HP
    /// </summary>
    private LimitedFloat hp = new LimitedFloat();

    /// <summary>
    /// 力
    /// </summary>
    private LimitedFloat pow = new LimitedFloat();

    /// <summary>
    /// 防御力
    /// </summary>
    private LimitedFloat def = new LimitedFloat();

    /// <summary>
    /// 攻撃を受けたフラグ
    /// </summary>
    private bool isAcceptedAttack = false;

    /// <summary>
    /// 攻撃が当たったフラグ
    /// </summary>
    private bool isHit = false;

    /// <summary>
    /// 実際に受けたダメージ
    /// </summary>
    private float acceptedDamage = 0;

    //-------------------------------------------------------------------------
    // コンストラクタ・セットアップ・リセット

    /// <summary>
    /// ステータス生成に必要なもの
    /// </summary>
    public class Props
    {
      public Props(float hp, float pow, float def)
      {
        HP = hp;
        Pow = pow;
        Def = def;
      }

      public float HP = 0;
      public float Pow = 0;
      public float Def = 0;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Status (Props props)
    {
      this.Setup(props);
    }

    /// <summary>
    /// セットアップ
    /// </summary>
    public void Setup(Props props)
    {
      this.hp.Setup(props.HP, props.HP);
      this.pow.Setup(props.Pow, props.Pow);
      this.def.Setup(props.Def, props.Def);
      this.isHit = false;
      this.isAcceptedAttack = false;
    }

    /// <summary>
    /// リセット
    /// </summary>
    public void Reset()
    {
      this.isHit          = false;
      this.isAcceptedAttack = false;
      this.acceptedDamage   = 0;
    }

    //-------------------------------------------------------------------------
    // Public Properity

    public IReadOnlyLimitedFloat HP => (this.hp);
    public IReadOnlyLimitedFloat Pow => (this.pow);
    public IReadOnlyLimitedFloat Def => (this.def);
    public bool IsAcceptedAttack => (this.isAcceptedAttack);
    public bool IsHit => (this.isHit);
    public bool IsDead => (this.hp.IsEmpty);
    public float AcceptedDamage => (this.acceptedDamage);
    public bool HasDamage => (0 < this.acceptedDamage);

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// 攻撃を受ける
    /// </summary>
    public void AcceptAttack(Status status)
    {
      this.isAcceptedAttack = true;

      // 攻撃は90パーの確立で当たる
      this.isHit = Random.Range(0f, 1f) <= Define.HIT_RATE;
      
      // 攻撃があたった場合はダメージ計算
      if (isHit)
      {
        this.acceptedDamage = Mathf.Max(0, status.Pow.Now - Def.Now);
        this.hp.Now -= acceptedDamage;
      }
    }

  }
}