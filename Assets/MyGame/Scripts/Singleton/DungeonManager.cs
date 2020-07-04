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
      //Util.Loop2D(Define.WIDTH, Define.HEIGHT, (int x, int y) => {
      //  this.chips[x, y] = MapChipFactory.Instance.CreateFieldChip(FieldType.None);
      //});
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

    public void Init()
    {
      Util.Loop2D(Define.WIDTH, Define.HEIGHT, (int x, int y) =>
      {
        this.chips[x, y] = MapChipFactory.Instance.CreateFieldChip(FieldType.None);
      });
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
        chip.transform.position = GetPositionBy(x, y);

      });
    }

    public IReadOnlyTile GetTile(Vector2Int coord)
    {
      return this.stage.GetTile(coord);
    }

    public IReadOnlyTile GetTile(Vector2Int coord, Direction direction)
    {
      return GetTile(GetCoord(coord, direction));
    }

    /// <summary>
    /// 指定座標に方向を加えた先の座標を取得する
    /// </summary>
    public Vector2Int GetCoord(Vector2Int coord, Direction dir)
    {
      // ダンジョン座標はY方向の上がマイナスなので、第一引数(yUp)にfalseを指定
      return coord + dir.ToVector(false);
    }

    /// <summary>
    /// XY座標から位置を取得する
    /// </summary>
    public Vector3 GetPositionBy(int x, int y)
    {
      return new Vector3(x * Define.CHIP_SCALE.x, -y * Define.CHIP_SCALE.y, 0);
    }

    /// <summary>
    /// Vector2Intから位置を取得する
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    public Vector3 GetPositionBy(Vector2Int coord)
    {
      return GetPositionBy(coord.x, coord.y);
    }

    /// <summary>
    /// XY座標+方向から位置を取得する
    /// </summary>
    public Vector3 GetPositionBy(Vector2Int coord, Direction direction)
    {
      var next = GetCoord(coord, direction);
      return GetPositionBy(next);
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

