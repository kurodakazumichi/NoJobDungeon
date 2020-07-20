using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public interface IReadOnlyActionResponse
  {
    bool IsHit { get; }
  }

  /// <summary>
  /// 攻撃後の情報を持っているクラス
  /// </summary>
  public class ActionResponse: IReadOnlyActionResponse
  {
    /// <summary>
    /// Actor側の名前
    /// </summary>
    public string ActorName { get; set; } = "";

    /// <summary>
    /// Target側の名前
    /// </summary>
    public string TargetName { get; set; } = "";

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
    public void Copy(ActionResponse res)
    {
      ActorName  = res.ActorName;
      TargetName = res.TargetName;
      IsAccepted = res.IsAccepted;
      IsHit      = res.IsHit;
      Damage     = res.Damage;
    }

    /// <summary>
    /// リセット
    /// </summary>
    public void Reset()
    {
      ActorName  = "";
      TargetName = "";
      IsAccepted = false;
      IsHit      = false;
      Damage     = 0;
    }
#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ
   
    public void DrawDebug()
    {
      GUILayout.Label("■ ActionResponse");

      using (var scope = new GUILayout.HorizontalScope())
      {
        GUILayout.Label($"Name1:{ActorName}");
        GUILayout.Label($"Name2:{TargetName}");
        GUILayout.Label($"IsAccepted:{IsAccepted}");
        GUILayout.Label($"IsHit:{IsHit}");
        GUILayout.Label($"Damage:{Damage}");
      }
    }
#endif
  }
}