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
    public Transform[] myHexagons { get; private set; }
    public Transform[] ReserveSeat { get; private set; }

    private Dictionary<Transform, Transform> opponentHexGridInfo; //第一个是棋子，第二个是位置

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
        myHexagons = hexGrid.GetMyHexGridTransform();
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

    public bool NewChess(GameObject chess_)
    {
        for (int i = 0; i < ReserveSeat.Length; i++)
        {
            if (/*reserveSeatAvailable[i]*/ HexGridLayout.isHexPositionAvailable(ReserveSeat[i]))
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
        foreach (var place in opponentHexagons)
        {
            if (!HexGridLayout.isHexPositionAvailable(place.transform))
            {
                Transform chess = place.GetChild(0);
                opponentHexGridInfo.Add(chess, place);
                Position pos = place.GetComponent<Position>();
                RectPosition rectPos = hexGrid.Reflect(pos.rect);
                hexGrid.SetChess(chess, rectPos);
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

    public (Position opponentPlace, Dictionary<Position, Position> route) FindNearestOpponentPlace(Position myChessPlacePos)
    {
        Position opponentPlace = null;
        Dictionary<Position, Position> route = null;
        int minDist = int.MaxValue;
        foreach (var pair in opponentHexGridInfo)
        {
            Position opponentPlaceHexPos = pair.Value.GetComponent<Position>();
            int dist = hexGrid.AStart(opponentPlaceHexPos, myChessPlacePos, out route).Count;
            if (dist < minDist)
            {
                opponentPlace = opponentPlaceHexPos;
                minDist = dist;
            }
        }
        return (opponentPlace, route);
    }

    public (Position myPlace, Dictionary<Position, Position> route) FindNearestMyPlace(Position opponentChessPlacePos)
    {
        Position myPlace = null;
        Dictionary<Position, Position> route = null;
        int minDist = int.MaxValue;
        foreach (var place in myHexagons)
        {
            if (!HexGridLayout.isHexPositionAvailable(place))
            {
                Position myPlaceHexPos = place.GetComponent<Position>();
                int dist = hexGrid.AStart(myPlaceHexPos, opponentChessPlacePos, out route).Count;
                if (dist < minDist)
                {
                    myPlace = myPlaceHexPos;
                    minDist = dist;
                }
            }
        }
        return (myPlace, route);
    }


}
