using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  public static class Util
  {
    /// <summary>
    /// XY座標から位置を取得する
    /// </summary>
    public static Vector3 GetPositionBy(int x, int y)
    {
      return new Vector3(x * Define.CHIP_SCALE.x, -y * Define.CHIP_SCALE.y, 0);
    }

    /// <summary>
    /// Vector2Intから位置を取得する
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    public static Vector3 GetPositionBy(Vector2Int coord)
    {
      return GetPositionBy(coord.x, coord.y);
    }

    /// <summary>
    /// XY座標+方向から位置を取得する
    /// </summary>
    public static Vector3 GetPositionBy(Vector2Int coord, Direction direction)
    {
      var next = GetCoord(coord, direction);
      return GetPositionBy(next);
    }

    /// <summary>
    /// 指定座標に方向を加えた先の座標を取得する
    /// </summary>
    public static Vector2Int GetCoord(Vector2Int coord, Direction dir)
    {
      // ダンジョン座標はY方向の上がマイナスなので、第一引数(yUp)にfalseを指定
      return coord + dir.ToVector(false);
    }
  }
}