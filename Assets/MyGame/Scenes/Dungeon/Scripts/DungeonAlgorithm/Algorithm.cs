using System.Collections.Generic;
using UnityEngine;
using MyExtension;

namespace MyGame.Dungeon {
  /// <summary>
  /// ダンジョン自動生成アルゴリズム(均等分割法)
  /// 配列を縦横に均等に分割する方法でダンジョンを生成する。
  /// 
  /// ※プレイヤーやアイテム、敵の配置などは行わない。
  /// </summary>
  public class Algorithm {
    private enum Flags : uint {
      Wall = 1 << 0, // 壁
      Room = 1 << 1, // 部屋
      Aisle = 1 << 2, // 通路
      ReservedAisle = 1 << 3, // 通路予定地
      Cross = 1 << 4, // 交差点(通路予定地の交差地点)
      Confluence = 1 << 5, // 合流地点(部屋から伸ばす通路と通路予定地の合流地点)
    }

    //-------------------------------------------------------------------------
    // ダンジョン生成に影響を与えるパラメーター郡
    #region 

    /// <summary>
    /// ダンジョンの空間分割数(X,Y)
    /// </summary>
    private Vector2Int size;

    /// <summary>
    /// 部屋作成率、数値が高いほど部屋が作られやすい。0~1の間で設定する。
    /// </summary>
    private float roomMakingRate = 0.7f;

    #endregion

    //-------------------------------------------------------------------------
    // ダンジョン生成中に利用するモノ
    #region

    /// <summary>
    /// ダンジョンのマップチップ配列
    /// </summary>
    private BitFlag[,] chips;

    /// <summary>
    /// ダンジョンの分割位置
    /// </summary>
    private List<int> splitPointsX;
    private List<int> splitPointsY;

    /// <summary>
    /// 部屋予定地のエリア情報
    /// </summary>
    private RectInt[,] reservedRooms;

    /// <summary>
    /// 部屋のエリア情報
    /// </summary>
    private Room[,] rooms;

    /// <summary>
    /// 通路を繋げる処理中に使用するフラグ変数
    /// </summary>
		private bool aisleCreationFlag;

    #endregion

    //-------------------------------------------------------------------------
    // Method

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Algorithm()
    {
      this.chips = new BitFlag[Dungeon.Define.WIDTH, Dungeon.Define.HEIGHT];
      this.splitPointsX = new List<int>();
      this.splitPointsY = new List<int>();

      this.aisleCreationFlag = false;

      SetConfig();
    }

    //-------------------------------------------------------------------------
    // ダンジョン生成 Methods

    /// <summary>
    /// ダンジョン生成パラメータの設定
    /// </summary>
    public void SetConfig(int sizeX = 1, int sizeY = 1, float rate = 1f)
    {
      // ダンジョンの分割数を設定、最低でも1x1になるようにフィルター
      this.size.Set(Mathf.Max(1, sizeX), Mathf.Max(1, sizeY));
      this.roomMakingRate = Mathf.Max(0, Mathf.Min(1f, rate));
    }

    /// <summary>
    /// ダンジョン生成
    /// </summary>
    /// <param name="stage"></param>
    public void Make(Dungeon.Stage stage)
    {
      // 初期化
      Init();

      // 準備
      Prepare();

      // マップチップ配列を分割
      SplitChipsX();
      SplitChipsY();

      // 通路予定地を埋める
      MarkupChipsWithReservedAisle();

      // ルームスペース確保
      MakeReservedRoom();

      // ルーム作成
      MakeRoom();
      MarkupRoom();

      // 通路作成(上下左右)
      MakeAisleLeft();
      MakeAisleRight();
      MakeAisleUp();
      MakeAisleDown();

      // 通路を繋げる
      ConnectAisleY();
      ConnectAisleX();

      // 不要な通路を消す
      DeleteUselessAisle();

      // ステージに内容を展開する
      DeployToStage(stage);
    }

