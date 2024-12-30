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

    private GameObject CreateChess(GameObject chess, Transform place)
    {
        GameObject chess_ = Instantiate(chess) as GameObject;
        Position.SetChess(chess_.transform, place);
        chess_.transform.localPosition = new Vector3(0f, 0f, 0f);
        chess_.GetComponent<ChessMove>().SetController(this);
        chess_.GetComponent<ChessMove>().SetShop(this.GetComponent<ChessShop>());
        chess_.GetComponent<ChessBase>().CopyProperties(chess.GetComponent<ChessBase>());
        return chess_;
    }

    public bool NewChess(GameObject chess_)
    {
        for (int i = 0; i < ReserveSeat.Length; i++)
        {
            if (/*reserveSeatAvailable[i]*/ Position.isPositionAvailable(ReserveSeat[i]))
            {
                GameObject chess = CreateChess(chess_, ReserveSeat[i]);
                //GameObject chess = Instantiate(chess_) as GameObject;
                //chess.transform.SetParent(ReserveSeat[i]);
                //chess.transform.localPosition = new Vector3(0f, 0f, 0f);
                //chess.GetComponent<ChessMove>().SetController(this);
                //chess.GetComponent<ChessMove>().SetShop(this.GetComponent<ChessShop>());

                //chess.GetComponent<ChessMove>().SetPosIndex(i);
                //reserveSeatAvailable[i] = false;

                UpgradeChess(chess.name);

                return true;
            }
        }
        return false;
    }

    private IEnumerable<Transform> TraversalMyPlace(string name)
    {
        for (int i = ReserveSeat.Length - 1; i >= 0; i--)
        {
            if (!Position.isPositionAvailable(ReserveSeat[i]) && Position.GetChessName(ReserveSeat[i]) == name) //有棋子
            {
                yield return ReserveSeat[i];
            }
        }
        for (int i = 0; i < myHexagons.Length; i++)
        {
            if (!Position.isPositionAvailable(myHexagons[i]) && Position.GetChessName(myHexagons[i]) == name) //有棋子
            {
                yield return myHexagons[i];
            }
        }
    }

    private void UpgradeChess(string name)
    {
        Queue<Transform>[] specifiedChess = new Queue<Transform>[ChessProperty.MAX_LEVEL - 1];
        for (int i = 0; i < specifiedChess.Length; i++)
        {
            specifiedChess[i] = new Queue<Transform>();
        }

        foreach (Transform place in TraversalMyPlace(name))
        {
            Transform chess = Position.GetChess(place);
            int levelMinusOne = chess.GetComponent<ChessBase>().level - 1;
            while (levelMinusOne < specifiedChess.Length)
            {
                specifiedChess[levelMinusOne].Enqueue(chess);
                if (specifiedChess[levelMinusOne].Count == ChessProperty.NUM_OF_UPGRADE)
                {
                    Destroy(specifiedChess[levelMinusOne].Dequeue().gameObject);
                    Destroy(specifiedChess[levelMinusOne].Dequeue().gameObject);
                    specifiedChess[levelMinusOne].Dequeue();
                    chess.GetComponent<ChessBase>().Upgrade();
                }
                else
                {
                    break;
                }

                levelMinusOne++;
            }
        }
    }

    public void PutOpponent(Transform[] opponentHexagons)
    {
        foreach (var place in opponentHexagons)
        {
            if (!Position.isPositionAvailable(place.transform))
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
        Dictionary<Position, Position> tempRoute = null;
        int minDist = int.MaxValue;
        foreach (var pair in opponentHexGridInfo)
        {
            Position opponentPlaceHexPos = pair.Key.parent.GetComponent<Position>();
            int dist = hexGrid.AStar(myChessPlacePos, opponentPlaceHexPos, out tempRoute);
            if (dist < minDist)
            {
                opponentPlace = opponentPlaceHexPos;
                minDist = dist;
                route = tempRoute;
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
            if (!Position.isPositionAvailable(place))
            {
                Position myPlaceHexPos = place.GetComponent<Position>();
                int dist = hexGrid.AStar(opponentChessPlacePos, myPlaceHexPos, out route);
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
