﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
  public static class Util
  {
    /// <summary>
    /// 範囲を指定したループ処理(Action版)
    /// </summary>
    static public void LoopByRange(int from, int to, System.Action<int> cb)
    {
      for (int i = from; i < to; ++i)
      {
        cb(i);
      }
    }

    /// <summary>
    /// 範囲を指定したループ処理(Func版)
    /// cbでfalseを返すとそこでループから抜ける。
    /// </summary>
    static public void LoopByRange(int from, int to, System.Func<int, bool> cb)
    {
      for (int i = from; i < to; ++i)
      {
        if (!cb(i)) break;
      }
    }

    /// <summary>
    /// ２次元ループ処理(Action版)
    /// </summary>
    static public void Loop2D(int rx, int ry, System.Action<int, int> cb)
    {
      for (int y = 0; y < ry; ++y)
      {
        for (int x = 0; x < rx; ++x)
        {
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

      for (int y = 0; y < ry; ++y)
      {
        for (int x = 0; x < rx; ++x)
        {
          isBreak = cb(x, y);

          if (isBreak) break;
        }

        if (isBreak) break;
      }
    }

    /// <summary>
    /// Rectを元にした二次元ループ処理(Action版)
    /// </summary>
    static public void LoopByRect(RectInt rect, System.Action<int, int> cb)
    {
      for (int y = rect.y; y < rect.y + rect.height; ++y)
      {
        for (int x = rect.x; x < rect.x + rect.width; ++x)
        {
          cb(x, y);
        }
      }
    }

    /// <summary>
    /// Rectを元にした二次元ループ処理(Action版)
    /// </summary>
    static public void LoopByRect(RectInt rect, System.Action<int, int, int> cb)
    {
      int index = 0;
      for (int y = rect.y; y < rect.y + rect.height; ++y)
      {
        for (int x = rect.x; x < rect.x + rect.width; ++x)
        {
          cb(x, y, index);
          ++index;
        }
      }
    }

    /// <summary>
    /// Rectを元にした二次元ループ処理(Func版)
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
    /// リストをもとにしたループ
    /// </summary>
    static public void Loop<T>(List<T> list, System.Action<T> cb)
    {
      list.ForEach(cb);
    }

    /// <summary>
    /// リストをもとにしたループ(Func版)
    /// </summary>
    static public void Loop<T>(List<T> list, System.Func<T, bool> cb)
    {
      for (int i = 0; i < list.Count; ++i)
      {
        if (!cb(list[i])) break;
      }
    }
  }
}