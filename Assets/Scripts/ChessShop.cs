using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Windows;
using System.Runtime.InteropServices;
using UnityEditor;
using System.Linq;
using System.Collections.ObjectModel;
using UnityEngine.EventSystems;

public class ChessShop : MonoBehaviour
{
    //[Header("ChessControl")]
    //public ChessControl controller;
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
    [Header("Refresh")]
    public GameObject refreshButton;
    [Header("Chess")]
    public GameObject chessContainer;
    public GameObject[] chessButtons = new GameObject[] { };
    [Header("Sell")]
    public GameObject sellArea;
    [Header("Probability")]
    public TextMeshProUGUI[] probabilities = new TextMeshProUGUI[] { };
    [Header("EventSystem")]
    [SerializeField]
    private EventSystem eventSystem;
    [Header("ChessPool")]
    [SerializeField]
    private ChessPool pool;

    private int level;
    private int currEXP;
    //private int numOfChess;
    private int maxLevel;
    private bool isLocked;
    private bool notFlashRed;

    private ChessCommodity[] chessOnSale;

    private class Pair<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public Pair(T1 Item1, T2 Item2)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
        }

        public Pair()
        {

        }
    }

    private class ChessCommodity
    {
        public string name;
        public int cost;
        public bool inUse;

        public ChessCommodity()
        {
            name = null;
            cost = -1;
            inUse = false;
        }

        public ChessCommodity(bool inUse)
        {
            this.name = null;
            this.cost = -1;
            this.inUse = inUse;
        }
    }

    void Start()
    {

        notFlashRed = true;

        costText.GetComponent<TextMeshProUGUI>().text = money.ToString();

        level = 2;
        //numOfChess = 1;
        currEXP = 0;
        maxLevel = pool.GetMaxLevel();

        levelText.GetComponent<TextMeshProUGUI>().text = String.Format("LEVEL {0}", level);
        EXPText.GetComponent<TextMeshProUGUI>().text = String.Format("{0}/{1}", currEXP, pool.GetEXP(level));
        upgradeButton.GetComponent<Button>().onClick.AddListener(() => { BuyEXPPoints(4, 4); });

        isLocked = false;
        lockButtonImage.GetComponent<Image>().sprite = (isLocked ? lockImage : unlockImage);
        lockButton.GetComponent<Button>().onClick.AddListener(() => { ConvertLock(); });

        refreshButton.GetComponent<Button>().onClick.AddListener(() => { Refresh(2); });

        chessOnSale = new ChessCommodity[chessButtons.Length];
        for (int i = 0; i < chessButtons.Length; i++)
        {
            chessOnSale[i] = new ChessCommodity(false);
        }

        chessButtons[0].GetComponent<Button>().onClick.AddListener(() => { BuyChess(0); });
        chessButtons[1].GetComponent<Button>().onClick.AddListener(() => { BuyChess(1); });
        chessButtons[2].GetComponent<Button>().onClick.AddListener(() => { BuyChess(2); });
        chessButtons[3].GetComponent<Button>().onClick.AddListener(() => { BuyChess(3); });
        chessButtons[4].GetComponent<Button>().onClick.AddListener(() => { BuyChess(4); });
        for (int i = 0; i < chessButtons.Length; i++)
        {
            ClearChessButton(i);
        }

        RefreshProbabilityText();

        chessContainer.SetActive(true);
        sellArea.SetActive(false);
    }

    void RefreshProbabilityText()
    {
        for (int i = 0; i < probabilities.Length; i++)
        {
            //probabilities[i].text = data.probability[level][i].ToString("0.##%");
            probabilities[i].text = pool.GetProbability(level, i + 1).ToString("0.##%");
        }
    }

    void BuyEXPPoints(int deltaEXP, int cost)
    {
        if (level < maxLevel && SpendMoney(cost))
        {
            Upgrade(deltaEXP);
        }
    }

    void Upgrade(int deltaEXP)
    {
        currEXP += deltaEXP;
        while (level < maxLevel && currEXP >= /*data.exp[level]*/ pool.GetEXP(level))
        {
            currEXP = currEXP - /*data.exp[level]*/pool.GetEXP(level);
            level++;

            RefreshProbabilityText();
        }
        levelText.GetComponent<TextMeshProUGUI>().text = String.Format("LEVEL {0}", level);
        if (level < maxLevel)
            EXPText.GetComponent<TextMeshProUGUI>().text = String.Format("{0}/{1}", currEXP, /*data.exp[level]*/pool.GetEXP(level));
        else
            EXPText.GetComponent<TextMeshProUGUI>().text = "---";
    }

    void ConvertLock()
    {
        isLocked = !isLocked;
        lockButtonImage.GetComponent<Image>().sprite = (isLocked ? lockImage : unlockImage);
    }

    void Lock()
    {
        isLocked = true;
        lockButtonImage.GetComponent<Image>().sprite = lockImage;
    }

    void Unlock()
    {
        isLocked = false;
        lockButtonImage.GetComponent<Image>().sprite = unlockImage;
    }

    bool SpendMoney(int cost)
    {
        if (money >= cost)
        {
            money -= cost;
            costText.GetComponent<TextMeshProUGUI>().text = money.ToString();
            return true;
        }
        else
        {
            if (notFlashRed)
            {
                notFlashRed = false;
                StartCoroutine(FlashRed<TextMeshProUGUI>(costText.GetComponent<TextMeshProUGUI>()));
            }
            return false;
        }
    }

    bool AbleToSpendMoney(int cost)
    {
        if (money >= cost)
        {
            return true;
        }
        else
        {
            if (notFlashRed)
            {
                notFlashRed = false;
                StartCoroutine(FlashRed<TextMeshProUGUI>(costText.GetComponent<TextMeshProUGUI>()));
            }
            return false;
        }
    }

    void SpendMoneyDirectly(int cost)
    {
        money -= cost;
        costText.GetComponent<TextMeshProUGUI>().text = money.ToString();
    }

    void AddMoneyDirectly(int cost)
    {
        money += cost;
        costText.GetComponent<TextMeshProUGUI>().text = money.ToString();
    }

    IEnumerator FlashRed<T>(T component) where T : Graphic
    {
        Color rawColor = component.color;
        Color[] colors = new Color[] { rawColor, Color.red};
        int colorIndex = 1;
        int count = 3;

        while (count > 0)
        {
            float duration = 0.15f; // 持续时间（秒）
            float elapsed = 0.0f;
            component.color = colors[colorIndex];
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            colorIndex = (colorIndex + 1) % colors.Length;
            count--;
        }

        component.color = rawColor;
        notFlashRed = true;
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

    //bool RandomGetChessByCost(int costMinusOne, out string name)
    //{
    //    if (pool.RandomGetChessName(costMinusOne, out name))
    //    {
    //        //chesses[chessCount.name].pull();
    //        pool.pull(costMinusOne, name);
    //        //GetChessSetByCost(cost).Decrease(chessCount);
    //        //pool.Decrease(cost, chessCountIndex);


    //        return true;
    //    }
    //    else
    //    {
    //        name = null;
    //        return false;
    //    }
    //}

    void PutChessBack(ChessCommodity chessCommodity)
    {
        if (chessCommodity.inUse == true)
        {
            //GetChessSetByCost(chessCommodity.cost).Increase(chessCommodity.chessCount);
            //pool.Increase(chessCommodity.name);
            //chesses[chessCommodity.chessCount.name].push();
            pool.Push(chessCommodity.cost, chessCommodity.name);
        }
    }

    void ClearChessButton(int index)
    {
        PutChessBack(chessOnSale[index]);
        chessButtons[index].GetComponent<Button>().enabled = false;
        chessButtons[index].GetComponent<Image>().enabled = false;
        chessButtons[index].transform.GetChild(0).GetComponent<Image>().enabled = false;
    }

    void SellAtChessButton(int index)
    {
        chessButtons[index].GetComponent<Button>().enabled = true;
        chessButtons[index].GetComponent<Image>().enabled = true;
        chessButtons[index].transform.GetChild(0).GetComponent<Image>().enabled = true;

        string chessName = chessOnSale[index].name;
        chessButtons[index].transform.GetChild(0).GetComponent<RectTransform>().localPosition = pool.GetChess2dRectTransformLocalPosition(chessName)/*chesses[chessOnSale[index].chessCount.name].c2d.GetComponent<RectTransform>().localPosition*/;
        chessButtons[index].transform.GetChild(0).GetComponent<Image>().sprite = pool.GetChess2dImageSprite(chessName)/*chesses[chessOnSale[index].chessCount.name].c2d.GetComponent<Image>().sprite*/;
    }

    //刷新
    void Refresh(int cost)
    {
        if (SpendMoney(cost))
        {
            Unlock();

            for (int i = 0; i < chessButtons.Length; i++)
            {
                ClearChessButton(i);

                //float currentProbability = UnityEngine.Random.value;
                //float totalProbability = 0f;
                //int costMinusOne = 0;
                //for (; costMinusOne < pool.GetMaxCost()/*data.probability[level].Length*/; costMinusOne++)
                //{
                //    totalProbability += /*data.probability[level][index]*/pool.GetProbability(level, costMinusOne);
                //    if (currentProbability < totalProbability)
                //    {
                //        break;
                //    }
                //} 

                if (pool.RandomGetChessNameByLevel(level, out chessOnSale[i].cost, out chessOnSale[i].name))
                {
                    pool.Pull(chessOnSale[i].cost, chessOnSale[i].name);

                    //chessOnSale[i].cost = /*data.pool[costMinusOne].cost*/costMinusOne + 1;
                    chessOnSale[i].inUse = true;

                    SellAtChessButton(i);
                }
                else
                {
                    chessOnSale[i].inUse = false;
                }
            }
        }
    }

    void BuyChess(int index)
    {
        string chessName = chessButtons[index].transform.GetChild(0).GetComponent<Image>().sprite.name;
        if (AbleToSpendMoney(chessOnSale[index].cost) && this.GetComponent<ChessControl>().NewChess(/*chesses[chessName].c25d*/pool.GetChess25d(chessName)))
        {
            SpendMoneyDirectly(chessOnSale[index].cost);
            chessOnSale[index].inUse = false;
            chessButtons[index].GetComponent<Image>().enabled = false;
            chessButtons[index].transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
    }

    public bool Sell(ChessBase chess, int cost)
    {
        // 创建一个 PointerEventData 来保存鼠标点击数据
        PointerEventData pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        // 创建一个 RaycastResult 列表，保存所有检测到的 UI 元素
        var results = new System.Collections.Generic.List<RaycastResult>();

        // 检测所有的 UI 元素
        eventSystem.RaycastAll(pointerEventData, results);

        if (results.Count > 0 && results[0].gameObject == sellArea)
        {
            AddMoneyDirectly(cost);
            pool.Push(chess.GetBaseCost(), chess.GetName(), chess.quantity);
            Destroy(chess.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DisplayPurchaseInterface()
    {
        chessContainer.SetActive(true);
        sellArea.SetActive(false);
    }

    public void DisplaySellingInterface(int cost)
    {
        chessContainer.SetActive(false);
        sellArea.GetComponent<Text>().text = string.Format("出售以获得{0}金币", cost);
        sellArea.SetActive(true);
    }

    //测试用
    //[MenuItem("test/draePrize")]
    //public static void test()
    //{

    //}

    //public static void PrintDictionary(Dictionary<int, string> dict, string name)
    //{
    //    print($"{name}:");
    //    foreach (var kvp in dict)
    //    {
    //        print($"  {kvp.Key}: {kvp.Value}");
    //    }
    //}
}
