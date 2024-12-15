using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public struct ChessPosOnRect
{
    public GameObject chess;
    public Vector2Int hexPos;
}

public class ChessControl : MonoBehaviour
{
    public HexGridLayout hexGrid;
    public GameObject reserveSeat;
    public GameObject checkerboard;

    public bool[] hexGridAvailable { get; private set; }
    public bool[] reserveSeatAvailable { get; private set; }
    public Vector3[] hexGridCoordinate { get; private set; }
    public Vector3[] reserveSeatCoordinate { get; private set; }

    

    // Start is called before the first frame update
    void Start()
    {
        GetPlaceCoordinate();

        //hexGrid.SetActive(false);
    }

    private void GetPlaceCoordinate()
    {
        hexGridCoordinate = hexGrid.GetMyHexGridPositions();
        hexGridAvailable = new bool[hexGridCoordinate.Length];
        for (int i = 0; i < hexGridAvailable.Length; i++)
        {
            //hexGridCoordinate.Add(hexGrid.transform.GetChild(i).position);
            hexGridAvailable[i] = true;
        }

        reserveSeatCoordinate = new Vector3[reserveSeat.transform.childCount];
        reserveSeatAvailable = new bool[reserveSeat.transform.childCount];
        for (int i = 0; i < reserveSeat.transform.childCount; i++)
        {
            reserveSeatCoordinate[i] = reserveSeat.transform.GetChild(i).position;
            reserveSeatAvailable[i] = true;
        }
    }

    public bool NewChess(GameObject chess_)
    {
        for (int i = 0; i < reserveSeatAvailable.Length; i++)
        {
            if (reserveSeatAvailable[i])
            {
                GameObject chess = Instantiate(chess_) as GameObject;
                chess.transform.position = reserveSeatCoordinate[i];
                chess.GetComponent<ChessMove>().SetController(this);
                chess.GetComponent<ChessMove>().SetShop(this.GetComponent<ChessShop>());
                chess.GetComponent<ChessMove>().SetPosIndex(i);
                reserveSeatAvailable[i] = false;
                return true;
            }
        }
        return false;
    }

    public void PutOpponent()
    {
        throw new NotImplementedException();
    }
}
