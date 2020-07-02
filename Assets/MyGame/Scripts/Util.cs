using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
  /// <summary>
  /// 範囲を指定したループ処理
  /// </summary>
  static public void MapByRange(int from, int to, System.Action<int> cb)
  {
    for(int i = from; i < to; ++i)
    {
      cb(i);
    }
  }

  static public void MapByRect(RectInt rect, System.Action<int, int> cb)
	{
		for (int x = rect.x; x < rect.x + rect.width; ++x)
		{
			for (int y = rect.y; y < rect.y + rect.height; ++y)
			{
				cb(x, y);
			}
		}
	}

}