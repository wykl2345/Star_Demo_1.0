using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;
using MapStateManager;
using Script.Tool;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private PlayerMove _player;
    public PlayerMove Player
    {
        get
        {
            return _player ??= FindObjectOfType<PlayerMove>();
        }
    }
    
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance is null)
            {
                var target = FindObjectOfType<GameManager>();
                _instance = target;

                if (target is null)
                {
                    var newMana = new GameObject("GameManager").AddComponent<GameManager>();
                    _instance = newMana;
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }

            return _instance;
        }
    }

    public int bagNum = 10;
    private MapManager _map;
    public MapManager Map
    {
        get
        {
            if (_map is null)
            {
                _map = FindObjectOfType<MapManager>();
            }

            return _map;
        }
    }

    private ShopManager _shopManager;

    public ShopManager Shop
    {
        get
        {
            if (_shopManager is null)
            {
                _shopManager = FindObjectOfType<ShopManager>();
            }

            return _shopManager;
        }
    }

    public List<Item> SeedList = new List<Item>();
    public List<GridInfo> MapGridInfos = new List<GridInfo>();
    public List<BagItemData> BagList = new List<BagItemData>();
    public Item SelectedItem;
    public int selectID;

    public Dictionary<int, string> IdWithMessage = new Dictionary<int, string>();

    public TimeMana timeMana = new TimeMana();

    public Button TestbuButton1;
    private void Start()
    {
        InitMessage();
        StartCoroutine(TimeUpdata(60));
        TestbuButton1.onClick.AddListener(PlantGrow);
    }

    void InitMessage()
    {
        MessageManager.Instance.Subscribe("锄头",HoeFunc);
    }

    private void InitBag()
    {
        if(BagList.Count != 0)
            return;
        
        for (int i = 0; i < bagNum; i++)
        {
            var go = new Item();
            var data = new BagItemData(go, 1);
            BagList.Add(data);
        }
        BagItemData data1 = new BagItemData(new Item(0),10);
        BagItemData data2 = new BagItemData(new Item(1),1);
        BagItemData data3 = new BagItemData(new Item(2),1);
        BagItemData data4 = new BagItemData(new Item(3),1);
        BagList[0] = data1;
        BagList[1] = data2;
        BagList[2] = data3;
        BagList[3] = data4;
    }

    private void Update()
    {
        ControlFunc();

        if (BagList.Count == 0)
        {
            InitBag();
            MessageManager.Instance.Dispatch("Item_Panel_Refresh");
        }

        
        
        UseT();

        if (SelectedItem is not null)
        {
            MessageManager.Instance.Dispatch("Item_Panel_Refresh");
            selectID = SelectedItem.ItemID;
        }
    }

    void PlantGrow()
    {
        foreach (var grid in MapGridInfos)
        {
            if (grid.SItem is not null)
            {
                var seed = grid.SItem as Seed;
                
                if(seed is null)
                    return;
                
                if (seed.NowStep < seed.Steps.Length - 1)
                {
                    seed.NowStep++;
                    string name = seed.Step_Images[seed.NowStep];
                    var tiles = Map.NameWithTiles[name];
                    Map.overlayMap.SetTile(grid.CellPos,tiles);
                    //var tile = Map.overlayMap.GetTile<Tile>(grid.CellPos);
                    //tile.sprite = grid.Seed.Icon[grid.Seed.SeedBase.NowStep];

                }else if (seed.NowStep == seed.Steps.Length - 1)
                {
                    
                }
            }
        }
    }
    
    void UseT()
    {
        if (Input.GetMouseButtonUp(1) && SelectedItem != null)
        {
            Vector3 mousepos = Input.mousePosition;
            Vector3 worldpos = Camera.main.ScreenToWorldPoint(new Vector3(mousepos.x, mousepos.y, 10));
            Vector3Int pos = Map.map.WorldToCell(worldpos);

            pos.z = 0;

            if (SelectedItem.Type == ItemType.Tool && SelectedItem.ItemID >= 0)
            {
                Vector3Int[] array = IsGet8MapGrid(Vector3Int.RoundToInt(Player.transform.position));

                if (array.Contains(pos))
                {
                    if (SelectedItem is Tool)
                    {
                        var tool = SelectedItem as Tool;
                        tool.UseFunc(new object[] { IdWithMessage[SelectedItem.ItemID], pos });
                    }
                }
            }
            else if (SelectedItem.Type == ItemType.Seed)
            {
                if (BagList[selectID].Quantity <= 0)
                {
                    var item = BagList[selectID];
                    BagList.Insert(selectID,new BagItemData(new Item(),1));
                    BagList.Remove(item);
                    selectID = -1;
                    MessageManager.Instance.Dispatch("Item_Panel_Refresh");
                    return;
                }

                Vector3Int playerPos = Map.map.WorldToCell(Player.transform.position);
                Vector3Int[] array = IsGet8MapGrid(playerPos);
                if (array.Contains(pos))
                {
                    Item item = new Item();

                    if (MapManager.Instance.PlantSeed(pos, SelectedItem))
                    {
                        BagList[selectID].Quantity--;
                    }
                }
            }
        }
    }

    /*void UseT()
    {
        if (Input.GetMouseButtonUp(1)&& SelectedItem != null)
        {
            Vector3 mousepos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(999);
            Vector3Int pos = Map.map.WorldToCell(mousepos);
            
            pos.z = 0;
            
            if (SelectedItem.Type == ItemType.tool && SelectedItem.ItemID >=0)
            {
                Vector3Int[] array = IsGet8MapGrid(Vector3Int.RoundToInt(Player.transform.position));
                
                if (array.Contains(pos))
                {
                    SelectedItem.Tool.UseFunc(new object[] {IdWithMessage[SelectedItem.ItemID], pos });
                }
            }
            else if (SelectedItem.Type == ItemType.seed)
            {
                Vector3Int[] array = IsGet8MapGrid(Vector3Int.RoundToInt(Player.transform.position));
                if (array.Contains(pos))
                {
                    Item item = new Item();
                    
                    MapManager.Instance.PlantSeed(pos,SelectedItem);
                }
            }
        }
    }*/

    public void HoeFunc(object[] objects)
    {  
        
        Vector3Int pos = (Vector3Int)objects[0];
        
        MapManager.Instance.ReclaimedGrid(pos);
    }

    private void ControlFunc()
    {
        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis > 0)
        {
            MessageManager.Instance.Dispatch("Mouse_Middle_Move", new object[] { true });
        }
        else if (axis < 0)
        {
            MessageManager.Instance.Dispatch("Mouse_Middle_Move", new object[] { false });
        }
    }

    private Vector3Int[] IsGet8MapGrid(Vector3Int targetPos)
    {
        Vector3Int[] posArray = new Vector3Int[9];
        
        posArray[0] = new Vector3Int(targetPos.x - 1, targetPos.y + 1, targetPos.z);
        posArray[1] = new Vector3Int(targetPos.x, targetPos.y + 1, targetPos.z);
        posArray[2] = new Vector3Int(targetPos.x + 1, targetPos.y + 1, targetPos.z);
        posArray[3] = new Vector3Int(targetPos.x - 1, targetPos.y, targetPos.z);
        posArray[4] = new Vector3Int(targetPos.x + 1, targetPos.y, targetPos.z);
        posArray[5] = new Vector3Int(targetPos.x - 1, targetPos.y - 1, targetPos.z);
        posArray[6] = new Vector3Int(targetPos.x, targetPos.y - 1, targetPos.z);
        posArray[7] = new Vector3Int(targetPos.x + 1, targetPos.y - 1, targetPos.z);
        posArray[8] = targetPos;

        return posArray;
    }

    private IEnumerator TimeUpdata(int timeScale,int passTime = 60)
    {
        while (true)
        {
            MessageManager.Instance.Dispatch("TimePass");
            yield return new WaitForSeconds(passTime);
            timeMana.TimePass(timeScale);    
        }
    }
}

