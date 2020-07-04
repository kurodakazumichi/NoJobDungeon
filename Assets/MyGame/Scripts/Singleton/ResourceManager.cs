using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Singleton
{
  public class ResourceManager : SingletonMonobehaviour<ResourceManager>
  {
    private class ResouceCache
    {
      public string Name
      {
        get
        {
          if( this.Object == null ) return string.Empty;
          return this.Object.name;
        }
      }

      public string MainAssetName
      {
        get
        {
          if (this.Object == null) return string.Empty;
          switch (this.Object)
          {
            case Sprite sprite: return sprite.texture.name;
          }
          return this.Object.name;
        }
      }

      public Type Type => Object.GetType();

      public UnityEngine.Object Object;

      public bool IsSubAsset
      {
        get
        {
          if (this.Object == null) return false;
          switch (this.Object)
          {
            case Sprite sprite: return true;
          }
          return false;
        }

      }
    }

    /// <summary>Dungeionラベル</summary>
    public const string DungeionLabel = "Dungeon";

    /// <summary>ロード中ラベル</summary>
    private string loadingLabel = string.Empty;

    /// <summary>リソースキャッシュ</summary>
    private List<ResouceCache> caches = new List<ResouceCache>();

    //// Start is called before the first frame update
    //void Start()
    //{
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //}

    /// <summary>
    /// ロード中か
    /// </summary>
    public bool IsLoading
    {
      get { return !string.IsNullOrEmpty(loadingLabel); }
    }

    /// <summary>
    /// 非同期読み込み
    /// </summary>
    public async void LoadAsync( string label )
    {
      if(IsLoading) return;

      var sw = new System.Diagnostics.Stopwatch();

      // 開始
      Debug.Log($"load start");
      sw.Start();
      caches.Clear();
      loadingLabel = label;

      // 読み込み 
      {
        var handle = Addressables.LoadAssetsAsync<UnityEngine.Object>(label, null);
        await handle.Task;
        CacheLoadResource( handle.Result );
      }

      // spriete読み込み.
      // サブアセットのため、別ロード.
      var locations = await Addressables.LoadResourceLocationsAsync(label, typeof(Sprite)).Task;
      foreach (var location in locations)
      {
        var handle = Addressables.LoadAssetAsync<IList<Sprite>>(location.PrimaryKey);
        await handle.Task;
        CacheLoadResource( handle.Result ); 
      }

      // 完了
      sw.Stop();
      Debug.Log($"load complete({sw.ElapsedMilliseconds.ToString()}ms)");
      loadingLabel = string.Empty;
    }

    /// <summary>
    /// 読み込んだリソースをキャッシュする
    /// </summary>
    private void CacheLoadResource<T>( IList<T> objects ) where T : UnityEngine.Object
    {
      foreach (var obj in objects)
      {
        Debug.Log($"load : {obj}");
        var cache = new ResouceCache() { Object = obj };
        caches.Add(cache);
      }
    }
    
    /// <summary>
    /// リソースの取得
    /// </summary>
    public T GetResource<T>(string name) where T : class
    {
      var cache = caches.Find( x => 
        x.Type.Equals(typeof(T)) 
        && (x.Name == name)
      );
      return cache?.Object as T;
    }

    /// <summary>
    /// リソースの取得
    /// </summary>
    public T[] GetResources<T>(string name) where T : class
    {
      var targets = this.caches.FindAll(x =>
        x.Type.Equals(typeof(T)) 
        && ( x.IsSubAsset ? x.MainAssetName == name : x.Name == name )
      );

      var array = new T[targets.Count];
      for (int i = 0; i < targets.Count; i++)
      {
        array[i] = targets[i].Object as T;
      }
      return array;
    }
  }
}