    /// <summary>
    /// メンバの初期化
    /// </summary>
    private void Init()
    {
      this.aisleCreationFlag = false;
      this.splitPointsX.Clear();
      this.splitPointsY.Clear();

      this.reservedRooms = new RectInt[this.size.x, this.size.y];
      this.rooms = new Room[this.size.x, this.size.y];

      MapForSize((int x, int y) => {
        this.rooms[x, y] = new Room();
      });
    }

    /// <summary>
    /// ステージを生成する準備として、まずchipsをFlags.Wallで埋める。
    /// </summary>
    private void Prepare()
    {
      MapForChip((int x, int y, BitFlag _) => {
        this.chips[x, y].Set((uint)Flags.Wall);
      });
    }

    /// <summary>
    /// ステージをX方向に分割する位置を決定する
    /// </summary>
    private void SplitChipsX()
    {
      // 均等分割なので単純に幅を分割数Xで割る
      int width = Define.WIDTH / this.size.x;

      // 縦に分割するx座標リストを作成
      this.splitPointsX.Add(1);

      for (int i = 1; i < this.size.x; ++i) {
        this.splitPointsX.Add(i * width);
      }

      this.splitPointsX.Add(Define.WIDTH - 2);
    }

    /// <summary>
    /// ステージをY方向に分割する位置を決定する
    /// </summary>
    private void SplitChipsY()
    {
      // 均等分割なので単純に高さを分割数Yで割る
      int width = Define.HEIGHT / this.size.y;

      // 横に分割するy座標リストを作成
      this.splitPointsY.Add(1);

      for (int i = 1; i < this.size.y; ++i) {
        this.splitPointsY.Add(i * width);
      }

      this.splitPointsY.Add(Define.HEIGHT - 2);
    }

    /// <summary>
    /// 分割した座標を元に通路予定地に印をつける。
    /// </summary>
    private void MarkupChipsWithReservedAisle()
    {
      // Y方向の処理
      this.splitPointsX.ForEach((int x) => {
        int y1 = 1;
        int y2 = Define.HEIGHT - 2;

        MyGame.Util.LoopByRange(y1, y2 + 1, (int y) => {
          this.chips[x, y].On((uint)Flags.ReservedAisle);
        });
      });

      // X方向の処理
      this.splitPointsY.ForEach((int y) => {
        int x1 = 1;
        int x2 = Define.WIDTH - 2;

        MyGame.Util.LoopByRange(x1, x2 + 1, (int x) => {
          this.chips[x, y].On((uint)Flags.ReservedAisle);
        });
      });

      // 通路予定地が交差するポイントにも印をつける。
      this.splitPointsX.ForEach((int x) => {
        this.splitPointsY.ForEach((int y) => {
          this.chips[x, y].On((uint)Flags.Cross);
        });
      });
    }

    /// <summary>
    /// ルーム予定地を作る
    /// </summary>
    private void MakeReservedRoom()
    {
      this.MapForSize((int x, int y) => {
        int x1 = this.splitPointsX[x] + 2;
        int x2 = this.splitPointsX[x + 1] - 2;
        int y1 = this.splitPointsY[y] + 2;
        int y2 = this.splitPointsY[y + 1] - 2;
        int w = x2 - x1 + 1;
        int h = y2 - y1 + 1;

        // ルーム予定地のx,y座標、幅高さを決定
        RectInt room = new RectInt(x1, y1, w, h);
        this.reservedRooms[x, y] = room;
      });

    }

    /// <summary>
    /// 部屋予定地の中に部屋を作る。
    /// </summary>
    private void MakeRoom()
    {
      // 部屋が存在しない列が出来ると部屋を繋げるのが大変になるので
      // 必ず縦列に１つ部屋を作る。
      // 4x2に分割されている場合だと、最低でも4つは部屋が出来る事になる。
      for (int x = 0; x < this.size.x; ++x) {
        int y = Random.Range(0, this.size.y);
        this.rooms[x, y] = CreateRoomByRect(this.reservedRooms[x, y]);
      }

      // 部屋がない空間に対してランダムで部屋を生成する。
      // これで多少ランダム性が生まれる。
      this.MapForSize((int x, int y) => {
        // 既に部屋があったらスキップ
        if (ExistsRoom(x, y)) return;

        // ランダムで部屋を作るかどうか決定
        if (this.roomMakingRate < Random.Range(0f, 1f)) return;

        this.rooms[x, y] = CreateRoomByRect(this.reservedRooms[x, y]);
      });
    }