public class TimeMana
{
    public int Year = 1;
    public int Month = 1;
    public int Day = 1;
    public int Hour = 0;
    public int Min = 0;

    public TimeMana()
    {
        
    }
    public TimeMana(int year, int month, int day, int hour, int min)
    {
        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Min = min;
    }

    public void TimePass(int minClip)
    {
        Min += minClip;  // 增加分钟

        var hourPass = Min / 60;
        Min %= 60;

        Hour += hourPass;
        var dayPass = Hour / 24;
        Hour %= 24;

        Day += dayPass;
        var monthPass = Day / 30;
        Day %= 30;
        
        Month += monthPass;
        var yearPass = Month / 4;
        Month %= 4;

        Year += yearPass;

        if (Day == 0)
        {
            Day = 1;
        }
        if (Month == 0)
        {
            Month = 1;
        }
        if (Year == 0)
        {
            Year = 1;
        }
    }
}

public class JsonSave
{
    //背包信息记录
    public List<BagItemData> Datas = new List<BagItemData>();
    //地图信息记录
    public List<GridInfo> MapGridInfos = new List<GridInfo>();
    //当前时间
    public TimeMana time = new TimeMana(); 
    public JsonSave()
    {
        time = GameManager.Instance.timeMana;
        Datas = GameManager.Instance.BagList;
        MapGridInfos = GameManager.Instance.MapGridInfos;
    }
    
    public void SaveDates()
    {
        VecJsonConverter converter = new VecJsonConverter();
        converter.RegisterNewType();
        string jsonString = JsonMapper.ToJson(this);
        
        if(File.Exists("Save"))
            File.Create("Save").Close();
        File.WriteAllText("savefile.json", jsonString);
    }

    public JsonSave LoadDates()
    {
        VecJsonConverter converter = new VecJsonConverter();
        converter.RegisterNewType();
        
        if(!File.Exists("Save"))
            return null;
        string saveString = File.ReadAllText("savefile.json");
        JsonSave jsonSave = JsonMapper.ToObject<JsonSave>(saveString);

        return jsonSave;
    }
}

public class BagItemData
{
    public Item Item = null;
    public int Quantity = 0;

    public BagItemData()
    {
        
    }

    public BagItemData(Item item, int quantity)
    {
        this.Item = item;
        this.Quantity = quantity;
    }
}


