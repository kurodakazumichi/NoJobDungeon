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
        bool hasAttacker = false;

        // 敵さんループ
        MyGame.Util.Loop(this.enemies, (enemy) =>
        {
          hasAttacker = enemy.Behavior == Enemy.BehaviorType.Attack;
          
          // アタッカーがいたらループを抜ける
          return hasAttacker;
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
      Enemy enemy = null;

      // アタッカーを探すために敵さんをループ
      MyGame.Util.Loop(this.enemies, (e) =>
      {
        // アタッカーじゃなければスキップ
        if (e.Behavior != Enemy.BehaviorType.Attack) return false;

        // 敵に攻撃の動きを命じる
        e.Attack();
        enemy = e;

        // ループを抜ける
        return true;
      });

      return enemy;
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
      // 新しく敵リストを用意する
      List<Enemy> newList = new List<Enemy>(this.enemies.Count);

      // 死んでる敵は破棄して、生きてる敵は新しいリストへ追加
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

      // 敵リストを更新
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