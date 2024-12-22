using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleTest : MonoBehaviour
{
    public TMP_InputField OpponentIndexInputField;
    public Button MoveOpponentButton;
    public Button AStarButton;

    private bool opponentInMyBoard;
    private bool drawAStar;

    private void Start()
    {
        opponentInMyBoard = false;
        drawAStar = false;

        OpponentIndexInputField.onValueChanged.AddListener(OnInputChanged);
        MoveOpponentButton.onClick.AddListener(OnMoveOpponentButtonClick);
        AStarButton.onClick.AddListener(OnAStarButtonClick);
    }

    private void Update()
    {
        if (drawAStar)
        {
            DrawAStarRoute();
        }
    }

    private void OnInputChanged(string text)
    {
        foreach (char c in text)
        {
            if(!char.IsDigit(c))
            {
                OpponentIndexInputField.text = text.Remove(text.IndexOf(c), 1);
                return;
            }
        }
    }

    private void OnMoveOpponentButtonClick()
    {
        if (opponentInMyBoard)
        {
            transform.parent.GetComponent<ChessControl>().RemoveOpponent();
            opponentInMyBoard = false;
            print("将敌人移回去");
        }
        else
        {
            string number = null;
            string input = transform.parent.parent.name;
            string pattern = @"\((\d+)\)"; // 匹配括号中的数字

            Match match = Regex.Match(input, pattern);
            if (match.Success)
            {
                number = match.Groups[1].Value; // 提取第一个捕获组中的数字
                //print($"提取的数字是: {number}");
            }
            else
            {
                print("未找到匹配的数字！");
                return;
            }

            if (number.Equals(OpponentIndexInputField.text))
            {
                print("敌人不能是自己");
                return;
            }

            GameObject player = GameObject.Find($"Player ({OpponentIndexInputField.text})");
            Transform opponentHexGrid = null;
            if (player != null && (opponentHexGrid = player.transform.Find("Canvas")) != null)
            {
                transform.parent.GetComponent<ChessControl>().PutOpponent(opponentHexGrid.GetComponent<ChessControl>().myHexagons);
                opponentInMyBoard = true;
                print("放置敌人到自己棋盘");
            }
            else
            {
                print("敌人id不存在");
            }
        }
    }

    private void OnAStarButtonClick()
    {
        transform.parent.GetComponent<ChessControl>().hexGrid.ActivateMyHexGrid();
        transform.parent.GetComponent<ChessControl>().hexGrid.ActivateOpponentHexGrid();
        drawAStar = !drawAStar;
        //DrawAStarRoute();
    }

    private void DrawAStarRoute()
    {
        ChessControl chessControl = transform.parent.GetComponent<ChessControl>();
        foreach (var place in chessControl.myHexagons.Where(t => !HexGridLayout.isHexPositionAvailable(t)))
        {
            var res = chessControl.FindNearestOpponentPlace(place.GetComponent<Position>());
            if (res.opponentPlace == null)
            {
                continue;
            }
            var start = res.opponentPlace;
            while (res.route[start] != null)
            {
                Debug.DrawLine(start.transform.position, res.route[start].transform.position, Color.red);
                start = res.route[start];
            }
        }
    }
}
