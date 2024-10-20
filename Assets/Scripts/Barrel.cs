using UnityEngine;

public class Barrel : MonoBehaviour {
    public float explosionForce = 5f; // Force of the explosion
    public float explosionRadius = 5f;   // Radius of the explosion
    public float upwardModifier = 1f;    // Modifier to control the upward force
    public GameObject explosionPrefab;
    public AudioClip explosionSound;
    public float explosionVolume = 0.5f;

    public SisyphusAgent agent;

    void OnCollisionEnter(Collision collision) {
        // Check if the colliding object has the tag 'Ball'
        if (collision.gameObject.CompareTag("Ball")) {
            //agent.AddReward(-10f);
            //agent.EndEpisode();
            //Explode();
        }
    }

    void Explode() {
        // Get all colliders in the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        if (explosionSound != null) {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);
        }

        foreach (Collider nearbyObject in colliders) {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null) {
                // Calculate upward force direction
                Vector3 explosionDirection = Vector3.up;
                Debug.Log("Boom");

                // Add upward force to the rigidbody
                rb.AddForce(explosionDirection * explosionForce * upwardModifier, ForceMode.Impulse);
            }
        }
        if (explosionPrefab != null) {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Instantiate(explosionSound, transform.position, Quaternion.identity);
        }
        // Destroy the cube's GameObject after the explosion
        Destroy(gameObject);
    }

    // Optionally, draw the explosion radius in the editor for visualization
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
