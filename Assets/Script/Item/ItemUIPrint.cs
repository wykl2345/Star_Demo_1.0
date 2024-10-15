using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUIPrint : MonoBehaviour
{
    private Item _item;

    public Item MyItem
    {
        get
        {
            if (_item is null)
            {
                _item = new Item();
            }

            return _item;
        }
        set
        {
            _item = value;
        }
    }
    
}
