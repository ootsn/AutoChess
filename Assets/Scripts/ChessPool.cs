using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//����
public class ChessPool : MonoBehaviour
{
    //[Header("Json")]
    //[SerializeField]
    //private TextAsset dataJson; //��������+���������ļ�

    //private JsonData data; //����json�ļ������õ�
    private Dictionary<string, ChessModelAndCount> chesses = new Dictionary<string, ChessModelAndCount>(); //���������ֵ䣬ͨ�������Ҷ�Ӧ������
    //private Pool[] pool;
    private int[] remainQuantityEachCost; //ÿ�����õ���ʣ�࿨����������Ϊ���ӻ������ã���1��ʼ������0����

    private readonly static string chess2dFolderPath = "Prefabs/Chess_2d"; //���2dģ�͵��ļ���·��
    private readonly static string chess25dFolderPath = "Prefabs/Chess_2.5d"; //���2.5dģ�͵��ļ���·��
    private readonly static int[] exp = new int[] { 0, 0, 2, 6, 10, 20, 36, 48, 76, 84 }; //������0��ʼ���ֱ���0-1��1-2��2-3......
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
    }; //��һ����������ҵȼ����ڶ������������ӷ�����������ҵȼ���2��ʼ�����ӷ��ô�1��ʼ
    private readonly static int[] quantityEachBaseCost = new int[] { 0, 30, 25, 18, 10, 9 }; //ÿ�������ʼ�Ŀ�����������1��ʼ������0����
    private static List<string>[] chessNameEachBaseCost; //ÿ�����ö�����Щ���ӣ���¼�������֣��������ǻ������ã�����0����
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

    //��ȡ���������ݣ���ʼ��
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

    //����Ԥ����
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
            Debug.LogError("�̵��ڵ�����������2d���������ϵ�����������2.5d����ͬ");
        }
    }

    //׼��һ������������ص�����
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

    //�������ӵ����ԣ����丳ֵ����Ӧ��2.5��ģ�͵���Ӧ�����
    //private void LoadChessProperties()
    //{
    //    //û���
    //    foreach (var p in data.properties)
    //    {
    //        //print(p.Key);
    //        //print(p.Value);
    //        //var obj = chesses[p.Key].c25d.GetComponent<ChessBase>();
    //        //print(obj);
    //        chesses[p.Key].c25d.GetComponent<ChessBase>().SetProperties(p.Value);
    //    }
    //}

    //��ȡ���ȼ�
    public int GetMaxLevel()
    {
        return exp.Length;
    }

    //���ݵȼ��ͷ��û�ȡ���������ӵĳ��ָ���
    public float GetProbability(int level, int cost)
    {
        return probability[level][cost];
    }

    //��ȡ�ӵ�ǰ�ȼ�������һ�����辭��ֵ
    public int GetEXP(int currentLevel)
    {
        return exp[currentLevel];
    }

    //public string GetChessNameFromPool(int cost, int index)
    //{
    //    return GetChessSetByCost(cost)[index].name;
    //}

    //���ݷ��������ȡ����
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

    //�жϸ�cost�Ƿ���ʣ������
    private bool RemainForCost(int cost)
    {
        return remainQuantityEachCost[cost] > 0;
    }

    //��ȡ�õȼ���ʣ��cost���ܸ��ʡ����磬������4û�����ӣ����ܸ���Ϊ����1��2��3��5�ڸõȼ��µĸ���֮��
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

    //���ݵȼ������ȡһ�����ӣ������н����ӵ�����
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

    //�ӿ����п۳�������
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
    
    //�����ӷ��뿨����
    public void Push(int cost, string name, int quantity = 1)
    {
        remainQuantityEachCost[cost] += quantity;
        chesses[name].push(quantity);
    }

    //public void push(int cost, int chessCountIndex)
    //{
    //    chesses[GetChessSetByCost(cost)[chessCountIndex].name].push();
    //}

    //��ȡ����2dģ�͵����λ��
    public Vector3 GetChess2dRectTransformLocalPosition(string name)
    {
        return chesses[name].c2d.GetComponent<RectTransform>().localPosition;
    }

    //��ȡ����2dģ�͵�ͼƬ
    public Sprite GetChess2dImageSprite(string name)
    {
        return chesses[name].c2d.GetComponent<Image>().sprite;
    }

    //��ȡ���ӵ�2.5dģ��
    public GameObject GetChess25d(string name)
    {
        return chesses[name].c25d;
    }
}

//����Json��ͷ������������json���ݵ�
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
//    public JsonPool[] pool; //����cost���У�Ϊ��1��ʼ������ֵ��1��2��3��4......
//    public int[] exp; //������0��ʼ���ֱ���0-1��1-2��2-3......
//    public float[][] probability; //������һ��
//    public Dictionary<string, JsonPropertiy> properties;
//}

//����ģ�ͺ�ʣ�����
class ChessModelAndCount
{
    public GameObject c2d; //2dģ�ͣ��̵�����ʾ��
    public GameObject c25d; //2.5dģ�ͣ���������ʾ��
    public int totality; //��ǰ������ʣ��������Ŀ

    public ChessModelAndCount()
    {
        c2d = null;
        c25d = null;
        totality = 0;
    }

    //�жϿ������Ƿ�������
    public bool Empty()
    {
        return totality <= 0;
    }

    //��ȡ���ӻ�������
    public int GetBaseCost()
    {
        return c25d.GetComponent<ChessBase>().GetBaseCost();
    }

    //�ⲿ�����ӽ����أ�������
    public void push(int quantity)
    {
        totality += quantity;
    }

    //�ӿ�����ȡ�����ӣ�������
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
