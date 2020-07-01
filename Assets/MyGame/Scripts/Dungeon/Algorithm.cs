using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{

	public class Algorithm
	{
		private enum Flags : uint
		{
			Wall = 1 << 0,
			Room = 1 << 1,
			Aisle = 1 << 2,
			ReservedAisle = 1 << 3,
		}

		/// <summary>
		/// ダンジョンのマップチップ配列
		/// </summary>
		private BitFlag[,] chips;

		/// <summary>
		/// ダンジョンの空間分割数(X,Y)
		/// </summary>
		private Vector2Int size;

		/// <summary>
		/// ダンジョンの分割位置
		/// </summary>
		private List<int> splitPointsX;
		private List<int> splitPointsY;

		/// <summary>
		/// 部屋を構築できるエリア情報の配列
		/// </summary>
		private List<RectInt> roomAreas;

		/// <summary>
    /// コンストラクタ
    /// </summary>
		public Algorithm()
		{
			this.chips = new BitFlag[Dungeon.Define.WIDTH, Dungeon.Define.HEIGHT];
			this.size         = new Vector2Int(0, 0);
			this.splitPointsX = new List<int>();
			this.splitPointsY = new List<int>();
			this.roomAreas    = new List<RectInt>();
		}

		public void Make(Dungeon.Stage stage, int sizeX, int sizeY)
		{
			// 設定
			this.size.Set(sizeX, sizeY);

			// 準備
			this.prepare();

			// 空間分割
			this.splitSpaceX();
			this.splitSpaceY();

			// 通路予定地を埋める
			this.fillReservedAisle();

			// ルームスペース確保
			// ルーム作成
			// 通路作成(上下左右)
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
    /// 生成したデータをステージへ展開する。
    /// </summary>
		private void deployToStage(Stage stage)
    {
			this.map((int x, int y, BitFlag flag) => {

				if (flag.Is((uint)Flags.Wall))
				{
					stage.Set(x, y, Tile.Wall);
        }
				if (flag.Contain((uint)Flags.ReservedAisle))
        {
					stage.Set(x, y, Tile.AisleCandidate);
        }

			});
    }

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
	}
}