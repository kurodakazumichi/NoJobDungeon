using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class Enemy : CharBase, IAttackable
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
    /// 行動
    /// </summary>
    private BehaviorType behavior = BehaviorType.None;

    /// <summary>
    /// ステータス
    /// </summary>
    private Status status = null;

    /// <summary>
    /// 移動予定の座標
    /// </summary>
    public Vector2Int nextCoord = Vector2Int.zero;

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// アイドル状態です
    /// </summary>
    public bool IsIdle => (Chip.IsIdle);

    /// <summary>
    /// 行動
    /// </summary>
    public BehaviorType Behavior => (this.behavior);

    /// <summary>
    /// ステータス
    /// </summary>
    public Status Status => (this.status);

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Enemy(Vector2Int coord)
    {
      Chip = MapChipFactory.Instance.CreateEnemyChip(EnemyChipType.Shobon);
      Coord = coord;
      Chip.transform.position = Util.GetPositionBy(coord);

      Status.Props props = new Status.Props(10, 4, 2);
      this.status = new Status(props);
    }

    /// <summary>
    /// AI: どんな行動をするか決定する処理
    /// </summary>
    public void Think()
    {
      // 自分の周囲１マスにプレイヤーがいるかどうか
      var player = DungeonManager.Instance.PlayerCoord;
      var v = player - Coord;
      
      // 周囲１マスにプレイヤーがいるならプレイヤーを攻撃
      if (Mathf.Abs(v.x) <= 1 && Mathf.Abs(v.y) <= 1)
      {
        this.behavior = BehaviorType.Attack;
        Chip.Direction = new Direction(v, false);
      }

      // プレイヤーがいないなら移動を考える
      else
      {
        // ランダムで移動方向を決める
        var dir = new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));

        // おそらく移動するであろう次の座標
        var maybeNext = Coord + dir;

        // 移動先のタイル情報を見て移動するかどうかを決める
        var tile = DungeonManager.Instance.GetTile(maybeNext);

        // 移動先に障害物はないね、移動しよう。
        if (!tile.IsObstacle)
        {
          // 座標と方向を更新
          Chip.Direction = new Direction(dir, false);
          this.nextCoord = maybeNext;
          this.behavior = BehaviorType.Move;
        }
      }
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が動き始める
    /// </summary>
    public void DoMoveMotion()
    {
      if (this.behavior != BehaviorType.Move) return;

      // ダンジョンの情報を書き換え
      DungeonManager.Instance.UpdateEnemyCoord(Coord, this.nextCoord);
      Coord = this.nextCoord;
      Chip.Move(Define.SEC_PER_TURN, Util.GetPositionBy(Coord));
      this.behavior = BehaviorType.None;
    }

    /// <summary>
    /// 攻撃予定の敵がこのメソッドを呼ばれると、攻撃の動きを開始する
    /// </summary>
    public void DoAttackMotion()
    {
      // アタッカーじゃなければ何もしない
      if (this.behavior != BehaviorType.Attack) return;

      // 攻撃の動きを開始
      Chip.Attack(Define.SEC_PER_TURN, 1f);
      this.behavior = BehaviorType.None;
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が痛がる
    /// </summary>
    public void DoOuchMotion()
    {
      // 攻撃を受けていなければ痛がらない
      if (!Status.IsAcceptedAttack) return;

      // 攻撃を受けていたら痛がる
      if (Status.IsHit)
      {
        if (Status.HasDamage)
        {
          Chip.Ouch(Define.SEC_PER_TURN);
          Debug.Log($"しょぼんは{Status.AcceptedDamage}のダメージをうけた。");
        }

        else
        {
          Debug.Log("しょぼんは攻撃をうけたがなんともなかった。");
        }
      }

      // 攻撃を避けていたらメッセージを表示
      else
      {
        Debug.Log($"しょぼんは攻撃をかわした。");
      }
      this.status.Reset();
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が消滅する
    /// </summary>
    public void DoVanishMotion()
    {
      // 死んでいなければ消えない
      if (!Status.IsDead) return;

      // マップ上の敵の情報を除去する
      DungeonManager.Instance.RemoveEnemyCoord(Coord);

      // 消滅モーション開始
      Chip.Vanish(Define.SEC_PER_TURN);
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Destory()
    {
      MapChipFactory.Instance.Release(Chip);
      Chip = null;
    }

    /// <summary>
    /// 攻撃をする
    /// </summary>
    public void Attack(IAttackable target)
    {
      target.AcceptAttack(this);
    }

    /// <summary>
    /// 攻撃を受ける
    /// </summary>
    public void AcceptAttack(IAttackable attacker)
    {
      // 攻撃を受ける
      this.status.AcceptAttack(attacker.Status);

      // 攻撃してきた奴の方を向く
      Chip.Direction = Direction.LookAt(Coord, attacker.Coord);
    }


#if _DEBUG
    public void DrawDebugMenu()
    {
      GUILayout.Label($"Current Coord: ({this.Coord})");
      GUILayout.Label($"Behavior:{this.behavior}" );;
      if(GUILayout.Button("Think"))
      {
        Think();
      }
      GUILayout.Label("CharChip");
      Chip.DrawDebugMenu();
    }
#endif
  }
}