using UnityEngine;

public class FenceController : MonoBehaviour {
    
    public float rotationSpeed = 50f;

    public void Update() {
        Vector3 rotateVec = new Vector3(0, rotationSpeed, 0);
        transform.Rotate(rotateVec * Time.deltaTime);
    }
}