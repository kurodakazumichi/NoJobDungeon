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
		ReservedRoom = 1 << 3,
		ReservedAisle = 1 << 4,
  }

	/// <summary>
  /// ダンジョンのマップチップ配列
  /// </summary>
	private BitFlag[,] chips;

	/// <summary>
  /// ダンジョンの空間分割数(X,Y)
  /// </summary>
	private Vector2Int divCount;

	/// <summary>
  /// ダンジョンの分割座標
  /// </summary>
	private int[,] divPoints;

	/// <summary>
  /// 部屋を構築できるエリア情報の配列
  /// </summary>
	private RectInt[,] roomAreas;

	public Algorithm()
  {
		this.chips = new BitFlag[Dungeon.Define.WIDTH, Dungeon.Define.HEIGHT];
		this.SetConfig(0, 0);
  }

	/// <summary>
  /// ダンジョン生成に関する設定を行う。
  /// </summary>
  /// <param name="divX">X方向の分割数</param>
  /// <param name="divY">Y方向の分割数</param>
	public void SetConfig(int divX, int divY)
  {
		this.divCount = new Vector2Int(divX, divY);
		this.divPoints = new int[divX, divY];
		this.roomAreas = new RectInt[divX, divY];
  }

	public void Make(Dungeon.Stage stage)
  {
		// リセット
		// 初期化
		// 空間分割
		// ルームスペース確保
		// ルーム作成
		// 通路作成(上下左右)
		// 通路を繋げる
		// 整理

  }

	private void reset()
  {

  }




}

}