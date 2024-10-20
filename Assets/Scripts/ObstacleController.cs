using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour {
    
    [Header("Obstacle")]
    public GameObject obstacle;
    public int obstacleCount;
    public float randomRangeX;
    public float offset;
    public float gapMin;
    public float gapMax;
    public float height;

    // Dependency
    private Transform[] _obstacles;
    private Vector3[] _initialPosition;
    private PenguinController[] _penguins;
    private MovingWallController[] _movingWall;

    public void Awake() {
        _obstacles = new Transform[obstacleCount];
        _initialPosition = new Vector3[obstacleCount];
        _penguins = new PenguinController[obstacleCount];
        _movingWall = new MovingWallController[obstacleCount];

        for (int i = 0; i < obstacleCount; i++) {
            GameObject childObject = Instantiate(obstacle, Vector3.zero, transform.rotation, transform);
            _obstacles[i] = childObject.transform;
            _initialPosition[i] = childObject.transform.position;

            PenguinController penguinController = _obstacles[i].GetComponent<PenguinController>();
            if(penguinController != null) {
                _penguins[i] = penguinController;
            }

            MovingWallController movingWallController = _obstacles[i].GetComponent<MovingWallController>();
            if(movingWallController != null) {
                _movingWall[i] = movingWallController;
            }
        }
    }

    public void ResetObstacles() {
        for (int i = 0; i < obstacleCount; i++) {

            while (true) {
                Vector3 newPosition = new Vector3(Random.Range(-randomRangeX, randomRangeX), height, offset * i);

                bool isOk = true;
                for (int j = 0; j < i; j++) {
                    if (!(Vector3.Distance(newPosition, _obstacles[j].localPosition) < gapMin)) {
                        continue;
                    }

                    isOk = false;
                    break;
                }

                if(i != 0 && Vector3.Distance(newPosition, _obstacles[i - 1].localPosition) > gapMax) {
                    isOk = false;
                }

                if (isOk) {
                    _obstacles[i].localPosition = newPosition;
                    break;
                }
            }

            if (_penguins[i]) {
                _penguins[i].ResetPenguin();
            }

            if (_movingWall[i]) {
                _movingWall[i].ResetMovingWall();
            }
        }
    }
}
