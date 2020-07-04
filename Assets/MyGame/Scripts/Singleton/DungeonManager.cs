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

    private MapChip.FieldChip[,] chips = new MapChip.FieldChip[Define.WIDTH, Define.HEIGHT];

    void Start()
    {
      Util.Loop2D(Define.WIDTH, Define.HEIGHT, (int x, int y) => {
        this.chips[x, y] = MapChipFactory.Instance.CreateFieldChip(FieldType.None);
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
    }

    public Vector2Int PlayerCoord
    {
      get { return this.stage.Find(Tiles.Player)[0]; }
    }

    // TODO: ここもここでやるのいや
    public void CreateMapChips()
    {
      this.stage.Map((int x, int y, Tile tile) => {

        var chip = this.chips[x, y];

        if(tile.IsAisle || tile.IsRoom) {
          chip.Type = FieldType.Floor;
          
        }

        else {
          chip.Type = FieldType.Wall;
        }

        chip.transform.localScale = Define.CHIP_SCALE;
        chip.transform.position = GetPositionFromCoord(x, y);

      });
    }

    public IReadOnlyTile GetTile(Vector2Int coord)
    {
      return this.stage.GetTile(coord);
    }

    public IReadOnlyTile GetTile(Vector2Int coord, Direction8 direction)
    {
      return GetTile(GetCoord(coord, direction));
    }

    /// <summary>
    /// 指定座標に方向を加えた先の座標を取得する
    /// </summary>
    public Vector2Int GetCoord(Vector2Int v, Direction8 dir)
    {
      int x = 0;
      int y = 0;

      switch(dir) {
        case Direction8.Left     : --x; break;
        case Direction8.Right    : ++x; break;
        case Direction8.Up       : --y; break;
        case Direction8.Down     : ++y; break;
        case Direction8.LeftUp   : --x; --y; break;
        case Direction8.LeftDown : --x; ++y; break;
        case Direction8.RightUp  : ++x; --y; break;
        case Direction8.RightDown: ++x; ++y; break;
      }
      
      v.x += x;
      v.y += y;
      return v;
    }

    /// <summary>
    /// 座標から位置を取得する
    /// </summary>
    public Vector3 GetPositionFromCoord(int x, int y)
    {
      return new Vector3(x * Define.CHIP_SCALE.x, -y * Define.CHIP_SCALE.y, 0);
    }
    public Vector2 GetPositionFromCoord(Vector2Int coord)
    {
      return GetPositionFromCoord(coord.x, coord.y);
    }

    public void UpdatePlayerCoord(Vector2Int coord)
    {
      this.stage.UpdatePlayerCoord(coord);
    }

#if UNITY_EDITOR

    public bool showDebug = true;

    private void OnGUI()
    {
      if (!this.showDebug) return;

      this.stage.OnGUI();
    }
    
#endif
  }

}

