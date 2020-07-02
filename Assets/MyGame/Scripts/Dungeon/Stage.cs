using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyExtension;

namespace Dungeon
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

    //-------------------------------------------------------------------------
    // タイル関連

		public void SetTileState(int x, int y, Tiles tile)
    {
			this.tiles[x, y].Set(tile);
    }

    public void AddTileState(int x, int y, params Tiles[] tiles)
    {
      this.tiles[x, y].On(tiles);
    }

    public void RemoveTileState(int x, int y, params Tiles[] tiles)
    {
      this.tiles[x, y].Off(tiles);
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
    // 配置関連

    /// <summary>
    /// ダンジョン内の配置可能なランダムな座標
    /// </summary>
    public Vector2Int PlaceableCoord
    {
      get {
        while(true) 
        {
          var room = this.rooms.Rand();

          var pos = room.RandomCoord;

          if (!this.tiles[pos.x, pos.y].IsEmpty) {
            continue;
          }
        
          return pos;
        }
      }
    }

    //-------------------------------------------------------------------------
    // その他
		private void Map(System.Action<int, int, Tile> cb)
		{
      Util.Loop2D(Define.WIDTH, Define.HEIGHT, (int x, int y) => {
        cb(x, y, this.tiles[x, y]);
      });
		}

    //-------------------------------------------------------------------------
    #if UNITY_EDITOR
    //-------------------------------------------------------------------------

    public void OnGUI()
    {
			GUIStyle sWall = new GUIStyle();
			GUIStyle sAisle = new GUIStyle();
			GUIStyle sRoom = new GUIStyle();
      GUIStyle sPlayer = new GUIStyle();

			GUIStyle style = null;
			sWall.normal.textColor  = Color.black;
			sAisle.normal.textColor = Color.gray;
			sRoom.normal.textColor  = Color.blue;
      sPlayer.normal.textColor = Color.yellow;

			this.Map((int x, int y, Tile tile) =>
			{
				if (tile.IsWall) style = sWall;
				if (tile.IsAisle) style = sAisle;
				if (tile.IsRoom)  style = sRoom;
        if (tile.IsPlayer) style = sPlayer;

        if (style != null) {
				  GUI.Label(new Rect(x * 10, y * 10 + 30, 10, 10), "■", style);
        }

			});
    }

    #endif
  }
}