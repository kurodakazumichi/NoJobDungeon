/// <summary>
/// ビットフラグを扱いやすくするためのクラス
/// </summary>
public class BitFlag
{
  private uint value;

  public BitFlag()
  {
    this.Clear();
  }

  public void Clear()
  {
    this.value = 0;
  }

  public void Set(uint value)
  {
    this.value = value;
  }

  public uint Get()
  {
    return this.value;
  }

  public void On(uint flag)
  {
    this.value |= flag;
  }

  public void Off(uint flag)
  {
    this.value &= ~flag;
  }

  public bool Is(uint flag)
  {
    return this.value == flag;
  }

  public bool Contain(uint flag)
  {
    return (this.value & flag) == flag;
  }

  public bool ContainEither(uint flag)
  {
    return (this.value & flag) != 0;
  }

  public override string ToString()
  {
    return this.value.ToString();
  }

}