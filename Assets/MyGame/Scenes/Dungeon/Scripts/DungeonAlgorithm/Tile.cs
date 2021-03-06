﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon {

  /// <summary>
  /// TileのReadOnly用インターフェース
  /// </summary>
  public interface IReadOnlyTile 
  {
    bool IsWall     { get; }
    bool IsRoom     { get; }
    bool IsAisle    { get; }
    bool IsPlayer   { get; }
    bool IsEnemy    { get; }
    bool IsItem     { get; }
    bool IsTrap     { get; }
    bool IsGoal     { get; }
    bool IsEmpty    { get; }
    bool IsObstacle { get; }
    bool IsClear    { get; }
    bool Contain(params Tiles[] tiles);
    bool ContainEither(params Tiles[] tiles);

  }

  /// <summary>
  /// ダンジョンのタイルの種類
  /// </summary>
	public enum Tiles
	{
		Wall   = 1 << 0,
		Room   = 1 << 1,
		Aisle  = 1 << 2,
		Player = 1 << 3,
		Enemy  = 1 << 4,
		Item   = 1 << 5,
		Trap   = 1 << 6,
    Goal   = 1 << 7,
    Clear  = 1 << 8,
	}

  /// <summary>
  /// タイルクラス
  /// </summary>
  public class Tile : IReadOnlyTile
  {
    /// <summary>
    /// タイルの状態
    /// </summary>
    private BitFlag state;

    /// <summary>
    /// デフォルトコンストラクタ
    /// </summary>
    public Tile()
    {
      this.state.Clear();
    }

    /// <summary>
    /// コピーコンストラクタ
    /// </summary>
    /// <param name="tile"></param>
    public Tile(Tile tile)
    {
      this.state = tile.state;
    }

    //-------------------------------------------------------------------------
    // プロパティ
    //-------------------------------------------------------------------------
    public bool IsWall
    {
      get { return this.state.Contain((uint)Tiles.Wall); }
    }

    public bool IsRoom
    {
      get { return this.state.Contain((uint)Tiles.Room); }
    }

    public bool IsAisle
    {
      get { return this.state.Contain((uint)Tiles.Aisle); }
    }

    public bool IsPlayer
    {
      get { return this.state.Contain((uint)Tiles.Player); }
    }

    public bool IsEnemy
    {
      get { return this.state.Contain((uint)Tiles.Enemy); }
    }

    public bool IsItem
    {
      get { return this.state.Contain((uint)Tiles.Item); }
    }

    public bool IsTrap
    {
      get { return this.state.Contain((uint)Tiles.Trap); }
    }

    public bool IsGoal
    {
      get { return this.state.Contain((uint)Tiles.Goal); }
    }

    public bool IsClear
    {
      get { return this.state.Contain((uint)Tiles.Clear); }
    }

    /// <summary>
    /// タイル上に何も存在しない(部屋か通路のいずれか)
    /// </summary>
    public bool IsEmpty
    {
      get {
        return (this.state.Is((uint)Tiles.Room) || this.state.Is((uint)Tiles.Aisle));
      }
    }

    /// <summary>
    /// 壁、敵、プレイヤーのいずれかに該当するタイルは障害物があるタイルとして扱う
    /// </summary>
    public bool IsObstacle
    {
      get {
        return this.ContainEither(Tiles.Wall, Tiles.Enemy, Tiles.Player);
      }
    }

    //-------------------------------------------------------------------------
    // メソッド
    //-------------------------------------------------------------------------
    public void Reset()
    {
      this.state.Set(0);
    }

    public void Set(Tiles tile)
    {
      this.state.Set((uint)tile);
    }

    public void On(params Tiles[] tiles)
    {
      this.state.On(ToFlags(tiles));
    }

    public void Off(params Tiles[] tiles)
    {
      this.state.Off(ToFlags(tiles));
    }

    public bool Contain(params Tiles[] tiles)
    {
      return this.state.Contain(ToFlags(tiles));
    }

    public bool ContainEither(params Tiles[] tiles)
    {
      return this.state.ContainEither(ToFlags(tiles));
    }

    private uint ToFlags(params Tiles[] tiles)
    {
      uint flag = 0;
      foreach(var tile in tiles) {
        flag |= (uint)tile;
      }
      return flag;
    }
  }
}

