using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {
    
    [Header("Raycast")]
    public float raycastLength;
    public int raycastCount;
    public float raycastAngle;
    public float raycastSlope;
    
    [Header("Dependency")]
    public SisyphusAgent agent;
    
    // Shared variable
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public List<float> depthMap;
    [HideInInspector] public float handControl;

    // Ball states
    private bool _isPushed;
    
    public void ResetBall() {
        transform.position = agent.transform.position + Vector3.forward * 3;
        transform.rotation = Random.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        _isPushed = false;
        handControl = 0f;
        depthMap = new List<float>(raycastCount);
        for (int i = 0; i < raycastCount; i++) {
            depthMap.Add(raycastLength / raycastLength);
        }
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();

        Physics.gravity = new Vector3(0, -20, 0);
    }

    private void Update() {
        float theta = raycastAngle / (raycastCount - 1);

        for (int i = 0; i < raycastCount; i++) {
            Vector3 ray = Quaternion.Euler(-raycastSlope, 0, 0) * Quaternion.Euler(0, theta * i, 0) * Vector3.left;

            if (Physics.Raycast(transform.position, ray, out RaycastHit hit, raycastLength)) {
                depthMap[i] = hit.distance / raycastLength;
                Debug.DrawRay(transform.position, ray * hit.distance, Color.green);
            } else {
                depthMap[i] = raycastLength / raycastLength;
                Debug.DrawRay(transform.position, ray * raycastLength, Color.green);
            }
        }
    }

    private void FixedUpdate() {
        if (!_isPushed) {
            return;
        }

        // Add force to agent for stable control.
        Vector3 playerDirection = (agent.transform.position - transform.position).normalized;
        playerDirection.y = playerDirection.z = 0;
        rb.AddForce(.1f * playerDirection);

        // Hand control 
        rb.AddForce(.4f * handControl * Vector3.right);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            _isPushed = true;
            // Modify mass for realistic control 
            rb.mass = .1f;
        }
        
        if (collision.gameObject.CompareTag("ObstacleFixedWall") 
         || collision.gameObject.CompareTag("ObstacleBarrel") 
         || collision.gameObject.CompareTag("ObstacleFence")
         || collision.gameObject.CompareTag("ObstacleMovingWall")
         || collision.gameObject.CompareTag("ObstaclePenguin")) {
            // Collision penalty
            // R_collision = -w2 or 0
            agent.EndEpisodeFail();
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            _isPushed = false;
            rb.mass = 1f;
        }
    }
}

