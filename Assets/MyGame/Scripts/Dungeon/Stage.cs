using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
	public enum Tile
	{
		Wall   = 1 << 0,
		Room   = 1 << 1,
		Aisle  = 1 << 2,
		Player = 1 << 3,
		Friend = 1 << 4,
		Enemy  = 1 << 5,
		Item   = 1 << 6,
		Trap   = 1 << 7,
	}

	public class Stage : MonoBehaviour
	{

		private Tile[,] tiles;
		private List<RectInt> rooms;

    private Algorithm algorithm;

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
      this.algorithm = new Algorithm();
		}

		private void Create()
		{
			int x = Random.Range(2, 6);
			int y = Random.Range(2, 4);
			float r = Random.Range(0f, 1f);

			algorithm.Make(this, x, y, r);
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

    private void OnGUI()
    {
			if (GUI.Button(new Rect(0, 0, 100, 20), "create"))
			{
				this.Init();
				this.Create();
			}

      this._toggleDetail = GUI.Toggle(
        new Rect(100, 0, 100, 20), 
        this._toggleDetail, 
        "Detail"
      );

      if (this._toggleDetail) {
        this.algorithm._drawDebug();
      } else {
        this._drawDebug();
      }
    }


    private void _drawDebug()
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
				if ((tile & Tile.Wall) == Tile.Wall)   style = sWall;
				if ((tile & Tile.Aisle) == Tile.Aisle) style = sAisle;
				if ((tile & Tile.Room) == Tile.Room)   style = sRoom;

				GUI.Label(new Rect(x * 10, y * 10 + 30, 10, 10), "■", style);
			});
		}

    #endif
  }
}