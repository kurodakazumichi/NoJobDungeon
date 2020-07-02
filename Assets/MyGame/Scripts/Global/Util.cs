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
  static public void Loop2D(int x, int y, System.Action<int, int> cb)
  {
    for(int ry = 0; ry < y; ++y) {
      for (int rx = 0; rx < x; ++x) {
        cb(x, y);
      }
    }
  }

  /// <summary>
  /// ２次元ループ処理(Func版)
  /// </summary>
  static public void Loop2D(int x, int y, System.Func<int, int, bool> cb)
  {
    bool isBreak = false;

    for(int ry = 0; ry < y; ++y) {
      for (int rx = 0; rx < x; ++x) 
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
}