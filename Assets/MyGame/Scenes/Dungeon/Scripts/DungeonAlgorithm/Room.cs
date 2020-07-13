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

    public int x => (Area.x);
    public int y => (Area.y);
    public int w => (Area.width);
    public int h => (Area.height);

    public int yMin => (Area.yMin);
    public int yMax => (Area.yMax);
    public int xMin => (Area.xMin);
    public int xMax => (Area.xMax);

    /// <summary>
    /// ランダムな座標を生成する
    /// 引数にpaddingを渡す事で部屋の内側に余白を設定した上でランダムな座標を生成可能
    /// </summary>
    public Vector2Int RandomCoord(int padding = 0)
    {
      // 余白は正の値のみ
      padding = Mathf.Abs(padding);

      // 部屋のサイズを超える余白が設定されないように調整(横幅)
      if (this.area.width - (padding * 2) < 0)
      {
        padding = 0;
      }

      // 部屋のサイズを超える余白が設定されないように調整(縦幅)
      if (this.area.height - (padding * 2) < 0)
      {
        padding = 0;
      }

      // ランダムな座標を生成
      int x = this.area.x + Random.Range(0 + padding, this.area.width - padding);
      int y = this.area.y + Random.Range(0 + padding, this.area.height - padding);

      return new Vector2Int(x, y);
    }

  }
}