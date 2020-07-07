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
    // メンバ変数
    
    /// <summary>
    /// 敵リスト
    /// </summary>
    private List<Enemy> enemies = new List<Enemy>();

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
    /// 敵の処理を開始
    /// </summary>
    public void StartEnemies()
    {
      foreach(var em in this.enemies)
      {
        em.Start();
      }
    }
  }
}