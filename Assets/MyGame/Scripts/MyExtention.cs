using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  /// <summary>
  /// 拡張メソッド
  /// </summary>
  public static class MyExtension
  {
    /// <summary>
    /// List<T>型にIndex付きのForEachを拡張
    /// </summary>
    /// <param name="items">リスト</param>
    /// <param name="cb">コールバック</param>
    public static void ForEach<T>(this List<T> items, System.Action<T, int> cb)
    {
      for(int i = 0; i < items.Count; ++i) cb(items[i], i);
    }

    public static T First<T>(this List<T> items)
    {
      return items[0];
    }

    public static T Last<T>(this List<T> items)
    {
      return items[items.Count - 1];
    }

    public static void Add<T>(this List<T> items, List<T> adds)
    {
      foreach(var item in adds) {
        items.Add(item);
      }
    }

    public static T TryGet<T>(this List<T> items, int index) where T:class
    {
      if (items.Count <= index) return null;
      return items[index];
    }

    public static T Rand<T>(this List<T> items)
    {
      int index = Random.Range(0, items.Count);
      return items[index];
    }
  }
}
