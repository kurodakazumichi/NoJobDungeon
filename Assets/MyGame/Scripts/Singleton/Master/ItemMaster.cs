using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public enum ItemId
  {
    ITEM001, // 回復薬
    ITEM002, // 大回復薬
    ITEM003, // 超回復薬
    ITEM004, // 命の薬
  }

  public enum ItemCategoryId
  {
    Weapon,    // 武器
    Shield,    // 盾
    Accessory, // アクセサリー
    Drug,      // 薬
    Book,      // 本
    Card,      // カード
  }

  public class ItemEntity
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ItemEntity(
      ItemId id,
      ItemCategoryId categoryId,
      ItemChipType chipType,
      string name
    )
    {
      Id = id;
      CategoryId = categoryId;
      ChipType = ChipType;
      Name = name;
    }

    public ItemId Id { get; private set; }
    public ItemCategoryId CategoryId { get; private set; }
    public ItemChipType ChipType { get; private set; }
    public string Name { get; private set; }

  }
  /// <summary>
  /// アイテムの情報を管理する
  /// </summary>
  public class ItemMaster : SingletonMonobehaviour<ItemMaster>
  {
    private Dictionary<ItemId, ItemEntity> repository = new Dictionary<ItemId, ItemEntity>();

    /// <summary>
    /// 最初にデータを生成する
    /// </summary>
    private void Start()
    {
      List<ItemEntity> entities = new List<ItemEntity>()
      {
        new ItemEntity(ItemId.ITEM001, ItemCategoryId.Drug, ItemChipType.Capsule, "回復薬"),
        new ItemEntity(ItemId.ITEM002, ItemCategoryId.Drug, ItemChipType.Capsule, "大回復薬"),
        new ItemEntity(ItemId.ITEM003, ItemCategoryId.Drug, ItemChipType.Capsule, "超回復薬"),
        new ItemEntity(ItemId.ITEM004, ItemCategoryId.Drug, ItemChipType.Capsule, "命の薬"),
      };

      Util.Loop(entities, (entity) => { 
        this.repository.Add(entity.Id, entity);
      });
    }

    public ItemEntity Get(ItemId id)
    {
      return this.repository[id];
    }

  }
}