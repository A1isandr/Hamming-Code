namespace Task5.Common;

public static class Int32Extensions
{
    public static bool IsPowerOfTwo(this int x) => 
        x != 0 && (x & (x - 1)) == 0;
}