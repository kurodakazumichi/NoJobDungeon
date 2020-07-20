using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public class FieldItem : UnitBase
  {
    /// <summary>
    /// FieldItemのセットアップに必要なパラメータ
    /// </summary>
    public class Props 
    { 
      public Props(Master.Item.Entity item, Master.ItemGroup.Entity group)
      {
        Item = item;
        Group = group;
      }
      public Master.Item.Entity Item;
      public Master.ItemGroup.Entity Group;
    }

    //-------------------------------------------------------------------------
    // Enum

    private enum BehaviorType
    {
      New,    // 新しく作られた状態
      Pickup, // 拾える
      Throw,  // 投げれる
      Use,    // 使える
    }

    //-------------------------------------------------------------------------
    // Member

    /// <summary>
    /// 振る舞い
    /// </summary>
    private BehaviorType behavior = BehaviorType.New;

    /// <summary>
    /// アイテムの情報
    /// </summary>
    private Props props = null;

    /// <summary>
    /// マップチップ
    /// </summary>
    private BasicChip chip = null;

    /// <summary>
    /// 次の座標
    /// </summary>
    protected Vector2Int nextCoord = Vector2Int.zero;

    //-------------------------------------------------------------------------
    // 基本的なメソッド

    /// <summary>
    /// 座標とPropsを指定したセットアップ
    /// </summary>
    public virtual FieldItem Setup(Props props)
    {
      this.props = props;
      this.chip = MapChipFactory.Instance.CreateItemChip(props.Group.ChipType);

      Status = new Status(new Status.Props(props.Item.Name, 1, 0, 0));
      return this;
    }

    /// <summary>
    /// 座標を設定
    /// </summary>
    public void SetCoord(Vector2Int coord)
    {
      Coord = coord;
      chip.transform.position = Util.GetPositionBy(coord);
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Destory()
    {
      MapChipFactory.Instance.Release(this.chip);

      this.chip = null;
      this.props = null;
      this.Coord = Vector2Int.zero;
    }

    //-------------------------------------------------------------------------
    // IActionableの実装

    /// <summary>
    /// アイドル状態です
    /// </summary>
    public override bool IsIdle {
      get {
        if (this.chip == null) return true;
        return chip.IsIdle;
      }
    }

    /// <summary>
    /// アクションを開始
    /// </summary>
    public override void StartAction()
    {
      List<IActionable> targets = new List<IActionable>();
      targets.Add(PlayerManager.Instance.Find(actionRequest.Area));
      targets.Add(EnemyManager.Instance.FindTarget(actionRequest.Area));

      var AM = ActionManager.Instance;
      AM.SetActor(this);
      AM.AddTargets(targets);
      AM.StartAction();
    }

    //-------------------------------------------------------------------------
    // IActionable Action Callbackの実装

    public override void OnActionStartWhenActor()
    {
      switch(this.behavior) {
        case BehaviorType.Throw: OnThrowActionStart(); break;
        default: break;
      }
    }

    public override void OnActionExitWhenActor(IActionable target)
    {
      switch(this.behavior) {
        case BehaviorType.Throw: OnThrowActionExit(target); break;
        default: break;
      }
    }

    private void OnThrowActionStart()
    {
      var v = Coord - this.nextCoord;
      DoMoveMotion(0.05f * v.magnitude, this.nextCoord);
      Coord = this.nextCoord;
    }

    private void OnThrowActionExit(IActionable target)
    {
      // ターゲットがいなければ床に落ちる
      if (target == null) {
        ItemManager.Instance.AddItem(this);
        return;
      }

      // ターゲットがいれば当たったかどうかで判断
      if (target.ActionResponse.IsHit) {
        Destory();
      }
      else {
        ItemManager.Instance.AddItem(this);
      }
    }

    //-------------------------------------------------------------------------
    // IActionable Motion系の実装

    /// <summary>
    /// 移動の動作を行う
    /// </summary>
    public override void DoMoveMotion(float time, Vector2Int coord)
    {
      this.chip.Move(time, Util.GetPositionBy(coord));
    }

    public void SetPickup()
    {
      this.behavior = BehaviorType.Pickup;
    }

    public void SetThrow(Vector2Int target)
    {
      this.behavior = BehaviorType.Throw;
      this.nextCoord = target;

      this.actionRequest.Name = Status.Name;
      this.actionRequest.Pow = 20;
      this.actionRequest.Coord = Coord;
      this.actionRequest.Area.Add(target);
    }

  }
}
