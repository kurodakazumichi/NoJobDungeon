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

    //-------------------------------------------------------------------------
    public Stage()
    {
      this.tiles = new Tile[Define.WIDTH, Define.HEIGHT];

      Map((int x, int y, Tile _) => {
        this.tiles[x, y] = new Tile();
      });

      this.Reset();
    }

    public void Reset()
    {
		  Map((int x, int y, Tile tile) => {
        tile.Reset();
      });

			this.rooms = new List<Room>();
    }

    public void AddRoom(RectInt area)
    {
      this.rooms.Add(new Room(area));
    }

		public void Set(int x, int y, Tiles tile)
    {
			this.tiles[x, y].Set(tile);
    }

    public int RoomCount {
      get { return this.rooms.Count; }
    }

    /// <summary>
    /// ダンジョン内の配置可能なランダムな座標を取得する 
    /// </summary>
    public Vector2Int GetPlaceableCoord()
    {
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

    public Vector2Int MovesPlayer(Vector2Int from, Vector2Int to)
    {
      this.tiles[from.x, from.y].Off(Tiles.Player);
      this.tiles[to.x, to.y].On(Tiles.Player);
      return to;
    }

    /// <summary>
    /// プレイヤーを配置する
    /// </summary>
    /// <returns>配置した座標</returns>
    public Vector2Int PlacesPlayer()
    {
      var pos = GetPlaceableCoord();
      this.tiles[pos.x, pos.y].On(Tiles.Player);
      return pos;
    }

    // 部屋の領域内に空きがあるか判定する
    private bool ChecksForRoomAvailability(Room room)
    {
      bool hasEmpty = false;
      Util.MapByRect(room.Area, (int x, int y) => 
      {
        if (this.tiles[x, y].IsEmpty) {
          hasEmpty = true;
          return false;
        }

        return true;
      });
      return hasEmpty;
    }

		private void Map(System.Action<int, int, Tile> cb)
		{
			for (int x = 0; x < Define.WIDTH; x++)
			{
				for (int y = 0; y < Define.HEIGHT; y++)
				{
					cb(x, y, this.tiles[x, y]);
				}
			}
		}

    //-------------------------------------------------------------------------
    #if UNITY_EDITOR
    //-------------------------------------------------------------------------
    private bool _toggleDetail = true;

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