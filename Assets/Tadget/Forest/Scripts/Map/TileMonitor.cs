namespace Tadget
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
		private TileID currentTileID;

		public delegate void TileEvent<TileID>(TileID tileID);
		public event TileEvent<TileID> OnTileEnter;

		public void Update () 
		{
			RaycastDown();
		}

		private void RaycastDown()
		{
			if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
			{
				Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.yellow);
				
				if(hit.transform.gameObject != currentTile)
				{
					currentTileID = hit.transform.GetComponent<TileID>();
					currentTile = currentTileID.gameObject;
					if(currentTileID == null)
					{
						tileDisplay = "Standing on object which has no TileID";
						Debug.LogWarningFormat("Hit {0} which has no TileID attached", currentTile);
					}
					else
					{
						tileDisplay = string.Format("{0}\n{1}", currentTileID.id, currentTileID.chunk_coord);
						OnTileEnter.Invoke(currentTileID);
					}	
				}
			}
		}
	}
}
