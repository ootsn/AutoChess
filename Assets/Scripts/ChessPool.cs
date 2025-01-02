using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//卡池
public class ChessPool : MonoBehaviour
{
    //[Header("Json")]
    //[SerializeField]
    //private TextAsset dataJson; //卡池数据+棋子属性文件

    //private JsonData data; //放置json文件数据用的
    private Dictionary<string, ChessModelAndCount> chesses = new Dictionary<string, ChessModelAndCount>(); //棋子数据字典，通过名字找对应的棋子
    //private Pool[] pool;
    private int[] remainQuantityEachCost; //每个费用的总剩余卡牌数，索引为棋子基础费用，从1开始，索引0保留

    private readonly static string chess2dFolderPath = "Prefabs/Chess_2d"; //存放2d模型的文件夹路径
    private readonly static string chess25dFolderPath = "Prefabs/Chess_2.5d"; //存放2.5d模型的文件夹路径
    private readonly static int[] exp = new int[] { 0, 0, 2, 6, 10, 20, 36, 48, 76, 84 }; //索引从0开始，分别是0-1、1-2、2-3......
    private readonly static float[][] probability = new float[][]
    {
        null,
        null,
        new float[] { 0.00f, 1.00f, 0.00f, 0.00f, 0.00f, 0.00f },
        new float[] { 0.00f, 0.75f, 0.25f, 0.00f, 0.00f, 0.00f },
        new float[] { 0.00f, 0.55f, 0.30f, 0.15f, 0.00f, 0.00f },
        new float[] { 0.00f, 0.45f, 0.33f, 0.20f, 0.02f, 0.00f },
        new float[] { 0.00f, 0.30f, 0.40f, 0.25f, 0.05f, 0.00f },
        new float[] { 0.00f, 0.19f, 0.30f, 0.40f, 0.10f, 0.01f },
        new float[] { 0.00f, 0.18f, 0.25f, 0.32f, 0.22f, 0.03f },
        new float[] { 0.00f, 0.15f, 0.20f, 0.25f, 0.30f, 0.10f },
        new float[] { 0.00f, 0.05f, 0.10f, 0.20f, 0.40f, 0.25f }
    }; //第一个参数是玩家等级，第二个参数是棋子费用索引。玩家等级从2开始，棋子费用从1开始
    private readonly static int[] quantityEachBaseCost = new int[] { 0, 30, 25, 18, 10, 9 }; //每个费用最开始的卡数，索引从1开始，索引0保留
    private static List<string>[] chessNameEachBaseCost; //每个费用都有哪些棋子，记录棋子名字，索引就是基础费用，索引0保留
    private readonly static int MIN_COST = 1;
    private readonly static int MAX_COST = quantityEachBaseCost.Length - 1;

    //private class ChessCount
    //{
    //    public string name;
    //    public int count;

    //    public ChessCount()
    //    {
    //        name = null;
    //        count = -1;
    //    }

    //    public ChessCount(string name, int count)
    //    {
    //        this.name = name;
    //        this.count = count;
    //    }
    //}

    //private class Pool : Collection<ChessCount>
    //{
    //    //private ChessCount defaultValue;
    //    private int totalCount;
    //    private Dictionary<string, int> nameToIndex;

    //    public Pool(ChessCount defaultValue) 
    //    {
    //        //this.defaultValue = defaultValue;
    //        totalCount = 0;
    //        nameToIndex = new Dictionary<string, int>();
    //    }

    //    protected override void InsertItem(int index, ChessCount newItem)
    //    {
    //        if (!this.Contains(newItem))
    //        {
    //            base.InsertItem(index, newItem);
    //            totalCount += newItem.count;
    //            nameToIndex.Add(newItem.name, index);
    //        }
    //    }

    //    public bool RandomGetChessName(out int result)
    //    {
    //        result = -1;
    //        if (this.totalCount == 0)
    //        {
    //            //item = defaultValue;
    //            return false;
    //        }
    //        else
    //        {
    //            int index = UnityEngine.Random.Range(0, this.totalCount);
    //            //item = null;
    //            for (int i = 0; i < this.Count; i++)
    //            {
    //                if (index < this[i].count)
    //                {
    //                    result = i;
    //                    break;
    //                }
    //                else
    //                {
    //                    index -= this[i].count;
    //                }
    //            }
    //            //foreach (ChessCount cc in this)
    //            //{
    //            //    if (index < cc.count)
    //            //    {
    //            //        item = cc;
    //            //        break;
    //            //    }
    //            //    else
    //            //    {
    //            //        index -= cc.count;
    //            //    }
    //            //}
    //            return true;
    //        }
    //    }

