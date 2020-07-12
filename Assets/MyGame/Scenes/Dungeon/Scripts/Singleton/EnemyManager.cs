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

    public bool HasAttacker
    {
      get
      {
        bool hasAttacker = false;
        MyGame.Util.Loop(this.enemies, (enemy) =>
        {
          hasAttacker = enemy.Behavior == Enemy.BehaviorType.Attack;
          return !hasAttacker;
        });

        return hasAttacker;
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
    /// 敵さんたちにどう行動するか考えてもらう
    /// </summary>
    public void OrderToThink()
    {
      MyGame.Util.Loop(this.enemies, (enemy) => { enemy.Think(); });
    }

    /// <summary>
    /// 敵さんたちに、移動について考えるように命じる
    /// </summary>
    public void OrderToThinkAboutMoving()
    {
      MyGame.Util.Loop(this.enemies, (enemy) => { enemy.ThinkAboutMoving(); });
    }

    /// <summary>
    /// 敵さん達に、移動しろと命じる
    /// </summary>
    public void OrderToMove()
    {
      MyGame.Util.Loop(this.enemies, (enemy) => { enemy.Move(); });
    }

    /// <summary>
    /// 敵さんに、攻撃しろと命じる
    /// 一気に攻撃するとおかしいので、この処理では
    /// 一度に１人だけ攻撃を命じる
    /// </summary>
    public IAttackable OrderToAttack()
    {
      IAttackable attacker = null;
      MyGame.Util.Loop(this.enemies, (enemy) =>
      {
        var attacked = enemy.Attack();

        if (attacked)
        {
          attacker = enemy;
          return false;
        }

        return true;
      });
      return attacker;
    }

    /// <summary>
    /// 敵さんたちに痛がるように命じる
    /// </summary>
    public void OrderToOuch()
    {
      MyGame.Util.Loop(this.enemies, (enemy) =>
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
      MyGame.Util.Loop(this.enemies, (enemy) =>
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

      MyGame.Util.Loop(this.enemies, (enemy) =>
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
        MyGame.Util.Loop(this.enemies, (enemy) =>
        {
          if (enemy.Coord.Equals(coord))
          {
            enemy.AcceptAttack(attacker);
          }
        });
      });
    }
  }
}