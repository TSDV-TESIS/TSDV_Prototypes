using UnityEngine;

namespace Player
{
    public class PlayerVfxController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem walkingParticles;
        [SerializeField] private ParticleSystem wallrideParticles;
        [SerializeField] private float wallrideParticleAngle = 69f;
        
        private float _lastSign;
        private Vector3 _lastWallridePosition;
        public void OnWalking(float sign)
        {
            if (walkingParticles.isStopped || !Mathf.Approximately(_lastSign, sign))
            {
                walkingParticles.Stop();
                walkingParticles.transform.eulerAngles = new Vector3(0, sign > 0 ? 270 : 90, 0);
                walkingParticles.Play();
                _lastSign = sign;
            }
        }

        public void OnWalkingStop()
        {
            if (walkingParticles.isPlaying)
            {
                walkingParticles.Stop();
            }
        }

        public void OnWallrideStart(Vector3 position, int sign)
        {
            if (wallrideParticles.isStopped || position != _lastWallridePosition)
            {
                wallrideParticles.Stop();
                wallrideParticles.transform.position = position;
                wallrideParticles.transform.eulerAngles =
                    new Vector3(sign > 0 ? wallrideParticleAngle * 2 : wallrideParticleAngle, 90, 0);
                wallrideParticles.Play();
                _lastWallridePosition = position;
            }
        }

        public void OnWallrideStop()
        {
            if (wallrideParticles.isPlaying)
            {
                wallrideParticles.Stop();
            }
        }
    }
}
