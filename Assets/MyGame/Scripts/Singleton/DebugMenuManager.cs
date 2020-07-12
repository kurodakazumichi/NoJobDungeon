#if _DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.DebugMenu;

namespace MyGame
{
  namespace DebugMenu
  {
    public enum Page
    {
      None,
      Back,

      Top,

      Stage,
    }

  }

  public class DebugMenuManager : SingletonMonobehaviour<DebugMenuManager>
  {
    /// <summary>
    /// 表示フラグ
    /// </summary>
    private bool isShow = false;

    private Page currentPage = Page.None;

    private Page nextPage = Page.None;

    private Stack<Page> pageStack = new Stack<Page>();

    private Dictionary<Page, DrawDebugMenu> pages = new Dictionary<Page, DrawDebugMenu>();

    private DebugMenuTop topMenu;

    public delegate void DrawDebugMenu();

    /// <summary>
    /// ページ在庫があるか
    /// </summary>
    private bool hasPageStock => (pageStack.Count > 0);



    //=========================================================================

    protected override void Awake()
    {
      base.Awake();

      // Key登録
      var pageArray = System.Enum.GetValues(typeof(Page));
      foreach (var page in pageArray)
      {
        Page castedPage = (Page)page;
        if( castedPage < Page.Top ) continue;       // 対象外
        if (pages.ContainsKey(castedPage)) continue;  // 登録済み
        pages.Add(castedPage, null);
      }

      // トップ画面生成
      this.topMenu = new DebugMenuTop();
    }


    private void Update()
    {
      // メニュー表示切替
      if (Input.GetMouseButtonDown(1))
      {
        this.isShow = !this.isShow;

        // トップページ表示
        RequestOpenPage(Page.Top);
      }

      // ページ遷移チェック
      if (this.nextPage != Page.None)
      {
        OpenNextPage();
      }
    }

    /// <summary>
    /// デバッグメニューに登録する
    /// </summary>
    public void JoinMenu(Page page, DrawDebugMenu callback)
    {
      if (this.pages.ContainsKey(page) == false)
      {
        this.pages.Add(page, callback);
      }
      else
      {
        this.pages[page] += callback;
      }
    }


    /// <summary>
    /// ページ遷移要求
    /// </summary>
    /// <param name="page"></param>
    public void RequestOpenPage(Page page)
    {
      if(this.currentPage == page ) return;
      this.nextPage = page;
    }

    /// <summary>
    /// 次のページ遷移
    /// </summary>
    /// <param name="page"></param>
    private void OpenNextPage()
    {
      if (this.nextPage == Page.Back)
      {
        // 戻るだった場合はスタックから取得
        this.nextPage = (hasPageStock) ? pageStack.Pop() : Page.None;
      }
      else 
      {
        if (this.currentPage != Page.None)
        {
          // 遷移前のページを記録
          pageStack.Push(this.currentPage);
        }
      }

      if(this.nextPage == Page.None ) return;

      // ページ更新
      this.currentPage  = this.nextPage;
      this.nextPage     = Page.None;
    }


    //=========================================================================
    // OnGUI

    private Rect windowRect = new Rect(0, 0, (int)(Screen.width*0.5), (int)(Screen.height* 0.5));

    private Vector2 contentScrollViewPosition = Vector2.zero;

    private const int MainWindowId = 0;

    private void OnGUI()
    {
      if (this.isShow == false) return;

      this.windowRect = GUILayout.Window(MainWindowId, this.windowRect, MainWindowCallBack, nameof(DebugMenuManager));

    }

    /// <summary>
    /// ウィンドウコールバック
    /// </summary>
    /// <param name="windowId"></param>
    private void MainWindowCallBack(int windowId)
    {
      // ヘッダー
      DrawHeader();

      // コンテンツ
      DrawContent();

      // ドラッグ可能
      GUI.DragWindow();

    }

    /// <summary>
    /// ヘッダー描画
    /// </summary>
    private void DrawHeader()
    {
      using (var v = new GUILayout.VerticalScope())
      {
        // ページタイトル
        GUILayout.Label(this.currentPage.ToString());

        // ボタン一覧
        using (var h = new GUILayout.HorizontalScope())
        {
          // 戻る
          {
            if (hasPageStock)
            {
              if (GUILayout.Button("戻る"))
              {
                RequestOpenPage( Page.Back );
              }
            }
            else
            {
              if (GUILayout.Button("閉じる"))
              {
                this.isShow = false;
              }
            }
            
          }
        }
      }

    }

    /// <summary>
    /// 内容描画
    /// </summary>
    private void DrawContent()
    {
      using (var sv = new GUILayout.ScrollViewScope(contentScrollViewPosition))
      {
        var menu = pages[currentPage];
        if (menu != null)
        {
          menu.Invoke();
        }
        else
        {
          GUILayout.Label($"{currentPage}のメニューはまだ登録されてないよ！");
        }
      }
    }

    
    /// <summary>
    /// トップメニュー
    /// </summary>
    private class DebugMenuTop
    {
      public DebugMenuTop()
      {
        DebugMenuManager.Instance.JoinMenu(Page.Top, DrawDebug);
      }

      private void DrawDebug()
      {
        var pages = DebugMenuManager.Instance.pages;
        foreach (var page in pages)
        {
          if( page.Key == Page.Top ) continue;
          if (GUILayout.Button($"{page.Key}"))
          {
            DebugMenuManager.Instance.RequestOpenPage( page.Key );
          }
        }
      }
    }
  }
}
#endif
