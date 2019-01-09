﻿namespace Tadget
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class TileMonitor : MonoBehaviour {

		public LayerMask layerMask;
		public string tileDisplay;
		private GameObject currentTile;

		private void OnValidate() 
		{
			Debug.AssertFormat(tileDisplay != null, "Missing Text for tileDisplay");	
		}

		public void Update () 
		{
			RaycastDown();
		}

		private void RaycastDown()
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
			{
				Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
				
				if(hit.transform.gameObject != currentTile)
				{
					TileID tileID = hit.transform.GetComponentInParent<TileID>();
					currentTile = tileID.gameObject;
					if(tileID == null)
					{
						tileDisplay = "Standing on object which has no TileID";
						Debug.LogWarningFormat("Hit {0} which has no TileID attached", currentTile);
					}
					else
					{
						tileDisplay = string.Format("{0}\n{1}", currentTile.name, tileID.gridPosition.ToString());
					}	
				}
			}
		}
	}
}
