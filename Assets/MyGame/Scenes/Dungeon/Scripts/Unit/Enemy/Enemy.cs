using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class Enemy
  {
    //-------------------------------------------------------------------------
    // メンバー

    /// <summary>
    /// 敵チップ
    /// </summary>
    private EnemyChip chip;

    /// <summary>
    /// 敵の座標
    /// </summary>
    private Vector2Int coord = Vector2Int.zero;

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
    /// 移動について考える
    /// </summary>
    public void ThinkAboutMoving()
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
        // ダンジョンの情報を書き換え
        DungeonManager.Instance.UpdateEnemyCoord(this.coord, maybeNext);

        // 座標と方向を更新
        this.chip.Direction = new Direction(dir, false);
        this.coord = maybeNext;
      }
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が動き始める
    /// </summary>
    public void Move()
    {
      this.chip.Move(Define.SEC_PER_TURN, Util.GetPositionBy(this.coord));
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
  }
}