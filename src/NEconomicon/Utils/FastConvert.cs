using System.Runtime.InteropServices;

namespace NEconomicon.Utils;

/// <summary>
/// Provides set of methods to fast bitwise convert between long and other types.
/// </summary>
public static class FastConvert
{
    [StructLayout(LayoutKind.Explicit)]
    private struct Helper
    {
        [FieldOffset(0)]
        public long Int64;

        [FieldOffset(0)]
        public double Float64;

        [FieldOffset(4)]
        public int Int32;

        [FieldOffset(4)]
        public float Float32;
    }

    public static long D2L(double value) => new Helper { Float64 = value }.Int64;
    public static double L2D(long value) => new Helper { Int64 = value }.Float64;
    public static long I2L(int value) => new Helper { Int32 = value }.Int64;
    public static int L2I(long value) => new Helper { Int64 = value }.Int32;
    public static long F2L(float value) => new Helper { Float32 = value }.Int64;
    public static float L2F(long value) => new Helper { Int64 = value }.Float32;
}