    //    //public void Decrease(ChessCount chessCount)
    //    //{
    //    //    chessCount.count--;
    //    //    this.totalCount--;
    //    //}

    //    //public void Increase(ChessCount chessCount)
    //    //{
    //    //    chessCount.count++;
    //    //    this.totalCount++;
    //    //}

    //    public void Decrease(int index)
    //    {
    //        this[index].count--;
    //        this.totalCount--;
    //    }

    //    public void Increase(int index)
    //    {
    //        this[index].count++;
    //        this.totalCount++;
    //    }
    //}

    private void Awake()
    {
        LoadData();
    }

    //读取并整理数据，初始化
    private void LoadData()
    {
        //data = JsonConvert.DeserializeObject<JsonData>(dataJson.text);
        LoadChessPrefab();
        PrepareChessQuantity();
        //PrepareChessCount();
        //LoadChessProperties();
        PrepareChessesEachBaseCost(chesses, MIN_COST, MAX_COST);
    }

    private static void PrepareChessesEachBaseCost(Dictionary<string, ChessModelAndCount> chesses, int minCost, int maxCost)
    {
        chessNameEachBaseCost = new List<string>[maxCost + 1];
        for (int i = minCost; i <= maxCost; i++)
        {
            chessNameEachBaseCost[i] = new List<string>();
        }
        foreach (var chess in chesses)
        {
            int cost = chess.Value.GetBaseCost();
            string name = chess.Key;
            chessNameEachBaseCost[cost].Add(name);
        }
    }

    //加载预制体
    private void LoadChessPrefab()
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
                ChessModelAndCount chessData = new ChessModelAndCount();
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

    //准备一下棋子数量相关的数据
    private void PrepareChessQuantity()
    {
        remainQuantityEachCost = new int[quantityEachBaseCost.Length];
        for (int i = 0; i < remainQuantityEachCost.Length; i++)
        {
            remainQuantityEachCost[i] = 0;
        }
        foreach (var chess in chesses)
        {
            int currentChessBaseCost = chess.Value.GetBaseCost();
            remainQuantityEachCost[currentChessBaseCost] += quantityEachBaseCost[currentChessBaseCost];
            chess.Value.totality = quantityEachBaseCost[currentChessBaseCost];
        }
        //for (int i = 0; i < data.pool.Length; i++)
        //{
        //    remainQuantityEachCost[i] = data.pool[i].count * data.pool[i].operators.Length;

        //    for (int j = 0; j < data.pool[i].operators.Length; j++)
        //    {
        //        string name = data.pool[i].operators[j];
        //        chesses[name].totality = data.pool[i].count;
        //    }
        //}
        //foreach (var pool in data.pool)
        //{
        //    foreach (var name in pool.operators)
        //    {
        //        chesses[name].totality = pool.count;
        //    }
        //}
    }

    //void PrepareChessCount()
    //{
    //    pool = new Pool[data.pool.Length];
    //    for (int i = 0; i < pool.Length; i++)
    //    {
    //        pool[i] = new Pool(null);
    //    }

    //    foreach (var pool in data.pool)
    //    {
    //        Pool set = GetChessSetByCost(pool.cost);
    //        foreach (var name in pool.operators)
    //        {
    //            set.Add(new ChessCount(name, pool.count));
    //        }
    //    }
    //}

    //Pool GetChessSetByCost(int cost)
    //{
    //    return pool[cost - 1];
    //}

    //整理棋子的属性，将其赋值到相应的2.5的模型的相应组件上
    //private void LoadChessProperties()
    //{
    //    //没完成
    //    foreach (var p in data.properties)
    //    {
    //        //print(p.Key);
    //        //print(p.Value);
    //        //var obj = chesses[p.Key].c25d.GetComponent<ChessBase>();
    //        //print(obj);
    //        chesses[p.Key].c25d.GetComponent<ChessBase>().SetProperties(p.Value);
    //    }
    //}

    //获取最大等级
    public int GetMaxLevel()
    {
        return exp.Length;
    }

    //根据等级和费用获取该类型棋子的出现概率
    public float GetProbability(int level, int cost)
    {
        return probability[level][cost];
    }

    //获取从当前等级升到下一级所需经验值
    public int GetEXP(int currentLevel)
    {
        return exp[currentLevel];
    }

    //public string GetChessNameFromPool(int cost, int index)
    //{
    //    return GetChessSetByCost(cost)[index].name;
    //}

    //根据费用随机抽取棋子
    private bool RandomGetChessName(int cost, out string name)
    {
        name = null;
        int index = Random.Range(0, remainQuantityEachCost[cost]);
        foreach (string chessName in chessNameEachBaseCost[cost])
        {
            if (index < chesses[chessName].totality)
            {
                name = chessName;
                break;
            }
            else
            {
                index -= chesses[chessName].totality;
            }
        }
        return name != null;
    }

