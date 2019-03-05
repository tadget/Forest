namespace Tadget
{
    using UnityEngine;
    using System.Collections;

    public class Spawn : Actions
    {
        public GameObject objToSpawn;
        public Transform whereToSpawn;

        public GameObject spawnParticle;

        // Set to 'true' if you want the objects to randomly be spawned
        public bool randomlySpawn;
        // If randomlySpawn is set to 'true' then give the radius of the square
        public float radiusOfSquare;

        /// Spawn objects depending on the field "objToSpawn"
        public override void Use()
        {
            // Particle effects are not required
            if (spawnParticle != null)
            {
                // Unparent the spawn particle
                spawnParticle.transform.parent = null;

                ParticleSystem particleSystem = spawnParticle.GetComponent<ParticleSystem>();
                particleSystem.Play();
            }

            if (!randomlySpawn)
            {
                Instantiate(objToSpawn, whereToSpawn.position, whereToSpawn.rotation);
            }
            // If spawning randomly
            else
            {
                Instantiate(objToSpawn, 
                    whereToSpawn.position + new Vector3(Random.Range(radiusOfSquare, -radiusOfSquare), 0, Random.Range(radiusOfSquare, -radiusOfSquare)), 
                    new Quaternion(Random.Range(0,180), Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
            }
        }
    }
}
