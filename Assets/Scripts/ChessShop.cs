using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChessShop : MonoBehaviour
{
    [Header("Cost")]
    public int money; //暂时放这，调试用。后期看看要不要移到private
    public GameObject costText;
    [Header("Level")]
    public GameObject levelText;
    public GameObject EXPText;
    public GameObject upgradeButton;
    [Header("Lock")]
    public Sprite lockImage;
    public Sprite unlockImage;
    public GameObject lockButtonImage;
    public GameObject lockButton;

    private int level;
    private int currEXP;
    private static readonly int[] EXP = new int[] { 0, 0, 2, 6, 10, 20, 36, 48, 76, 84 };
    private int numOfChess;
    private int maxLevel;
    private bool isLocked;

    // Start is called before the first frame update
    void Start()
    {
        costText.GetComponent<TextMeshProUGUI>().text = money.ToString();

        level = 2;
        numOfChess = 1;
        currEXP = 0;
        maxLevel = EXP.Length;

        levelText.GetComponent<TextMeshProUGUI>().text = String.Format("LEVEL {0}", level);
        EXPText.GetComponent<TextMeshProUGUI>().text = String.Format("{0}/{1}", currEXP, EXP[level]);
        upgradeButton.GetComponent<Button>().onClick.AddListener(() => { BuyEXPPoints(4, 4); });

        isLocked = false;
        lockButtonImage.GetComponent<Image>().sprite = (isLocked ? lockImage : unlockImage);
        lockButton.GetComponent<Button>().onClick.AddListener(() => { Lock(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuyEXPPoints(int deltaEXP, int cost)
    {
        if (money >= cost && level < maxLevel)
        {
            money -= cost;
            Upgrade(deltaEXP);
            costText.GetComponent<TextMeshProUGUI>().text = money.ToString();
        }
    }

    public void Upgrade(int deltaEXP)
    {
        currEXP += deltaEXP;
        while (level < maxLevel && currEXP >= EXP[level])
        {
            currEXP = currEXP - EXP[level];
            level++;  
        }
        levelText.GetComponent<TextMeshProUGUI>().text = String.Format("LEVEL {0}", level);
        if (level < maxLevel)
            EXPText.GetComponent<TextMeshProUGUI>().text = String.Format("{0}/{1}", currEXP, EXP[level]);
        else
            EXPText.GetComponent<TextMeshProUGUI>().text = "---";
    }

    public void Lock()
    {
        isLocked = !isLocked;
        lockButtonImage.GetComponent<Image>().sprite = (isLocked ? lockImage : unlockImage);
    }

    void ShowComponents(GameObject obj)
    {
        // 获取当前 GameObject 上的所有组件
        Component[] components = obj.GetComponents<Component>();

        // 遍历并打印每个组件的名称
        foreach (Component component in components)
        {
            Debug.Log(component.GetType().Name);
        }
    }
}
