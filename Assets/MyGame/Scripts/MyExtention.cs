using System.Collections.Generic;

namespace MyExtension
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
  }
}