    /// <summary>
    /// 部屋を埋める
    /// </summary>
    private void MarkupRoom()
    {
      MapForRoom((int rx, int ry, Room room) => {
        MyGame.Util.LoopByRect(room.Area, (int x, int y) => {
          this.chips[x, y].Set((uint)Flags.Room);
        });
      });
    }


    /// <summary>
    /// 部屋から左方向への通路を伸ばす
    /// </summary>
    private void MakeAisleLeft()
    {
      MapForRoom((int rx, int ry, Room room) => {
        // 左端の部屋は左方向の通路を伸ばさない。
        if (rx == 0) return;

        // 通路のY座標を決める
        int y = Random.Range(room.yMin, room.yMax);

        // 通路のX座標(始点と終点)を求める。
        int x2 = room.xMin - 1;
        int x1 = SearchChipToLeft(x2, y, (uint)Flags.ReservedAisle);

        // 通路予定地が見つからなかったら終了
        if (x1 < 0) return;

        // 部屋から通路予定地に通路を作成
        MyGame.Util.LoopByRange(x1, x2 + 1, (int x) => {
          this.chips[x, y].On((uint)Flags.Aisle);
        });

        // 通路予定地と通路が合流したポイントをマークする
        this.chips[x1, y].On((uint)Flags.Confluence);
      });
    }

    /// <summary>
    /// 部屋から右方向への通路を伸ばす
    /// </summary>
    private void MakeAisleRight()
    {
      this.MapForRoom((int rx, int ry, Room room) => {
        // 右端の部屋は右方向の通路を作らない。
        if (rx == this.size.x - 1) return;

        // 通路のY座標を決める
        int y = Random.Range(room.yMin, room.yMax);

        // 通路のX座標(始点と終点)を求める。
        int x1 = room.xMax;
        int x2 = SearchChipToRight(x1, y, (uint)Flags.ReservedAisle);

        // 通路予定地が見つからなかったら終了
        if (x2 < 0) return;

        // 部屋から通路予定地に通路を作成
        MyGame.Util.LoopByRange(x1, x2 + 1, (int x) => {
          this.chips[x, y].On((uint)Flags.Aisle);
        });

        // 通路予定地と通路が合流したポイントをマークする
        this.chips[x2, y].On((uint)Flags.Confluence);
      });
    }

    /// <summary>
    /// 部屋から上方向に通路を伸ばす
    /// </summary>
    private void MakeAisleUp()
    {
      this.MapForRoom((int rx, int ry, Room room) => {
        // 上端の部屋は上方向の通路を作らない
        if (ry == 0) return;

        // 上方向の通路は50%の確立で作られる事にしておく。
        if (Random.Range(0f, 1f) < 0.5f) return;

        // 通路のX座標を決める
        int x = Random.Range(room.xMin, room.xMax);

        // 通路のY座標(始点と終点)を求める。
        int y2 = room.yMin - 1;
        int y1 = SearchChipToUp(x, y2, (uint)Flags.ReservedAisle);

        // 通路予定地が見つからなかったら終了
        if (y1 < 0) return;

        // 部屋から通路予定地に通路を作成
        MyGame.Util.LoopByRange(y1, y2 + 1, (int y) => {
          this.chips[x, y].On((uint)Flags.Aisle);
        });

        // 通路予定地と通路が合流したポイントをマークする
        this.chips[x, y1].On((uint)Flags.Confluence);
      });
    }

