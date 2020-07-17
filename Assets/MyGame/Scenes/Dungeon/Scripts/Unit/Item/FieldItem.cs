using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public interface IReadOnlyFieldItem
  {
    string Name { get; }
  }

  public class FieldItem : IReadOnlyFieldItem
  {
    /// <summary>
    /// FieldItemの生成に必要なパラメータ
    /// </summary>
    public class Props 
    { 
      public string Id;
      public string Name;
      public ItemChipType ChipType;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="props"></param>
    public FieldItem(Props props)
    {
      this.props = props;
    }

    //-------------------------------------------------------------------------
    // Member

    /// <summary>
    /// アイテムの情報
    /// </summary>
    private Props props = null;

    /// <summary>
    /// 座標
    /// </summary>
    public Vector2Int Coord { get; set; }

    /// <summary>
    /// マップチップ
    /// </summary>
    private BasicChip chip = null;

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// アイテム名
    /// </summary>
    public string Name => ((this.props != null)? this.props.Name : "");

    /// <summary>
    /// アイドル状態です
    /// </summary>
    public bool IsIdle => (this.chip.IsIdle);

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// 引数に座標を渡すだけのセットアップ
    /// </summary>
    /// <param name="coord"></param>
    public void Setup(Vector2Int coord)
    {
      if (this.props == null) return;

      this.Setup(coord, this.props);
    }

    /// <summary>
    /// 座標とPropsを指定したセットアップ
    /// </summary>
    public void Setup(Vector2Int coord, Props props)
    {
      this.props = props;

      Coord = coord;

      this.chip = MapChipFactory.Instance.CreateItemChip(props.ChipType);
      chip.transform.position = Util.GetPositionBy(coord);
    }

    /// <summary>
    /// 移動の動作を行う
    /// </summary>
    public void DoMoveMotion(float time, Vector2Int coord)
    {
      this.chip.Move(time, Util.GetPositionBy(coord));
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
  }

}
