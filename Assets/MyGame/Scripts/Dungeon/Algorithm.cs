using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
	/// <summary>
  /// ダンジョン自動生成アルゴリズム(均等分割法)
  /// 配列を縦横に均等に分割する方法でダンジョンを生成する。
  /// </summary>
	public class Algorithm
	{
		private enum Flags : uint
		{
			Wall          = 1 << 0,
			Room          = 1 << 1,
			Aisle         = 1 << 2,
			ReservedAisle = 1 << 3,
		}

		//----------------------------------------------------------------
    // ダンジョン生成に影響を与えるパラメーター郡
    #region 

    /// <summary>
    /// ダンジョンの空間分割数(X,Y)
    /// </summary>
    private Vector2Int size;

		/// <summary>
    /// 部屋作成率、数値が高いほど部屋が作られやすい。0~1の間で設定する。
    /// </summary>
		private float roomMakingRate = 0.7f;

    #endregion
		
		//----------------------------------------------------------------
    // ダンジョン生成中に利用するモノ
		#region

		/// <summary>
		/// ダンジョンのマップチップ配列
		/// </summary>
		private BitFlag[,] chips;

    /// <summary>
    /// ダンジョンの分割位置
    /// </summary>
    private List<int> splitPointsX;
		private List<int> splitPointsY;

		/// <summary>
		/// 部屋予定地のエリア情報
		/// </summary>
		private List<RectInt> reservedRooms;

		/// <summary>
    /// 部屋のエリア情報
    /// </summary>
		private List<RectInt> rooms;

    #endregion

		//----------------------------------------------------------------
    // Method

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Algorithm()
		{
			this.chips = new BitFlag[Dungeon.Define.WIDTH, Dungeon.Define.HEIGHT];
			this.splitPointsX  = new List<int>();
			this.splitPointsY  = new List<int>();
			this.reservedRooms = new List<RectInt>();
			this.rooms         = new List<RectInt>();
		}

		//----------------------------------------------------------------
    // ダンジョン生成 Methods

		public void Make(Dungeon.Stage stage, int sizeX, int sizeY)
		{
			// ダンジョンの分割数を設定、最低でも1x1になるようにフィルター
			this.size.Set(Mathf.Max(1, sizeX), Mathf.Max(1, sizeY));

			// 準備
			this.prepare();

			// 空間分割
			this.splitSpaceX();
			this.splitSpaceY();

			// 通路予定地を埋める
			this.fillReservedAisle();

			// ルームスペース確保
			this.decideReservedRoom();

			// ルーム作成
			this.makeRoom();
			this.fillRoom();

			// 通路作成(上下左右)
			this.makeAisleLeft();
			this.makeAisleRight();
			this.makeAisleUp();
			this.makeAisleDown();

			// 通路を繋げる
			// 整理

			// ステージに内容を展開する
			this.deployToStage(stage);
		}


		/// <summary>
    /// ステージを生成する準備として、まずchipsをFlags.Wallで埋める。
    /// </summary>
		private void prepare()
    {
			this.map((int x, int y, BitFlag _) =>
			{
				this.chips[x, y].Set((uint)Flags.Wall);
			});
		}

		/// <summary>
    /// ステージをX方向に分割する位置を決定する
    /// </summary>
		private void splitSpaceX()
    {
			int width = Define.WIDTH  / this.size.x;

			this.splitPointsX.Add(1);

			for(int i = 1; i < this.size.x; ++i)
			{
				this.splitPointsX.Add(i * width);
			}

			this.splitPointsX.Add(Define.WIDTH - 2);
    }

		/// <summary>
    /// ステージをY方向に分割する位置を決定する
    /// </summary>
		private void splitSpaceY()
    {
			int width = Define.HEIGHT / this.size.y;

			this.splitPointsY.Add(1);

			for(int i = 1; i < this.size.y; ++i)
      {
				this.splitPointsY.Add(i * width);
      }

			this.splitPointsY.Add(Define.HEIGHT - 2);
    }

		/// <summary>
    /// 通路予定地を埋める
    /// </summary>
		private void fillReservedAisle()
    {
			this.splitPointsX.ForEach((int x) =>
      {
				for (int y = 1; y < Define.HEIGHT - 1; ++y)
        {
					this.chips[x, y].Set((uint)Flags.ReservedAisle);
        }
      });

			this.splitPointsY.ForEach((int y) =>
      {
				for (int x = 1; x < Define.WIDTH - 1; ++x)
        {
					this.chips[x, y].Set((uint)Flags.ReservedAisle);
        }
      });
    }

		/// <summary>
    /// ルーム予定地を決める
    /// </summary>
		private void decideReservedRoom()
    {
			for(int y = 0; y < this.size.y; ++y)
      {
				for(int x = 0; x < this.size.x; ++x)
        {
					int x1 = this.splitPointsX[x]   + 2;
					int x2 = this.splitPointsX[x+1] - 2;
					int y1 = this.splitPointsY[y]   + 2;
					int y2 = this.splitPointsY[y+1] - 2;
					int w  = x2 - x1 + 1;
					int h  = y2 - y1 + 1;

					// ルーム予定地のx,y座標、幅高さを決定
					RectInt room = new RectInt(x1, y1, w, h);
					this.reservedRooms.Add(room);
        }
      }
    }

		/// <summary>
    /// 部屋予定地の中に部屋を作る。
    /// 部屋は最低でも１部屋作られる。
    /// </summary>
		private void makeRoom()
    {
			int roomCount = 0;

			// 部屋予定地に部屋を作成する
			this.reservedRooms.ForEach((RectInt area) =>
      {
				// 部屋なしの場合はRectIntの中見が全て0
				RectInt room = new RectInt(0, 0, 0, 0);

				// 部屋が１個以下
				if (roomCount < 1 || Random.Range(0f, 1f) < this.roomMakingRate)
        {
					// 部屋のサイズをランダムに決める(部屋予定地に収まるように)
					room.width  = Random.Range(Define.MIN_ROOM_SIZE, area.width);
					room.height = Random.Range(Define.MIN_ROOM_SIZE, area.height);

					// 部屋の位置をランダムに決める。(部屋予定地に収まるように)
					room.x      = Random.Range(area.xMin, area.xMax - room.width);
					room.y      = Random.Range(area.yMin, area.yMax - room.height);
					++roomCount;
        }

				this.rooms.Add(room);
      });

    }

		/// <summary>
    /// 部屋を埋める
    /// </summary>
		private void fillRoom()
    {
			this.rooms.ForEach((RectInt room) => {
				this.fillByRect(room, (uint)Flags.Room);
			});
    }

		/// <summary>
    /// 部屋から左方向への通路を伸ばす
    /// </summary>
		private void makeAisleLeft()
    {
			this.mapForRoom((int sx, int sy, RectInt room) =>
      {
				int x = room.xMin - 1;
				int y = Random.Range(room.yMin, room.yMax);

        while (sx != 0)
        {
          var chip = this.chips[x, y];

          this.chips[x, y].On((uint)Flags.Aisle);
          --x;

          if (this.satisfyConditionsToBreakTheAisleLoop(x, y, chip)) break;
        }
      });
    }

		/// <summary>
    /// 部屋から右方向への通路を伸ばす
    /// </summary>
		private void makeAisleRight()
    {
			this.mapForRoom((int sx, int sy, RectInt room) =>
      {
				int x = room.xMax;
				int y = Random.Range(room.yMin, room.yMax);

				while (sx != this.size.x - 1)
        {
					var chip = this.chips[x, y];

					this.chips[x, y].On((uint)Flags.Aisle);
					++x;

					if (this.satisfyConditionsToBreakTheAisleLoop(x, y, chip)) break;
        }
      });
    }

		/// <summary>
    /// 部屋から上方向に通路を伸ばす
    /// </summary>
		private void makeAisleUp()
    {
			this.mapForRoom((int sx, int sy, RectInt room) =>
      {
				if (Random.Range(0, 1f) < 0.5f) return;

				int x = Random.Range(room.xMin,  room.xMax);
				int y = room.yMin - 1;

				while(sy != 0)
        {
					var chip = this.chips[x, y];

					this.chips[x, y].On((uint)Flags.Aisle);
					--y;

					if (this.satisfyConditionsToBreakTheAisleLoop(x, y, chip)) break;
        }
      });
    }

		/// <summary>
    /// 部屋から下方向に通路を伸ばす
    /// </summary>
		private void makeAisleDown()
    {
			this.mapForRoom((int sx, int sy, RectInt room) =>
      {
				if (Random.Range(0, 1f) < 0.5f) return;

				int x = Random.Range(room.xMin, room.xMax);
				int y = room.yMax;

				while(sy != this.size.y - 1)
        {
					var chip = this.chips[x, y];

					this.chips[x, y].On((uint)Flags.Aisle);
					++y;

					if (this.satisfyConditionsToBreakTheAisleLoop(x, y, chip)) break;
        }
      });
    }


		/// <summary>
    /// 生成したデータをステージへ展開する。
    /// </summary>
		private void deployToStage(Stage stage)
    {
			this.map((int x, int y, BitFlag flag) => {

				if (flag.Is((uint)Flags.Wall))
				{
					stage.Set(x, y, Tile.Wall);
        }
				if (flag.Contain((uint)Flags.Room))
        {
					stage.Set(x, y, Tile.Room);
        }
				if (flag.Contain((uint)Flags.ReservedAisle))
        {
					stage.Set(x, y, Tile.AisleCandidate);
        }
				if (flag.Contain((uint)Flags.Aisle))
        {
					stage.Set(x, y, Tile.Aisle);
        }

			});
    }

		//----------------------------------------------------------------
    // Private Utility Methods

		private void map(System.Action<int, int, BitFlag> cb)
		{
			for (int x = 0; x < Define.WIDTH; ++x)
      {
				for (int y = 0; y < Define.HEIGHT; ++y)
        {
					cb(x, y, this.chips[x, y]);
        }
      }
		}

		private void mapForSize(System.Action<int, int, int> cb)
    {
			for(int y = 0; y < this.size.y; ++y)
      {
				for (int x = 0; x < this.size.x; ++x)
        {
					cb(x, y, y * this.size.x + x);
        }
      }
    }

		private void mapForRoom(System.Action<int, int, RectInt> cb) {
			this.mapForSize((int x, int y, int index) =>
      {
				var room = this.rooms[index];
				if (this.isEnableAs(room) == false) return;
				cb(x, y, room);
      });
		}


		private void fillByRect(RectInt rect, uint flag)
		{
			for (int x = rect.x; x < rect.x + rect.width; ++x)
			{
				for (int y = rect.y; y < rect.y + rect.height; ++y)
				{
					this.chips[x, y].Set(flag);
				}
			}
		}

		private bool satisfyConditionsToBreakTheAisleLoop(int x, int y, BitFlag chip)
    {
			if (chip.Contain((uint)Flags.ReservedAisle)) return true;
			if (x <= 0) return true;
			if (Define.WIDTH -1 <= x) return true;
			if (y <= 0) return true;
			if (Define.HEIGHT  - 1 <= y) return true;

			return false;
    }

		/// <summary>
    /// 部屋として有効かどうか
    /// </summary>
		private bool isEnableAs(RectInt room)
    {
			if (room.x == 0) return false;
			if (room.y == 0) return false;
			if (room.width == 0) return false;
			if (room.height == 0) return false;

			return true;
    }
	}
}