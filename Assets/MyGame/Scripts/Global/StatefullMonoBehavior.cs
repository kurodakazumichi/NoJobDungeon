using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefullMonoBehavior : MyMonoBehaviour
{
  /// <summary>
  /// 最初にupdateFuncが呼ばれる前に一度だけ呼ばれる
  /// </summary>
  private System.Action startFunc = null;
  private System.Action reservedStartFunc = null;

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
  protected void SetFunc(System.Action start, System.Func<bool> update, System.Action finalize = null)
  {
    this.reservedStartFunc    = start;
    this.reservedUpdateFunc   = update;
    this.reservedFinalizeFunc = finalize;
  }

  /// <summary>
  /// アイドル状態にする
  /// </summary>
  protected bool SetIdle()
  {
    SetFunc(null, null, null);
    return false;
  }

  /// <summary>
  /// 更新処理
  /// </summary>
  void Update()
  {
    this.BeforeUpdate();
    this.MainUpdate();
    this.AfterUpdate();
  }

  /// <summary>
  /// メインの更新処理
  /// </summary>
  private void MainUpdate()
  {
    // 開始処理
    if (this.startFunc != null) {
      this.startFunc();
      this.startFunc = null;
    }

    // 更新処理
    if (this.updateFunc != null) {
      if (this.updateFunc()) return;
    }

    // 終了処理
    if (this.finalizeFunc != null) {
      this.finalizeFunc();
    }

    // 予約されている関数を登録
    this.startFunc    = this.reservedStartFunc;
    this.updateFunc   = this.reservedUpdateFunc;
    this.finalizeFunc = this.reservedFinalizeFunc;
    this.reservedStartFunc    = null;
    this.reservedFinalizeFunc = null;
    this.reservedUpdateFunc   = null;
  }

  /// <summary>
  /// MainUpdateの前に常に呼ばれる
  /// </summary>
  protected virtual void BeforeUpdate()
  {

  }

  /// <summary>
  /// MainUpdateの後に常に呼ばれる
  /// </summary>
  protected virtual void AfterUpdate()
  {

  }
}
