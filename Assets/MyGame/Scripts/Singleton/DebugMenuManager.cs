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

      PageStart, // ページの追加はここより↓
      Top,
      Input,
      MapChip,
      Dungeon,
      Player,
      Enemy,
      ItemMaster,
      ItemGroupMaster,
      EnemyMaster,
      
      PageChild, // トップ画面に表示しないものはここより↓
      Algorithm,
      EnemyDetail,

      PageEnd, // ページの追加はここより↑
    }

    public delegate void DrawMenu( MenuWindow menuWindow );
  }

  public class DebugMenuManager : SingletonMonobehaviour<DebugMenuManager>
  {

    private Dictionary<Page, List<MenuContent>> pageContents = new Dictionary<Page, List<MenuContent>>();

    public IEnumerable<Page> Pages => (pageContents.Keys);

    private DebugMenuTop topMenu;

    private List<MenuWindow> menuWindows = new List<MenuWindow>();

    private int windowNumber = 0;

    public readonly Vector2 DefaultWindowSize = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f );

    //=========================================================================

    public List<MenuContent> GetMenuContent(Page page)
    {
      if (pageContents.ContainsKey(page))
      {
        return pageContents[page];
      }
      return null;
    }

    protected override void Awake()
    {
      base.Awake();

      // Key登録
      for (int i = (int)Page.PageStart + 1; i < (int)Page.PageEnd; i++)
      {
        Page page = (Page)i; 
        if(page == Page.PageChild ) continue;          // 対象外 
        if (pageContents.ContainsKey(page)) continue;  // 登録済み
        pageContents.Add(page, new List<MenuContent>());
      }

      // トップ画面生成
      this.topMenu = new DebugMenuTop();

      // 初期化
      this.windowNumber = 0;
    }


    private void Update()
    {
      if (Input.GetMouseButtonDown(1))
      {
        if (menuWindows.Count > 0)
        {
          // 全て閉じる
          menuWindows.Clear();
          windowNumber = 0;
        }
        else
        {
          // 新規ウィンドウ

          // Y座標の原点が違うので調整
          Vector2 pos = new Vector2(Input.mousePosition.x, Screen.height- Input.mousePosition.y);
          OpenWindow(Page.Top, new Rect(pos, DefaultWindowSize));
        }
      }

      // ウィンドウ更新
      for( int i = menuWindows.Count-1; i >=0;i-- )
      {
        var window = menuWindows[i];
        window.Update();
        if (window.IsClosed)
        {
          menuWindows.RemoveAt(i);
        }
      }
    }

    /// <summary>
    /// ウィンドウを開く
    /// </summary>
    public void OpenWindow(Page page, DrawMenu callback = null )
    {
      var newWindow = new MenuWindow();
      newWindow.Open(windowNumber++, page, callback);
      menuWindows.Add(newWindow);
    }

    /// <summary>
    /// ウィンドウを開く(Rect指定あり）
    /// </summary>
    public void OpenWindow(Page page, Rect rect, DrawMenu callback = null)
    {
      var newWindow = new MenuWindow();
      newWindow.Open(windowNumber++, rect, page, callback);
      menuWindows.Add(newWindow);
    }

    /// <summary>
    /// デバッグメニューに登録する
    /// </summary>
    public void RegisterMenu(Page page, DrawMenu callback, string title = "")
    {
      if (this.pageContents.ContainsKey(page) == false)
      {
        // 新規追加
        var content = new MenuContent
        {
          Title     = title,
          DrawMenu  = callback
        };
        this.pageContents.Add(page, new List<MenuContent>() { content });
      }
      else
      {
        // 同一タイトルものを探す
        var content = this.pageContents[page].Find( x => x.Title == title );
        if (content != null)
        {
          // コールバック追加
          content.DrawMenu += callback;
        }
        else
        {
          // 新規タイトル
          content = new MenuContent
          {
            Title = title,
            DrawMenu = callback
          };
          this.pageContents[page].Add(content);
        }
      }
    }


    //=========================================================================
    // OnGUI

    /// <summary>
    /// タイトルラベル用スタイル
    /// </summary>
    public GUIStyle ContentTitleStyle;

    private void OnGUI()
    {
      // スタイルの初期化
      InitGUIStyle();

      foreach (var window in menuWindows)
      {
        window.DrawDebugMenu();
      }
    }

    /// <summary>
    /// スタイル初期化
    /// </summary>
    private void InitGUIStyle()
    {
      if (ContentTitleStyle == null)
      {
        ContentTitleStyle = new GUIStyle(GUI.skin.label)
        {
          alignment = TextAnchor.MiddleCenter
        };
      }
    }
  }
}
#endif
