using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// 敵の管理クラス
  /// </summary>
  public class EnemyManager : SingletonMonobehaviour<EnemyManager>, IDebuggeable
  {
    //-------------------------------------------------------------------------
    // Member
    
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
        foreach(var e in this.enemies) {
          if (!e.IsIdle) return true;
        }
        return false;
      }
    }

    /// <summary>
    /// Actorがいる
    /// </summary>
    public bool HasActor
    {
      get
      {
        foreach(var enemy in this.enemies) {
          if (enemy.Behavior == Enemy.BehaviorType.Action) {
            return true;
          }
        }
        return false;
      }
    }

    //-------------------------------------------------------------------------
    // 基本的なメソッド

    /// <summary>
    /// 全ての敵を破棄してリストを空にする。
    /// </summary>
    public void Reset()
    {
      Loop((e) => { e.Destory(); });
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
    /// 敵の更新処理
    /// </summary>
    public void UpdateEnemies()
    {
      foreach(var e in this.enemies) {
        e.Update();
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
      foreach (var e in this.enemies)
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

    //-------------------------------------------------------------------------
    // シーンのフェーズ変化時にコールされるメソッド群

    public void OnSceneMoveEnter()
    {
      foreach(var e in this.enemies) {
        e.OnSceneMoveEnter();
      }
    }

    public void OnSceneMoveExit()
    {
      foreach(var e in this.enemies) {
        e.OnSceneMoveExit();
      }
    }

    public void OnSceneActionEnter()
    {
      foreach(var e in this.enemies)
      {
        if (e.Behavior == Enemy.BehaviorType.Action)
        {
          e.OnSceneActionEnter();
          break;
        }
      }
    }

    public void OnSceneActionExit()
    {
      Loop((e) => { e.OnSceneActionExit(); });
    }

    public void OnSceneTurnEndEnter()
    {
      Loop((e) => { e.OnSceneTurnEndEnter(); });
    }

    //-------------------------------------------------------------------------
    // モーション系

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

    //-------------------------------------------------------------------------
    // 敵を探す

    /// <summary>
    /// 指定された座標に一致する攻撃
    /// </summary>
    public List<IActionable> FindTarget(List<Vector2Int> coords)
    {
      List<IActionable> found = new List<IActionable>();

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

    //-------------------------------------------------------------------------
    // 攻撃関連

    public List<ActionResponse> AcceptAttack(ActionRequest req)
    {
      var result = new List<ActionResponse>();

      var targets = FindTarget(req.Area);

      foreach(var target in targets)
      {
        var res = target.AcceptAction(req);
        result.Add(res);
      }

      return result;
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    public void Attack(IActionable target)
    {
      var attacker = FindAttacker();

      if (attacker != null)
      {
        //attacker.Attack(target);
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
        if (enemy.Behavior != Enemy.BehaviorType.Action) continue;

        return enemy;
      }

      return null;
    }

    //-------------------------------------------------------------------------
    // その他
    
    private void Loop(System.Action<Enemy> cb)
    {
      foreach(var e in this.enemies) { cb(e); }
    }


#if _DEBUG
    //-------------------------------------------------------------------------
    // デバッグ

    void IDebuggeable.Draw(MyDebug.Window window)
    {
      this.enemies.ForEach((e) =>
      {
        if (GUILayout.Button($"{e.Coord} 詳細"))
        {
          DebugManager.Instance.OpenWindow("EnemyDetail", 
            ( newWindow ) => 
            {
              e.DrawDebugMenu();
            });
        }
      });
    }
#endif

  }
}