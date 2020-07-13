using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class Enemy : IAttackable
  {
    /// <summary>
    /// 敵の行動一覧
    /// </summary>
    public enum BehaviorType { 
      None,
      Move,
      Attack,
    }

    //-------------------------------------------------------------------------
    // メンバー

    /// <summary>
    /// 敵チップ
    /// </summary>
    private CharChip chip;

    /// <summary>
    /// 敵の座標
    /// </summary>
    private Vector2Int coord = Vector2Int.zero;

    /// <summary>
    /// 行動
    /// </summary>
    private BehaviorType behavior = BehaviorType.None;

    /// <summary>
    /// 体力
    /// TODO: 仮実装
    /// </summary>
    private int hp = 10;

    /// <summary>
    /// 攻撃を受けたフラグ
    /// TODO: 仮実装
    /// </summary>
    public bool isAcceptAttack = false;

    /// <summary>
    /// 移動予定の座標
    /// </summary>
    public Vector2Int nextCoord = Vector2Int.zero;

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// アイドル状態です
    /// </summary>
    public bool IsIdle => (this.chip.IsIdle);

    /// <summary>
    /// 死んでいます
    /// TODO: 仮実装
    /// </summary>
    public bool IsDead => (this.hp <= 0);

    /// <summary>
    /// 敵の座標
    /// </summary>
    public Vector2Int Coord => (this.coord);

    /// <summary>
    /// 行動
    /// </summary>
    public BehaviorType Behavior => (this.behavior);

    /// <summary>
    /// 攻撃力
    /// TODO: 仮実装
    /// </summary>
    public int Atk => (10);

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Enemy(Vector2Int coord)
    {
      this.chip = MapChipFactory.Instance.CreateEnemyChip(EnemyChipType.Shobon);
      this.coord = coord;
      this.chip.transform.position = Util.GetPositionBy(coord);
    }

    /// <summary>
    /// AI: どんな行動をするか決定する処理
    /// </summary>
    public void Think()
    {
      // 自分の周囲１マスにプレイヤーがいるかどうか
      var player = DungeonManager.Instance.PlayerCoord;
      var v = player - this.coord;
      
      // 周囲１マスにプレイヤーがいるならプレイヤーを攻撃
      if (Mathf.Abs(v.x) <= 1 && Mathf.Abs(v.y) <= 1)
      {
        this.behavior = BehaviorType.Attack;
        this.chip.Direction = new Direction(v, false);
        Debug.Log(this.chip.Direction.value);
      }

      // プレイヤーがいないなら移動を考える
      else
      {
        // ランダムで移動方向を決める
        var dir = new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));

        // おそらく移動するであろう次の座標
        var maybeNext = this.coord + dir;

        // 移動先のタイル情報を見て移動するかどうかを決める
        var tile = DungeonManager.Instance.GetTile(maybeNext);

        // 移動先に障害物はないね、移動しよう。
        if (!tile.IsObstacle)
        {
          // 座標と方向を更新
          this.chip.Direction = new Direction(dir, false);
          this.nextCoord = maybeNext;
          this.behavior = BehaviorType.Move;
        }
      }
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が動き始める
    /// </summary>
    public void Move()
    {
      if (this.behavior == BehaviorType.Move)
      {
        // ダンジョンの情報を書き換え
        DungeonManager.Instance.UpdateEnemyCoord(this.coord, this.nextCoord);
        this.coord = this.nextCoord;
        this.chip.Move(Define.SEC_PER_TURN, Util.GetPositionBy(this.coord));
        this.behavior = BehaviorType.None;
      }
    }

    /// <summary>
    /// 攻撃予定の敵がこのメソッドを呼ばれると、攻撃の動きを開始する
    /// </summary>
    public void Attack()
    {
      // アタッカーじゃなければ何もしない
      if (this.behavior != BehaviorType.Attack) return;

      // 攻撃の動きを開始
      this.chip.Attack(Define.SEC_PER_TURN, 1f);
      this.behavior = BehaviorType.None;
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が痛がる
    /// </summary>
    public void Ouch()
    {
      this.chip.Oush(Define.SEC_PER_TURN);
      this.isAcceptAttack = false;
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が消滅する
    /// </summary>
    public void Vanish()
    {
      DungeonManager.Instance.RemoveEnemyCoord(this.coord);
      this.chip.Vanish(Define.SEC_PER_TURN);
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Destory()
    {
      MapChipFactory.Instance.Release(this.chip);
      this.chip = null;
    }

    /// <summary>
    /// 攻撃を受ける
    /// </summary>
    public void AcceptAttack(IAttackable attacker)
    {
      // ここで攻撃を受けて、残りの体力や死亡などの判定を行う
      this.hp -= attacker.Atk;
      this.isAcceptAttack = true;
    }


#if _DEBUG
    public void DrawDebugMenu()
    {
      GUILayout.BeginArea(new Rect(500, 0, 500, 500));
      {
        GUILayout.Label($"Current Coord: ({this.Coord})");
        GUILayout.Label($"Behavior:{this.behavior}" );;
        if(GUILayout.Button("Think"))
        {
          Think();
        }
      }
      GUILayout.EndArea();
    }
#endif
  }
}