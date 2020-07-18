using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MyGame.MyDebug
{
  public class DisabledScope : IDisposable
  {
    public DisabledScope( bool isDisable )
    {
      GUI.enabled = !isDisable;
    }

    void IDisposable.Dispose()
    {
      GUI.enabled = true;
    }
  }
}
