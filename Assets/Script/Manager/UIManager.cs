using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script.Tool;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject bagListUI;
    public GameObject shopUI;
    public GameObject dialogUI;
    #region BagSys
    public GameObject bagUI;
    public GameObject bagContent;
    public Text itemIntro;
    public Image itemIcon;
    public int selectID = -1;
    #endregion
    
    
    
    public GameObject itemStoreUI;
    public GameObject selectedIma;
    public Button saveButton;
    public Button loadButton;
    public Text timePrint;
    
    private Transform itemUItrans;
    public List<Item> itemList = new List<Item>();
    private void Awake()
    {
        MessageManager.Instance.Subscribe("TimePass",OnTimePass);
        MessageManager.Instance.Subscribe("Item_Panel_Refresh",ReFreshPanel);
        MessageManager.Instance.Subscribe("Mouse_Middle_Move",SelectMove);
        
        MessageManager.Instance.Subscribe("Open_Shop",OpenShop);
        MessageManager.Instance.Subscribe("Close_Shop",CloseShop);
        
        MessageManager.Instance.Subscribe("Open_Bag",OpenBag);
        
        MessageManager.Instance.Subscribe("Open_Dialog",OpenDialog);
        MessageManager.Instance.Subscribe("Close_Dialog",CloseDialog);
        
        
        MessageManager.Instance.Subscribe("Lift_Click_Item",SelectItem);
        MessageManager.Instance.Subscribe("Right_Click_Item",ExchangeItem);
    }


    public void CloseDialog(object[] args)
    {
        dialogUI.SetActive(false);
        //bagListUI.SetActive(true);
    }

    public void CloseShop(object[] args)
    {
        shopUI.SetActive(false);
        bagListUI.SetActive(true);
        dialogUI.SetActive(false);
    }
    
    public void OpenDialog(object[] args)
    {
        dialogUI.SetActive(true);
        
        
    }
    public void OpenShop(object[] args)
    {
        if (shopUI.activeInHierarchy)
        {
            return;
        }
        shopUI.SetActive(true);
        bagListUI.SetActive(false);
        MessageManager.Instance.Dispatch("CreateDiaButton_Close_Shop");
    }

    public void OpenBag(object[] args)
    {
        if (bagUI.activeInHierarchy)
        {
            return;
        }
        bagUI.SetActive(true);
        bagListUI.SetActive(true);
    }
    
    public void Start()
    {
        itemUItrans = itemStoreUI.transform;
        selectedIma.transform.position = itemUItrans.GetChild(0).position;
        if (itemList.Count == 0)
        {
            for (int i = 0; i < itemUItrans.childCount; i++)
            {
                itemList.Add(itemUItrans.GetChild(i).GetComponent<ItemUIPrint>().MyItem);
            }
        }
        //saveButton.onClick.AddListener(Save);
        //loadButton.onClick.AddListener(Load);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.B)&&bagUI.activeInHierarchy){
            bagUI.SetActive(false);
        }else if (Input.GetKeyUp(KeyCode.B) && !bagUI.activeInHierarchy)
        {
            bagUI.SetActive(true);
            BagInit();
        }
    }

    public void Save()
    {
        JsonSave save = new JsonSave();
        save.SaveDates();
    }
    
    public void Load()
    {
        JsonSave load = new JsonSave();
        load.LoadDates();
    }

    public void OnTimePass(object[] args)
    {
        timePrint.text = $"{GameManager.Instance.timeMana.Hour}:{GameManager.Instance.timeMana.Min}";
    }
    
    public void ReFreshPanel(object[] args)
    {
        
        ReDraw();
        if (GameManager.Instance.SelectedItem is null)
        {
            selectedIma.transform.position = itemStoreUI.transform.GetChild(0).position;
            GameManager.Instance.SelectedItem = itemList[0];
        }
        else if(itemList.Contains(GameManager.Instance.SelectedItem))
        {
            var index = itemList.IndexOf(GameManager.Instance.SelectedItem);
            selectedIma.transform.position = itemUItrans.GetChild(index).position;
        }
        
        
    }

    public void ReDraw()
    {
        var count = GameManager.Instance.BagList.Count > 7 ? 7 : GameManager.Instance.BagList.Count;
        for (int i = 0; i < count; i++)
        {
            itemList[i] = GameManager.Instance.BagList[i].Item;
            if (itemList[i].ItemID == -1)
            {
                var item = itemUItrans.GetChild(i);
                Image icon = item.GetComponent<Image>();
                Text quaText = item.GetComponentInChildren<Text>();
                icon.sprite = itemList[i].Icon;
                quaText.text = "";
            }
            else
            {
                var item = itemUItrans.GetChild(i);
                Image icon = item.GetComponent<Image>();
                Text quaText = item.GetComponentInChildren<Text>();
                if(itemList[i].Icon is not null)
                    icon.sprite = itemList[i].Icon;
                quaText.text = GameManager.Instance.BagList[i].Quantity.ToString();
            }
        }
    }
    public void OnDestroy()
    {
        MessageManager.Instance.Unsubscribe("Item_Panel_Refresh");
        MessageManager.Instance.Unsubscribe("Mouse_Middle_Move");
        MessageManager.Instance.Unsubscribe("TimePass");
    }

    public void SelectMove(object[] args)
    {
        for (int i = 0; i < 7; i++)
        {
            itemList[i] = GameManager.Instance.BagList[i].Item;
        }
        ReFreshPanel(null);
        bool upOrDown = (bool)args[0];
        if (upOrDown)
        {
            var index = itemList.IndexOf(GameManager.Instance.SelectedItem);
            index = (index - 1 + 7) % 7;
            selectedIma.transform.position = itemUItrans.GetChild(index).position;
            GameManager.Instance.SelectedItem = itemList[index];
        }
        else
        {
            var index = itemList.IndexOf(GameManager.Instance.SelectedItem);
            index = (index + 1 + 7) % 7;
            selectedIma.transform.position = itemUItrans.GetChild(index).position;
            GameManager.Instance.SelectedItem = itemList[index];
        }
    }

    public void BagInit()
    {
        ToolsHelp.RemoveChildren(bagContent);
        
        var list = GameManager.Instance.BagList;
        
        for (int i = 0; i < list.Count; i++)
        {
            GameObject prefabs = Instantiate(itemPrefab, bagContent.transform, true);
            var uiBase = prefabs.GetComponent<ItemUIBase>();
            uiBase.SetID(i);
            uiBase.ReLoad(list[i].Quantity.ToString(),list[i].Item.Icon);
        }
    }

    public void PrintItemInfo(Sprite icon, string info)
    {
        itemIcon.sprite = icon;
        itemIntro.text = info;
    }

    public void SelectItem(object[] args)
    {
        selectID = int.Parse(args[0].ToString());
        if (selectID < 0 || selectID >= GameManager.Instance.BagList.Count)
            return;
        var item = GameManager.Instance.BagList[selectID];
        PrintItemInfo(item.Item.Icon,item.Item.Description);
    }

    public void ExchangeItem(object[] args)
    {
        var otherId = int.Parse(args[0].ToString());
        var list = GameManager.Instance.BagList;
        Debug.Log(selectID);
        if (selectID < 0 || selectID >= GameManager.Instance.BagList.Count)
            return;
        if (otherId < 0 || otherId >= list.Count)
            return;
        (list[selectID], list[otherId]) 
            = (list[otherId], list[selectID]);
        var trans1 = bagContent.transform.GetChild(selectID);
        var trans2 = bagContent.transform.GetChild(otherId);
        var item1 = trans1.GetComponent<ItemUIBase>();
        var item2 = trans2.GetComponent<ItemUIBase>();
        item1.gridID = otherId;
        item2.gridID = selectID;
        trans2.SetSiblingIndex(selectID);
        trans1.SetSiblingIndex(otherId);
        selectID = -1;
    }
}
