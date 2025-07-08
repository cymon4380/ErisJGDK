using UnityEngine;

namespace ErisJGDK.Base.UI
{
    public class Credits : MonoBehaviour
    {
        [SerializeField] private float _speed = .6f;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void FixedUpdate()
        {
            _transform.position += new Vector3(0f, Time.fixedDeltaTime * _speed);
        }
    }
}
