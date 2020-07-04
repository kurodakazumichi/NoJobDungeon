using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
  /// <summary>
  /// 範囲を指定したループ処理(Action版)
  /// </summary>
  static public void LoopByRange(int from, int to, System.Action<int> cb)
  {
    for(int i = from; i < to; ++i) {
      cb(i);
    }
  }

  /// <summary>
  /// 範囲を指定したループ処理(Func版)
  /// cbでfalseを返すとそこでループから抜ける。
  /// </summary>
  static public void LoopByRange(int from, int to, System.Func<int, bool> cb)
  {
    for (int i = from; i < to; ++i) {
      if (!cb(i)) break;
    }
  }

  /// <summary>
  /// ２次元ループ処理(Action版)
  /// </summary>
  static public void Loop2D(int rx, int ry, System.Action<int, int> cb)
  {
    for(int y = 0; y < ry; ++y) {
      for (int x = 0; x < rx; ++x) {
        cb(x, y);
      }
    }
  }

  /// <summary>
  /// ２次元ループ処理(Func版)
  /// </summary>
  static public void Loop2D(int rx, int ry, System.Func<int, int, bool> cb)
  {
    bool isBreak = false;

    for(int y = 0; y < ry; ++y) {
      for (int x = 0; x < rx; ++x) 
      {
        isBreak = cb(x, y);

        if (isBreak) break;
      }

      if(isBreak) break;
    }
  }

  /// <summary>
  /// Rectを元にした二次元ループ処理(Action版)
  /// </summary>
  static public void LoopByRect(RectInt rect, System.Action<int, int> cb)
	{
		for (int y = rect.y; y < rect.y + rect.height; ++y) 
    {
			for (int x = rect.x; x < rect.x + rect.width; ++x) {
				cb(x, y);
			}
		}
	}

  /// <summary>
  /// Rectを元にした二次元ループ処理(Func版)
  /// 
  /// </summary>
  static public void LoopByRect(RectInt rect, System.Func<int, int, bool> cb)
  {
    bool isBreak = false;

		for (int y = rect.y; y < rect.y + rect.height; ++y) 
		{
			for (int x = rect.x; x < rect.x + rect.width; ++x)
			{
        isBreak = cb(x, y);
				
        if (isBreak) break;
			}

      if (isBreak) break;
		}
  }

  /// <summary>
  /// 斜め方向です。
  /// </summary>
 public static bool IsDiagonal(Direction8 direction) 
  {
    switch(direction)
    {
      case Direction8.LeftDown:
      case Direction8.LeftUp:
      case Direction8.RightDown:
      case Direction8.RightUp:
        return true;
      default:
        return false;
    }
  }

  /// <summary>
  /// まっすぐです
  /// </summary>
  public static bool IsStraight(Direction8 direction)
  {
    switch(direction)
    {
      case Direction8.Left:
      case Direction8.Right:
      case Direction8.Up:
      case Direction8.Down:
        return true;
      default:
        return false;
    }
  }
}