using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class SpatialCode
{
    private const long XMUL = 10000000;

    public static (int x, int y, int z) deg2tile(double lon, double lat, int zoom)
    {
        int min = 1, max = 20;
        int z = Math.Max(min, zoom);
        z = Math.Min(max, z);

        return (
            (int)(Math.Floor(((lon + 180.0) / 360.0) * Math.Pow(2.0,z))),
            (int)(Math.Floor(((1.0 - Math.Log(Math.Tan((lat * Math.PI) / 180.0) + 1.0 / Math.Cos((lat * Math.PI) / 180.0)) / Math.PI) / 2.0) * Math.Pow(2.0, z))),
            z
            );
    }

    public static (double lon, double lat) tile2deg(int x,int y,int z)
    {
        double n = Math.PI - 2.0 * Math.PI * y / Math.Pow(2.0, z);

        return (
            (x / Math.Pow(2.0, z) * 360.0 - 180.0),
            (180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n))))
            );
    }
    public static long toSpatialCode(int x, int y)
    {
        return (long)x * XMUL + (long)y;
    }

    public static (int x, int y) fromSpatialCode(long spatialCode)
    {
        return ((int)(spatialCode / XMUL), (int)(spatialCode % XMUL));
    }
}