    /// <summary>
    /// 部屋から下方向に通路を伸ばす
    /// </summary>
    private void MakeAisleDown()
    {
      this.MapForRoom((int rx, int ry, Room room) => {
        // 下端の部屋からは下方向の通路を伸ばさない。
        if (ry == this.size.y - 1) return;

        // 下方向の通路は50%の確立で作られる事にしておく。
        if (Random.Range(0, 1f) < 0.5f) return;

        // 通路のX座標を決める
        int x = Random.Range(room.xMin, room.xMax);

        // 通路のY座標(始点と終点)を求める。
        int y1 = room.yMax;
        int y2 = SearchChipToDown(x, y1, (uint)Flags.ReservedAisle);

        // 通路予定地が見つからなかったら終了
        if (y2 < 0) return;

        // 部屋から通路予定地に通路を作成
        MyGame.Util.LoopByRange(y1, y2 + 1, (int y) => {
          this.chips[x, y].On((uint)Flags.Aisle);
        });

        // 通路予定地と通路が合流したポイントをマークする
        this.chips[x, y2].On((uint)Flags.Confluence);
      });
    }

    /// <summary>
    /// Y方向について、通路を繋げる
    /// このアルゴリズムではここの処理で全ての部屋が繋がるような道を作る。
    /// </summary>
    private void ConnectAisleY()
    {
      this.splitPointsX.ForEach((int x, int index) => {
        // 部屋と通路の合流地点を探す(Y方向)
        var found = FindChipsY(x, (uint)Flags.Confluence);

        // 合流地点が１つ以下ならスキップ
        if (found.Count <= 1) return;

        // 合流地点が奇数の場合、もしくは奇数列の場合は
        // 最初と最後の地点を結ぶ。
        // こうする事で到達できない部屋が作られなくなる(はず)
        if (found.Count % 2 == 1 || index % 2 == 1) {
          MyGame.Util.LoopByRange(found.First(), found.Last(), (int y) => {
            this.chips[x, y].On((uint)Flags.Aisle);
          });
          return;
        }

        // 合流地点が偶数の場合
        for (int i = 0; i < found.Count; i += 2) {
          MyGame.Util.LoopByRange(found[i], found[i + 1], (int y) => {
            this.chips[x, y].On((uint)Flags.Aisle);
          });
        }
      });
    }

    /// <summary>
    /// X方向について、間の通路を作成する
    /// Y方向の通路作成処理で全ての部屋は繋がるはずなので
    /// X方向の通路はダンジョンのバリエーションを増やす方向で作成する。
    /// </summary>
    private void ConnectAisleX()
    {
      this.splitPointsY.ForEach((int y) => {
        // 部屋と通路の合流地点を探す
        var found = FindChipsX(y, (uint)Flags.Confluence);

        found.ForEach((int x) => {
          this.aisleCreationFlag = false;
          ProcConnectAisleX(x + 1, y);
        });
      });
    }


    /// <summary>
    /// 無駄な通路を消す。
    ///
    /// 合流地点の周囲に２つ以上の通路が確認できれば
    /// その合流地点はどこかしらに繋がっているが
    /// 合流地点の周囲に通路が1つしかない場合は無駄な道になっているので消す対象。
    /// 
    /// 縦方向に存在する合流地点は必ずどこかと繋がっているので
    /// 確認するのは横方向の合流地点のみでよい。
    /// </summary>
		private void DeleteUselessAisle()
    {
      this.splitPointsY.ForEach((int y) => {
        var found = this.FindChipsX(y, (uint)Flags.Confluence);

        // 合流地点の上下左右に繋がる通路があるかを見る
        found.ForEach((int x) => {
          // 周囲に通路が1つしかないなら無駄な通路なので消す。
          if (this.CountAdjacencies(x, y, (uint)Flags.Aisle) == 1) {
            this.ProcDeleteUselessAnsle(x, y);
          }
        });
      });
    }

    /// <summary>
    /// 生成したデータをステージへ展開する。
    /// </summary>
    private void DeployToStage(Stage stage)
    {
      stage.Reset();

      // マップチップを展開
      this.MapForChip((int x, int y, BitFlag flag) => {

        if (flag.Contain((uint)Flags.Wall)) {
          stage.SetTileState(x, y, Tiles.Wall);
        }
        if (flag.Contain((uint)Flags.Room)) {
          stage.SetTileState(x, y, Tiles.Room);
        }
        if (flag.Contain((uint)Flags.Aisle)) {
          stage.SetTileState(x, y, Tiles.Aisle);
        }
      });

      // 部屋を登録
      this.MapForRoom((int x, int y, Room room) => {
        stage.AddRoom(room.Area);
      });
    }

