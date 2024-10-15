using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SQLite;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

//using System.Data.sq;

public class SqlManager : MonoBehaviour
{
    private const string SqlStore = "DataBase"; 
    private SQLiteConnection _mdbConnection;

    public List<ItemType> itemPool = new List<ItemType>(); 
    // Start is called before the first frame update

    void OnLoaded(AsyncOperationHandle<DefaultAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            if (handle.Result is not null)
            {
                _mdbConnection = new SQLiteConnection("Data Source = " + AssetDatabase.GetAssetPath(handle.Result));
                
                _mdbConnection.Open();
            }
        }

        LoadData();
    }

    SQLiteDataReader LoadData()
    {
        string sql = "select * from item";
        SQLiteCommand command = new SQLiteCommand(sql, _mdbConnection);
        SQLiteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            
            Item baseInfo = new Item();
            
            Debug.Log(reader["item_id"] +" name:"+ reader["name"]);
            
            baseInfo.ItemID = int.Parse(reader["item_id"].ToString().Trim());
            baseInfo.Name = reader["name"].ToString();
            baseInfo.Description = reader["description"].ToString();
            baseInfo.IconsName = reader["image_name"].ToString();
            baseInfo.Type = (ItemType)Enum.Parse(typeof(ItemType), reader["item_type"].ToString());
            baseInfo.Price = int.Parse(reader["price"].ToString().Trim());

            if (reader["type"].Equals(ItemType.Tool.ToString()))
            {
                //Tool toolItem = new Tool();
                
            }else if (reader["item_type"].Equals(ItemType.Seed.ToString()))
            {
                //Seed seedItem = new Seed();
                Seed seed = new Seed(baseInfo);
                seed.Drop_Item_ID = int.Parse(reader["drop_item_id"].ToString().Trim());
                seed.Drop_Amount = int.Parse(reader["drop_amount"].ToString().Trim());
                seed.Step_Images = reader["step_images"].ToString().Split(";");
                //图片自动加载其他模块功能——根据图片名称查找图片资源功能——获取内容
                seed.Best_Season = (Season)Enum.Parse(typeof(Season), reader["best_season"].ToString());
                var gspeed = reader["grow_speed"].ToString().Trim().Split(";");
                seed.GrowSpeed[0] = float.Parse(gspeed[0]);
                seed.GrowSpeed[1] = float.Parse(gspeed[1]);
                seed.Steps = Array.ConvertAll<string, int>(reader["stage_threshold"].ToString().Trim().Split(";"),
                    int.Parse);
                seed.HarvestTime = int.Parse(reader["growth_time"].ToString().Trim());
                seed.Return_stage = int.Parse(reader["return_stage"].ToString().Trim());
                //生成完成后的保留时间，默认是3天
                seed.Retention_time = int.Parse(reader["retention_time"].ToString().Trim());
                
                //非默认属性要求
                seed.SunnyDayGRate = int.Parse(reader["sunny_growth_rate"].ToString().Trim());
                //itemPool.Add(seed);
            }else if (reader["item_type"].Equals(ItemType.Consumables.ToString()))
            {
                
            }
            
        }
            
            
        return null;
    }
    
    void Start()
    {
        Addressables.LoadAssetAsync<DefaultAsset>(SqlStore).Completed += OnLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
