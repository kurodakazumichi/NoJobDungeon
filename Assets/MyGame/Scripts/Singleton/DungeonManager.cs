using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.MapChip;
using MyGame.Dungeon;

namespace MyGame.Singleton 
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

    private Algorithm algorithm;
    private Stage stage;

    private MapChip.FieldChip[,] chips = new MapChip.FieldChip[Define.WIDTH, Define.HEIGHT];

    protected override void Awake()
    {
      base.Awake();
      this.algorithm = new Algorithm();
      this.stage     = new Stage();
    }

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
        chip.transform.position   = Dungeon.Util.GetPositionBy(x, y);

      });
    }

    public IReadOnlyTile GetTile(Vector2Int coord)
    {
      return this.stage.GetTile(coord);
    }

    public IReadOnlyTile GetTile(Vector2Int coord, Direction direction)
    {
      return GetTile(Dungeon.Util.GetCoord(coord, direction));
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

