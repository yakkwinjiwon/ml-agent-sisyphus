using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour {

    public GameObject agentPrefab;

    public int _numberOfAgents_x5;
    public int offsetX;
    public int offsetZ;

    // Dependency
    private CameraFollow cameraFollow;

    public void Awake() {
        cameraFollow = GetComponent<CameraFollow>();
    }

    public void Start() {
        for (int i = 0; i < _numberOfAgents_x5; i++) {
            for (int j = 0; j < 5; j++) {
                GameObject agent = Instantiate(agentPrefab, new Vector3(j * offsetX, 0, i * offsetZ), Quaternion.identity);
                cameraFollow.targets.Add(agent.transform.GetChild(0));
            }
        }
    }
}
