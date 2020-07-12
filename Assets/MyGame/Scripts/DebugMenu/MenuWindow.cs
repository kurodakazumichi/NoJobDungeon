#if _DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.DebugMenu
{

  public class MenuContent
  {
    public string Title;
    public DrawMenu DrawMenu;
  }

  /// <summary>
  /// デバッグメニューウィンドウ
  /// </summary>
  public class MenuWindow
  {
    public bool IsClosed { get; private set; } = false;

    private Page currentPage = Page.None;

    private Page nextPage = Page.None;

    private Stack<Page> pageStack = new Stack<Page>();

    private object[] currentArguments;

    private object[] nextArguments;

    /// <summary>
    /// ページ在庫があるか
    /// </summary>
    private bool hasPageStock => (pageStack.Count > 0);

    private Rect windowRect;

    private Vector2 scrollPosition = Vector2.zero;

    public int Id { get; private set; }  = 0;


    /// <summary>
    /// ウィンドウを開く
    /// </summary>
    public void Open(int id, Page page, params object[] args )
    {
      const float offset = 10f;

      this.windowRect     = new Rect( id * offset, id * offset, Screen.width * 0.5f, Screen.height * 0.5f ) ;
      this.Id             = id;
      this.scrollPosition = Vector2.zero;
      this.IsClosed       = false;
      OpenPage(page, args);
    }

    /// <summary>
    /// ウィンドウを開く（Rect指定あり）
    /// </summary>
    public void Open(int id, Rect rect, Page page, params object[] args)
    {
      this.windowRect     = rect;
      this.Id             = id;
      this.scrollPosition = Vector2.zero;
      this.IsClosed       = false;
      OpenPage(page, args);
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    public void Update()
    {
      if(this.IsClosed) return;

      // ページ遷移チェック
      if (this.nextPage != Page.None)
      {
        TransitionNextPage();
      }
    }

    /// <summary>
    /// ページを開く
    /// </summary>
    public void OpenPage(Page page, params object[] args)
    {
      if (this.currentPage == page) return;
      this.nextPage          = page;
      this.nextArguments = args;
    }

    /// <summary>
    /// 次のページ遷移
    /// </summary>
    private void TransitionNextPage()
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

      if (this.nextPage == Page.None) return;

      // ページ更新
      this.currentPage        = this.nextPage;
      this.currentArguments      = this.nextArguments;
      this.nextPage           = Page.None;
      this.nextArguments  = null;
    }

    //=========================================================================
    // Draw


    public void DrawDebugMenu()
    {
      if( this.IsClosed ) return;

      this.windowRect = GUILayout.Window(Id, this.windowRect, DrawWindowCallBack, $"{nameof(DebugMenuManager)}[{Id}]");
    }


    /// <summary>
    /// ウィンドウコールバック
    /// </summary>
    /// <param name="windowId"></param>
    private void DrawWindowCallBack(int windowId)
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
                OpenPage(Page.Back);
              }
            }
            else
            {
              if (GUILayout.Button("閉じる"))
              {
                this.IsClosed = true;
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
      using (var sv = new GUILayout.ScrollViewScope(scrollPosition))
      {
        scrollPosition = sv.scrollPosition;
        var content = DebugMenuManager.Instance.GetMenuContent(currentPage);
        if (content != null && content.Count > 0)
        {
          // ページ引数
          content.ForEach((x) =>
            {
              // タイトル
              GUILayout.Label($"{x.Title}", DebugMenuManager.Instance.ContentTitleStyle);
              
              x.DrawMenu?.Invoke(this, currentArguments);
            });
        }
        else
        {
          GUILayout.Label($"{currentPage}のメニューはまだ登録されてないよ！");
        }
      }
    }

  }
}
#endif