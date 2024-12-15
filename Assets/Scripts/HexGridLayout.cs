using System;
using System.Collections;
using System.Collections.Generic;
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
    public bool xReverse;
    public bool yReverse;
    public Material material;

    private GameObject opponentHexGrid;
    private GameObject myHexGrid;
    private List<GameObject> opponentHexes;
    private List<GameObject> myHexes; 

    private void Awake()
    {
        Init();
        LayoutGrid();
    }

    private void OnValidate()
    {
        if (Application.isPlaying && opponentHexes != null && myHexes != null)
        {
            foreach (GameObject obj in opponentHexes)
            {
                Destroy(obj);
            }
            opponentHexes.Clear();
            foreach (GameObject obj in myHexes)
            { 
                Destroy(obj);
            }
            myHexes.Clear();
            LayoutGrid();
        }
    }

    public void ActivateMyHexGrid()
    {
        myHexGrid.SetActive(true);
    }

    public void DeactivateMyHexGrid()
    {
        myHexGrid.SetActive(false);
    }

    public Vector3[] GetMyHexGridPositions()
    {
        Vector3[] myHexPositions;
        GetGameObjectPositions(myHexes, out myHexPositions);
        return myHexPositions;
    }

    public void GetOpponentHexGridPositions(out Vector3[] opponentHexPositions)
    {
        GetGameObjectPositions(opponentHexes, out opponentHexPositions);
    }

    private void GetGameObjectPositions(List<GameObject> objects, out Vector3[] positions)
    {
        int len = objects.Count;
        positions = new Vector3[len];
        for (int i = 0; i < len;i++)
        {
            positions[i] = objects[i].transform.position;
        }
    }

    private void Init()
    {
        opponentHexGrid = new GameObject("OpponentHexGrid");
        opponentHexGrid.transform.SetParent(transform, false);
        opponentHexGrid.SetActive(false);
        myHexGrid = new GameObject("MyHexGrid");
        myHexGrid.transform.SetParent(transform, false);
        myHexGrid.SetActive(false);

        opponentHexes = new List<GameObject>();
        myHexes = new List<GameObject>();
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

        for (int x = 0; x < gridSize.x; x++)
        {
            int y = 0;
            for (; y < gridSize.y / 2; y++)
            {
                opponentHexes.Add(NewHexTile(x, y, opponentHexGrid.transform));
            }
            for (; y < gridSize.y; y++)
            {
                myHexes.Add(NewHexTile(x, y, myHexGrid.transform));
            }
        }
    }

    private GameObject NewHexTile(int x, int y, Transform parent)
    {
        GameObject tile = new GameObject($"Hex {y},{x}", new Type[] { typeof(HexRender), typeof(HexPosition) });

        tile.transform.SetParent(parent);
        tile.transform.localPosition = GetPositionForHexFromCoordinate(new Vector2Int(x, y));
        tile.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

        HexRender hexRender = tile.GetComponent<HexRender>();
        hexRender.isFlatTopped = isFlatTopped;
        hexRender.outerSize = outerSize;
        hexRender.innerSize = innerSize;
        hexRender.height = height;
        hexRender.SetMaterial(material);
        hexRender.DrawMesh();

        int q, r;
        HexPosition.Rect2Hex(y, x, out q, out r);
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
}