    //-------------------------------------------------------------------------
    // Private Utility Methods

    private void MapForChip(System.Action<int, int, BitFlag> cb)
    {
      for (int x = 0; x < Define.WIDTH; ++x) {
        for (int y = 0; y < Define.HEIGHT; ++y) {
          cb(x, y, this.chips[x, y]);
        }
      }
    }

    private void MapForSize(System.Action<int, int> cb)
    {
      for (int y = 0; y < this.size.y; ++y) {
        for (int x = 0; x < this.size.x; ++x) {
          cb(x, y);
        }
      }
    }

    private void MapForRoom(System.Action<int, int, Room> cb)
    {
      MapForSize((int x, int y) => {
        var room = this.rooms[x, y];

        if (room.isEnable) {
          cb(x, y, room);
        }
      });
    }

    /// <summary>
    /// 座標に隣接するflagsの数を数える。
    /// </summary>
    /// <returns>隣接するflagsの数</returns>
    private int CountAdjacencies(int x, int y, uint flags)
    {
      // 上下左右に2つ以上の通路がなければ無効
      int count = 0;
      if (this.chips[x, y - 1].Contain(flags)) ++count;
      if (this.chips[x, y + 1].Contain(flags)) ++count;
      if (this.chips[x + 1, y].Contain(flags)) ++count;
      if (this.chips[x - 1, y].Contain(flags)) ++count;

      return count;
    }

    /// <summary>
    /// 指定位置から左方向へ探索し、flagsに一致するチップの場所を返す。
    /// 見つからなければ-1を返す。
    /// </summary>
    private int SearchChipToLeft(int x, int y, uint flags)
    {
      for (; 0 < x; --x) {
        if (this.chips[x, y].Contain(flags)) return x;
      }
      return -1;
    }

    /// <summary>
    /// 指定位置から右方向へ探索し、flagsに一致するチップの場所を返す。
    /// 見つからなければ-1を返す。
    /// </summary>
    private int SearchChipToRight(int x, int y, uint flags)
    {
      for (; x < Define.WIDTH - 1; ++x) {
        if (this.chips[x, y].Contain(flags)) return x;
      }
      return -1;
    }

    /// <summary>
    /// 指定位置から上方向へ探索し、flagsに一致するチップの場所を返す。
    /// 見つからなければ-1を返す。
    /// </summary>
    private int SearchChipToUp(int x, int y, uint flags)
    {
      for (; 0 < y; --y) {
        if (this.chips[x, y].Contain(flags)) return y;
      }
      return -1;
    }

    /// <summary>
    /// 指定位置から下方向へ探索し、flagsに一致するチップの場所を返す。
    /// 見つからなければ-1を返す。
    /// </summary>
    private int SearchChipToDown(int x, int y, uint flags)
    {
      for (; y < Define.HEIGHT - 1; ++y) {
        if (this.chips[x, y].Contain(flags)) return y;
      }
      return -1;
    }

    /// <summary>
    /// Y方向のチップ情報を見つける
    /// </summary>
    /// <param name="x">指定したx列が走査の対象</param>
    /// <param name="chips">見つけたいチップ</param>
    /// <returns>該当するchipのY座標リスト</returns>
    private List<int> FindChipsY(int x, uint chips, bool isEither = true)
    {
      List<int> found = new List<int>();

      for (int y = 1; y < Define.HEIGHT - 1; ++y) {
        var chip = this.chips[x, y];

        // isEitherフラグをみて判定メソッドを切り替える。
        bool flag = (isEither)
          ? chip.ContainEither(chips)
          : chip.Contain(chips);

        if (flag) found.Add(y);
      }

      return found;
    }

