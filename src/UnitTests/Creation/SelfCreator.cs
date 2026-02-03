

namespace WrathTools.UnitTests
{
  public sealed class SelfCreator
  {

    internal SelfCreator()
    {

    }

    [Creator]
    public static SelfCreator Create()
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1) 
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2) 
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4, int a5)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4, int a5, int a6)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4, int a5, int a6, int a7)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12, int a13)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12, int a13, int a14)
      => new SelfCreator();

    [Creator]
    public static SelfCreator Create(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12, int a13, int a14, int a15)
  => new SelfCreator();

  }
}