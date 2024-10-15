using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TurnTemaprent : MonoBehaviour
{
    public Tilemap ColliderMap;
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Acac");
        if (col.gameObject.tag.Equals("Player"))
        {
            var contact = col.GetContact(0);
            Vector3 pos = contact.point;
            Debug.Log(pos);
            Vector3Int cellpos = ColliderMap.WorldToCell(pos);
            Debug.Log(cellpos);
            var tile = ColliderMap.GetTile(cellpos);
            Debug.Log(tile is null);
            ColliderMap.SetColor(cellpos,new Color(1,1,1,.5f));
            
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        /*if (col.gameObject.tag.Equals("Player"))
        {
            ContactPoint2D contact = col.contacts[0];
            Vector3 pos = contact.point;
            Vector3Int cellpos = ColliderMap.WorldToCell(pos);
            ColliderMap.SetColor(cellpos,new Color(1,1,1,1));
            
        }*/
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
