using UnityEngine;

public class MovingWallController : MonoBehaviour {
    
    public float speed;
    public float interval;

    private Vector3 _startPos;

    private float _direction;

    private void Awake() {
        _startPos = transform.position;
    }

    private void Update() {
        transform.Translate(speed * _direction * Time.deltaTime * Vector3.forward);

        if (Vector3.Distance(_startPos, transform.position) >= interval) {
            transform.position = _startPos + _direction * interval * Vector3.right;
            _direction *= -1;
        }
    }

    public void ResetMovingWall() {
        transform.position = _startPos + Vector3.left * Random.Range(-interval, interval);
        _direction = Random.Range(0, 2) == 1 ? 1f : -1f;
    }
}