using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public struct ChessPosOnRect
{
    public GameObject chess;
    public Vector2Int hexPos;
}

public class ChessControl : MonoBehaviour
{
    public HexGridLayout hexGrid;
    public GameObject reserveSeatParent;
    public GameObject checkerboard;

    //public bool[] hexGridAvailable { get; private set; }
    //public bool[] reserveSeatAvailable { get; private set; }
    public Transform[] Hexagons { get; private set; }
    public Transform[] ReserveSeat { get; private set; }

    private Dictionary<Transform, Transform> opponentHexGridInfo;

    // Start is called before the first frame update
    void Start()
    {
        opponentHexGridInfo = new Dictionary<Transform, Transform>();

        GetPlaceCoordinate();

        hexGrid.DeactivateMyHexGrid();
        hexGrid.DeactivateOpponentHexGrid();
    }

    private void GetPlaceCoordinate()
    {
        Hexagons = hexGrid.GetMyHexGridTransform();
        //hexGridAvailable = new bool[hexGridCoordinate.Length];
        //for (int i = 0; i < hexGridAvailable.Length; i++)
        //{
        //    //hexGridCoordinate.Add(hexGrid.transform.GetChild(i).position);
        //    hexGridAvailable[i] = true;
        //}

        ReserveSeat = new Transform[reserveSeatParent.transform.childCount];
        //reserveSeatAvailable = new bool[reserveSeatParent.transform.childCount];
        for (int i = 0; i < reserveSeatParent.transform.childCount; i++)
        {
            ReserveSeat[i] = reserveSeatParent.transform.GetChild(i);
            //reserveSeatAvailable[i] = true;
        }
    }

    public bool isPlaceAvailable(Transform placeTransform)
    {
        return placeTransform.childCount == 0;
    }

    public bool NewChess(GameObject chess_)
    {
        for (int i = 0; i < ReserveSeat.Length; i++)
        {
            if (/*reserveSeatAvailable[i]*/ isPlaceAvailable(ReserveSeat[i]))
            {
                GameObject chess = Instantiate(chess_) as GameObject;
                chess.transform.SetParent(ReserveSeat[i]);
                chess.transform.localPosition = new Vector3(0f, 0f, 0f);
                chess.GetComponent<ChessMove>().SetController(this);
                chess.GetComponent<ChessMove>().SetShop(this.GetComponent<ChessShop>());
                //chess.GetComponent<ChessMove>().SetPosIndex(i);
                //reserveSeatAvailable[i] = false;
                return true;
            }
        }
        return false;
    }

    public void PutOpponent(Transform[] opponentHexagons)
    {
        int x, y, newX, newY;
        foreach (var place in opponentHexagons)
        {
            if (!isPlaceAvailable(place.transform))
            {
                Transform chess = place.GetChild(0);
                opponentHexGridInfo.Add(chess, place);
                HexPosition hexPos = place.GetComponent<HexPosition>();
                HexPosition.Hex2Rect(hexPos.q, hexPos.r, out x, out y);
                hexGrid.Reflect(x, y, out newX, out newY);
                print(string.Format("({0}, {1}) => ({2}, {3})", x, y, newX, newY));
                hexGrid.SetChess(chess, newX, newY);
            }
        }
    }

    public void RemoveOpponent()
    {
        foreach (var pair in opponentHexGridInfo)
        {
            pair.Key.SetParent(pair.Value);
            pair.Key.localPosition = new Vector3(0f, 0f, 0f);
        }
        opponentHexGridInfo.Clear();
    }
}
