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

    /// <summary>
    /// 行動力
    /// </summary>
    private LimitedFloat energy = new LimitedFloat();

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
      this.energy.Setup(1f, 1f);
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
    public bool HasEnergy => (this.energy.IsEmpty == false);

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
    /// エネルギーを使う
    /// </summary>
    public void UseEnergy()
    {
      this.energy.Now -= 1f;
    }

    /// <summary>
    /// エネルギーを満タンにする
    /// </summary>
    public void FullEnergy()
    {
      this.energy.BeToFull();
    }

    /// <summary>
    /// 攻撃を受ける
    /// </summary>
    public ActionResponse AcceptAttack(ActionRequest req)
    {
      var res = new ActionResponse();
      res.ActorName = req.Name;
      res.TargetName = Name;
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

#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ
    public void DrawDebug()
    {
      GUILayout.Label("■ Status");
      using (var scope = new GUILayout.HorizontalScope())
      {
        GUILayout.Label($"Name:{name}");
        GUILayout.Label($"HP:{hp.Now}/{hp.Max}");
        GUILayout.Label($"Pow:{pow.Now}/{pow.Max}");
        GUILayout.Label($"Def:{def.Now}/{def.Max}");
        GUILayout.Label($"Energy:{energy.Now}/{energy.Max}");
      }
    }
#endif
  }
}