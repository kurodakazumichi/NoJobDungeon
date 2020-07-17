using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class Enemy : CharBase
  {
    /// <summary>
    /// 敵生成に必要なパラメータ
    /// </summary>
    public class Props
    {
      public Props(Vector2Int coord, EnemyChipType chipType, Status.Props statusProps)
      {
        Coord = coord;
        ChipType = chipType;
        StatusProps = statusProps;
      }

      public Vector2Int Coord = Vector2Int.zero;
      public EnemyChipType ChipType = default;
      public Status.Props StatusProps = null;
    }

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
    /// 移動予定の座標
    /// ThinkのタイミングでCoordを更新してしまうと攻撃判定が移動後の座標で行われるため
    /// ThinkのタイミングではnextCoordを更新し実際に動く際にCoordを更新する。
    /// </summary>
    public Vector2Int nextCoord = Vector2Int.zero;

    /// <summary>
    /// ダンジョンマップ上と同期している座標
    /// この座標には常にダンジョンマップ上の座標が入っている
    /// </summary>
    private Vector2Int syncCoord = Vector2Int.zero;

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// 行動
    /// </summary>
    public BehaviorType Behavior => (this.behavior);

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// セットアップ
    /// </summary>
    virtual public void Setup(Props props)
    {
      Chip = MapChipFactory.Instance.CreateEnemyChip(props.ChipType);
      Coord = this.syncCoord = props.Coord;
      
      Chip.transform.position = Util.GetPositionBy(Coord);
      Chip.Direction = Direction.Random();

      Status = new Status(props.StatusProps);
    }

    /// <summary>
    /// AI: どんな行動をするか決定する処理
    /// </summary>
    public void Think()
    {
      // 自分の周囲１マスにプレイヤーがいるかどうか
      var player = DungeonManager.Instance.PlayerCoord;
      var v = player - Coord;
      
      // 周囲１マスにプレイヤーがいる、かつその方向に攻撃可能であれば
      if (Mathf.Abs(v.x) <= 1 && Mathf.Abs(v.y) <= 1 && CanAttackTo(new Direction(v, false)))
      {
        // かつ攻撃できる方向であれば攻撃
        this.behavior = BehaviorType.Attack;
        Chip.Direction = new Direction(v, false);

        // 攻撃要求をセット
        AttackRequest.Name = Status.Name;
        AttackRequest.Pow = Status.Pow;
        AttackRequest.Coord = Coord;
        AttackRequest.Area.Add(player);
      }

      // プレイヤーがいないなら移動を考える
      else
      {
        // ランダムで移動量を決める
        var move = new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));

        // 移動方向を生成
        var moveDir = new Direction(move, false);

        // 移動可能であれば移動する
        if (CanMoveTo(moveDir))
        {
          Chip.Direction = moveDir;
          this.nextCoord = Coord + move;
          this.behavior = BehaviorType.Move;

          // ダンジョンの情報を書き換え
          MoveMapCoord(Coord, this.nextCoord);
        }
      }
    }

    private void MoveMapCoord(Vector2Int from, Vector2Int to)
    {
      DungeonManager.Instance.UpdateEnemyCoord(from, to);
      syncCoord = to;
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が動き始める
    /// </summary>
    public void DoMoveMotion()
    {
      if (this.behavior != BehaviorType.Move) return;

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
      if (!AttackResponse.IsAccepted) return;

      // 攻撃を受けていたら痛がる
      if (AttackResponse.IsHit && AttackResponse.HasDamage)
      {
        Chip.Ouch(Define.SEC_PER_TURN);
      }
      AttackResponse.Reset();
    }

    /// <summary>
    /// このメソッドを呼ぶと敵が消滅する
    /// </summary>
    public void DoVanishMotion()
    {
      // 死んでいなければ消えない
      if (!Status.IsDead) return;

      // マップ上の敵の情報を除去する
      DungeonManager.Instance.RemoveEnemyCoord(syncCoord);

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