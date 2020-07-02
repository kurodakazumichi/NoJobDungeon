using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {

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
	}

  /// <summary>
  /// タイルクラス
  /// </summary>
  public class Tile 
  {
    /// <summary>
    /// タイルの状態
    /// </summary>
    private BitFlag state;

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

    public bool IsTrap
    {
      get { return this.state.Contain((uint)Tiles.Trap); }
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

