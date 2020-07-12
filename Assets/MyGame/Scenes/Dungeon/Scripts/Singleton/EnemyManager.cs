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

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// 敵を生成する
    /// </summary>
    public void CreateEnemies()
    {
      DungeonManager.Instance.Map((int x, int y, IReadOnlyTile tile) =>
      {
        if (tile.IsEnemy)
        {
          var enemy = new Enemy(new Vector2Int(x, y));
          this.enemies.Add(enemy);
        }
      });
    }

    /// <summary>
    /// 敵さんたちに、移動について考えるように命じる
    /// </summary>
    public void OrderToThinkAboutMoving()
    {
      Map((enemy) => { enemy.ThinkAboutMoving(); });
    }

    /// <summary>
    /// 敵さん達に、移動しろと命じる
    /// </summary>
    public void OrderToMove()
    {
      Map((enemy) => { enemy.Move(); });
    }

    /// <summary>
    /// 敵さんたちに痛がるように命じる
    /// </summary>
    public void OrderToOuch()
    {
      Map((enemy) =>
      {
        if (enemy.isAcceptAttack)
        {
          enemy.Ouch();
        }
      });
    }

    /// <summary>
    /// 敵さんたちに消滅するように命じる
    /// </summary>
    public void OrderToVanish()
    {
      Map((enemy) =>
      {
        if (enemy.IsDead)
        {
          enemy.Vanish();
        }
      });
    }

    /// <summary>
    /// 死んだ敵は破棄する
    /// </summary>
    public void DestoryDeadEnemies()
    {
      List<Enemy> newList = new List<Enemy>(this.enemies.Count);

      Map((enemy) =>
      {
        if (enemy.IsDead)
        {
          enemy.Destory();
        }
        
        else
        {
          newList.Add(enemy);
        }
      });

      this.enemies.Clear();
      this.enemies = newList;
    }

    /// <summary>
    /// 指定された座標にいる敵さんに攻撃を与える
    /// </summary>
    public void AttackEnemies(IAttackable attacker, List<Vector2Int> targets)
    {
      targets.ForEach((coord) =>
      {
        Map((enemy) =>
        {
          if (enemy.Coord.Equals(coord))
          {
            enemy.AcceptAttack(attacker);
          }
        });
      });
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

    //-------------------------------------------------------------------------
    // Util

    /// <summary>
    /// 管理してる敵のリストを全ループする
    /// </summary>
    private void Map(System.Action<Enemy> cb)
    {
      this.enemies.ForEach((e) =>
      {
        cb(e);
      });
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