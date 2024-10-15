using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item
{
    public int ItemID;
    public string Name;
    public string Description;
    public string IconsName;
    public ItemType Type;
    public Sprite Icon;
    public int Price;
    
    public Item()
    {
        this.ItemID = -1;
        this.Name = "";
        this.Description = "";
        this.Type = ItemType.Tool;
        this.Icon = ResManager.Instance.ItemResource["Init"];
    }
    public Item(int itemBaseID)
    {
        this.ItemID = itemBaseID;
    }

    
    
    public virtual void UseItem()
    {
        return;
    }
    
    public virtual Item DeepCopy()
    {
        Item copy = new Item();

        copy.ItemID = this.ItemID;
        copy.Name = this.Name;
        copy.Description = this.Description;
        copy.IconsName = this.IconsName;
        copy.Type = this.Type;

        if (this.Icon is not null)
        {
            copy.Icon = this.Icon;
        }

        return copy;
    }
}

public enum ItemType
{
    Seed = 0,
    Tool = 1,
    Consumables = 2,
    
}

public enum Season
{
    Spring = 1,
    Summer = 2,
    Autumn = 4,
    Winter = 8,
}

public enum Magic{
    None = -1,
    地 = 0,
    火 = 1,
    风 = 2,
    土 = 3
}



public class Seed :Item
{
    //数据库配置的属性
    public int Drop_Item_ID;
    public string[] Step_Images = new string[]{};
    public Sprite[] Step_Sprite = new Sprite[]{};
    public int Drop_Amount;
    public Season Best_Season;
    public float[] GrowSpeed = new float[2]{0,0};
    public int[] Steps = new int[]{};
    public int HarvestTime;
    public int Return_stage;
    public int Retention_time;
    //额外的添加的非植物默认的属性
    public float SunnyDayGRate;
    //运行时需要的属性
    public int[] magic = new int[4];
    public int[,] GrowTime;
    public Magic magiccal;
    public int NowStep = 0;
    
    

    public override void UseItem()
    {
        
    }

    public Seed(Item item) : base(item.ItemID)
    {
        // 继承父类的构造函数，传递 ItemID 参数
        this.Name = item.Name;
        this.Description = item.Description;
        this.IconsName = item.IconsName;
        this.Type = item.Type;
        this.Price = item.Price;
    }
    
    public Seed(float[] growSpeed,int[] steps, int Drop_Amount, int price, Season seasonType)
    {
        GrowSpeed[0] = growSpeed[0];
        GrowSpeed[1] = growSpeed[1];
        
        Steps = steps;
        this.Drop_Amount = Drop_Amount;
        Price = price;
        Best_Season = seasonType;
    }
    
    public Seed DeepCopy()
    {
        Seed copy = new Seed(GrowSpeed, Steps, Drop_Amount, Price, Best_Season);
        copy.NowStep = NowStep;
        
        
        copy.magic = new int[magic.Length];
        Array.Copy(magic, copy.magic, magic.Length);
        magiccal = Magic.None;
        
        return copy;
    }
}

public interface IToolFunc
{
    public abstract void UseFunc(object[] objects);
}

public class Tool:Item,IToolFunc
{
    public string FuncName = "";
    
    public void UseFunc(object[] objects)
    {
        string message = (string)objects[0];
        Vector3Int pos = (Vector3Int)objects[1];

        MessageManager.Instance.Dispatch(message,new object[]{pos});
    }

    public new Tool DeepCopy()
    {
        // 创建一个新的 Tool 实例，将原始 Tool 实例的属性复制到新实例中
        Tool copy = new Tool();
        // 如果 Tool 类有其他属性，也要在这里进行复制
        copy.ItemID = this.ItemID;
        copy.Name = this.Name;
        copy.Description = this.Description;
        copy.IconsName = this.IconsName;
        copy.Type = this.Type;
        if (this.Icon is not null)
        {
            copy.Icon = this.Icon;
        }
        copy.FuncName = this.FuncName;
        
        return copy;
    }
}