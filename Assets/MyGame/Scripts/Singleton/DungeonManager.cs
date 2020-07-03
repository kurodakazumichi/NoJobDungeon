using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dungeon;
using Field;

namespace Singleton 
{
  /// <summary>
  /// ダンジョンマネージャー
  /// </summary>
  public class DungeonManager : SingletonMonobehaviour<DungeonManager>
  {
    private Algorithm algorithm = new Algorithm();
    private Stage stage = new Stage();

    private MapChip[,] chips = new MapChip[Define.WIDTH, Define.HEIGHT];

    void Start()
    {
      Util.Loop2D(Define.WIDTH, Define.HEIGHT, (int x, int y) => {
        this.chips[x, y] = MapChipFactory.Instance.Create(MapChipType.None);
      });
    }

    void Update()
    {
      
    }

    public void CreateStage()
    {
      this.algorithm.Make(this.stage, 3, 2, 0.5f);

      

      this.CreateMapChips();
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

        chip.transform.position = GetPosition(x, y);

      });
    }

    public Vector3 GetPosition(int x, int y)
    {
      return new Vector3(x, -y, 0);
    }




#if UNITY_EDITOR
    private void OnGUI()
    {

			if (GUI.Button(new Rect(0, 0, 100, 20), "create"))
			{
				this.CreateStage();
			}

      this.stage.OnGUI();

    }
    
#endif
  }

}

