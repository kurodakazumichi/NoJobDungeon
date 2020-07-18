using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Master
{
  /// <summary>
  /// Item Master
  /// </summary>
  public class EnemyMaster : MasterBase<EnemyMaster, Enemy.Entity>, IDebuggeable
  {
    /// <summary>
    /// DebugMenuを登録
    /// </summary>
    protected override void Awake()
    {
      base.Awake();
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
      // JSONを読み込んで辞書に登録
      var repo = Load<Enemy.Json>("Master/Enemy");

      foreach (var entity in repo.list)
      {
        this.repository.Add(entity.id, new Enemy.Entity(entity));
      }
    }


#if _DEBUG
    //-------------------------------------------------------------------------
    // Debug
    void IDebuggeable.Draw(MyDebug.Window window)
    {
      // データを列挙
      foreach (var entity in this.repository)
      {
        if (GUILayout.Button($"{entity.Value.Name}"))
        {
          DebugManager.Instance.OpenWindow(nameof(EnemyMaster), (win) =>
          {
            this.DrawDebugDetail(entity.Value);
          });
        }
      }
    }

    private void DrawDebugDetail(Enemy.Entity entity)
    {
      GUILayout.Label($"id:{entity.Id}");
      GUILayout.Label($"name:{entity.Name}");

      using (var scope = new GUILayout.HorizontalScope())
      {
        GUILayout.Label($"HP:{entity.HP}");
        GUILayout.Label($"Pow:{entity.Pow}");
        GUILayout.Label($"Def:{entity.Def}");
        GUILayout.Label($"ChipType:{entity.ChipType}");
      }
    }
#endif

  }

  namespace Enemy
  {
    /// <summary>
    /// Jsonをプログラム内で利用しやすい形にしたもの
    /// </summary>
    public class Entity
    {
      public Entity(Json json)
      {
        Id = json.id;
        Name = json.name;
        HP = json.hp;
        Pow = json.pow;
        Def = json.def;
        ChipType = Util.StrToEnum<EnemyChipType>(json.enemyChipType);
      }

      public string Id { get; private set; }
      public string Name { get; private set; }
      public float HP { get; private set; }
      public float Pow { get; private set; }
      public float Def { get; private set; }
      public EnemyChipType ChipType { get; private set; }
    }

    /// <summary>
    /// Json読み込み用
    /// </summary>
    [System.Serializable]
    public class Json
    {
      public string id = "";
      public string name = "";
      public float hp = 0;
      public float pow = 0;
      public float def = 0;
      public string enemyChipType = "";
    }
  }

}