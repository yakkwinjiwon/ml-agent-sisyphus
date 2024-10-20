using System.Collections;
using UnityEngine;

public class PenguinController : MonoBehaviour {

    public float speed;
    public float rotationAngle;

    private Vector3 _initDirection;
    private bool _isFlip;
    private bool _flipAvailable;

    private void Awake() {
        _initDirection = -transform.forward;
    }

    public void FixedUpdate() {
        Vector3 move = speed * Time.fixedDeltaTime * Vector3.forward;
        transform.Translate(move, Space.Self);
    }

    // Flip when collisionEnter
    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("ObstaclePenguin") || collision.gameObject.CompareTag("Wall")) {
            if(!_flipAvailable) {
                return;
            }
            _flipAvailable = false;

            StartCoroutine(ResetFlipAvailable());

            if (_isFlip) {
                transform.LookAt(transform.position + Quaternion.Euler(0, rotationAngle, 0) * _initDirection);
            } else {
                transform.LookAt(transform.position + Quaternion.Euler(0, -rotationAngle, 0) * _initDirection);
            }
            _isFlip = !_isFlip;
        } else if (collision.gameObject.CompareTag("Ball")) {
            ResetPenguin();
        }
    }

    private IEnumerator ResetFlipAvailable() {
        yield return new WaitForSeconds(0.2f);
        _flipAvailable = true;
    }

    public void ResetPenguin() {
        _flipAvailable = true;
        _isFlip = Random.Range(0, 2) == 1 ? true : false;
        if (_isFlip) {
            transform.LookAt(transform.position + Quaternion.Euler(0, -rotationAngle, 0) * _initDirection);
        } else {
            transform.LookAt(transform.position + Quaternion.Euler(0, rotationAngle, 0) * _initDirection);
        }
    }
}