    /// <summary>
    /// X方向のチップ情報を見つける
    /// </summary>
    /// <param name="y">指定したy行が操作の対象</param>
    /// <param name="chips">見つけたいchip</param>
    /// <returns>該当するchipのX座標リスト</returns>
    private List<int> FindChipsX(int y, uint chips, bool isEither = true)
    {
      List<int> found = new List<int>();

      for (int x = 1; x < Define.WIDTH - 1; ++x) {
        var chip = this.chips[x, y];

        // isEitherフラグをみて判定メソッドを切り替える。
        bool flag = (isEither)
          ? chip.ContainEither(chips)
          : chip.Contain(chips);

        if (flag) found.Add(x);
      }

      return found;
    }


    //-------------------------------------------------------------------------
    // 部屋作成に関する処理

    /// <summary>
    /// 矩形情報に収まるように部屋の位置、大きさを決定し矩形情報を返す。
    /// </summary>
    /// <param name="area">部屋作成が可能な領域</param>
    private Room CreateRoomByRect(RectInt area)
    {
      // 部屋のサイズをランダムに決める(部屋予定地に収まるように)
      int width = Random.Range(Define.MIN_ROOM_SIZE, area.width);
      int height = Random.Range(Define.MIN_ROOM_SIZE, area.height);

      // 部屋の位置をランダムに決める。(部屋予定地に収まるように)
      int x = Random.Range(area.xMin, area.xMax - width);
      int y = Random.Range(area.yMin, area.yMax - height);

      return new Room(x, y, width, height);
    }

    /// <summary>
    /// 部屋が存在するかの判定
    /// </summary>
    private bool ExistsRoom(int x, int y)
    {
      return this.rooms[x, y].isEnable;
    }

    //-------------------------------------------------------------------------
    // 通路を繋げることに関する処理

    /// <summary>
    /// 指定された位置からX方向に通路を繋げていく再帰処理
    /// 
    /// [仕様]
    /// #:壁, X:交差点, C:合流地点, 0:通路
    /// 
    /// ①合流地点から合流地点の場合はその間を通路化する
    /// ###C########C####
    /// ↓
    /// ###C00000000C####
    /// 
    /// ②合流地点から交差点の間はランダムで通路化する
    /// ###C########X####
    /// ↓
    /// ###C00000000X####
    /// or
    /// ###C########X####
    /// </summary>
    private void ProcConnectAisleX(int x, int y)
    {
      var chip = this.chips[x, y];

      // 合流地点に到達したら通路作成フラグをON
      if (chip.Contain((uint)Flags.Confluence)) {
        this.aisleCreationFlag = true;
        return;
      }

      // 交差点に到達した場合
      if (chip.Contain((uint)Flags.Cross)) {
        // マップの右端の交差点の場合もあるので、その場合は無視する。
        if (x == Define.WIDTH - 2) return;

        // 上下左右どこにもつながってない交差点には繋げても意味ないので無視する。
        if (CountAdjacencies(x, y, (uint)Flags.Aisle) == 0) {
          return;
        }

        // 20%の確率で通路作成フラグをONにする
        this.aisleCreationFlag = (Random.Range(0f, 1f) < 0.2f);

        // 通路作成
        if (this.aisleCreationFlag) {
          this.chips[x, y].On((uint)Flags.Aisle);
        }
        return;
      }

      // 右端に到達したら終了
      if (Define.WIDTH - 1 <= x) return;

      // 再帰処理
      ProcConnectAisleX(x + 1, y);

      // 通路作成フラグがONだったら通路化する
      if (this.aisleCreationFlag) {
        this.chips[x, y].On((uint)Flags.Aisle);
      }
    }


