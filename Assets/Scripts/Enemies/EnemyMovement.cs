using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Events;

public class EnemyMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public SplineContainer splineContainer;

    public EnemyStats enemyStats;

    private float currentSpeed;

    public bool loopPath = false;

    private float _currentPathAngle;
    public float CurrentPathAngle => _currentPathAngle; // Public getter
    
    public UnityEvent onEnemyReachedEndOfPath;
    public UnityEvent OnFacingRight;
    public UnityEvent OnFacingLeft;


    bool is_blocked = false;
    private float distanceAlongSpline = 0f;
    private bool reached_end_of_path = false;
    private bool _isFacingRight = true; // Initial assumption, will be updated
    private float _totalSplineLength = 0f;

    


    void Awake()
    {
        currentSpeed = enemyStats.movementSpeed/8000;
    }

    void Start()
    {
        if (splineContainer == null || splineContainer.Spline == null)
        {
            Debug.LogError("SplineContainer is not assigned or contains no spline.", this);
            enabled = false; // Disable script if no spline is set
            return;
        }

        // It's good practice to initialize UnityEvents if they might be null
        if (onEnemyReachedEndOfPath == null)
            onEnemyReachedEndOfPath = new UnityEvent();
        if (OnFacingRight == null)
            OnFacingRight = new UnityEvent();
        if (OnFacingLeft == null)
            OnFacingLeft = new UnityEvent();

        _totalSplineLength = splineContainer.CalculateLength();
        if (_totalSplineLength <= 0f)
        {
            Debug.LogWarning("Spline length is zero or less. Enemy will not move.", this);
            enabled = false;
            return;
        }

        // Set initial position and orientation
        UpdateEnemyTransformAndDirection(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (splineContainer == null || splineContainer.Spline == null || _totalSplineLength <= 0f)
        {
            return; // Guard clause
        }

        if (!is_blocked && !reached_end_of_path)
        {
            distanceAlongSpline += currentSpeed * Time.deltaTime;

            if (distanceAlongSpline >= _totalSplineLength)
            {
                if (loopPath)
                {
                    distanceAlongSpline -= _totalSplineLength;
                    
                }
                else
                {
                    reached_end_of_path = true;
                    distanceAlongSpline = _totalSplineLength; 
                    onEnemyReachedEndOfPath.Invoke();
                    
                }
            }
        }
        else if (reached_end_of_path && !loopPath)
        {
            // If path ended and not looping, no further processing needed for movement
            // but ensure the final position is maintained by UpdateEnemyTransformAndDirection
        }

       
        distanceAlongSpline = Mathf.Clamp(distanceAlongSpline, 0f, _totalSplineLength);

        UpdateEnemyTransformAndDirection(false);
    }
    private void UpdateEnemyTransformAndDirection(bool isInitialSetup)
    {
        if (splineContainer == null || splineContainer.Spline == null || _totalSplineLength <= 0f) return;

        // Evaluate spline at the current distance
        // SplineContainer methods return Unity.Mathematics.float3, which needs casting to UnityEngine.Vector3
        Vector3 positionOnSpline = (Vector3)splineContainer.EvaluatePosition(distanceAlongSpline);
        Vector3 tangentOnSpline = (Vector3)splineContainer.EvaluateTangent(distanceAlongSpline);

        // Update enemy's position
        transform.position = positionOnSpline;

        if (tangentOnSpline.sqrMagnitude > 0.0001f) // Ensure tangent is valid
        {
            tangentOnSpline.Normalize(); // Ensure consistent direction vector

            // Calculate the angle of the tangent
            // Angle is in degrees, 0 degrees is along the positive X-axis
            _currentPathAngle = Mathf.Atan2(tangentOnSpline.y, tangentOnSpline.x) * Mathf.Rad2Deg;

            // Determine facing direction based on the tangent's x component
            bool previousFacingRight = _isFacingRight;

            if (tangentOnSpline.x > 0.01f) // Moving predominantly right
            {
                _isFacingRight = true;
            }
            else if (tangentOnSpline.x < -0.01f) // Moving predominantly left
            {
                _isFacingRight = false;
            }
            // If tangentOnSpline.x is very close to zero (vertical movement), maintain current facing direction.

            if (!isInitialSetup)
            {
                if (_isFacingRight && !previousFacingRight)
                {
                    OnFacingRight.Invoke();
                    // Debug.Log("Facing Right Event Triggered");
                }
                else if (!_isFacingRight && previousFacingRight)
                {
                    OnFacingLeft.Invoke();
                    // Debug.Log("Facing Left Event Triggered");
                }
            }
            else // Initial setup
            {
                 // Just set _isFacingRight, events will trigger on first change during Update
            }
        }
        else
        {
            // Tangent is zero (e.g., at a sharp cusp or if spline is malformed)
            // Maintain previous angle or set to a default
            // _currentPathAngle remains unchanged or could be set e.g. transform.eulerAngles.z
        }
    }
    public void ResetToStart()
    {
        distanceAlongSpline = 0f;
        reached_end_of_path = false;
        is_blocked = false; // Optionally reset block state
        if (splineContainer != null && splineContainer.Spline != null && _totalSplineLength > 0f)
        {
            UpdateEnemyTransformAndDirection(true);
        }
    }
}

