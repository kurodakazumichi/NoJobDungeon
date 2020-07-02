using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
	

	public enum Tile
	{
		Wall = 1 << 0,
		Room = 1 << 1,
		Aisle = 1 << 2,
		Player = 1 << 3,
		Friend = 1 << 4,
		Enemy = 1 << 5,
		Item = 1 << 6,
		Trap = 1 << 7,
		AisleCandidate = 1 << 10,
		RoomCandidate = 1 << 11,
	}

	public class Stage : MonoBehaviour
	{

		private Tile[,] tiles;
		private List<RectInt> rooms;

		void Awake()
		{
			this.Init();
		}

		// Use this for initialization
		void Start()
		{
			this.Create();
		}

		public void Set(int x, int y, Tile tile)
    {
			this.tiles[x, y] = tile;
    }

		private void Init()
		{
			this.tiles = new Tile[Define.WIDTH, Define.HEIGHT];
			this.rooms = new List<RectInt>();
		}

		private void Create()
		{
			int x = Random.Range(2, 6);
			int y = Random.Range(2, 4);
			float r = Random.Range(0f, 1f);

			new Algorithm().Make(this, x, y, r);
		}

		private void OnGUI()
		{
			GUIStyle styleWall = new GUIStyle();
      GUIStyle styleWall2 = new GUIStyle();
			GUIStyle styleAisle = new GUIStyle();
			GUIStyle styleAisleCandidate = new GUIStyle();
			GUIStyle styleRoomCandidate = new GUIStyle();
			GUIStyle styleRoom = new GUIStyle();

			GUIStyle style = null;
			styleWall.normal.textColor = Color.black;
      styleWall2.normal.textColor = Color.white;
			styleAisle.normal.textColor = Color.gray;
			styleAisleCandidate.normal.textColor = Color.magenta;
			styleRoomCandidate.normal.textColor = Color.cyan;
			styleRoom.normal.textColor = Color.blue;

			this.Map((int x, int y, Tile tile) =>
			{
				if ((tile & Tile.Wall) == Tile.Wall){
          if (x % 10 == 0 && y % 10 == 0)
          {
            style = styleWall2;
          }

          else
          {
            style = styleWall;
          }
        } 
				if ((tile & Tile.AisleCandidate) == Tile.AisleCandidate) style = styleAisleCandidate;
				if ((tile & Tile.RoomCandidate) == Tile.RoomCandidate) style = styleRoomCandidate;
				if ((tile & Tile.Aisle) == Tile.Aisle) style = styleAisle;
				if ((tile & Tile.Room) == Tile.Room) style = styleRoom;

				GUI.Label(new Rect(x * 10, y * 10, 10, 10), "■", style);
			});

			if (GUI.Button(new Rect(0, 0, 100, 20), "create"))
			{
				this.Init();
				this.Create();
			}

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
	}

}