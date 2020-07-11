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
    /// 動いてる敵がいたらtrueを返す
    /// </summary>
    public bool hasOnMoveEnemy
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
    public void orderToThinkAboutMoving()
    {
      Map((enemy) => { enemy.ThinkAboutMoving(); });
    }

    /// <summary>
    /// 敵さん達に、移動しろと命じる
    /// </summary>
    public void orderToMove()
    {
      Map((enemy) => { enemy.Move(); });
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
  }
}