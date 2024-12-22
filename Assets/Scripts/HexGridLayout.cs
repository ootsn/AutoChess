using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UIElements;

public class HexGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int gridSize;

    [Header("Tile Settings")]
    public float innerSize = 0f;
    public float outerSize = 1f;
    public float height = 1f;
    public bool isFlatTopped;
    public bool WhetherYAsRow;
    public bool xReverse;
    public bool yReverse;
    public Material material;

    private GameObject opponentHexGrid;
    private GameObject myHexGrid;
    private GameObject[][] hexRect;
    private int row;
    private int col;

    private delegate void Func(Transform transform);

    private void Awake()
    {
        Init();
        LayoutGrid();
    }

    private void OnValidate()
    {
        if (Application.isPlaying && opponentHexGrid != null && myHexGrid != null)
        {
            for (int i = 0; i < opponentHexGrid.transform.childCount; i++)
            {
                Destroy(opponentHexGrid.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < myHexGrid.transform.childCount; i++)
            {
                Destroy(myHexGrid.transform.GetChild(i).gameObject);
            }
            LayoutGrid();
        }
    }

    private void ApplyFuncToHexagon(Func func, Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            func(parent.GetChild(i));
        }
    }

    public void ActivateMyHexGrid()
    {
        Func func = (Transform transform) =>
        {
            transform.GetComponent<MeshRenderer>().enabled = true;
        };
        ApplyFuncToHexagon(func, myHexGrid.transform);
    }

    public void DeactivateMyHexGrid()
    {
        Func func = (Transform transform) =>
        {
            transform.GetComponent<MeshRenderer>().enabled = false;
        };
        ApplyFuncToHexagon(func, myHexGrid.transform);
    }

    public void ActivateOpponentHexGrid()
    {
        Func func = (Transform transform) =>
        {
            transform.GetComponent<MeshRenderer>().enabled = true;
        };
        ApplyFuncToHexagon(func, opponentHexGrid.transform);
    }

    public void DeactivateOpponentHexGrid()
    {
        Func func = (Transform transform) =>
        {
            transform.GetComponent<MeshRenderer>().enabled = false;
        };
        ApplyFuncToHexagon(func, opponentHexGrid.transform);
    }

    public Vector3[] GetMyHexGridPositions()
    {
        Vector3[] myHexPositions;
        GetGameObjectPositions(myHexGrid.transform, out myHexPositions);
        return myHexPositions;
    }

    public Transform[] GetMyHexGridTransform()
    {
        Transform[] myHexGridTransform;
        GetGameObjectTransform(myHexGrid.transform, out myHexGridTransform);
        return myHexGridTransform;
    }

    public Transform[] GetOpponentHexGridTransform()
    {
        Transform[] opponentHexGridTransform;
        GetGameObjectTransform(opponentHexGrid.transform, out opponentHexGridTransform);
        return opponentHexGridTransform;
    }

    public void GetOpponentHexGridPositions(out Vector3[] opponentHexPositions)
    {
        GetGameObjectPositions(opponentHexGrid.transform, out opponentHexPositions);
    }

    private void GetGameObjectPositions(Transform parent, out Vector3[] positions)
    {
        int len = parent.childCount;
        positions = new Vector3[len];
        for (int i = 0; i < len; i++)
        {
            positions[i] = parent.GetChild(i).transform.position;
        }
    }

    private void GetGameObjectTransform(Transform parent, out Transform[] transform)
    {
        int len = parent.childCount;
        transform = new Transform[len];
        for (int i = 0; i < len; i++)
        {
            transform[i] = parent.GetChild(i).transform;
        }
    }

    private void Init()
    {
        opponentHexGrid = new GameObject("OpponentHexGrid");
        opponentHexGrid.transform.SetParent(transform, false);
        myHexGrid = new GameObject("MyHexGrid");
        myHexGrid.transform.SetParent(transform, false);

        if (WhetherYAsRow)
        {
            row = gridSize.y;
            col = gridSize.x;
        }
        else
        {
            row = gridSize.x;
            col = gridSize.y;
        }

        hexRect = new GameObject[row][];
        for (int i = 0; i < row; i++)
        {
            hexRect[i] = new GameObject[col];
        }
    }

    private void LayoutGrid()
    {
        //for (int y = 0; y < gridSize.y; y++)
        //{
        //    for (int x = 0; x < gridSize.x; x++)
        //    {
        //        GameObject tile = new GameObject($"Hex {x},{y}", typeof(HexRender));

        //        tile.transform.SetParent(transform);
        //        tile.transform.localPosition = GetPositionForHexFromCoordinate(new Vector2Int(x, y));

        //        HexRender hexRender = tile.GetComponent<HexRender>();
        //        hexRender.isFlatTopped = isFlatTopped;
        //        hexRender.outerSize = outerSize;
        //        hexRender.innerSize = innerSize;
        //        hexRender.height = height;
        //        hexRender.SetMaterial(material);
        //        hexRender.DrawMesh();
        //    }
        //}

        for (int y = 0; y < col; y++)
        {
            int x = 0;
            for (; x < row; x++)
            {
                RectPosition rectPos = new RectPosition(x, y);
                HexPosition hexPos = Position.Rect2Hex(rectPos);
                Transform parent = (x < row / 2 ? opponentHexGrid.transform : myHexGrid.transform);
                hexRect[x][y] = NewHexTile(rectPos, hexPos, parent);
            }
        }
    }

    public static string GetHexName(RectPosition rectPosition)
    {
        return $"Hex {rectPosition.x},{rectPosition.y}";
    }

    public static string GetHexName(HexPosition hexPosition)
    {
        return GetHexName(Position.Hex2Rect(hexPosition));
    }

    private GameObject NewHexTile(RectPosition rectPos, HexPosition hexPos, Transform parent)
    {
        GameObject tile = new GameObject(GetHexName(rectPos), new Type[] { typeof(HexRender), typeof(Position) });

        tile.transform.SetParent(parent);
        tile.transform.localPosition = GetPositionForHexFromCoordinate(WhetherYAsRow ? new Vector2Int(rectPos.y, rectPos.x) : new Vector2Int(rectPos.x, rectPos.y));
        tile.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

        HexRender hexRender = tile.GetComponent<HexRender>();
        hexRender.isFlatTopped = isFlatTopped;
        hexRender.outerSize = outerSize;
        hexRender.innerSize = innerSize;
        hexRender.height = height;
        hexRender.SetMaterial(material);
        hexRender.DrawMesh();

        tile.GetComponent<Position>().SetPosition(hexPos);

        return tile;
    }

    private Vector3 GetPositionForHexFromCoordinate(Vector2Int coordinate)
    {
        int column = coordinate.x;
        int row = coordinate.y;
        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = outerSize;

        if (!isFlatTopped)
        {
            shouldOffset = (row % 2) == 0;
            width = Mathf.Sqrt(3) * size;
            height = 2f * size;

            horizontalDistance = width;
            verticalDistance = height * 3f / 4f;

            offset = (xReverse ? shouldOffset : !shouldOffset) ? width / 2 : 0;

            xPosition = column * horizontalDistance + offset;
            yPosition = row * verticalDistance;
        }
        else
        {
            shouldOffset = (column % 2) == 0;
            width = 2f * size;
            height = Mathf.Sqrt(3) * size;

            horizontalDistance = width * 3f / 4f;
            verticalDistance = height;

            offset = (yReverse ? shouldOffset : !shouldOffset) ? height / 2 : 0;

            xPosition = column * horizontalDistance;
            yPosition = row * verticalDistance - offset;
        }

        return new Vector3(xPosition, 0, -yPosition);
    }

    //public void Reflect(int x, int y, out int newX, out int newY)
    //{
    //    newX = row - x - 1;
    //    newY = col - y - 1;
    //}

    public RectPosition Reflect(RectPosition rectPos)
    {
        return new RectPosition(row - rectPos.x - 1, col - rectPos.y - 1);
    }

    //public void SetChess(Transform chess, int x, int y)
    //{
    //    chess.SetParent(hexRect[x][y].transform);
    //    chess.localPosition = new Vector3(0f, 0f, 0f);
    //}

    public void SetChess(Transform chess, RectPosition rectPos)
    {
        chess.SetParent(hexRect[rectPos.x][rectPos.y].transform);
        chess.localPosition = new Vector3(0f, 0f, 0f);
    }

    private bool RectIndexValid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < row && y < col;
    }

    private bool RectPositionValid(RectPosition rectPos)
    {
        return rectPos.x >= 0 && rectPos.y >= 0 && rectPos.x < row && rectPos.y < col;
    }

    public static bool isHexPositionAvailable(Transform placeTransform)
    {
        return placeTransform.childCount == 0;
    }

    delegate bool WhetherMeetCondition(Transform transform);

    private class AStarHexPosition
    {
        public Position HexPosition;
        public int Cost;
    }

    private class AStarHexPositionComparer : IComparer<AStarHexPosition>
    {
        public int Compare(AStarHexPosition x, AStarHexPosition y)
        {
            return x.Cost.CompareTo(y.Cost);
        }
    }

    private int GetCost(Position end)
    {
        return isHexPositionAvailable(end.transform) ? 1 : 1000000;
    }

    private IEnumerable<Position> GetNeighbors(Position position, HexGridLayout parent)
    {
        for (int i = 0; i < HexPosition.Direction.Length; i++)
        {
            //result[i] = new HexPosition(position.hex.q + HexPosition.Direction[i].q, position.hex.r + HexPosition.Direction[i].r);
            HexPosition hexPosition = new HexPosition(position.hex.q + HexPosition.Direction[i].q, position.hex.r + HexPosition.Direction[i].r);
            RectPosition rectPosition = Position.Hex2Rect(hexPosition);
            if (RectPositionValid(rectPosition))
            {
                Transform neighbor = hexRect[rectPosition.x][rectPosition.y].transform;
                yield return neighbor.GetComponent<Position>();
            }
        }
    }

    public int AStar(Position start, Position end, out Dictionary<Position, Position> came_from)
    {
        const int Capacity = 20;
        var frontier = new PriorityQueue<AStarHexPosition>(Capacity, new AStarHexPositionComparer());
        frontier.Push(new AStarHexPosition { HexPosition = start, Cost = 0 });
        came_from = new Dictionary<Position, Position>();
        var cost_so_far = new Dictionary<Position, int>();
        came_from[start] = null;
        cost_so_far[start] = 0;

        while (frontier.Count > 0)
        {
            var current = frontier.Top.HexPosition;
            frontier.Pop();

            if (current == end)
            {
                break;
            }

            foreach (var next in GetNeighbors(current, this))
            {
                var new_cost = cost_so_far[current] + GetCost(next);
                if (!cost_so_far.TryGetValue(next, out int next_cost) || new_cost < next_cost)
                {
                    cost_so_far[next] = new_cost;
                    var priority = new_cost + Position.Distance(end, current);
                    frontier.Push(new AStarHexPosition { HexPosition = next, Cost = priority });
                    came_from[next] = current;
                }
            }
        }

        return cost_so_far[end];
    }
}
