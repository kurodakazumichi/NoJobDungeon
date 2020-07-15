using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
	public static class Define
	{
		public const int WIDTH = 60;
		public const int HEIGHT = 40;
		public const int MIN_ROOM_SIZE = 3;

    /// <summary>
    /// １ターン当たりの秒数
    /// </summary>
    public const float SEC_PER_TURN = 0.15f;

    /// <summary>
    /// 基本命中率
    /// </summary>
    public const float HIT_RATE = 0.9f;

    public static Vector3 CHIP_SCALE
    {
      get { return new Vector3(1, 1 ,1); }
    }
	}
}