    /// <summary>
    /// 指定された位置から再帰的に通路を消す。
		private void ProcDeleteUselessAnsle(int x, int y)
    {
      var chip = this.chips[x, y];

      // 部屋までたどり着いたら終了
      if (chip.Contain((uint)Flags.Room)) return;

      // 通路情報を消す
      this.chips[x, y].Off((uint)(Flags.Aisle | Flags.Confluence));

      // 上下左右を見に行く
      if (this.chips[x, y + 1].Contain((uint)Flags.Aisle)) {
        ProcDeleteUselessAnsle(x, y + 1);
      }

      if (this.chips[x, y - 1].Contain((uint)Flags.Aisle)) {
        ProcDeleteUselessAnsle(x, y - 1);
      }

      if (this.chips[x + 1, y].Contain((uint)Flags.Aisle)) {
        ProcDeleteUselessAnsle(x + 1, y);
      }

      if (this.chips[x - 1, y].Contain((uint)Flags.Aisle)) {
        ProcDeleteUselessAnsle(x - 1, y);
      }
    }


#if UNITY_EDITOR

    /// <summary>
    /// デバッグ表示
    /// </summary>
    public void OnGUI()
    {
      GUIStyle sWall = new GUIStyle();
      GUIStyle sRoom = new GUIStyle();
      GUIStyle sAisle = new GUIStyle();
      GUIStyle sReservedAisle = new GUIStyle();
      GUIStyle sConfluence = new GUIStyle();
      GUIStyle sCross = new GUIStyle();

      sWall.normal.textColor = Color.black;
      sRoom.normal.textColor = Color.blue;
      sAisle.normal.textColor = Color.white;
      sReservedAisle.normal.textColor = Color.gray;
      sConfluence.normal.textColor = Color.magenta;
      sCross.normal.textColor = Color.cyan;

      MapForChip((int x, int y, BitFlag chip) => {
        GUIStyle style = null;

        if (chip.Contain((uint)Flags.Wall)) style = sWall;
        if (chip.Contain((uint)Flags.Room)) style = sRoom;
        if (chip.Contain((uint)Flags.ReservedAisle)) style = sReservedAisle;
        if (chip.Contain((uint)Flags.Aisle)) style = sAisle;
        if (chip.Contain((uint)Flags.Confluence)) style = sConfluence;
        if (chip.Contain((uint)Flags.Cross)) style = sCross;

        if (style != null) {
          GUI.Label(new Rect(x * 10, y * 10 + 30, 10, 10), "■", style);
        }

      });
    }
#endif

#if _DEBUG
    public void DrawDebugMenu( DebugMenu.MenuWindow menuWindow )
    {
      GUIStyle sWall = new GUIStyle();
      GUIStyle sRoom = new GUIStyle();
      GUIStyle sAisle = new GUIStyle();
      GUIStyle sReservedAisle = new GUIStyle();
      GUIStyle sConfluence = new GUIStyle();
      GUIStyle sCross = new GUIStyle();

      sWall.normal.textColor = Color.black;
      sRoom.normal.textColor = Color.blue;
      sAisle.normal.textColor = Color.white;
      sReservedAisle.normal.textColor = Color.gray;
      sConfluence.normal.textColor = Color.magenta;
      sCross.normal.textColor = Color.cyan;

      using (var h = new GUILayout.HorizontalScope())
      {
        MapForChip((int x, int y, BitFlag chip) =>
        {
          GUIStyle style = null;

          if (chip.Contain((uint)Flags.Wall)) style = sWall;
          if (chip.Contain((uint)Flags.Room)) style = sRoom;
          if (chip.Contain((uint)Flags.ReservedAisle)) style = sReservedAisle;
          if (chip.Contain((uint)Flags.Aisle)) style = sAisle;
          if (chip.Contain((uint)Flags.Confluence)) style = sConfluence;
          if (chip.Contain((uint)Flags.Cross)) style = sCross;

          if (style != null)
          {
            bool isNewColumn = (y % Define.HEIGHT == 0);
            bool isEndColumn = (y != 0 && y % (Define.HEIGHT - 1) == 0);

            if (isNewColumn)
            {
              GUILayout.BeginVertical();
            }

            const float s = 7;
            GUILayout.Label("■", style, GUILayout.Width(s), GUILayout.Height(s));

            if (isEndColumn)
            {
              GUILayout.EndVertical();
            }
          }

        });
      }
    }
#endif

  }


}