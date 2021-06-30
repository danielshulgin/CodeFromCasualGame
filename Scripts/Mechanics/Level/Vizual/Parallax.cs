using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Range(0f, 1f)] public float parallaxEffect = 0.5f;

    private float _length;
    
    private float _startPosition;

    private void Start()
    {
        _startPosition = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        var distanceFromNewPositionToCamera = Camera.main.transform.position.x * (1 - parallaxEffect);
        var parallaxOffset = Camera.main.transform.position.x * parallaxEffect;
        transform.position = new Vector3(_startPosition + parallaxOffset, transform.position.y, transform.position.z);

        if (distanceFromNewPositionToCamera > _startPosition + _length)
        {
            _startPosition += _length;
        }
        else if(distanceFromNewPositionToCamera < _startPosition - _length)
        {
            _startPosition -= _length;
        }
    }
}