    //判断该cost是否还有剩余棋子
    private bool RemainForCost(int cost)
    {
        return remainQuantityEachCost[cost] > 0;
    }

    //获取该等级下剩余cost的总概率。例如，若费用4没有棋子，则总概率为费用1、2、3、5在该等级下的概率之和
    private float GetCurrentTotalProbabilityByLevel(int level)
    {
        float p = 0f;
        for (int cost = MIN_COST; cost <= MAX_COST; cost++)
        {
            if (RemainForCost(cost) && GetProbability(level, cost) > 0)
            { 
                p += GetProbability(level, cost);
            }
        }
        return p;
    }

    //根据等级随机抽取一个棋子，返回中奖棋子的名字
    public bool RandomGetChessNameByLevel(int level, out int cost, out string name)
    {
        float currentProbability = Random.Range(0f, GetCurrentTotalProbabilityByLevel(level));
        float totalProbability = 0f;
        cost = MIN_COST;
        for (; cost <= MAX_COST; cost++)
        {
            if (RemainForCost(cost) && GetProbability(level, cost) > 0)
            {
                totalProbability += GetProbability(level, cost);
                if (currentProbability < totalProbability)
                {
                    break;
                }
            }
        }
        if (cost > MAX_COST)
        {

            name = null;
            return false;
        }
        else
        {
            return RandomGetChessName(cost, out name);
        }
    }

    //public void Decrease(int cost, int index)
    //{
    //    GetChessSetByCost(cost).Decrease(index);
    //}

    //public void Increase(int cost, int index)
    //{
    //    GetChessSetByCost(cost).Increase(index);
    //}

    //从卡池中扣除该棋子
    public bool Pull(int cost, string name, int quantity = 1)
    {
        bool result = chesses[name].pull(quantity);
        if (result)
        {
            remainQuantityEachCost[cost] -= quantity;
        }
        return result;
    }

    //public bool pull(int cost, int chessCountIndex)
    //{
    //    return chesses[GetChessSetByCost(cost)[chessCountIndex].name].pull();
    //}
    
    //将棋子放入卡池中
    public void Push(int cost, string name, int quantity = 1)
    {
        remainQuantityEachCost[cost] += quantity;
        chesses[name].push(quantity);
    }

    //public void push(int cost, int chessCountIndex)
    //{
    //    chesses[GetChessSetByCost(cost)[chessCountIndex].name].push();
    //}

    //获取棋子2d模型的相对位置
    public Vector3 GetChess2dRectTransformLocalPosition(string name)
    {
        return chesses[name].c2d.GetComponent<RectTransform>().localPosition;
    }

    //获取棋子2d模型的图片
    public Sprite GetChess2dImageSprite(string name)
    {
        return chesses[name].c2d.GetComponent<Image>().sprite;
    }

    //获取棋子的2.5d模型
    public GameObject GetChess25d(string name)
    {
        return chesses[name].c25d;
    }
}

//三个Json开头的类是用来存json数据的
//class JsonPool
//{
//    public int cost;
//    public int count;
//}

//public class JsonPropertiy
//{
//    public int baseCost;
//    public int[] hp;
//    public int[] atk;
//    public int[] def;
//    public float[] magicResistance;
//    public int attackRange;
//    public float attackSpeed;
//}

//class JsonData
//{
//    public JsonPool[] pool; //根据cost排列，为从1开始的连续值。1、2、3、4......
//    public int[] exp; //索引从0开始，分别是0-1、1-2、2-3......
//    public float[][] probability; //和上面一样
//    public Dictionary<string, JsonPropertiy> properties;
//}

//棋子模型和剩余个数
class ChessModelAndCount
{
    public GameObject c2d; //2d模型，商店里显示的
    public GameObject c25d; //2.5d模型，棋盘上显示的
    public int totality; //当前卡池中剩余棋子数目

    public ChessModelAndCount()
    {
        c2d = null;
        c25d = null;
        totality = 0;
    }

    //判断卡池中是否还有棋子
    public bool Empty()
    {
        return totality <= 0;
    }

    //获取棋子基础费用
    public int GetBaseCost()
    {
        return c25d.GetComponent<ChessBase>().GetBaseCost();
    }

    //外部加棋子进卡池，如卖牌
    public void push(int quantity)
    {
        totality += quantity;
    }

    //从卡池中取出棋子，如买牌
    public bool pull(int quantity)
    {
        if (totality < quantity)
        {
            return false;
        }
        else
        {
            totality -= quantity;
            return true;
        }
    }
}
