﻿using System.Collections.Generic;
using System.Linq;
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
			Wall          = 1 << 0, // 壁
			Room          = 1 << 1, // 部屋
			Aisle         = 1 << 2, // 通路
			ReservedAisle = 1 << 3, // 通路予定
			Cross         = 1 << 4, // 交差点
			Confluence    = 1 << 5, // 合流地点
		}

    //-------------------------------------------------------------------------
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
		
		//-------------------------------------------------------------------------
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
		private RectInt[,] reservedRooms;

		/// <summary>
    /// 部屋のエリア情報
    /// </summary>
		private RectInt[,] rooms;

    /// <summary>
    /// 通路を繋げる処理中に使用するフラグ変数
    /// </summary>
		private bool flagForConnectingAisles;

    #endregion

    //-------------------------------------------------------------------------
    // Method

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Algorithm()
		{
			this.chips         = new BitFlag[Dungeon.Define.WIDTH, Dungeon.Define.HEIGHT];
			this.splitPointsX  = new List<int>();
			this.splitPointsY  = new List<int>();

      this.flagForConnectingAisles = false;
		}

		//-------------------------------------------------------------------------
    // ダンジョン生成 Methods

		public void Make(Dungeon.Stage stage, int sizeX, int sizeY, float rate)
		{
			// ダンジョンの分割数を設定、最低でも1x1になるようにフィルター
			this.size.Set(Mathf.Max(1, sizeX), Mathf.Max(1, sizeY));
			this.roomMakingRate = Mathf.Max(0, Mathf.Min(1f, rate));
      this.reservedRooms = new RectInt[this.size.x, this.size.y];
      this.rooms         = new RectInt[this.size.x, this.size.y];

			// 準備
			this.Prepare();

			// 空間分割
			this.SplitChipsByXDirection();
			this.SplitChipsByYDirection();

			// 通路予定地を埋める
			this.MarkupChipsWithReservedAisle();

			// ルームスペース確保
			this.MakeReservedRoom();

			// ルーム作成
			this.MakeRoom();
			this.MarkupRoom();

			// 通路作成(上下左右)
			this.MakeAisleLeft();
			this.MakeAisleRight();
			this.MakeAisleUp();
			this.MakeAisleDown();

			// 通路を繋げる
			this.MakeAisleBetweenForY();
			this.MakeAisleBetweenForX();

			// 不要な通路を消す
			this.DeleteUselessAisle();

			// 整理
			this.Cleaning();

			// ステージに内容を展開する
			this.DeployToStage(stage);
		}


		/// <summary>
    /// ステージを生成する準備として、まずchipsをFlags.Wallで埋める。
    /// </summary>
		private void Prepare()
    {
			this.Map((int x, int y, BitFlag _) =>
			{
				this.chips[x, y].Set((uint)Flags.Wall);
			});
		}

		/// <summary>
    /// ステージをX方向に分割する位置を決定する
    /// </summary>
		private void SplitChipsByXDirection()
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
		private void SplitChipsByYDirection()
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
    /// 分割した座標を元に通路予定地に印をつける。
    /// </summary>
		private void MarkupChipsWithReservedAisle()
    {
			// Y方向の処理
			this.splitPointsX.ForEach((int x) =>
      {
				int y1 = 1;
				int y2 = Define.HEIGHT - 2;
				this.SetChipsY(x, y1, y2, (uint)Flags.ReservedAisle);
      });

			// X方向の処理
			this.splitPointsY.ForEach((int y) =>
      {
				int x1 = 1;
				int x2 = Define.WIDTH - 2;
				this.SetChipsX(y, x1, x2, (uint)Flags.ReservedAisle);
      });

			// 通路予定地が交差するポイントにも印をつける。
			this.splitPointsX.ForEach((int x) =>
      {
				this.splitPointsY.ForEach((int y) =>
        {
					this.chips[x, y].On((uint)Flags.Cross);
        });
      });
    }

		/// <summary>
    /// ルーム予定地を作る
    /// </summary>
		private void MakeReservedRoom()
    {
			this.MapForSize((int x, int y, int _) =>
      {
				int x1 = this.splitPointsX[x]   + 2;
				int x2 = this.splitPointsX[x+1] - 2;
				int y1 = this.splitPointsY[y]   + 2;
				int y2 = this.splitPointsY[y+1] - 2;
				int w  = x2 - x1 + 1;
				int h  = y2 - y1 + 1;

				// ルーム予定地のx,y座標、幅高さを決定
				RectInt room = new RectInt(x1, y1, w, h);
				this.reservedRooms[x, y] = room;
      });

    }



    private RectInt CreateRoomByRect(RectInt area)
    {
      // 部屋なしの場合はRectIntの中見が全て0
      var room = new RectInt();

			// 部屋のサイズをランダムに決める(部屋予定地に収まるように)
			room.width  = Random.Range(Define.MIN_ROOM_SIZE, area.width);
			room.height = Random.Range(Define.MIN_ROOM_SIZE, area.height);

			// 部屋の位置をランダムに決める。(部屋予定地に収まるように)
			room.x      = Random.Range(area.xMin, area.xMax - room.width);
			room.y      = Random.Range(area.yMin, area.yMax - room.height);
      
      return room;
    }
		/// <summary>
    /// 部屋予定地の中に部屋を作る。
    /// </summary>
		private void MakeRoom()
    {
      // 縦列に１つ部屋を作る
      for(int x = 0; x < this.size.x; ++x)
      {
        int y = Random.Range(0, this.size.y);
        this.rooms[x, y] = this.CreateRoomByRect(this.reservedRooms[x, y]);
      }

			// 残りの空間にランダムで部屋を作成する
      this.MapForSize((int x, int y, int _) =>
      {
        // ランダムで部屋を作るかどうか決定
        if (this.roomMakingRate < Random.Range(0f, 1f)) return;

        // 既に部屋があったらスキップ
        if (HasRoom(x, y)) return;

        this.rooms[x, y] = this.CreateRoomByRect(this.reservedRooms[x, y]);
      });
    }

    private bool HasRoom(int x, int y)
    {
      return this.IsEnableAsRoom(this.rooms[x, y]);
    }

		/// <summary>
    /// 部屋を埋める
    /// </summary>
		private void MarkupRoom()
    {
      this.MapForRoom((int x, int y, RectInt room) =>
      {
        this.SetChipsByRect(room, (uint)Flags.Room);
      });
    }

		/// <summary>
    /// 部屋から左方向への通路を伸ばす
    /// </summary>
		private void MakeAisleLeft()
    {
			this.MapForRoom((int sx, int sy, RectInt room) =>
      {
				int x = room.xMin - 1;
				int y = Random.Range(room.yMin, room.yMax);

        while (sx != 0)
        {
          var chip = this.chips[x, y];

					this.SetAisleChipWhenMakeAisle(x, y, chip);
          --x;

          if (this.SatisfyConditionsToBreakTheAisleLoop(x, y, chip)) break;
        }
      });
    }

		/// <summary>
    /// 部屋から右方向への通路を伸ばす
    /// </summary>
		private void MakeAisleRight()
    {
			this.MapForRoom((int sx, int sy, RectInt room) =>
      {
				int x = room.xMax;
				int y = Random.Range(room.yMin, room.yMax);

				while (sx != this.size.x - 1)
        {
					var chip = this.chips[x, y];

					this.SetAisleChipWhenMakeAisle(x, y, chip);
					++x;

					if (this.SatisfyConditionsToBreakTheAisleLoop(x, y, chip)) break;
        }
      });
    }

		/// <summary>
    /// 部屋から上方向に通路を伸ばす
    /// </summary>
		private void MakeAisleUp()
    {
			this.MapForRoom((int sx, int sy, RectInt room) =>
      {
				if (Random.Range(0f, 1f) < 0.5f) return;

				int x = Random.Range(room.xMin,  room.xMax);
				int y = room.yMin - 1;

				while(sy != 0)
        {
					var chip = this.chips[x, y];

					this.SetAisleChipWhenMakeAisle(x, y, chip);
					--y;

					if (this.SatisfyConditionsToBreakTheAisleLoop(x, y, chip)) break;
        }
      });
    }

		/// <summary>
    /// 部屋から下方向に通路を伸ばす
    /// </summary>
		private void MakeAisleDown()
    {
			this.MapForRoom((int sx, int sy, RectInt room) =>
      {
				if (Random.Range(0, 1f) < 0.5f) return;

				int x = Random.Range(room.xMin, room.xMax);
				int y = room.yMax;

				while(sy != this.size.y - 1)
        {
					var chip = this.chips[x, y];

					this.SetAisleChipWhenMakeAisle(x, y, chip);
					++y;

					if (this.SatisfyConditionsToBreakTheAisleLoop(x, y, chip)) break;
        }
      });
    }

		/// <summary>
    /// Y方向について、間の通路を作成する
    /// </summary>
		private void MakeAisleBetweenForY()
    {
      int count = -1;

			this.splitPointsX.ForEach((int x) =>
      {
        count++;

				// 部屋と通路の合流地点を探す(Y方向)
				var found = LookForChipsY(x, (uint)Flags.Confluence);

				// 合流地点が１つ以下ならスキップ
				if (found.Count <= 1) return;

        // 合流地点が奇数の場合は最初と最後の地点を結ぶ
        if (found.Count % 2 == 1 || count % 2 == 1)
        {
          this.AddChipsY(x, found.First(), found.Last(), (uint)Flags.Aisle);
          return;
        }

        // 合流地点が偶数、かつ偶数列の場合
        for (int i = 0; i < found.Count; i+=2)
        {
          this.AddChipsY(x, found[i], found[i+1], (uint)Flags.Aisle);
        }
        
 
				
      });
    }

		/// <summary>
    /// X方向について、間の通路を作成する
    /// </summary>
		private void MakeAisleBetweenForX()
    {
			this.splitPointsY.ForEach((int y) =>
      {
				var found = this.LookForChipsX(y, (uint)Flags.Confluence);

				found.ForEach((int x) =>
        {
					this.flagForConnectingAisles = false;
					this.ConnectAisleBetweenForX(x + 1, y);
        });
      });
    }

		private void ConnectAisleBetweenForX(int x, int y)
    {
			var chip = this.chips[x, y];

			if (chip.Contain((uint)Flags.Confluence))
      {
				this.flagForConnectingAisles = true;
				return;
      }

			if (chip.Contain((uint)Flags.Cross))
      {
				if (x == Define.WIDTH - 2) return;

				// 上下左右どこにもつながってない交差点なら繋げない
				if (this.CountConnectedAisle(x, y) == 0)
        {
					return;
        }

				this.flagForConnectingAisles = (Random.Range(0f, 1f) < 0.2f);
				if (this.flagForConnectingAisles)
        {
					this.chips[x, y].On((uint)Flags.Aisle);
        }
				return;
      }

			if (Define.WIDTH - 1 <= x) return;

			this.ConnectAisleBetweenForX(x+1, y);

			if (this.flagForConnectingAisles)
      {
				this.chips[x, y].On((uint)Flags.Aisle);
      }
    }

		private void DeleteUselessAisle()
    {
			this.splitPointsY.ForEach((int y) =>
      {
				var found = this.LookForChipsX(y, (uint)Flags.Confluence);

				// 合流地点の上下左右に繋がる通路があるかを見る
				found.ForEach((int x) =>
        {
					// 繋がる通路が１つしかないなら使えない通路なので消す
					if (this.CountConnectedAisle(x, y) == 1)
          {
						this.DeleteUselessAnsle2(x, y);
          }
        });
      });
    }

		private void DeleteUselessAnsle2(int x, int y)
    {
			var chip = this.chips[x, y];

			// 部屋までたどり着いたら終了
			if (chip.Contain((uint)Flags.Room)) return;

			// 通路情報を消す
			this.chips[x, y].Off((uint)Flags.Aisle);

			// 上下左右を見に行く
			if (this.chips[x, y + 1].Contain((uint)Flags.Aisle))
      {
				this.DeleteUselessAnsle2(x, y + 1);
      }

			if (this.chips[x, y - 1].Contain((uint)Flags.Aisle))
      {
				this.DeleteUselessAnsle2(x, y - 1);
      }

			if (this.chips[x + 1, y].Contain((uint)Flags.Aisle)){
				this.DeleteUselessAnsle2(x + 1, y);
			}

			if (this.chips[x - 1, y].Contain((uint)Flags.Aisle)){
				this.DeleteUselessAnsle2(x - 1, y);
			}			

    }

		private void Cleaning()
    {
			Map((int x, int y, BitFlag _) =>
      {
				var chip = this.chips[x, y];

				if (chip.Contain((uint)Flags.Room))
        {
					this.chips[x, y].Set((uint)Flags.Room);
					return;
        }

				if (chip.Contain((uint)Flags.Aisle))
        {
					this.chips[x, y].Set((uint)Flags.Aisle);
					return;
        }

				this.chips[x, y].Set((uint)Flags.Wall);
      });
    }

		/// <summary>
    /// 生成したデータをステージへ展開する。
    /// </summary>
		private void DeployToStage(Stage stage)
    {
			this.Map((int x, int y, BitFlag flag) => {

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

		//-------------------------------------------------------------------------
    // Private Utility Methods

		private void Map(System.Action<int, int, BitFlag> cb)
		{
			for (int x = 0; x < Define.WIDTH; ++x)
      {
				for (int y = 0; y < Define.HEIGHT; ++y)
        {
					cb(x, y, this.chips[x, y]);
        }
      }
		}

		private void MapForSize(System.Action<int, int, int> cb)
    {
			for(int y = 0; y < this.size.y; ++y)
      {
				for (int x = 0; x < this.size.x; ++x)
        {
					cb(x, y, y * this.size.x + x);
        }
      }
    }

		private void MapForRoom(System.Action<int, int, RectInt> cb) {
			this.MapForSize((int x, int y, int index) =>
      {
				var room = this.rooms[x, y];
				if (this.IsEnableAsRoom(room) == false) return;
				cb(x, y, room);
      });
		}


		private void SetChipsByRect(RectInt rect, uint flag)
		{
			for (int x = rect.x; x < rect.x + rect.width; ++x)
			{
				for (int y = rect.y; y < rect.y + rect.height; ++y)
				{
					this.chips[x, y].Set(flag);
				}
			}
		}

		private void SetChipsY(int x, int fromY, int toY, uint chips)
    {
			for(int y = fromY; y <= toY; ++y)
      {
				this.chips[x, y].Set(chips);
      }
    }

		private void SetChipsX(int y, int fromX, int toX, uint chips)
    {
			for(int x = fromX; x <= toX; ++x)
      {
				this.chips[x, y].Set(chips);
      }
    }

		private void AddChipsY(int x, int fromY, int toY, uint chips)
    {
			for(int y = fromY; y <= toY; ++y)
      {
				this.chips[x, y].On(chips);
      }
    }


		private void AddChipsX(int y, int fromX, int toX, uint chips)
    {
			for(int x = fromX; x <= toX; ++x)
      {
				this.chips[x, y].On(chips);
      }
    }

		/// <summary>
    /// 部屋として有効かどうか
    /// </summary>
		private bool IsEnableAsRoom(RectInt room)
    {
			if (room.x == 0) return false;
			if (room.y == 0) return false;
			if (room.width == 0) return false;
			if (room.height == 0) return false;

			return true;
    }

    /// <summary>
    /// 指定した座標の周囲に通路がいくつあるか数える
    /// </summary>
    /// <returns>隣接する通路の数</returns>
    private int CountConnectedAisle(int x, int y)
    {
      // 上下左右に2つ以上の通路がなければ無効
      int count = 0;
			if (this.chips[x, y - 1].Contain((uint)Flags.Aisle)) ++count;
			if (this.chips[x, y + 1].Contain((uint)Flags.Aisle)) ++count;
			if (this.chips[x + 1, y].Contain((uint)Flags.Aisle)) ++count;
			if (this.chips[x - 1, y].Contain((uint)Flags.Aisle)) ++count;

      return count;
    }
    


		/// <summary>
    /// Y方向のチップ情報を見つける
    /// </summary>
    /// <param name="x">指定したx列が走査の対象</param>
    /// <param name="chips">見つけたいチップ</param>
    /// <returns>該当するchipのY座標リスト</returns>
		private List<int> LookForChipsY(int x, uint chips, bool isEither = true)
    {
			List<int> found = new List<int>();

			for(int y = 1;  y < Define.HEIGHT - 1; ++y)
      {
				var chip = this.chips[x, y];

				// isEitherフラグをみて判定メソッドを切り替える。
				bool flag = (isEither)
					? chip.ContainEither(chips)
					: chip.Contain(chips);

				if (flag) found.Add(y);
      }

			return found;
    }

		/// <summary>
    /// X方向のチップ情報を見つける
    /// </summary>
    /// <param name="y">指定したy行が操作の対象</param>
    /// <param name="chips">見つけたいchip</param>
    /// <returns>該当するchipのX座標リスト</returns>
		private List<int> LookForChipsX(int y, uint chips, bool isEither = true)
    {
			List<int> found = new List<int>();

			for(int x = 1;  x < Define.WIDTH - 1; ++x)
      {
				var chip = this.chips[x, y];

				// isEitherフラグをみて判定メソッドを切り替える。
				bool flag = (isEither)
					? chip.ContainEither(chips)
					: chip.Contain(chips);

				if (flag) found.Add(x);
      }

			return found;
    }

		//-------------------------------------------------------------------------
    // 部屋から通路を伸ばす際に使用する限定的な処理

		private bool SatisfyConditionsToBreakTheAisleLoop(int x, int y, BitFlag chip)
    {
			if (chip.Contain((uint)Flags.ReservedAisle)) return true;
			if (x <= 0) return true;
			if (Define.WIDTH -1 <= x) return true;
			if (y <= 0) return true;
			if (Define.HEIGHT  - 1 <= y) return true;

			return false;
    }

		private void SetAisleChipWhenMakeAisle(int x, int y, BitFlag chip)
    {
			uint aisle = (uint)Flags.Aisle;
			if (chip.Contain((uint)Flags.ReservedAisle))
      {
				aisle |= (uint)Flags.Confluence;
      }
			this.chips[x, y].On(aisle);
		}
	}
}