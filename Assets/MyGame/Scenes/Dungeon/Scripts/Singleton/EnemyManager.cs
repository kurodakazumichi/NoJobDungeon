using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// 敵の管理クラス
  /// </summary>
  public class EnemyManager : SingletonMonobehaviour<EnemyManager>
  {
    //-------------------------------------------------------------------------
    // メンバ
    
    /// <summary>
    /// 敵リスト
    /// </summary>
    private List<Enemy> enemies = new List<Enemy>();

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// Activeな(何かしら活動している)敵がいるかどうか
    /// </summary>
    public bool HasActiveEnemy
    {
      get
      {
        for(int i = 0; i < this.enemies.Count; ++i)
        {
          if (!this.enemies[i].IsIdle) return true;
        }
        return false;
      }
    }

    /// <summary>
    /// アタッカーがいる
    /// </summary>
    public bool HasAttacker
    {
      get
      {
        foreach(var enemy in this.enemies)
        {
          if (enemy.Behavior == Enemy.BehaviorType.Attack)
          {
            return true;
          }
        }
        return false;
      }
    }

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// 全ての敵を破棄してリストを空にする。
    /// </summary>
    public void Reset()
    {
      foreach(var enemy in this.enemies)
      {
        enemy.Destory();
      }
      this.enemies = new List<Enemy>();
    }

    /// <summary>
    /// 敵を生成する
    /// </summary>
    public void CreateEnemies()
    {
      Reset();

      DungeonManager.Instance.Map((int x, int y, IReadOnlyTile tile) =>
      {
        var ids = Master.EnemyMaster.Instance.Ids();

        if (tile.IsEnemy)
        {
          var id = ids[Random.Range(0, ids.Count)];

          var enemy = CreateEnemy(id, new Vector2Int(x, y));

          this.enemies.Add(enemy);
        }
      });
    }

    /// <summary>
    /// 敵を生成する
    /// </summary>
    private Enemy CreateEnemy(string id, Vector2Int coord)
    {
      var data = Master.EnemyMaster.Instance.FindById(id);

      var enemy = new Enemy();

      var props = new Enemy.Props(
        coord,
        data.ChipType,
        new Status.Props(data.Name, data.HP, data.Pow, data.Def)
      );
      enemy.Setup(props);

      return enemy;
    }

    /// <summary>
    /// 敵さんたちにどう行動するか考えてもらう
    /// </summary>
    public void Think()
    {
      foreach(var enemy in this.enemies) { 
        enemy.Think(); 
      }
    }

    /// <summary>
    /// 敵さん達に、移動しろと命じる
    /// </summary>
    public void DoMoveMotion()
    {
      foreach (var enemy in this.enemies) {
        enemy.DoMoveMotion(); 
      };
    }

    /// <summary>
    /// 敵さんたちに痛がるように命じる
    /// </summary>
    public void DoOuchMotion()
    {
      foreach(var e in this.enemies) 
      { 
        e.DoOuchMotion();
      }
    }

    /// <summary>
    /// 敵さんたちに消滅するように命じる
    /// </summary>
    public void DoVanishMotion()
    {
      foreach(var e in this.enemies)
      {
        e.DoVanishMotion();
      }
    }

    /// <summary>
    /// 死んだ敵は破棄する
    /// </summary>
    public void DestoryDeadEnemies()
    {
      // 新しく敵リストを用意する
      List<Enemy> newList = new List<Enemy>(this.enemies.Count);

      // 死んでる敵は破棄して、生きてる敵は新しいリストへ追加
      foreach(var e in this.enemies)
      {
        if (e.Status.IsDead) 
        {
          e.Destory();
        } 
        
        else 
        {
          newList.Add(e);
        }
      }

      // 敵リストを更新
      this.enemies.Clear();
      this.enemies = newList;
    }

    /// <summary>
    /// 指定された座標に一致する攻撃
    /// </summary>
    public List<IAttackable> FindTarget(List<Vector2Int> coords)
    {
      List<IAttackable> found = new List<IAttackable>();

      foreach (var coord in coords)
      {
        var enemy = GetEnemyBy(coord);

        if (enemy == null) continue;

        found.Add(enemy);
      }

      return found;
    }

    /// <summary>
    /// 座標に一致する敵を取得する
    /// </summary>
    public Enemy GetEnemyBy(Vector2Int coord)
    {
      foreach(var enemy in this.enemies)
      {
        if (enemy.Coord.Equals(coord)) return enemy;
      }

      return null;
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    public void Attack(IAttackable target)
    {
      var attacker = FindAttacker();

      if (attacker != null)
      {
        attacker.Attack(target);
        attacker.DoAttackMotion();
      }
    }

    /// <summary>
    /// 行動が攻撃になっている敵を探す
    /// </summary>
    /// <returns></returns>
    private Enemy FindAttacker()
    {
      foreach(var enemy in this.enemies)
      {
        if (enemy.Behavior != Enemy.BehaviorType.Attack) continue;

        return enemy;
      }

      return null;
    }

    //-------------------------------------------------------------------------
    // Protected Method

    protected override void Awake()
    {
      base.Awake();
#if _DEBUG
      DebugMenuManager.Instance.RegisterMenu(DebugMenu.Page.Enemy, DrawDebugMenu, nameof(EnemyManager));
#endif
    }

#if _DEBUG
    private void DrawDebugMenu(DebugMenu.MenuWindow menuWindow)
    {
      this.enemies.ForEach((e) =>
      {
        if (GUILayout.Button($"{e.Coord} 詳細"))
        {
          DebugMenuManager.Instance.OpenWindow(DebugMenu.Page.EnemyDetail, 
            ( window ) => 
            {
              e.DrawDebugMenu();
            });
        }
      });
    }
#endif

  }
}