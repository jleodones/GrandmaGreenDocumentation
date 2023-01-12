using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Garden
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/GardenVFX")]
    public class GardenVFX : ScriptableObject
    {

        public ParticleSystem FertilizerParticleBurst;
        public ParticleSystem WateringParticleBurst;
        public ParticleSystem DryingUpBurst;

        static readonly Quaternion PARTICLE_ORIENTATION = Quaternion.Euler(-110, 0, 0);
        static readonly Vector3 PARTICLE_OFFSET = new Vector3(0, 0, -2);

        public void PlayFertilizerParticle(Vector3 position) => CreateParticleSystem(FertilizerParticleBurst, position);
        public void PlayWaterParticle(Vector3 position) => CreateParticleSystem(WateringParticleBurst, position);
        public void PlayDryParticle(Vector3 position) => CreateParticleSystem(DryingUpBurst, position);


        void CreateParticleSystem(ParticleSystem particleSystem, Vector3 position)
        {
            Instantiate(particleSystem, position + PARTICLE_OFFSET, PARTICLE_ORIENTATION);
        }
    }
}
