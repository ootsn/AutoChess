using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ∆Â≈ÃΩ≈±æ
/// </summary>
[RequireComponent(typeof(HexGridLayout))]
public class CheckBoard : MonoBehaviour
{
    private HexGridLayout m_gridLayout;
    private GameObject OpponentHexGrid => m_gridLayout.OpponentHexGrid;
    private GameObject MyHexGrid => m_gridLayout.MyHexGrid;
    private GameObject[][] HexRect => m_gridLayout.HexRect;

    public Transform MyHexParent { get { return MyHexGrid.transform; } }
    public Transform OpponentHexParent { get { return OpponentHexGrid.transform; } }

    private delegate void Func(Transform transform);

    private void Awake()
    {
        m_gridLayout = GetComponent<HexGridLayout>();
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
        ApplyFuncToHexagon(func, MyHexGrid.transform);
    }

    public void DeactivateMyHexGrid()
    {
        Func func = (Transform transform) =>
        {
            transform.GetComponent<MeshRenderer>().enabled = false;
        };
        ApplyFuncToHexagon(func, MyHexGrid.transform);
    }

    public void ActivateOpponentHexGrid()
    {
        Func func = (Transform transform) =>
        {
            transform.GetComponent<MeshRenderer>().enabled = true;
        };
        ApplyFuncToHexagon(func, OpponentHexGrid.transform);
    }

    public void DeactivateOpponentHexGrid()
    {
        Func func = (Transform transform) =>
        {
            transform.GetComponent<MeshRenderer>().enabled = false;
        };
        ApplyFuncToHexagon(func, OpponentHexGrid.transform);
    }

    public Vector3[] GetMyHexGridPositions()
    {
        Vector3[] myHexPositions;
        GetGameObjectPositions(MyHexGrid.transform, out myHexPositions);
        return myHexPositions;
    }

    public Transform[] GetMyHexGridTransform()
    {
        Transform[] myHexGridTransform;
        GetGameObjectTransform(MyHexGrid.transform, out myHexGridTransform);
        return myHexGridTransform;
    }

    public Transform[] GetOpponentHexGridTransform()
    {
        Transform[] opponentHexGridTransform;
        GetGameObjectTransform(OpponentHexGrid.transform, out opponentHexGridTransform);
        return opponentHexGridTransform;
    }

    public void GetOpponentHexGridPositions(out Vector3[] opponentHexPositions)
    {
        GetGameObjectPositions(OpponentHexGrid.transform, out opponentHexPositions);
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

    public void SetChess(Transform chess, RectPosition rectPos)
    {
        chess.SetParent(HexRect[rectPos.x][rectPos.y].transform);
        chess.localPosition = new Vector3(0f, 0f, 0f);
    }

    public RectPosition Reflect(RectPosition rectPos)
    {
        return new RectPosition(m_gridLayout.Row - rectPos.x - 1, m_gridLayout.Col - rectPos.y - 1);
    }

    private bool RectPositionValid(RectPosition rectPos)
    {
        return rectPos.x >= 0 && rectPos.y >= 0 && rectPos.x < m_gridLayout.Row && rectPos.y < m_gridLayout.Col;
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
        return Position.isPositionAvailable(end.transform) ? 1 : 1000000;
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
                Transform neighbor = HexRect[rectPosition.x][rectPosition.y].transform;
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

            foreach (var next in GetNeighbors(current, m_gridLayout))
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
