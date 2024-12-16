using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPosition : MonoBehaviour
{
    public int q { get; private set; }
    public int r { get; private set; }

    public HexPosition()
    {
        this.q = 0;
        this.r = 0;
    }

    public HexPosition(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public void SetPosition(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public static void Rect2Hex(int x, int y, out int q, out int r)
    {
        q = y - x / 2;
        r = x;
    }

    public static void Hex2Rect(int q, int r, out int x, out int y)
    {
        x = r;
        y = q + r / 2;
    }
}
