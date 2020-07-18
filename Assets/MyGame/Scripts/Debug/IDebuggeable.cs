using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
  public interface IDebuggeable
  {
#if _DEBUG
    void Draw(MyDebug.Window window);
#endif
  }
}
