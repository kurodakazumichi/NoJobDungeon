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

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="props"></param>
    public FieldItem() {}

    //-------------------------------------------------------------------------
    // Member

    /// <summary>
    /// アイテムの情報
    /// </summary>
    private Props props = null;

    /// <summary>
    /// マップチップ
    /// </summary>
    private BasicChip chip = null;

    //-------------------------------------------------------------------------
    // 基本的なメソッド

    /// <summary>
    /// 座標とPropsを指定したセットアップ
    /// </summary>
    public FieldItem Setup(Props props)
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
    public override bool IsIdle => (this.chip.IsIdle);

    /// <summary>
    /// 移動の動作を行う
    /// </summary>
    public void DoMoveMotion(float time, Vector2Int coord)
    {
      this.chip.Move(time, Util.GetPositionBy(coord));
    }

  }
}
