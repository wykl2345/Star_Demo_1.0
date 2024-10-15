using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResManager : MonoBehaviour
{
    private static ResManager _instance;
    public static ResManager Instance
    {
        get
        {
            if (_instance is null)
            {
                var target = FindObjectOfType<ResManager>();

                if (target is null)
                {
                    var newMana = new GameObject("ResourceManager").AddComponent<ResManager>();
                    _instance = newMana;
                    DontDestroyOnLoad(newMana.gameObject);
                }

                _instance = target;
            }
            return _instance;
        }
    }

    private const string SpritePath = "Sprite/Item_Sprite/Item";
    public Dictionary<string, Sprite> _itemResource = new Dictionary<string, Sprite>();
    private List<Item> _itemBases = new List<Item>();
    private const string DataPath = "Assets/Resources_moved/Data/DataSet.xlsx";

    private void Awake()
    {
        InitItemsData();
    }

    public Item GetItem(int id)
    {
        foreach (var t in _itemBases)
        {
            if (t.ItemID == id)
            {
                return t;
            }
        }

        return null;
    }
    public List<Item> ItemBases
    {
        get
        {
            if (_itemBases is null)
            {
                _itemBases = new List<Item>();
                InitItemsData();
            }
            return _itemBases;
        }
    }
    public Dictionary<string, Sprite> ItemResource
    {
        get
        {
            if (_itemResource.Count == 0)
            {
                var sprites = Resources.LoadAll<Sprite>(SpritePath);
                
                foreach (var sprite in sprites)
                {
                    _itemResource.Add(sprite.name,sprite);
                }
            }

            return _itemResource;
        }
    }

    void OnCompleted(AsyncOperationHandle<DefaultAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            
            var re = handle.Result;
            
            using (FileStream stream = new FileStream(AssetDatabase.GetAssetPath(re),FileMode.Open,FileAccess.Read))
            {
                // 创建工作簿
                IWorkbook workbook = new XSSFWorkbook(stream);
                // 获取第一个工作表
                ISheet sheet = workbook.GetSheetAt(0);
                
                IRow headerRow = sheet.GetRow(0);
                Dictionary<string, int> columnIndexMap = new Dictionary<string, int>();
                for (int cellIndex = 0; cellIndex < headerRow.LastCellNum; cellIndex++)
                {
                    string columnName = headerRow.GetCell(cellIndex).ToString();
                    
                    columnIndexMap[columnName] = cellIndex;
                }
                
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow dataRow = sheet.GetRow(rowIndex);

                    // 创建 ItemBase 对象
                    Item item = new Item();
                    // 设置属性值
                    item.ItemID = Convert.ToInt32(dataRow.GetCell(0).ToString().Trim());
                    item.Name = dataRow.GetCell(1).ToString().Trim();
                    item.Description = dataRow.GetCell(2).ToString().Trim();
                    item.IconsName ??= dataRow.GetCell(3).ToString().Trim();
                    
                    
                    item.Type = (ItemType)Enum.Parse(typeof(ItemType), dataRow.GetCell(5).ToString().Trim());
                    item.Price = Convert.ToInt32(dataRow.GetCell(10).ToString().Trim());
                    
                    
                    
                    
                    _itemBases.Add(item);
                }
            }
        }
    }
    
    public void InitItemsData()
    {
        Addressables.LoadAssetAsync<DefaultAsset>("DataSet").Completed += OnCompleted;
        //Debug.Log(AssetDatabase.GetAssetPath(excel));
        
        // 创建文件流
        /*using (FileStream stream = new FileStream(DataPath,FileMode.Open,FileAccess.Read))
        {
            // 创建工作簿
            IWorkbook workbook = new XSSFWorkbook(stream);
            // 获取第一个工作表
            ISheet sheet = workbook.GetSheetAt(0);
            
            IRow headerRow = sheet.GetRow(0);
            Dictionary<string, int> columnIndexMap = new Dictionary<string, int>();
            for (int cellIndex = 0; cellIndex < headerRow.LastCellNum; cellIndex++)
            {
                string columnName = headerRow.GetCell(cellIndex).ToString();
                
                columnIndexMap[columnName] = cellIndex;
            }
            
            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow dataRow = sheet.GetRow(rowIndex);

                // 创建 ItemBase 对象
                ItemBase item = new ItemBase();
                // 设置属性值
                item.ItemID = Convert.ToInt32(dataRow.GetCell(0).ToString().Trim());
                item.Name = dataRow.GetCell(1).ToString().Trim();
                item.Intro = dataRow.GetCell(2).ToString().Trim();
                item.IconsName ??= dataRow.GetCell(3).ToString().Trim().Split(';');
                item.GrowTime = Convert.ToInt32(dataRow.GetCell(4).ToString().Trim());
                item.Type = (ItemType)Enum.Parse(typeof(ItemType), dataRow.GetCell(5).ToString().Trim());
                item.Price = Convert.ToInt32(dataRow.GetCell(10).ToString().Trim());
                
                #region ToolSet工具的数据配置
                if (item.Type == ItemType.tool)
                {
                    item.Tool = new Tool();
                    GameManager.Instance.IdWithMessage.Add(item.ItemID,item.Name);
                }
                #endregion

                #region 种子的数据配置
                
                if (item.Type == ItemType.seed)
                {
                    var season = (Season)Enum.Parse(typeof(Season), dataRow.GetCell(6).ToString().Trim());
                    var speed = Convert.ToInt32(dataRow.GetCell(7).ToString().Trim());
                    string[] steps = dataRow.GetCell(8).ToString().Trim().Split(";");
                    var step = new int[steps.Length];
                    for (int i = 0; i < steps.Length; i++)
                    {
                        step[i] = int.Parse(steps[i]);
                    }
                    var qua= Convert.ToInt32(dataRow.GetCell(9).ToString().Trim());
                    var price = Convert.ToInt32(dataRow.GetCell(10).ToString().Trim());
                    item.SeedBase = new SeedInfo(speed, step, qua, price, season);
                }
                
                #endregion

                Sprite[] icons = new Sprite[item.IconsName.Length];
                // 根据名称获取贴图
                for (int i = 0; i < icons.Length; i++)
                {
                    Sprite sprite1;
                    ItemResource.TryGetValue(item.IconsName[i],out sprite1);
                    if (sprite1 is not null)
                    {
                        icons[i] = sprite1;
                    }
                }

                item.Icon = icons;
                _itemBases.Add(item);
            }
        }*/
    }
}
