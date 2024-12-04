using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.VersionControl;
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
    [Header("Json")]
    public TextAsset dataJson;

    private int level;
    private int currEXP;  
    private int numOfChess;
    private int maxLevel;
    private bool isLocked;
    private bool notFlashRed;
    private JsonData data;
    private Dictionary<string, ChessData> chesses = new Dictionary<string, ChessData>();
    private ChessCommodity[] chessOnSale;
    private ChessPool[] chessPool;
    private Camera mainCamera;

    private readonly string chess2dFolderPath = "Prefabs/Chess_2d";
    private readonly string chess25dFolderPath = "Prefabs/Chess_2.5d";

    [System.Serializable]
    private class OperatorPool
    {
        public int cost;
        public int count;
        public string[] operators;
    }

    [System.Serializable]
    private class JsonData
    {
        public OperatorPool[] pool;
        public int[] exp;
        public float[][] probability;
    }
    private class ChessPool : Collection<ChessCount>
    {
        private ChessCount defaultValue;
        private int totalCount;
        private Dictionary<string, int> nameToIndex;

        public ChessPool(ChessCount defaultValue)
        {
            this.defaultValue = defaultValue;
            totalCount = 0;
            nameToIndex = new Dictionary<string, int>();
        }

        protected override void InsertItem(int index, ChessCount newItem)
        {
            if (!this.Contains(newItem))
            {
                base.InsertItem(index, newItem);
                totalCount += newItem.count;
                nameToIndex.Add(newItem.name, index);
            }
        }

        public bool RandomGetChessName(out ChessCount item)
        {
            if (this.totalCount == 0)
            {
                item = defaultValue;
                return false;
            }
            else
            {
                int index = UnityEngine.Random.Range(0, this.totalCount);
                item = null;
                foreach (ChessCount cc in this)
                {
                    if (index < cc.count)
                    {
                        item = cc;
                        break;
                    }
                    else
                    {
                        index -= cc.count;
                    }
                }
                return true;
            }
        }

        public void Decrease(ChessCount chessCount)
        {
            chessCount.count--;
            this.totalCount--;
        }

        public void Increase(ChessCount chessCount)
        {
            chessCount.count++;
            this.totalCount++;
        }
    }

    private class ChessData
    {
        public GameObject c2d;
        public GameObject c25d;
        public int totality;
        
        public ChessData()
        {
            c2d = null;
            c25d = null;
            totality = 0;
        }

        public bool Empty()
        {
            return totality <= 0;
        }

        public void push()
        {
            totality++;
        }

        public bool pull()
        {
            if (totality <= 0)
            {
                return false;
            }
            else
            {
                totality--;
                return true;
            }
        }
    }

    private class ChessCount
    {
        public string name;
        public int count;

        public ChessCount()
        {
            name = null;
            count = -1;
        }

        public ChessCount(string name, int count)
        {
            this.name = name;
            this.count = count;
        }
    }

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
        public ChessCount chessCount;
        public int cost;
        public bool inUse;

        public ChessCommodity()
        {
            chessCount = null;
            cost = 0;
            inUse = false;
        }

        public ChessCommodity(ChessCount chessCount, int cost, bool inUse)
        {
            this.chessCount = chessCount;
            this.cost = cost;
            this.inUse = inUse;
        }
    }

    void Start()
    {
        data = JsonConvert.DeserializeObject<JsonData>(dataJson.text);

        notFlashRed = true;

        costText.GetComponent<TextMeshProUGUI>().text = money.ToString();

        level = 2;
        numOfChess = 1;
        currEXP = 0;
        maxLevel = data.exp.Length;

        levelText.GetComponent<TextMeshProUGUI>().text = String.Format("LEVEL {0}", level);
        EXPText.GetComponent<TextMeshProUGUI>().text = String.Format("{0}/{1}", currEXP, data.exp[level]);
        upgradeButton.GetComponent<Button>().onClick.AddListener(() => { BuyEXPPoints(4, 4); });

        isLocked = false;
        lockButtonImage.GetComponent<Image>().sprite = (isLocked ? lockImage : unlockImage);
        lockButton.GetComponent<Button>().onClick.AddListener(() => { ConvertLock(); });

        refreshButton.GetComponent<Button>().onClick.AddListener(() => { Refresh(2); });

        LoadData();

        chessOnSale = new ChessCommodity[chessButtons.Length];
        for (int i = 0; i < chessButtons.Length; i++)
        {
            chessOnSale[i] = new ChessCommodity(new ChessCount(), -1, false);
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

        mainCamera = Camera.main;

        chessContainer.SetActive(true);
        sellArea.SetActive(false);
    }

    void RefreshProbabilityText()
    {
        for (int i = 0; i < probabilities.Length; i++)
        {
            probabilities[i].text = data.probability[level][i].ToString("0.##%");
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
        while (level < maxLevel && currEXP >= data.exp[level])
        {
            currEXP = currEXP - data.exp[level];
            level++;

            RefreshProbabilityText();
        }
        levelText.GetComponent<TextMeshProUGUI>().text = String.Format("LEVEL {0}", level);
        if (level < maxLevel)
            EXPText.GetComponent<TextMeshProUGUI>().text = String.Format("{0}/{1}", currEXP, data.exp[level]);
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

    void LoadChessPrefab()
    {
        var chess2dList = Resources.LoadAll<GameObject>(chess2dFolderPath);
        var chess25dList = Resources.LoadAll<GameObject>(chess25dFolderPath);

        if (chess2dList.Length == chess25dList.Length)
        {
            Dictionary<string, GameObject> chess25d = new Dictionary<string, GameObject>();
            foreach (var obj in chess25dList)
            {
                chess25d.Add(obj.name, obj);
            }

            for (int i = 0; i < chess2dList.Length; i++)
            {
                ChessData chessData = new ChessData();
                chessData.c2d = chess2dList[i];
                chessData.c25d = chess25d[chess2dList[i].name];
                chesses.Add(chess2dList[i].name, chessData);
            }
        }
        else
        {
            Debug.LogError("商店内的棋子种数（2d）和棋盘上的棋子种数（2.5d）不同");
        }
    }

    void PerfectChessCount()
    {        
        foreach (var pool in data.pool)
        {
            foreach (var name in pool.operators)
            {
                chesses[name].totality = pool.count;
            }
        }
    }

    void PrepareChessPool()
    {
        chessPool = new ChessPool[data.pool.Length];
        for (int i = 0;i < chessPool.Length;i++)
        {
            chessPool[i] = new ChessPool(null);
        }

        foreach (var pool in data.pool)
        {
            ChessPool set = GetChessSetByCost(pool.cost);
            foreach (var name in pool.operators)
            {
                set.Add(new ChessCount(name, pool.count));
            }
        }
    }

    ChessPool GetChessSetByCost(int cost)
    {
        return chessPool[cost - 1];
    }

    void LoadData()
    {
        LoadChessPrefab();
        PerfectChessCount();
        PrepareChessPool();
    }

    bool RandomGetChessByCost(out ChessCount chessCount, int cost)
    {
        if (GetChessSetByCost(cost).RandomGetChessName(out chessCount))
        {
            chesses[chessCount.name].pull();
            GetChessSetByCost(cost).Decrease(chessCount);

            return true;
        }
        else
        {
            chessCount = null;
            return false;
        }
    }

    void PutChessBack(ChessCommodity chessCommodity)
    {
        if (chessCommodity.inUse == true)
        {
            GetChessSetByCost(chessCommodity.cost).Increase(chessCommodity.chessCount);
            chesses[chessCommodity.chessCount.name].push();
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

        chessButtons[index].transform.GetChild(0).GetComponent<RectTransform>().localPosition = chesses[chessOnSale[index].chessCount.name].c2d.GetComponent<RectTransform>().localPosition;
        chessButtons[index].transform.GetChild(0).GetComponent<Image>().sprite = chesses[chessOnSale[index].chessCount.name].c2d.GetComponent<Image>().sprite;
    }

    void Refresh(int cost)
    {
        if (SpendMoney(cost))
        {
            Unlock();

            for (int i = 0; i < chessButtons.Length; i++)
            {
                ClearChessButton(i);

                float currentProbability = UnityEngine.Random.value;
                float totalProbability = 0f;
                int index = 0;
                for (; index < data.probability[level].Length; index++)
                {
                    totalProbability += data.probability[level][index];
                    if (currentProbability < totalProbability)
                    {
                        break;
                    }
                } 

                if (RandomGetChessByCost(out chessOnSale[i].chessCount, data.pool[index].cost))
                {
                    chessOnSale[i].cost = data.pool[index].cost;
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
        if (AbleToSpendMoney(chessOnSale[index].cost) && this.GetComponent<ChessControl>().newChess(chesses[chessName].c25d))
        {
            SpendMoneyDirectly(chessOnSale[index].cost);
            chessOnSale[index].inUse = false;
            chessButtons[index].GetComponent<Image>().enabled = false;
            chessButtons[index].transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
    }

    public bool Sell(GameObject gameObject, int cost)
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
            Destroy(gameObject);
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
