namespace Tadget
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class LayoutTile {

		public MapTile mapTile;
		public Vector3 gridPosition;
		public Vector3 worldPosition;

		public LayoutTile(MapTile mapTile, Vector3 gridPosition, Vector3 worldPosition)
		{
			this.mapTile = mapTile;
			this.gridPosition = gridPosition;
			this.worldPosition = worldPosition;
		}
	}
}
