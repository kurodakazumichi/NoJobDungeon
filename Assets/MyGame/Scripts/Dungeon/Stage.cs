using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

			GUIStyle style = null;
			sWall.normal.textColor  = Color.black;
			sAisle.normal.textColor = Color.gray;
			sRoom.normal.textColor  = Color.blue;

			this.Map((int x, int y, Tile tile) =>
			{
				if (tile.IsWall) style = sWall;
				if (tile.IsAisle) style = sAisle;
				if (tile.IsRoom)  style = sRoom;

				GUI.Label(new Rect(x * 10, y * 10 + 30, 10, 10), "■", style);
			});
    }

    #endif
  }
}