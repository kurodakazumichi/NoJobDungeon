#if _DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.MyDebug;

namespace MyGame
{

  public class DebugManager : SingletonMonobehaviour<DebugManager>
  {

    public IEnumerable<string> Pages => (IDebuggeables.Keys);

    private static Top top;

    private List<Window> windows = new List<Window>();

    private int windowNumber = 0;

    public Vector2 DefaultWindowSize => new Vector2(Screen.width * 0.6f, Screen.height * 0.6f );
    public Vector2 DefaultWindowHalfSize => DefaultWindowSize * 0.5f;


    private Dictionary<string, System.Func<IDebuggeable>> IDebuggeables = new Dictionary<string, System.Func<IDebuggeable>>()
    {
      { Window.PageTop,
        () => { return top; } },

      { nameof(Dungeon.DungeonScene),
        () => { return FindObjectOfType<Dungeon.DungeonScene>(); }  },

      { nameof(Dungeon.DungeonManager), 
        () => { return Dungeon.DungeonManager.HasInstance ? Dungeon.DungeonManager.Instance : null; } },

      { nameof(InputManager),           
        () => { return InputManager.HasInstance ? InputManager.Instance : null; } },

      { nameof(MapChipFactory),         
        () => { return MapChipFactory.HasInstance ? MapChipFactory.Instance : null; } },

      { nameof(Dungeon.PlayerManager),  
        () => { return Dungeon.PlayerManager.HasInstance ? Dungeon.PlayerManager.Instance : null; } },

      { nameof(Dungeon.EnemyManager),   
        () => { return Dungeon.EnemyManager.HasInstance ? Dungeon.EnemyManager.Instance : null; } },

      { nameof(Master.ItemMaster),      
        () => { return Master.ItemMaster.HasInstance ? Master.ItemMaster.Instance : null; } },

      { nameof(Master.ItemGroupMaster),
        () => { return Master.ItemGroupMaster.HasInstance ? Master.ItemGroupMaster.Instance : null; } },

      { nameof(Master.EnemyMaster),
        () => { return Master.EnemyMaster.HasInstance ? Master.EnemyMaster.Instance : null; } },

      { nameof(Dungeon.ActionManager),
        () => { return Dungeon.ActionManager.HasInstance ? Dungeon.ActionManager.Instance : null; }},
    };

    //=========================================================================


    protected override void Awake()
    {
      base.Awake();

      // トップ画面生成
      top = top ?? new Top();

      // 初期化
      this.windowNumber = 0;
    }


    private void Update()
    {
      if (Input.GetMouseButtonDown(1))
      {
        if (windows.Count > 0)
        {
          // 全て閉じる
          windows.Clear();
          windowNumber = 0;
        }
        else
        {
          // OnGUIに合わせたマウス位置
          var mousePosition = new Vector2(Input.mousePosition.x, (Screen.height - Input.mousePosition.y));

          // 新規ウィンドウ
          float x = mousePosition.x - (DefaultWindowHalfSize.x);
          float y = mousePosition.y - DefaultWindowSize.y * 0.1f;

          Vector2 pos = new Vector2(x, y);
          OpenWindow(Window.PageTop, new Rect(pos, DefaultWindowSize));
        }
      }

      // ウィンドウ更新
      for( int i = windows.Count-1; i >=0;i-- )
      {
        var window = windows[i];
        window.Update();
        if (window.IsClosed)
        {
          windows.RemoveAt(i);
        }
      }
    }

    /// <summary>
    /// 登録されたページ描画取得
    /// </summary>
    public IDebuggeable GetDrawer(string page)
    {
      if (IDebuggeables.TryGetValue(page, out System.Func<IDebuggeable> access )
          && access != null)
      {
        var iDebuggeable = access.Invoke();
        return iDebuggeable;
      }

      return null;
    }

    /// <summary>
    /// ウィンドウを開く
    /// </summary>
    public void OpenWindow(string page, Draw callback = null )
    {
      var newWindow = new Window();
      newWindow.Open(windowNumber++, page, callback);
      windows.Add(newWindow);
    }

    /// <summary>
    /// ウィンドウを開く(Rect指定あり）
    /// </summary>
    public void OpenWindow(string page, Rect rect, Draw callback = null )
    {
      var newWindow = new Window();
      newWindow.Open(windowNumber++, rect, page, callback);
      windows.Add(newWindow);
    }

    //=========================================================================
    // OnGUI

    private void OnGUI()
    {
      // スタイルの初期化
      InitGUIStyle();

      foreach (var window in windows)
      {
        window.Draw();
      }
    }

    /// <summary>
    /// スタイル初期化
    /// </summary>
    private void InitGUIStyle()
    {

    }
  }
}
#endif
