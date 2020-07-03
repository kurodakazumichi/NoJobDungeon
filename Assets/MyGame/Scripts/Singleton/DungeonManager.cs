using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dungeon;
using MapChip;

namespace Singleton 
{
  /// <summary>
  /// ダンジョンマネージャー
  /// </summary>
  public class DungeonManager : SingletonMonobehaviour<DungeonManager>
  {

    private PlayerManager PlayerMan
    {
      get { return PlayerManager.Instance; }
    }

    private CameraManager CameraMan
    {
      get { return CameraManager.Instance; }
    }

    private Algorithm algorithm = new Algorithm();
    private Stage stage = new Stage();

    private MapChip.SimpleChip[,] chips = new MapChip.SimpleChip[Define.WIDTH, Define.HEIGHT];

    void Start()
    {
      Util.Loop2D(Define.WIDTH, Define.HEIGHT, (int x, int y) => {
        this.chips[x, y] = MapChipFactory.Instance.Create(MapChipType.None);
      });
    }

    void Update()
    {
      
    }

    /// <summary>
    /// ダンジョンの生成
    /// </summary>
    public void CreateStage()
    {
      this.algorithm.SetConfig(3, 2, 0.5f);
      this.stage.Make(this.algorithm);

      // マップを生成
      this.CreateMapChips();

      // プレイヤーを生成
      var coord = this.stage.Find(Tiles.Player)[0];
       PlayerMan.CreatePlayer(GetPosition(coord.x, coord.y), coord);

      // カメラの設定
      CameraMan.SetDungeonMode(PlayerMan.PlayerObject);
    }

    public void CreateMapChips()
    {
      this.stage.Map((int x, int y, Tile tile) => {

        var chip = this.chips[x, y];

        if(tile.IsAisle || tile.IsRoom) {
          chip.Type = MapChipType.Floor;
          
        }

        else {
          chip.Type = MapChipType.Wall;
        }

        chip.transform.localScale = Define.CHIP_SCALE;
        chip.transform.position = GetPosition(x, y);

      });
    }



    public Vector3 GetPosition(int x, int y)
    {
      return new Vector3(x * Define.CHIP_SCALE.x, -y * Define.CHIP_SCALE.y, 0);
    }

#if UNITY_EDITOR

    public bool showDebug = true;

    private void OnGUI()
    {
      if (!this.showDebug) return;

			if (GUI.Button(new Rect(0, 0, 100, 20), "create"))
			{
				this.CreateStage();
			}

      this.stage.OnGUI();

    }
    
#endif
  }

}

