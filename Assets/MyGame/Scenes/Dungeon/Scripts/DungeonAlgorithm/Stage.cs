using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// ダンジョンステージクラス
  /// </summary>
	public class Stage
  {
    /// <summary>
    /// ステージ配列
    /// </summary>
		private Tile[,] tiles;

    /// <summary>
    /// ルームリスト
    /// </summary>
		private List<Room> rooms;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Stage()
    {
      this.tiles = new Tile[Define.WIDTH, Define.HEIGHT];

      Map((int x, int y, Tile _) => {
        this.tiles[x, y] = new Tile();
      });

      this.Reset();
    }

    /// <summary>
    /// メンバ変数の中身をリセットする
    /// </summary>
    public void Reset()
    {
		  Map((int x, int y, Tile tile) => {
        tile.Reset();
      });

			this.rooms = new List<Room>();
    }

    /// <summary>
    /// ダンジョン生成
    /// </summary>
    public void Make(Algorithm algorithm)
    {
      // マップの生成
      algorithm.Make(this);

      // プレイヤーの生成
      MakePlayer();

      // ゴールの生成
      MakeGoal();

      // アイテムの配置
      MakeItem();

      // 敵の配置
      MakeEnemy();

      // 罠の配置
    }

    //-------------------------------------------------------------------------
    // タイル関連

    public IReadOnlyTile[,] Tiles
    {
      get { return this.tiles; }
    }

    public IReadOnlyTile GetTile(int x, int y)
    {
      return this.tiles[x, y];
    }

    public IReadOnlyTile GetTile(Vector2Int coord)
    {
      return this.tiles[coord.x, coord.y];
    }

		public void SetTileState(int x, int y, Tiles tile)
    {
			this.tiles[x, y].Set(tile);
    }

    public void AddTileState(int x, int y, params Tiles[] tiles)
    {
      this.tiles[x, y].On(tiles);
    }
    public void AddTileState(Vector2Int coord, params Tiles[] tiles)
    {
      AddTileState(coord.x, coord.y, tiles);
    }

    public void RemoveTileState(int x, int y, params Tiles[] tiles)
    {
      this.tiles[x, y].Off(tiles);
    }
    public void RemoveTileState(Vector2Int coord, params Tiles[] tiles)
    {
      RemoveTileState(coord.x, coord.y, tiles);
    }

    //-------------------------------------------------------------------------
    // ルーム関連

    /// <summary>
    /// 部屋の数
    /// </summary>
    public int RoomCount {
      get { return this.rooms.Count; }
    }

    /// <summary>
    /// 部屋を追加する
    /// </summary>
    public void AddRoom(RectInt area)
    {
      this.rooms.Add(new Room(area));
    }

    //-------------------------------------------------------------------------
    // 生成関連

    /// <summary>
    /// プレイヤーの生成
    /// </summary>
    public void MakePlayer()
    {
      var pos = GetPlaceableCoord(1);
      AddTileState(pos.x, pos.y, Dungeon.Tiles.Player);
    }

    /// <summary>
    /// ゴール生成
    /// </summary>
    public void MakeGoal()
    {
      var pos = GetPlaceableCoord(1);
      AddTileState(pos.x, pos.y, Dungeon.Tiles.Goal);
    }

    /// <summary>
    /// アイテムの生成
    /// </summary>
    public void MakeItem()
    {
      MyGame.Util.LoopByRange(0, 10, (int i) => {
        var pos = GetPlaceableCoord();
        AddTileState(pos.x, pos.y, Dungeon.Tiles.Item);
      });
    }

    public void MakeEnemy()
    {
      MyGame.Util.LoopByRange(0, 10, (int i) => {
        var pos = GetPlaceableCoord();
        AddTileState(pos.x, pos.y, Dungeon.Tiles.Enemy);
      });
    }

    //-------------------------------------------------------------------------
    // 配置関連

    /// <summary>
    /// ダンジョン内の配置可能なランダムな座標を取得
    /// </summary>
    private Vector2Int GetPlaceableCoord(int padding = 0)
    {
      while(true)
      {
        var room = this.rooms.Rand();

        var coord = room.RandomCoord(padding);

        if (!this.tiles[coord.x, coord.y].IsEmpty)
        {
          continue;
        }
        
        return coord;
      }
    }

    //-------------------------------------------------------------------------
    // 探す

    /// <summary>
    /// 指定したタイルの位置を見つける
    /// </summary>
    public List<Vector2Int> Find(params Tiles[] tiles)
    {
      var list = new List<Vector2Int>();

      // 指定されたタイルが見つかったらループを抜ける
      Map((int x, int y, Tile tile) => 
      {
        var isFound = tile.Contain(tiles);

        if (isFound) {
          list.Add(new Vector2Int(x, y));
        }

        return isFound;
      });

      return list;
    }

    /// <summary>
    /// 指定したタイルに該当する座標を全て取得する
    /// </summary>
    public List<Vector2Int> FindAll(params Tiles[] tiles)
    {
      var list = new List<Vector2Int>();

      Map((int x, int y, Tile tile) => 
      {
        if (tile.Contain(tiles)) {
          list.Add(new Vector2Int(x, y));
        }

      });

      return list;
    }

    //-------------------------------------------------------------------------
    // マップの踏破

    /// <summary>
    /// 指定された座標をもとに踏破フラグを更新する
    /// 基本は指定された座標の周囲１マスを更新するが
    /// 指定された座標が部屋内だった場合は部屋を含むその周囲１マスを更新する。
    /// </summary>
    public void UpdateClearFlag(Vector2Int coord)
    {
      // 指定座標の周囲１マスの範囲を定義
      var rect = new RectInt(coord.x - 1, coord.y - 1, 3, 3);

      // 指定された座標が部屋の範囲内だった場合は
      // 該当する部屋の周囲１マスを含む範囲に変更
      var room = GetRoomBy(coord);

      if (room != null)
      {
        rect = new RectInt(room.x - 1, room.y - 1, room.w + 2, room.h + 2);
      }

      // 矩形範囲の踏破フラグを更新
      UpdateClearFlag(rect);
    }

    /// <summary>
    /// 矩形情報をもとに踏破フラグを更新する
    /// </summary>
    private void UpdateClearFlag(RectInt rect)
    {
      MyGame.Util.LoopByRect(rect, (x, y) =>
      {
        AddTileState(x, y, Dungeon.Tiles.Clear);
      });
    }

    /// <summary>
    /// 指定された座標に存在する部屋を取得する
    /// </summary>
    private Room GetRoomBy(Vector2Int coord)
    {
      Room foundRoom = null;

      foreach(var room in this.rooms) 
      { 
        if (room.Area.Contains(coord))
        {
          foundRoom = room;
          break;
        }
      };

      return foundRoom;
    }

    //-------------------------------------------------------------------------
    // その他

    public void Map(System.Action<int, int, Tile> cb)
		{
      MyGame.Util.Loop2D(Define.WIDTH, Define.HEIGHT, (int x, int y) => {
        cb(x, y, this.tiles[x, y]);
      });
		}

    public void Map(System.Func<int, int, Tile, bool> cb)
    {
      MyGame.Util.Loop2D(Define.WIDTH, Define.HEIGHT, (int x, int y) => {
        return cb(x, y, this.tiles[x, y]);
      });
    }

#if _DEBUG
    private int _mode = 0;

    private string[] _modes = new string[]
    {
      "Full", "Normal", "Clear"
    };

    public void DrawDebugMenu( MyDebug.Window window )
    {
      this._mode = GUILayout.SelectionGrid(this._mode, this._modes, this._modes.Length);

      GUIStyle sWall   = new GUIStyle();
      GUIStyle sAisle  = new GUIStyle();
      GUIStyle sRoom   = new GUIStyle();
      GUIStyle sPlayer = new GUIStyle();
      GUIStyle sGoal   = new GUIStyle();
      GUIStyle sItem   = new GUIStyle();
      GUIStyle sEnemy  = new GUIStyle();

      GUIStyle style = null;
      sWall.normal.textColor   = Color.black;
      sAisle.normal.textColor  = Color.gray;
      sRoom.normal.textColor   = Color.blue;
      sPlayer.normal.textColor = Color.white;
      sGoal.normal.textColor   = Color.yellow;
      sItem.normal.textColor   = Color.cyan;
      sEnemy.normal.textColor  = Color.red;

      this.Map((int x, int y, Tile tile) =>
      {
        if (tile.IsWall) style   = sWall;
        if (tile.IsAisle) style  = sAisle;
        if (tile.IsRoom) style   = sRoom;
        if (tile.IsPlayer) style = sPlayer;
        if (tile.IsGoal) style   = sGoal;
        if (tile.IsItem) style   = sItem;
        if (tile.IsEnemy) style  = sEnemy;

        // 通常モードの場合、踏破してない場所は壁で代用
        if (this._mode == 1 && !tile.IsClear)
        {
          style = sWall;
        }

        // 踏破モードの場合は踏破している箇所のみ色を付ける
        if (this._mode == 2)
        {
          style = (tile.IsClear)? sGoal : sWall;
        }

        if (style != null)
        {
          bool isNewLine = ( x % Define.WIDTH == 0 );
          bool isEndLine = ( x != 0 && x % (Define.WIDTH-1) == 0);

          if (isNewLine)
          {
            GUILayout.BeginHorizontal();
          }

          const int s = 7;
          GUILayout.Label( "■", style, GUILayout.Width(s), GUILayout.Height(s));

          if (isEndLine)
          {
            GUILayout.EndHorizontal();
          }
        }
      });

    }
#endif
  }
}