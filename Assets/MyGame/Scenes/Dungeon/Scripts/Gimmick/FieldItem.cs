﻿using System.Collections;
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
    /// アイテムの情報
    /// </summary>
    private ItemEntity entity = null;

    /// <summary>
    /// 座標
    /// </summary>
    public Vector2Int Coord { get; set; }

    /// <summary>
    /// マップチップ
    /// </summary>
    private BasicChip chip = null;

    /// <summary>
    /// アイテム名
    /// </summary>
    public string Name => ((this.entity != null)? this.entity.Name : "");

    /// <summary>
    /// セットアップ
    /// </summary>
    public void Setup(Vector2Int coord, ItemEntity entity)
    {
      this.entity = entity;

      Coord = coord;

      this.chip = MapChipFactory.Instance.CreateItemChip(entity.ChipType);
      chip.transform.position = Util.GetPositionBy(coord);
    }

    /// <summary>
    /// 破棄
    /// </summary>
    public void Destory()
    {
      MapChipFactory.Instance.Release(this.chip);

      this.chip = null;
      this.entity = null;
      this.Coord = Vector2Int.zero;
    }
  }

}
