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
    private Algorithm algorithm;
    private Stage stage;


    protected override void Awake()
    {
      base.Awake();
      this.algorithm = new Algorithm();
      this.stage     = new Stage();
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

    public void Map(System.Action<int, int, IReadOnlyTile> cb)
    {
      this.stage.Map(cb);
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

