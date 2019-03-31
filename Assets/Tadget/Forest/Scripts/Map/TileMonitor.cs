namespace Tadget.Map
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class TileMonitor : MonoBehaviour {

        public LayerMask layerMask;
        public string tileDisplay;
        private GameObject currentTile;
        private RaycastHit hit;
        private TileData currentTileData;
        private Vector3Int lastChunkCoord = new Vector3Int(int.MaxValue,int.MaxValue,int.MaxValue);

        public delegate void TileEvent<TileID>(TileID tileID);
        public event TileEvent<TileData> OnTileEnter;
        public event TileEvent<TileData> OnChunkEnter;

        public void FixedUpdate ()
        {
            RaycastDown();
        }

        private void RaycastDown()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);

                if(hit.transform.gameObject != currentTile)
                {
                    currentTileData = hit.transform.GetComponentInParent<TileData>();

                    if(currentTileData == null)
                    {
                        tileDisplay = "Standing on object which has no TileID";
                        Debug.LogWarningFormat("Hit {0} which has no TileID attached", hit.transform.gameObject);
                    }
                    else
                    {
                        currentTile = currentTileData.gameObject;
                        tileDisplay = string.Format("{0}\n{1}", currentTileData.id, currentTileData.chunk.coord);
                        if(OnTileEnter != null)
                            OnTileEnter.Invoke(currentTileData);
                        if (lastChunkCoord != currentTileData.chunk.coord)
                        {
                            if (OnChunkEnter != null)
                                OnChunkEnter.Invoke(currentTileData);
                            lastChunkCoord = currentTileData.chunk.coord;
                        }
                    }
                }
            }
        }
    }
}
