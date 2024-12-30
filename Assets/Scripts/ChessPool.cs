using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//卡池
public class ChessPool : MonoBehaviour
{
    [Header("Json")]
    [SerializeField]
    private TextAsset dataJson; //卡池数据+棋子属性文件

    private JsonData data; //放置json文件数据用的
    private Dictionary<string, ChessModelAndCount> chesses = new Dictionary<string, ChessModelAndCount>(); //棋子数据字典，通过名字找对应的棋子
    //private Pool[] pool;
    private int[] quantityEachCost; //每个费用的总剩余卡牌数，索引为费用减1

    private readonly string chess2dFolderPath = "Prefabs/Chess_2d"; //存放2d模型的文件夹路径
    private readonly string chess25dFolderPath = "Prefabs/Chess_2.5d"; //存放2.5d模型的文件夹路径

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
        data = JsonConvert.DeserializeObject<JsonData>(dataJson.text);
        LoadChessPrefab();
        PrepareChessQuantity();
        //PrepareChessCount();
        LoadChessProperties();
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
        quantityEachCost = new int[data.pool.Length];
        for (int i = 0; i < data.pool.Length; i++)
        {
            quantityEachCost[i] = data.pool[i].count * data.pool[i].operators.Length;

            for (int j = 0; j < data.pool[i].operators.Length; j++)
            {
                string name = data.pool[i].operators[j];
                chesses[name].totality = data.pool[i].count;
            }
        }
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
    private void LoadChessProperties()
    {
        //没完成
        foreach (var p in data.properties)
        {
            //print(p.Key);
            //print(p.Value);
            //var obj = chesses[p.Key].c25d.GetComponent<ChessBase>();
            //print(obj);
            chesses[p.Key].c25d.GetComponent<ChessBase>().SetProperties(p.Value);
        }
    }

    //获取最大等级
    public int GetMaxLevel()
    {
        return data.exp.Length;
    }

    //根据等级和费用获取该类型棋子的出现概率
    public float GetProbability(int level, int costMinusOne)
    {
        return data.probability[level][costMinusOne];
    }

    //获取从当前等级升到下一级所需经验值
    public int GetEXP(int currentLevel)
    {
        return data.exp[currentLevel];
    }

    //public string GetChessNameFromPool(int cost, int index)
    //{
    //    return GetChessSetByCost(cost)[index].name;
    //}

    //根据费用随机抽取棋子
    private bool RandomGetChessName(int costMinusOne, out string name)
    {
        name = null;
        int index = Random.Range(0, quantityEachCost[costMinusOne]);
        foreach (string chessName in data.pool[costMinusOne].operators)
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
    private bool RemainForCost(int costMinusOne)
    {
        return quantityEachCost[costMinusOne] > 0;
    }

    //获取该等级下剩余cost的总概率。例如，若费用4没有棋子，则总概率为费用1、2、3、5在该等级下的概率之和
    private float GetCurrentTotalProbabilityByLevel(int level)
    {
        float p = 0f;
        for (int costMinusOne = 0; costMinusOne < GetMaxCost(); costMinusOne++)
        {
            if (RemainForCost(costMinusOne) && GetProbability(level, costMinusOne) > 0)
            { 
                p += GetProbability(level, costMinusOne);
            }
        }
        return p;
    }

    //根据等级随机抽取一个棋子，返回中奖棋子的名字
    public bool RandomGetChessNameByLevel(int level, out int cost, out string name)
    {
        float currentProbability = Random.Range(0f, GetCurrentTotalProbabilityByLevel(level));
        float totalProbability = 0f;
        int costMinusOne = 0;
        for (; costMinusOne < GetMaxCost(); costMinusOne++)
        {
            if (RemainForCost(costMinusOne) && GetProbability(level, costMinusOne) > 0)
            {
                totalProbability += GetProbability(level, costMinusOne);
                if (currentProbability < totalProbability)
                {
                    break;
                }
            }
        }
        cost = costMinusOne + 1;
        if (costMinusOne == GetMaxCost())
        {

            name = null;
            return false;
        }
        else
        {
            return RandomGetChessName(costMinusOne, out name);
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
    public bool pull(int costMinusOne, string name)
    {
        bool result = chesses[name].pull();
        if (result)
        {
            quantityEachCost[costMinusOne]--;
        }
        return result;
    }

    //public bool pull(int cost, int chessCountIndex)
    //{
    //    return chesses[GetChessSetByCost(cost)[chessCountIndex].name].pull();
    //}
    
    //将棋子放入卡池中
    public void push(int costMinusOne, string name)
    {
        quantityEachCost[costMinusOne]++;
        chesses[name].push();
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


    //获取最大费用
    private int GetMaxCost()
    {
        return data.pool.Length;
    }
}

//三个Json开头的类是用来存json数据的
class JsonPool
{
    public int cost;
    public int count;
    public string[] operators;
}

public class JsonPropertiy
{
    public int baseCost;
    public int[] hp;
    public int[] atk;
    public int[] def;
    public float[] magicResistance;
    public int attackRange;
    public float attackSpeed;
}

class JsonData
{
    public JsonPool[] pool; //根据cost排列，为从1开始的连续值。1、2、3、4......
    public int[] exp; //索引从0开始，分别是0-1、1-2、2-3......
    public float[][] probability; //和上面一样
    public Dictionary<string, JsonPropertiy> properties;
}

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

    //外部加棋子进卡池，如卖牌
    public void push()
    {
        totality += c25d.GetComponent<ChessBase>().quantity;
    }

    //从卡池中取出棋子，如买牌
    public bool pull()
    {
        if (this.Empty())
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
