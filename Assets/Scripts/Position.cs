using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    public HexPosition hex { get; private set; }
    public RectPosition rect { get; private set; }

    public Position()
    {
        hex = new HexPosition();
        rect = new RectPosition();
    }

    public Position(HexPosition hexPosition)
    {
        hex = hexPosition;
        rect = Hex2Rect(hex);
    }

    public Position(RectPosition rectPosition)
    {
        rect = rectPosition;
        hex = Rect2Hex(rect);
    }

    public void SetPosition(HexPosition hexPosition)
    {
        hex = hexPosition;
        rect = Hex2Rect(hex);
    }

    public void SetPosition(RectPosition rectPosition)
    {
        rect = rectPosition;
        hex = Rect2Hex(rect);
    }

    public static IEnumerable<Position> GetNeighbors(Position position, HexGridLayout parent)
    {
        for (int i = 0; i < HexPosition.Direction.Length; i++)
        {
            //result[i] = new HexPosition(position.hex.q + HexPosition.Direction[i].q, position.hex.r + HexPosition.Direction[i].r);
            HexPosition hexPosition = new HexPosition(position.hex.q + HexPosition.Direction[i].q, position.hex.r + HexPosition.Direction[i].r);
            Transform neighbor = parent.transform.Find(HexGridLayout.GetHexName(hexPosition));
            if (neighbor != null)
            {
                yield return neighbor.GetComponent<Position>();
            }
        }
    }

    private static HexPosition Subtract(HexPosition a, HexPosition b)
    {
        return new HexPosition(a.q - b.q, a.r - b.r);
    }

    public static int Distance(Position a, Position b)
    {
        HexPosition vec = Subtract(a.hex, b.hex);
        return (Math.Abs(vec.q) + Math.Abs(vec.q + vec.r) + Math.Abs(vec.r)) / 2;
    }


    public static HexPosition Rect2Hex(RectPosition rectPos)
    {
        return new HexPosition(rectPos.y - rectPos.x / 2, rectPos.x);
        //q = y - x / 2;
        //r = x;
    }

    public static RectPosition Hex2Rect(HexPosition hexPos)
    {
        return new RectPosition(hexPos.r, hexPos.q + hexPos.r / 2);
        //x = r;
        //y = q + r / 2;
    }
}

public class HexPosition
{
    public int q { get; private set; }
    public int r { get; private set; }

    public static readonly HexPosition[] Direction = new HexPosition[6]{new HexPosition(1, 0), new HexPosition(1, -1), new HexPosition(0, -1),
                                                                        new HexPosition(-1, 0), new HexPosition(-1, 1), new HexPosition(0, 1) };

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

    public void SetPosition(HexPosition hexPos)
    {
        this.q = hexPos.q;
        this.r = hexPos.r;
    }
}

public class RectPosition
{
    public int x { get; private set; }
    public int y { get; private set; }

    public RectPosition()
    {
        this.x = 0;
        this.y = 0;
    }

    public RectPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetPosition(RectPosition rectPos)
    {
        this.x = rectPos.x;
        this.y = rectPos.y;
    }
}
