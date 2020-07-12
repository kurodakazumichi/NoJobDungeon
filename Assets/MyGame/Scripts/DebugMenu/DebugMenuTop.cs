#if _DEBUG
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.DebugMenu
{
  /// <summary>
  /// デバッグメニュートップページ
  /// </summary>
  public class DebugMenuTop
  {
    private List<Page> pages = new List<Page>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DebugMenuTop()
    {
      DebugMenuManager.Instance.RegisterMenu(Page.Top, DrawDebug);

      pages.Clear();
      for (int i = (int)Page.PageStart + 1; i < (int)Page.PageChild; i++)
      {
        Page page = (Page)i;
        if (page == Page.Top) continue;
        pages.Add( page );
      }
    }

    private void DrawDebug(MenuWindow menuWindow, object[] args)
    {
      foreach (var page in this.pages)
      {
        if (GUILayout.Button($"{page}"))
        {
          DebugMenuManager.Instance.OpenWindow(page);
        }
      }
    }
  }
}
#endif