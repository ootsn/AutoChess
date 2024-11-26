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

    private void OnEnable()
    {
        LayoutGrid();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            LayoutGrid();
        }
    }

    private void LayoutGrid()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                GameObject tile = new GameObject($"Hex {x},{y}", typeof(HexRender));

                tile.transform.SetParent(transform);
                tile.transform.localPosition = GetPositionForHexFromCoordinate(new Vector2Int(x, y));

                HexRender hexRender = tile.GetComponent<HexRender>();
                hexRender.isFlatTopped = isFlatTopped;
                hexRender.outerSize = outerSize;
                hexRender.innerSize = innerSize;
                hexRender.height = height;
                hexRender.SetMaterial(material);
                hexRender.DrawMesh();
            }
        }
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
