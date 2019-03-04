namespace Tadget
{
    using UnityEngine;

    public class Spawn : Actions
    {
        public GameObject objToSpawn;
        public Transform whereToSpawn;

        public ParticleSystem spawnParticle;

        /// Spawn objects depending on the field "objToSpawn"
        public override void Use()
        {
            Instantiate(objToSpawn, whereToSpawn.position, whereToSpawn.rotation);

            // Particle effects are not required
            if (spawnParticle != null)
            {
                spawnParticle.Play();
            }
        }
    }
}
