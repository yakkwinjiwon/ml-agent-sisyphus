using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Vector3 offset;
    public float followSpeed = 10f;

    [HideInInspector] public List<Transform> targets;
    private int targetIndex = 0;

    private void Awake() {
        targets = new List<Transform>();
    }

    private void Update() {
        for(int i = 0; i < targets.Count; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                targetIndex = i;
            }
        }
    }

    void FixedUpdate() {
        FollowTarget();
    }

    void FollowTarget() {
        Vector3 targetPosition = targets[targetIndex].position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
