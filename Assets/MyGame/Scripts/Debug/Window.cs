#if _DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.MyDebug
{
  /// <summary>デバッグメニューウィンドウ描画</summary>
  public delegate void Draw(Window window);

  /// <summary>
  /// デバッグメニューウィンドウ
  /// </summary>
  public class Window
  {
    /// <summary>閉じたか</summary>
    public bool IsClosed { get; private set; } = false;

    /// <summary>現在のページ</summary>
    private string currentPage = string.Empty;

    /// <summary>遷移先のページ</summary>
    private string nextPage = string.Empty;

    /// <summary>ページ履歴</summary>
    private Stack<string> pageHistory = new Stack<string>();

    /// <summary>ページ履歴があるか</summary>
    private bool hasPageHistory => (pageHistory.Count > 0);

    /// <summary>ウィンドウrect</summary>
    private Rect windowRect;

    /// <summary>スクロール位置</summary>
    private Vector2 scrollPosition = Vector2.zero;

    /// <summary>ウィンドウID</summary>
    public int Id { get; private set; } = 0;

    private Dictionary<string, Draw> pageDraws = new Dictionary<string, Draw>();

    private IDebuggeable drawerCache = null;

    public const string PageTop   = "Top";
    public const string PageBack  = "Back";

    /// <summary>
    /// ウィンドウを開く
    /// </summary>
    public void Open(int id, string page, Draw callback)
    {
      // 位置オフセット
      const float offset = 10f;

      this.windowRect     = new Rect(id * offset, id * offset, Screen.width * 0.5f, Screen.height * 0.5f);
      this.Id             = id;
      this.scrollPosition = Vector2.zero;
      this.IsClosed       = false;
      this.drawerCache    = null;
      pageDraws.Clear();
      if (callback != null)
      {
        pageDraws.Add(page, callback);
      }

      OpenPage(page);
    }

    /// <summary>
    /// ウィンドウを開く（Rect指定あり）
    /// </summary>
    public void Open(int id, Rect rect, string page, Draw callback)
    {
      this.windowRect     = rect;
      this.Id             = id;
      this.scrollPosition = Vector2.zero;
      this.IsClosed       = false;
      this.drawerCache    = null;
      pageDraws.Clear();
      if (callback != null)
      {
        RegisterDraw(page, callback);
      }

      OpenPage(page);
    }

    /// <summary>
    /// 閉じる
    /// </summary>
    public void Close()
    {
      this.IsClosed = true;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    public void Update()
    {
      if (this.IsClosed) return;

      // ページ遷移チェック
      if (this.nextPage != string.Empty)
      {
        TransitionNextPage();
      }
    }

    /// <summary>
    /// ページを開く
    /// </summary>
    public void OpenPage(string page)
    {
      if (this.currentPage == page) return;
      this.nextPage = page;
    }

    /// <summary>
    /// ページ描画の登録
    /// </summary>
    public void RegisterDraw(string page, Draw callback)
    {
      if (pageDraws.ContainsKey(page))
      {
        pageDraws[page] = callback;
      }
      else
      {
        pageDraws.Add(page, callback);
      }
    }

    /// <summary>
    /// 次のページ遷移
    /// </summary>
    private void TransitionNextPage()
    {
      if (this.nextPage == PageBack)
      {
        // 戻るだった場合はスタックから取得
        this.nextPage = (hasPageHistory) ? pageHistory.Pop() : string.Empty;
      }
      else
      {
        if (this.currentPage != string.Empty)
        {
          // 遷移前のページを記録
          pageHistory.Push(this.currentPage);
        }
      }

      if (this.nextPage == string.Empty) return;

      // ページ更新
      this.currentPage  = this.nextPage;
      this.nextPage     = string.Empty;
      this.drawerCache    = null;
    }

    //=========================================================================
    // Draw

    public void Draw()
    {
      if( this.IsClosed ) return;

      string title = $"DebugMenu[{Id}] - {currentPage}";
      this.windowRect = GUILayout.Window(Id, this.windowRect, DrawWindowCallBack, title);
    }


    /// <summary>
    /// ウィンドウコールバック
    /// </summary>
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
      //using (var v = new GUILayout.VerticalScope())
      {
        // ボタン一覧
        using (var h = new GUILayout.HorizontalScope())
        {
          float buttonSize = 30f;

          // 戻る
          using (var d = new DisabledScope(hasPageHistory == false))
          {
            if (GUILayout.Button("←", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
            {
              OpenPage(PageBack);
            }
          }

          // スペース
          GUILayout.FlexibleSpace();

          // 閉じる
          if (GUILayout.Button("×", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize)))
          {
            this.Close();
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
        // スクロール更新
        scrollPosition = sv.scrollPosition;

        // ページ内容描画
        if (pageDraws.TryGetValue(currentPage, out Draw draw))
        {
          draw?.Invoke(this);
        }
        else
        {
          var drawer = DebugManager.Instance.GetDrawer(currentPage);
          if (drawer != null)
          {
            drawer?.Draw(this);
            drawerCache = drawer;
          }
        }
      }
    }
  }
}
#endif