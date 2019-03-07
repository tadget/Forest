namespace Tadget.Map
{
	using UnityEngine;

    [DisallowMultipleComponent]
    public class TileData : MonoBehaviour 
	{
        public int id { get; private set; }
        public int local_chunk_id  { get; private set; }
		public Chunk chunk { get; private set; }

        public TileData Init(int id, int local_chunk_id, Chunk chunk)
        {
            this.id = id;
            this.local_chunk_id = local_chunk_id;
            this.chunk = chunk;
            return this;
        }
	}
}
