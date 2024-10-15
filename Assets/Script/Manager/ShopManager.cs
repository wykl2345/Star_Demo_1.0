using System;
using System.Collections;
using System.Collections.Generic;
using Script.Tool;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject shopContent;
    public GameObject shopItem;
    
    public Text totalMoney;

    public Button buy;

    public List<Item> ShopList =new List<Item>();

    public Dictionary<Item, int> ShopCar = new Dictionary<Item, int>();
    // Start is called before the first frame update
    void Start()
    {
        MessageManager.Instance.Subscribe("UpdateShopCar",OnUpdateSale);
        buy.onClick.AddListener(BuyItem);
        if (ShopList.Count <= 0)
        {
            ShopList.Add(ResManager.Instance.GetItem(0));
            ShopList.Add(ResManager.Instance.GetItem(1));
            ShopList.Add(ResManager.Instance.GetItem(2));
            ShopList.Add(ResManager.Instance.GetItem(3));
        }
        ShowShop();
    }

    private void OnEnable()
    {
        ShowShop();
    }

    public void BuyItem()
    {
        var list = GameManager.Instance.BagList;
        
        foreach (var t in ShopCar)
        {
            //Debug.Log(t.Key.Name +";"+t.Value.ToString());
            // 尝试在背包中找到现有的项目
            var existingItem = list.Find(s => s.Item.ItemID == t.Key.ItemID);

            if (existingItem != null)
            {
                // 项目已存在，只需更新数量
                existingItem.Quantity += t.Value;
            }
            else
            {
                // 项目不存在，因此添加新条目
                list.Add(new JsonData(t.Key, t.Value));
            }
        }
        MessageManager.Instance.Dispatch("Item_Panel_Refresh");
    }

    
    public void OnUpdateSale(object[] args)
    {
        Item item = args[0] as Item;
        int count = int.Parse(args[1].ToString());
        if(item is null)
            return;
        if (ShopCar.ContainsKey(item))
        {
            if (count <= 0)
            {
                ShopCar.Remove(item);
            }
            else
            {
                ShopCar[item] = count;    
            }
        }
        else
        {
            ShopCar.Add(item,count);
        }

        int fin = 0;    
        
        foreach (var t in ShopCar)
        {
            fin += t.Value * t.Key.Price;
        }

        totalMoney.text = fin.ToString() + "￥";
    }

    public void AddItem(Item item)
    {
        ShopList.Add(item);
    }
    
    public void ShowShop()
    {
        totalMoney.text = "0";
        ToolsHelp.RemoveChildren(shopContent);
        ShopCar.Clear();
        if (ShopList.Count <= 0)
        {
            return;
        }

        foreach (var t in ShopList)
        {
            var item = Instantiate(shopItem, shopContent.transform);
            var info = item.GetComponent<ShopItemControl>();
            info.SetItem(t);
            info.Reload();
        }
    }
    
}
