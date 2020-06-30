using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{

enum Tile
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
	private int divCountV;
	private int divCountH;
	private List<int> vAislePoints;
	private List<int> hAislePoints;
	private List<RectInt> roomSpacies;
	private List<RectInt> rooms;

	private void Awake()
	{
		Random.InitState(System.DateTime.Now.Millisecond);
		this.init();
	}

	// Use this for initialization
	void Start()
	{
		create();
	}

	private void init()
	{
		this.tiles = new Tile[Define.WIDTH, Define.HEIGHT];
		this.divCountV = 3;
		this.divCountH = 2;
		this.vAislePoints = new List<int>();
		this.hAislePoints = new List<int>();
		this.roomSpacies = new List<RectInt>();
		this.rooms = new List<RectInt>();
	}

	private void create()
	{
		this.fillWithWall();
		this.devideSpace();
		this.makeRoom();
		this.makeAisleLeft();
		this.makeAisleRight();
		this.makeAisleUp();
		this.makeAisleDown();
		this.linkAisle();
		this.organize();
	}

	private void fillWithWall()
	{
		this.map((int x, int y, Tile tile) => {
			this.tiles[x, y] = Tile.Wall;
		});
	}


	private void devideSpace()
	{
		int divWidth = Define.WIDTH / this.divCountV;
		int divHeight = Define.HEIGHT / this.divCountH;

		this.vAislePoints.Add(1);
		for (int i = 1; i < divCountV; ++i)
		{
			this.vAislePoints.Add(divWidth * i);
		}
		this.vAislePoints.Add(Define.WIDTH - 2);

		this.hAislePoints.Add(1);
		for (int i = 1; i < divCountH; ++i)
		{
			this.hAislePoints.Add(divHeight * i);
		}
		this.hAislePoints.Add(Define.HEIGHT - 2);

		for (int h = 0; h < divCountH; ++h)
		{
			for (int v = 0; v < divCountV; ++v)
			{
				int x1 = this.vAislePoints[v];
				int x2 = this.vAislePoints[v + 1];
				int y1 = this.hAislePoints[h];
				int y2 = this.hAislePoints[h + 1];

				int width = x2 - x1 - 3;
				int height = y2 - y1 - 3;


				RectInt space = new RectInt(x1 + 2, y1 + 2, width, height);
				this.roomSpacies.Add(space);
			}
		}


		this.vAislePoints.ForEach((int x) =>
		{
			for (int y = 1; y < Define.HEIGHT - 1; ++y)
			{
				this.tiles[x, y] |= Tile.AisleCandidate;
			}
		});

		this.hAislePoints.ForEach((int y) =>
		{
			for (int x = 1; x < Define.WIDTH - 1; ++x)
			{
				this.tiles[x, y] |= Tile.AisleCandidate;
			}
		});
	}

	private void makeRoom()
	{
		const int ROOM_MIN_SIZE = 3;

		this.roomSpacies.ForEach((RectInt space) =>
		{
			int x = 0;
			int y = 0;
			int width = 0;
			int height = 0;

			if (this.rooms.Count < 3 || Random.Range(0f, 1f) < 0.7f)
			{
				width = Random.Range(ROOM_MIN_SIZE, space.width);
				height = Random.Range(ROOM_MIN_SIZE, space.height);

				x = Random.Range(space.xMin, space.xMax - width);
				y = Random.Range(space.yMin, space.yMax - height);
			}

			this.rooms.Add(new RectInt(x, y, width, height));
		});

		this.rooms.ForEach((RectInt room) =>
		{
			this.fillByRect(room, Tile.Room);
		});
	}

	private void makeAisleLeft()
	{
		int count = 0;
		this.rooms.ForEach((RectInt room) =>
		{
			int x = room.xMin - 1;
			int y = Random.Range(room.yMin, room.yMax);

			while (count % this.divCountV != 0 && room.x != 0)
			{
				if (count % this.divCountV == 0) break;

				Tile tile = this.tiles[x, y];

				this.tiles[x, y] |= Tile.Aisle;

				if ((tile & Tile.AisleCandidate) == Tile.AisleCandidate) break;

				--x;
				if (x <= 0) break;
			}

			++count;
		});
	}



	private void makeAisleRight()
	{
		int count = 0;
		this.rooms.ForEach((RectInt room) =>
		{
			int x = room.xMax;
			int y = Random.Range(room.yMin, room.yMax);

			while (count % this.divCountV != divCountV - 1 && room.x != 0)
			{
				Tile tile = this.tiles[x, y];
				this.tiles[x, y] |= Tile.Aisle;

				if ((tile & Tile.AisleCandidate) == Tile.AisleCandidate) break;
				++x;
				if (Define.WIDTH - 1 <= x) break;
			}

			count++;
		});

	}

	private void makeAisleUp()
	{
		int count = 0;
		this.rooms.ForEach((RectInt room) =>
		{
			int x = Random.Range(room.xMin, room.xMax);
			int y = room.yMin - 1;

			while (count / this.divCountV != 0 && room.x != 0)
			{
				Tile tile = this.tiles[x, y];
				this.tiles[x, y] |= Tile.Aisle;

				if ((tile & Tile.AisleCandidate) == Tile.AisleCandidate) break;
				--y;
				if (y <= 0) break;
			}

			++count;

		});
	}

	private void makeAisleDown()
	{
		int count = 0;
		this.rooms.ForEach((RectInt room) =>
		{
			int x = Random.Range(room.xMin, room.xMax);
			int y = room.yMax;

			while (count / this.divCountV != this.divCountH - 1 && room.x != 0)
			{
				Tile tile = this.tiles[x, y];
				this.tiles[x, y] |= Tile.Aisle;

				if ((tile & Tile.AisleCandidate) == Tile.AisleCandidate) break;
				++y;
				if (Define.HEIGHT - 1 <= y) break;
			}

			++count;

		});
	}

	private void linkAisle()
	{
		this.vAislePoints.ForEach((int x) =>
		{
			bool padding = false;

			for (int y = 1; y < Define.HEIGHT - 1; ++y)
			{
				Tile tile = this.tiles[x, y];

				bool isAisle = ((tile & Tile.Aisle) == Tile.Aisle);

				if (padding == false && isAisle == true) { padding = true; }
				else if (padding == true && isAisle == true) padding = false;


				if (padding)
				{
					this.tiles[x, y] = Tile.Aisle;
				}

			}

		});

		this.hAislePoints.ForEach((int y) =>
		{
			bool padding = false;

			for (int x = 1; x < Define.WIDTH - 1; ++x)
			{
				Tile tile = this.tiles[x, y];

				bool isAisle = ((tile & Tile.Aisle) == Tile.Aisle);

				if (padding == false && isAisle == true) { padding = true; }
				else if (padding == true && isAisle == true) padding = false;


				if (padding)
				{
					this.tiles[x, y] = Tile.Aisle;
				}

			}

		});
	}

	private void organize()
	{
		this.map((int x, int y, Tile tile) =>
		{
			this.tiles[x, y] = tile & ~Tile.AisleCandidate;
		});
	}


	// Update is called once per frame
	void Update()
	{


	}

	private void OnGUI()
	{
		GUIStyle styleWall = new GUIStyle();
		GUIStyle styleAisle = new GUIStyle();
		GUIStyle styleAisleCandidate = new GUIStyle();
		GUIStyle styleRoomCandidate = new GUIStyle();
		GUIStyle styleRoom = new GUIStyle();

		GUIStyle style = null;
		styleWall.normal.textColor = Color.black;
		styleAisle.normal.textColor = Color.gray;
		styleAisleCandidate.normal.textColor = Color.magenta;
		styleRoomCandidate.normal.textColor = Color.cyan;
		styleRoom.normal.textColor = Color.blue;

		this.map((int x, int y, Tile tile) =>
		{
			if ((tile & Tile.Wall) == Tile.Wall) style = styleWall;
			if ((tile & Tile.AisleCandidate) == Tile.AisleCandidate) style = styleAisleCandidate;
			if ((tile & Tile.RoomCandidate) == Tile.RoomCandidate) style = styleRoomCandidate;
			if ((tile & Tile.Aisle) == Tile.Aisle) style = styleAisle;
			if ((tile & Tile.Room) == Tile.Room) style = styleRoom;

			GUI.Label(new Rect(x * 10, y * 10, 10, 10), "■", style);
		});

		if (GUI.Button(new Rect(0, 0, 100, 20), "create"))
		{
			this.init();
			this.divCountV = Random.Range(2, 5);
			this.divCountH = Random.Range(2, 4);
			this.create();
		}

	}

	private void map(System.Action<int, int, Tile> cb)
	{
		for (int x = 0; x < Define.WIDTH; x++)
		{
			for (int y = 0; y < Define.HEIGHT; y++)
			{
				cb(x, y, this.tiles[x, y]);
			}
		}
	}

	private void fillByRect(RectInt rect, Tile fill)
	{
		for (int x = rect.x; x < rect.x + rect.width; ++x)
		{
			for (int y = rect.y; y < rect.y + rect.height; ++y)
			{
				this.tiles[x, y] = fill;
			}
		}
	}


}

}