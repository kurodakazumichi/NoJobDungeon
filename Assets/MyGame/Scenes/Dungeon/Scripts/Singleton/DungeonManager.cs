using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Dungeon
{
  /// <summary>
  /// ダンジョンマネージャー
  /// </summary>
  public class DungeonManager : SingletonMonobehaviour<DungeonManager>, IDebuggeable
  {
    //-------------------------------------------------------------------------
    // メンバ変数
    /// <summary>
    /// ダンジョン生成アルゴリズム
    /// </summary>
    private Algorithm algorithm;

    /// <summary>
    /// ダンジョン配列
    /// </summary>
    private Stage stage;

    /// <summary>
    /// ダンジョンの階数
    /// </summary>
    private int floor = 1;

    //-------------------------------------------------------------------------
    // Public Properity

    /// <summary>
    /// 現在の階層
    /// </summary>
    public int Floor => (this.floor);

    //-------------------------------------------------------------------------
    // Public Method

    /// <summary>
    /// このメソッドを実行するたびに、新しくダンジョンデータが作成される。
    /// </summary>
    public void CreateStage()
    {
      this.algorithm.SetConfig(3, 2, 0.5f);
      this.stage.Make(this.algorithm);
    }

    /// <summary>
    /// プレイヤーの座標
    /// </summary>
    public Vector2Int PlayerCoord
    {
      get { 
        return this.stage.Find(Tiles.Player)[0]; 
      }
    }

    /// <summary>
    /// 次のフロアへ行ける
    /// </summary>
    public bool CanGoNextFloor
    {
      get
      {
        return this.stage.Find(Tiles.Goal)[0].Equals(PlayerCoord);
      }
    }

    //-------------------------------------------------------------------------
    // 取得

    /// <summary>
    /// ステージのタイル情報を取得する
    /// </summary>
    public IReadOnlyTile GetTile(int x, int y)
    {
      return this.stage.GetTile(x, y);
    }

    /// <summary>
    /// ステージのタイル情報を取得する
    /// </summary>
    public IReadOnlyTile GetTile(Vector2Int coord)
    {
      return this.stage.GetTile(coord);
    }

    public IReadOnlyTile GetTile(Vector2Int coord, Direction direction)
    {
      return GetTile(Dungeon.Util.GetCoord(coord, direction));
    }

    /// <summary>
    /// プレイヤーの座標を更新する
    /// </summary>
    public void UpdatePlayerCoord(Vector2Int from, Vector2Int to)
    {
      this.stage.RemoveTileState(from, Tiles.Player);
      this.stage.AddTileState(to, Tiles.Player);
    }

    /// <summary>
    /// 敵の座標を更新する
    /// </summary>
    public void UpdateEnemyCoord(Vector2Int from, Vector2Int to)
    {
      this.stage.RemoveTileState(from, Tiles.Enemy);
      this.stage.AddTileState(to, Tiles.Enemy);
    }

    /// <summary>
    /// 指定した座標から敵の情報を除去する
    /// </summary>
    public void RemoveEnemyCoord(Vector2Int coord)
    {
      this.stage.RemoveTileState(coord, Tiles.Enemy);
    }

    /// <summary>
    /// 指定した座標にアイテム情報を追加する
    /// </summary>
    public void AddItemCoord(Vector2Int coord)
    {
      this.stage.AddTileState(coord, Tiles.Item);
    }

    /// <summary>
    /// 指定した座標のアイテム情報を削除する
    /// </summary>
    /// <param name="coord"></param>
    public void RemoveItemCoord(Vector2Int coord)
    {
      this.stage.RemoveTileState(coord, Tiles.Item);
    }

    /// <summary>
    /// フロアを上る
    /// </summary>
    public void upFloor()
    {
      ++this.floor;
    }

    /// <summary>
    /// フロア数を減らす
    /// </summary>
    public void downFloor()
    {
      this.floor = Mathf.Max(1, --this.floor);
    }

    /// <summary>
    /// 踏破フラグを更新する
    /// </summary>
    public void UpdateClearFlags()
    {
      this.stage.UpdateClearFlag(PlayerCoord);
    }

    //-------------------------------------------------------------------------
    // その他

    public void Map(System.Action<int, int, IReadOnlyTile> cb)
    {
      this.stage.Map(cb);
    }

    /// <summary>
    /// 中心座標から周囲 distance マス を見る
    /// 範囲外に該当する座標はスキップする
    /// </summary>
    public void LookAround(Vector2Int center, int distance, System.Action<int, int, IReadOnlyTile> cb)
    {
      // LoopByRectで回すためのRect情報を作成
      var rx   = center.x - distance;
      var ry   = center.y - distance;
      var size = distance * 2 + 1;
      var rect = new RectInt(rx, ry, size, size);

      // ループしてコールバックを呼ぶ
      MyGame.Util.LoopByRect(rect, (x, y) =>
      {
        // 範囲外アクセスは無視
        if (x < 0 || Define.WIDTH  - 1 <= x) return;
        if (y < 0 || Define.HEIGHT - 1 <= y) return;

        cb(x, y, GetTile(x, y));
      });
    }

    //-------------------------------------------------------------------------
    // 主要なメソッド

    /// <summary>
    /// メンバの初期化
    /// </summary>
    protected override void Awake()
    {
      base.Awake();
      this.algorithm = new Algorithm();
      this.stage = new Stage();
      this.floor = 1;
    }

#if _DEBUG
    void IDebuggeable.Draw(MyDebug.Window window)
    {
      GUILayout.Label($"Floor:{this.floor}");

      this.stage.DrawDebugMenu(window);

      if (GUILayout.Button("Algorithm"))
      {
        DebugManager.Instance.OpenWindow(
          "Algorithm",
          this.algorithm.DrawDebugMenu
        );
      }
    }
#endif
  }

}

