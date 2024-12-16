using System;
using System.Collections;
using System.Collections.Generic;
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

        int q, r;
        for (int y = 0; y < col; y++)
        {
            int x = 0;
            for (; x < row; x++)
            {
                Transform parent = (x < row / 2 ? opponentHexGrid.transform : myHexGrid.transform);
                HexPosition.Rect2Hex(x, y, out q, out r);
                hexRect[x][y] = NewHexTile(x, y, q, r, parent);
            }
        }
    }

    private GameObject NewHexTile(int x, int y, int q, int r, Transform parent)
    {
        GameObject tile = new GameObject($"Hex {x},{y}", new Type[] { typeof(HexRender), typeof(HexPosition) });

        tile.transform.SetParent(parent);
        tile.transform.localPosition = GetPositionForHexFromCoordinate(WhetherYAsRow ? new Vector2Int(y, x) : new Vector2Int(x, y));
        tile.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

        HexRender hexRender = tile.GetComponent<HexRender>();
        hexRender.isFlatTopped = isFlatTopped;
        hexRender.outerSize = outerSize;
        hexRender.innerSize = innerSize;
        hexRender.height = height;
        hexRender.SetMaterial(material);
        hexRender.DrawMesh();

        tile.GetComponent<HexPosition>().SetPosition(q, r);

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

    public void Reflect(int x, int y, out int newX, out int newY)
    {
        newX = row - x - 1;
        newY = col - y - 1;
    }

    public void SetChess(Transform chess, int x, int y)
    {
        chess.SetParent(hexRect[x][y].transform);
        chess.localPosition = new Vector3(0f, 0f, 0f);
    }
}
