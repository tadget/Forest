namespace Tadget
{
	using UnityEngine;

    [DisallowMultipleComponent]
    public class TileData : MonoBehaviour 
	{
        public int id { get; private set; }
        public int local_chunk_id  { get; private set; }
		public int chunk_id  { get; private set; }
		public Vector3Int chunk_coord { get; private set; }

        public TileData Init(int id, int local_chunk_id, int chunk_id, Vector3Int chunk_coord)
        {
            this.id = id;
            this.local_chunk_id = local_chunk_id;
            this.chunk_id = chunk_id;
            this.chunk_coord = chunk_coord;
            return this;
        }
	}
}
