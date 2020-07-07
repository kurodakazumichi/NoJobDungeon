using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// ルームクラス
  /// </summary>
  public class Room {

    /// <summary>
    /// 部屋の領域情報
    /// </summary>
    private RectInt area;

    //-------------------------------------------------------------------------
    // コンストラクタ
    //-------------------------------------------------------------------------
    public Room()
    {
      this.area = new RectInt();
    }

    public Room(RectInt rect)
    {
      this.area = rect;
    }

    public Room(int x, int y, int w, int h)
    {
      this.area = new RectInt(x, y, w, h);
    }

    //-------------------------------------------------------------------------
    // プロパティ
    //-------------------------------------------------------------------------

    public bool isEnable
    {
      get 
      {
        if (this.area.x == 0) return false;
        if (this.area.y == 0) return false;
        if (this.area.width == 0) return false;
        if (this.area.height == 0) return false;

        return true;
      }
    }

    public RectInt Area
    {
      get { return this.area; }
    }

    public int yMin
    {
      get { return this.area.yMin; }
    }

    public int yMax
    {
      get { return this.area.yMax; }
    }

    public int xMin
    {
      get { return this.area.xMin; }
    }

    public int xMax
    {
      get { return this.area.xMax; }
    }

    public Vector2Int RandomCoord
    {
      get {
        int x = this.area.x + Random.Range(0, this.area.width);
        int y = this.area.y + Random.Range(0, this.area.height);
        return new Vector2Int(x, y);
      }
    }

  }
}