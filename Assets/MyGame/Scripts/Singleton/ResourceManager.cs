using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Singleton
{
  public class ResourceManager : SingletonMonobehaviour<ResourceManager>
  {

    private class Loader
    {
      private AsyncOperationHandle<IList<UnityEngine.Object>> objectLoadHandle;
      private AsyncOperationHandle<IList<Sprite>> spriteLoadHandle;

      public bool IsLoading = false;

      /// <summary>
      /// 読み込み（非同期）
      /// </summary>
      public async Task<List<UnityEngine.Object>> Load(string label)
      {
        if (IsLoading) return null;

        IsLoading = true;
        var list = new List<UnityEngine.Object>();

        {
          var handle = Addressables.LoadAssetsAsync<UnityEngine.Object>(label, null);
          await handle.Task;
          if (handle.Status == AsyncOperationStatus.Succeeded)
          {
            this.objectLoadHandle = handle;
            list.AddRange(handle.Result);
          }
        }

        // spriete読み込み.
        // サブアセットのため、別ロード.
        if (this.objectLoadHandle.IsValid())
        {
          var locationHandle = Addressables.LoadResourceLocationsAsync(label, typeof(Sprite));
          var locations = await locationHandle.Task;
          foreach (var location in locations)
          {
            var handle = Addressables.LoadAssetAsync<IList<Sprite>>(location.PrimaryKey);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
              this.spriteLoadHandle = handle;
              list.AddRange(handle.Result);
            }
          }
          Addressables.Release(locationHandle);
        }

        IsLoading = false;
        return list;
      }

      /// <summary>
      /// 解放
      /// </summary>
      public void Unload()
      {
        if (this.spriteLoadHandle.IsValid())
        {
          Addressables.Release(this.spriteLoadHandle);
        }

        if (this.objectLoadHandle.IsValid())
        {
          Addressables.Release(this.objectLoadHandle);
        }
      }

    }

    private class Asset
    {
      public string Label { get; private set; }
      private Loader loader = new Loader();
      private List<UnityEngine.Object> cache = new List<UnityEngine.Object>();

      /// <summary>
      /// 読み込み（非同期）
      /// </summary>
      public async Task LoadAsync(string label)
      {
        this.Label = label;
        this.cache = await loader.Load(this.Label);
      }

      /// <summary>
      /// 解放
      /// </summary>
      public void Unload()
      {
        this.cache.Clear();
        loader.Unload();
        this.Label = string.Empty;
      }

      /// <summary>
      /// 指定アセットの取得
      /// </summary>
      public T Find<T>(string name) where T : UnityEngine.Object
      {
        foreach (var obj in cache)
        {
          if (obj.GetType() == typeof(T) && obj.name == name)
          {
            return obj as T;
          }
        }

        return null;
      }

      /// <summary>
      /// 指定アセットの全取得
      /// </summary>
      public List<T> FindAll<T>(string name) where T : UnityEngine.Object
      {
        var list = new List<T>();

        bool isSprite = typeof(T) == typeof(Sprite);
        foreach (var obj in cache)
        {
          if (obj.GetType() == typeof(T))
          {
            var assetName = ( isSprite ) ? ( obj as Sprite ).texture.name : obj.name;
            if (assetName == name)
            {
              list.Add(obj as T);
            }
          }
        }

        return list;
      }
    }

    /// <summary>Dungeionラベル</summary>
    public const string DungeionLabel = "Dungeon";

    /// <summary>ロード中ラベル</summary>
    private string loadingLabel = string.Empty;

    /// <summary>リソースキャッシュ</summary>
    private List<Asset> assets = new List<Asset>();

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
      Debug.Log($"load start {label}");
      sw.Start();
      loadingLabel = label;

      // 読み込み
      bool isLoaded = assets.Find( x => x.Label == label ) != null ;
      if (isLoaded == false)
      {
        var asset = new Asset();
        await asset.LoadAsync(label);
        assets.Add(asset);
      }
    
      // 完了
      sw.Stop();
      Debug.Log($"load complete {sw.ElapsedMilliseconds.ToString()}ms");
      loadingLabel = string.Empty;
    }

    /// <summary>
    /// 解放
    /// </summary>
    public void Unload(string label )
    {
      var index = assets.FindIndex(x => x.Label == label);
      if (index >= 0)
      {
        var asset = assets[index];
        asset.Unload();
        assets.RemoveAt( index );
      }
    }
    
    /// <summary>
    /// リソースの取得
    /// </summary>
    public T GetResource<T>(string name) where T : UnityEngine.Object
    {
      foreach (var asset in assets)
      {
        var resource = asset.Find<T>(name);
        if (resource != null)
        {
          return resource;
        }
      }

      return null;
    }

    /// <summary>
    /// リソースの取得
    /// </summary>
    public T[] GetResources<T>(string name) where T : UnityEngine.Object
    {
      var list = new List<T>();
      foreach (var asset in assets)
      {
        var resources = asset.FindAll<T>(name);
        list.AddRange( resources ); 
      }

      return list.ToArray();
    }
  }
}
