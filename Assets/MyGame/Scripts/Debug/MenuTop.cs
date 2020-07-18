#if _DEBUG
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.MyDebug
{
  /// <summary>
  /// デバッグメニュートップページ
  /// </summary>
  public class MenuTop : IDebuggeable
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MenuTop()
    {
    }

#if _DEBUG
    void IDebuggeable.Draw(MenuWindow window)
    {
      foreach (var page in DebugManager.Instance.Pages)
      {
        // トップが自分なので除外
        if( page == MenuWindow.PageTop ) continue;
        
        var draw = DebugManager.Instance.GetDrawer(page);
        using (var d = new DisabledScope(draw == null))
        {
          if (GUILayout.Button($"{page}"))
          {
            DebugManager.Instance.OpenWindow(page);
          }
        }
      }
    }
#endif
  }
}
#endif