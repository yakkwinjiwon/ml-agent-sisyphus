using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;

public class SisyphusAgent : Agent {

    [Header("Agent Settings")]
    [Range(1f, 10f)] public float runningSpeed;
    [Range(1f, 10f)] public float pushingSpeed;

    [Header("Dependency")]
    public BallController ball;
    public Transform target;
    public ObstacleController[] obstacleControllers;
    public MovingWallController[] movingWallControllers;

    // Agent components
    private Rigidbody _rb;
    private Animator _animator;

    // Dynamic agent states
    private bool _isRunning;
    private bool _isPushing;
    private float _speed;
    private Vector3 _moveInput;
    private float _distanceToBall;
    private float _prevDistToGoal;

    // Private agent settings
    private static readonly int State = Animator.StringToHash("state");
    private const float RotationSpeed = 10f;
    private const float EdgeDistance = 10f;
    private Vector3 _startingPosition;
    private bool _isHeuristic;
    
    public override void Initialize() {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _startingPosition = transform.position;

        // Increase gravity for realistic control
        Physics.gravity = new Vector3(0, -20, 0);
    }

    public override void OnEpisodeBegin() {
        // Reset environment
        ResetAgent();
        ball.ResetBall();
        ResetDistance();
        foreach (ObstacleController obstacleController in obstacleControllers) {
            obstacleController.ResetObstacles();
        }
        foreach (MovingWallController movingWallController in movingWallControllers) {
            movingWallController.ResetMovingWall();
        }
    }

    public override void CollectObservations(VectorSensor sensor) {
        // Agent
        sensor.AddObservation(transform.rotation.eulerAngles.y);
        sensor.AddObservation(_rb.velocity);
        sensor.AddObservation(_isPushing);
        sensor.AddObservation(_distanceToBall);

        // Ball
        sensor.AddObservation(ball.transform.rotation);
        sensor.AddObservation(ball.rb.velocity);
        sensor.AddObservation(ball.rb.angularVelocity);
        
        // Depth map
        for (int i = 0; i < ball.raycastCount; i++) {
            sensor.AddObservation(ball.depthMap[i]);
        }

        // Target direction
        Vector3 targetVector = target.position - ball.transform.position;
        sensor.AddObservation(targetVector.normalized);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        int sequence = -1;

        float angle = actions.ContinuousActions[++sequence] * 180;
        float moveAmount = (actions.ContinuousActions[++sequence] + 1) * .5f;
        _moveInput = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * moveAmount;
        
        // Heuristic mode
        if (_isHeuristic) {
            float xInput = Input.GetAxisRaw("Horizontal");
            float zInput = Input.GetAxisRaw("Vertical");
            _moveInput = new Vector3(xInput, 0, zInput).normalized;
        }

        if (_moveInput is { x: 0, z: 0 }) {
            _isRunning = false;
        } else {
            _isRunning = true;
        }

        //Set speed
        _speed = _isPushing ? pushingSpeed : runningSpeed;

        // Hand control input
        ball.handControl = actions.ContinuousActions[++sequence];

        RewardStrategy();
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        _isHeuristic = true;
        int i = -1;

        actionsOut.ContinuousActions.Array[++i] = 0;
        actionsOut.ContinuousActions.Array[++i] = 0;
        
        if (Input.GetMouseButton(0)) {
            actionsOut.ContinuousActions.Array[++i] = -1;
        } else if (Input.GetMouseButton(1)) {
            actionsOut.ContinuousActions.Array[++i] = 1;
        } else {
            actionsOut.ContinuousActions.Array[++i] = 0;
        }
    }

    private void Update() {
        Animate();
    }

    private void FixedUpdate() {
        Move();
        Rotate();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ball")) {
            _isPushing = true;
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Ball")) {
            _isPushing = false;
        }
    }

    // Reset agent state
    private void ResetAgent() {
        // Transform
        Vector3 offset = new Vector3(Random.Range(-12f, 12f), 0, Random.Range(-2f, 0f));
        transform.position = _startingPosition + offset;
        transform.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
        
        // Rigidbody
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        // State
        _isRunning = false;
        _isPushing = false;
        _speed = runningSpeed;
        _moveInput = Vector3.zero;
    }

    // Reset distance after positions of agent and ball are fixed.
    private void ResetDistance() {
        _prevDistToGoal = Vector3.Distance(ball.transform.position, target.position);
        _distanceToBall = Vector3.Distance(transform.position, ball.transform.position);
    }
    
    private void RewardStrategy() {
        // Pushing ball reward
        if (_isPushing) {
            AddReward(.01f);
        }

        // Distance to ball reward
        _distanceToBall = Vector3.Distance(transform.position, ball.transform.position);
        if (_distanceToBall < EdgeDistance) {
            float distanceToBallReward = .001f * (EdgeDistance - _distanceToBall) / EdgeDistance;
            AddReward(distanceToBallReward);
        } else {
            EndEpisodeFail();
        }

        // Goal reward
        // R_goal = w1 * (prevDist - currDist)
        float currDistToGoal = Vector3.Distance(ball.transform.position, target.position);
        float goalReward = 5.0f * (_prevDistToGoal - currDistToGoal);
        _prevDistToGoal = currDistToGoal;
        AddReward(goalReward);

        if (transform.position.z > target.transform.position.z - 2f) {
            EndEpisodeSuccess();
        }

        // Smooth reward
        // R_smooth = -w3 * FLOOD(v, v_min, v_max) - w4 * FLOOD(w, w_min, w_max)
        float smoothReward = -4.0f * Flood(_rb.velocity.magnitude, -0.5f, 1.5f) 
                           - 1.0f * Flood(_rb.angularVelocity.magnitude, -Mathf.PI / 4, Mathf.PI / 4);
        AddReward(smoothReward);
    }

    public void EndEpisodeSuccess() {
        GameManager.Instance.IncreaseTotalCount();
        GameManager.Instance.IncreaseSuccessCount();
        EndEpisode();
    }

    public void EndEpisodeFail() {
        GameManager.Instance.IncreaseTotalCount();
        AddReward(-6f);
        EndEpisode();
    }
    
    private void Move() {
        // Move agent
        Vector3 move = _speed * Time.deltaTime * _moveInput;
        _rb.MovePosition(_rb.position + move);
    }

    private void Rotate() {
        if (_isPushing) {
            // Look at the ball
            Vector3 targetPosition = new Vector3(ball.transform.position.x, transform.position.y, ball.transform.position.z);
            transform.LookAt(targetPosition);
        } else {
            // Rotate agent
            if (_moveInput != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(_moveInput);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime);
            } else {
                _rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void Animate() {
        if (_isPushing) {
            _animator.SetInteger(State, 3);
        } else if (_moveInput == Vector3.zero) {
            _animator.SetInteger(State, 0);
        } else if (_isRunning) {
            _animator.SetInteger(State, 2);
        } else {
            _animator.SetInteger(State, 1);
        }
    }
    
    private static float Flood(float x, float xMin, float xMax) {
        return Mathf.Abs(Mathf.Min(x - xMin, 0)) + Mathf.Abs(Mathf.Max(x - xMax, 0));
    }
}
