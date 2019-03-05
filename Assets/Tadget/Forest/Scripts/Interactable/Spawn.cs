namespace Tadget
{
    using UnityEngine;
    using System.Collections;

    public class Spawn : Actions
    {
        public GameObject objToSpawn;
        public Transform whereToSpawn;

        public GameObject spawnParticle;

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

            Instantiate(objToSpawn, whereToSpawn.position, whereToSpawn.rotation);
        }
    }
}
