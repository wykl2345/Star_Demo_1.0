using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapStateManager
{
    public enum TileType
    {
        CantDo,
        Normal,
        Reclaimed,
        Watered,
    }
    
    public class MapManager : MonoBehaviour
    {
        public Tilemap map;
        public Tilemap colliderMap;
        public Tilemap overlayMap;

        private static MapManager _instance;
        private PlayerMove _player;

        public Dictionary<Tile, TileType> TileWithName = new Dictionary<Tile, TileType>();
        public Dictionary<string, Tile> NameWithTiles = new Dictionary<string, Tile>();
        
        public PlayerMove Player
        {
            get
            {
                return _player ??= FindObjectOfType<PlayerMove>();
            }
        }

        private void Awake()
        {
            
        }

        private void Start()
        {
            InitBaseParams();
        }

        private void InitBaseParams()
        {
            Tile[] tileArray = Resources.LoadAll<Tile>("map");
            foreach (var tile in tileArray)
            {
                if (tile.name.Contains(TileType.Normal.ToString()))
                {
                    TileWithName.Add(tile,TileType.Normal);
                }
                else if (tile.name.Contains(TileType.Reclaimed.ToString()))
                {
                    TileWithName.Add(tile,TileType.Reclaimed);
                }
                else if (tile.name.Contains(TileType.Watered.ToString()))
                {
                    TileWithName.Add(tile,TileType.Watered);
                }
            }

            Tile[] itemTileArrat = Resources.LoadAll<Tile>("Sprite/ItemTile");
            foreach (var itemTile in itemTileArrat)
            {
                NameWithTiles.Add(itemTile.name,itemTile);
            }
        }

        public static MapManager Instance
        {
            get
            {
                if (_instance is null)
                {
                    var ins = FindObjectOfType<MapManager>();
                    if (ins is null)
                    {
                        var manager = new GameObject("MapManager").AddComponent<MapManager>();
                    }

                    _instance = ins;
                }

                return _instance;
            }
        }

        public TileBase GetTile(Vector3 pos)
        {
            TileBase ans = null;
            //Debug.Log("Real Position"+pos);
            Vector3Int localPos = map.WorldToCell(pos);
            //Debug.Log("Cell Position"+localPos);
            ans = map.GetTile<TileBase>(localPos);
            
            return ans;
        }
        public bool PlantSeed(Vector3Int pos,Item seed)
        {
            var ans = map.GetTile<Tile>(pos);

            var tileT = TileWithName.GetValueOrDefault(ans);

            if (tileT == TileType.Reclaimed)
            {
                var itemTile = NameWithTiles[seed.IconsName];
                if(itemTile is null) return false;
                
                overlayMap.SetTile(pos,itemTile);
 
                var ishave = GameManager.Instance.MapGridInfos.FirstOrDefault(tar => tar.CellPos == pos);
                if (ishave is null)
                {
                    return false;
                }
                else
                {
                    if (ishave.SItem.Type is ItemType.Seed)
                    {
                        
                        ishave.Plant(seed.DeepCopy());
                        return true;
                    }
                }
            }

            return false;
        }
        public void ReclaimedGrid(Vector3Int pos)
        {
            var ans = map.GetTile<Tile>(pos);

            var tileT = TileWithName.GetValueOrDefault(ans);

            if (tileT == TileType.Normal)
            {
                var tilesType = TileWithName.FirstOrDefault(kv => kv.Value == TileType.Reclaimed).Key;
                map.SetTile(pos,tilesType);
                GridInfo grid = new GridInfo(pos,tilesType);
                GameManager.Instance.MapGridInfos.Add(grid);
            }
        }
    }
    
    public class GridInfo
    {
        public Vector3Int CellPos;
        public Tile LandType;
        public Item SItem;
        public int Step = 0;
        public TimeMana StartTime = new TimeMana();
        public TimeMana NowTime = new TimeMana();
        public int CellPosX => CellPos.x;
        public int CellPosY => CellPos.y;
        public int CellPosZ => CellPos.z;
        public GridInfo(Vector3Int cellPos,Tile type,TimeMana startTime)  
        {
            this.CellPos = cellPos;
            this.LandType = type;
            this.StartTime = startTime;
            this.NowTime = startTime;
        }

        public void Plant(Item seed)
        {
            this.SItem = seed;
        }

        public GridInfo(Vector3Int pos,Tile type)
        {
            this.CellPos = pos;
            this.LandType = type;
        }

        public void Growing()
        {
            int len = 0;
            if (SItem is not Seed)
            {
                return;
            }
            var seed = SItem as Seed;
            
            foreach (var VARIABLE in seed.magic)
            {
                len += VARIABLE;
                if (len >= 100)
                {
                    break;
                }
            }
            Array.Sort(seed.magic);
            seed.magiccal = (Magic)seed.magic[0];
            
            Step++;
            if (Step > seed.Steps.Length)
            {
                return;
            }else
                LandType.sprite = seed.Icon;
            
        }
    }

}
