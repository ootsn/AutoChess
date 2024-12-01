using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class ChessControl : MonoBehaviour
{
    public GameObject hexGrid;
    public GameObject reserveSeat;
    public GameObject checkerboard;

    public bool[] hexGridAvailable { get; private set; }
    public bool[] reserveSeatAvailable { get; private set; }
    public List<Vector3> hexGridCoordinate { get; private set; }
    public List<Vector3> reserveSeatCoordinate { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        GetPlaceCoordinate();

        hexGrid.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetPlaceCoordinate()
    {
        hexGridCoordinate = new List<Vector3>();
        hexGridAvailable = new bool[hexGrid.transform.childCount];
        for (int i = 0; i < hexGrid.transform.childCount; i++)
        {
            hexGridCoordinate.Add(hexGrid.transform.GetChild(i).position);
            hexGridAvailable[i] = true;
        }

        reserveSeatCoordinate = new List<Vector3>();
        reserveSeatAvailable = new bool[reserveSeat.transform.childCount];
        for (int i = 0; i < reserveSeat.transform.childCount; i++)
        {
            reserveSeatCoordinate.Add(reserveSeat.transform.GetChild(i).position);
            reserveSeatAvailable[i] = true;
        }
    }

    public bool newChess(GameObject chess_)
    {
        for (int i = 0; i < reserveSeatAvailable.Length; i++)
        {
            if (reserveSeatAvailable[i])
            {
                GameObject chess = Instantiate(chess_) as GameObject;
                chess.transform.position = reserveSeatCoordinate[i];
                chess.GetComponent<ChessMove>().SetController(this.GetComponent<ChessControl>());
                chess.GetComponent<ChessMove>().SetPosIndex(i);
                reserveSeatAvailable[i] = false;
                return true;
            }
        }
        return false;
    }
}
