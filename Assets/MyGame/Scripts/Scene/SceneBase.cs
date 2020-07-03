using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scene {

  public class SceneBase : MyMonoBehaviour 
  {
    /// <summary>
    /// 更新処理、戻り値でtrueを返す限り呼ばれ続ける。
    /// falseを返すと終了しfinalizeFuncの呼び出しに移る。
    /// </summary>
    private System.Func<bool> updateFunc = null;
    private System.Func<bool> reservedUpdateFunc = null;

    /// <summary>
    /// 後処理
    /// </summary>
    private System.Action finalizeFunc = null;
    private System.Action reservedFinalizeFunc = null;

    /// <summary>
    /// 更新処理と後処理を登録する
    /// </summary>
    /// <param name="update"></param>
    /// <param name="finalize"></param>
    protected void SetFunc(System.Func<bool> update, System.Action finalize = null)
    {
      this.reservedUpdateFunc = update;
      this.reservedFinalizeFunc = finalize;
    }

    /// <summary>
    /// アイドル状態にする
    /// </summary>
    protected bool SetIdle()
    {
      SetFunc(null, null);
      return false;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
      // 更新処理
      if (this.updateFunc != null) {
        if (this.updateFunc()) return;
      }

      // 終了処理
      if (this.finalizeFunc != null) {
        this.finalizeFunc();
      }

      // 予約されている関数を登録
      this.updateFunc   = this.reservedUpdateFunc;
      this.finalizeFunc = this.reservedFinalizeFunc;
      this.reservedFinalizeFunc = null;
      this.reservedUpdateFunc   = null;
    }
  }

}