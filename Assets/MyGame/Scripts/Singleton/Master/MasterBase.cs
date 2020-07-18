using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Master
{
  /// <summary>
  /// Master系のシングルトンベースクラス
  /// SingletonMonoBehaviourを継承できないので、Master用のシングルトンを別途作成
  /// </summary>
  public abstract class MasterBase<T, Entity> : MonoBehaviour
    where T : MonoBehaviour
    where Entity : class
  {

    //-------------------------------------------------------------------------
    // シングルトンの定義
    private static T instance;

    public static T Instance
    {
      get
      {
        if (instance == null)
        {
          instance = (T)FindObjectOfType(typeof(T));
          if (instance == null)
          {
            Debug.LogError(typeof(T) + "がシーンに存在しません。");
          }
        }
        return instance;
      }
    }

    public static bool HasInstance => ( instance != null );

    virtual protected void Awake()
    {
      if (this != Instance)
      {
        Destroy(this);
        return;
      }
    }

    //-------------------------------------------------------------------------
    // Masterに関する定義

    /// <summary>
    /// データ格納庫
    /// </summary>
    protected Dictionary<string, Entity> repository = new Dictionary<string, Entity>();

    /// <summary>
    /// ロード、JSONを読み込んでパースした結果を返す
    /// </summary>
    protected JsonRepository<EntityOfJson> Load<EntityOfJson> (string path)
    {
      var json = Resources.Load(path) as TextAsset;
      var repo = JsonUtility.FromJson<JsonRepository<EntityOfJson>>(json.text);
      return repo;
    }

    /// <summary>
    /// ID検索
    /// </summary>
    public Entity FindById(string id)
    {
      if (!this.repository.ContainsKey(id)) return null;

      return this.repository[id];
    }

    /// <summary>
    /// IDリスト
    /// </summary>
    public List<string> Ids()
    {
      List<string> ids = new List<string>();

      foreach (var key in this.repository.Keys)
      {
        ids.Add(key);
      }

      return ids;
    }
  }

  //---------------------------------------------------------------------------
  // Jsonを読み込む際に使用するリポジトリクラス
  // 全てのJsonファイル共通で利用する
  [System.Serializable]
  public class JsonRepository<Entity>
  {
    public List<Entity> list = null;
  }
}