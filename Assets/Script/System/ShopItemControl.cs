using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.POIFS.FileSystem;
using UnityEngine;
using UnityEngine.UI;

public interface ShopFunc
{
    public void Buy(){}
}
public class ShopItemControl : MonoBehaviour
{
    public Item item;
    public Image icon;
    public Text itemName;
    public Text intro;
    public Text price;
    public InputField inputNum;
    public Button addButton;
    public Button subButton;

    private void Start()
    {
        inputNum.contentType = InputField.ContentType.DecimalNumber;
        inputNum.onEndEdit.AddListener(OnLimitNum);
        addButton.onClick.AddListener(OnAddButtonClick);
        subButton.onClick.AddListener(OnSubButtonClick);
    }

    void OnAddButtonClick()
    {
        var value = int.Parse(inputNum.text);
        value += 1;
        inputNum.text = value.ToString();
        
        MessageManager.Instance.Dispatch("UpdateShopCar",new object[]{item,int.Parse(inputNum.text)});
    }

    void OnSubButtonClick()
    {
        var value = int.Parse(inputNum.text);
        
        if(value <= 0)
            return;
        
        value -= 1;
        inputNum.text = value.ToString();
        
        MessageManager.Instance.Dispatch("UpdateShopCar",new object[]{item,int.Parse(inputNum.text)});
    }
    void OnLimitNum(string value)
    {
        if (value.Length <= 0)
        {
            inputNum.text = "0";
            return;
        }

        if (int.Parse(value) < 0)
        {
            inputNum.text = "0";
        }
        
        MessageManager.Instance.Dispatch("UpdateShopCar",new object[]{item,int.Parse(inputNum.text)});
    }

    public void SetItem(Item itemBase)
    {
        item = item;
    }
    
    public void Reload()
    {
        if (item is not null)
        {
            icon.sprite = item.Icon;
            intro.text = item.Description;
            price.text = item.Price.ToString()+"￥";
            itemName.text = item.Name;
        }
    }
}
