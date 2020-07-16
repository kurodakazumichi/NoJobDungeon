using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class Status
  {
    //-------------------------------------------------------------------------
    // Member

    /// <summary>
    /// 名前
    /// </summary>
    private string name = "";

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

    //-------------------------------------------------------------------------
    // コンストラクタ・セットアップ・リセット

    /// <summary>
    /// ステータス生成に必要なもの
    /// </summary>
    public class Props
    {
      public Props(string name, float hp, float pow, float def)
      {
        Name = name;
        HP = hp;
        Pow = pow;
        Def = def;
      }

      public string Name = "";
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
      this.name = props.Name;
      this.hp.Setup(props.HP, props.HP);
      this.pow.Setup(props.Pow, props.Pow);
      this.def.Setup(props.Def, props.Def);
    }

    //-------------------------------------------------------------------------
    // Public Properity

    public int HP => ((int)this.hp.Now);
    public int MaxHP => ((int)this.hp.Max);
    public float RateHP => ((float)HP/MaxHP);
    public int Pow => ((int)this.pow.Now);
    public int Def => ((int)this.def.Now);
    public bool IsDead => (((int)this.hp.Now < 1));
    public string Name => (this.name);

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// HPを追加
    /// </summary>
    /// <param name="num"></param>
    public void AddHP(float num)
    {
      this.hp.Now += num;
    }

    /// <summary>
    /// 力を追加
    /// </summary>
    public void AddPow(float num)
    {
      this.pow.Now += num;
    }

    /// <summary>
    /// 攻撃を受ける
    /// </summary>
    public AttackResponse AcceptAttack(AttackRequest req)
    {
      var res = new AttackResponse();
      res.Name = Name;
      res.IsAccepted = true;

      res.IsHit = Random.Range(0, 1f) <= Define.HIT_RATE;

      if (res.IsHit)
      {
        res.Damage = (int)(Mathf.Max(0, req.Pow - def.Now));
        this.hp.Now -= res.Damage;
      }

      // TODO:何らかの形でこの情報を外へ出す
      if (res.IsHit)
      {
        if (0 < res.Damage)
        {
          Debug.Log($"{req.Name}は{Name}に{res.Damage}ダメージを与えた。");
        }
        else
        {
          Debug.Log($"{Name}はダメージをうけなかった");
        }
      }

      else
      {
        Debug.Log($"{Name}は攻撃をかわした。");
      }

      return res;
    }
  }
}