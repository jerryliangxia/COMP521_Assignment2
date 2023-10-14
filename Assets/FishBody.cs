using UnityEngine;

public class FishBody : MonoBehaviour
{
    public GameObject fishEye;
    public GameObject pointPrefab;
    public GameObject linePrefab;
    private GameObject[] points;
    private GameObject[] lines;

    private Vector3[] positions;
    private bool[] isFixed;
    private float timer;
    private Vector3[] previousPositions;
    private Vector3[] accelerations;

    public float fishSizeRatio = 1.0f;
    public float stiffness = 0.1f;
    public float damping = 0.1f;
    
    private Vector2 point1 = new Vector2(1, 0.5f);
    private Vector2 point2 = new Vector2(0, 3);
    private Vector2 point3 = new Vector2(-1, 0.5f);

    void Start()
    {
        int numVertices = 9;
        points = new GameObject[numVertices];
        lines = new GameObject[numVertices];
        positions = new Vector3[numVertices];
        previousPositions = new Vector3[numVertices];
        accelerations = new Vector3[numVertices];
        isFixed = new bool[numVertices];
        timer = 0f;

        // Define positions relative to fishEye to form a fish shape
        positions[0] = new Vector3(-0.2f, 0.1f, 0); // Top of the fish
        positions[1] = new Vector3(0.1f, 0.2f, 0); // Upper right
        positions[2] = new Vector3(0.3f, 0.1f, 0); // Middle right (start of tail)
        positions[3] = new Vector3(0.5f, 0.15f, 0); // Upper tail
        positions[4] = new Vector3(0.5f, -0.15f, 0); // Lower tail
        positions[5] = new Vector3(0.3f, -0.1f, 0); // Middle right (end of tail)
        positions[6] = new Vector3(0.1f, -0.2f, 0); // Lower right
        positions[7] = new Vector3(-0.2f, -0.1f, 0); // Bottom of the fish
        positions[8] = new Vector3(-0.16f, 0, 0); // Mouth of the fish

        for (int i = 0; i < numVertices; i++)
        {
            // Instantiate point
            points[i] = Instantiate(pointPrefab, fishEye.transform);

            // Instantiate line
            lines[i] = Instantiate(linePrefab, fishEye.transform);
            LineRenderer lr = lines[i].GetComponent<LineRenderer>();
            lr.positionCount = 2;

            // Set initial positions
            Vector3 position = positions[i] * fishSizeRatio;
            if (fishEye.transform.position.x < 0)
            {
                position.x *= -1;
            }
            points[i].transform.localPosition = position;
            previousPositions[i] = points[i].transform.localPosition;
        }

        // Connect the lines between the points to form a fish shape
        for (int i = 0; i < numVertices; i++)
        {
            LineRenderer lr = lines[i].GetComponent<LineRenderer>();
            lr.SetPosition(0, points[i].transform.position);
            lr.SetPosition(1, points[(i + 1) % numVertices].transform.position);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 point = new Vector2(points[i].transform.position.x, points[i].transform.position.y);

            // Check if point is inside triangle or on the ground
            if (timer > 1f && !isFixed[i])
            {
                
                int maxIterations = 10;
                for (int iteration = 0; iteration < maxIterations; iteration++)
                {
                    bool constraintCheck = (IsInsideTriangle(point, point1, point2, point3) || isInLeftPond(point) ||
                                            isInRightPond(point));
                    isFixed[i] = constraintCheck; // Stop updating this point
                }
            }

            if (isFixed[i])
            {
                continue; // Skip updating this point
            }

            // Calculate acceleration based on Hooke's law
            Vector3 displacement = points[i].transform.localPosition - positions[i] * fishSizeRatio;
            accelerations[i] = -stiffness * displacement - damping * (points[i].transform.localPosition - previousPositions[i]) / Time.deltaTime;

            // Update positions using Verlet integration
            Vector3 newPosition = 2 * points[i].transform.localPosition - previousPositions[i] + accelerations[i] * Time.deltaTime * Time.deltaTime;
            previousPositions[i] = points[i].transform.localPosition;
            points[i].transform.localPosition = newPosition;

            // Update line positions
            LineRenderer lr = lines[i].GetComponent<LineRenderer>();
            lr.SetPosition(0, points[i].transform.position);
            lr.SetPosition(1, points[(i + 1) % points.Length].transform.position);
        }
    }
    
    bool isInLeftPond(Vector2 point)
    {
        return (point.x >= -10.5f && point.x <= -7.5f && point.y <= 0.5f);
    }
        
    bool isInRightPond(Vector2 point)
    {
        return (point.x >= 7.5f && point.x <= 10.5f && point.y <= 0.5f);
    }
    
    bool IsInsideTriangle(Vector2 point, Vector2 point1, Vector2 point2, Vector2 point3)
    {
        float alpha = ((point2.y - point3.y) * (point.x - point3.x) + (point3.x - point2.x) * (point.y - point3.y)) /
                      ((point2.y - point3.y) * (point1.x - point3.x) + (point3.x - point2.x) * (point1.y - point3.y));
        float beta = ((point3.y - point1.y) * (point.x - point3.x) + (point1.x - point3.x) * (point.y - point3.y)) /
                     ((point2.y - point3.y) * (point1.x - point3.x) + (point3.x - point2.x) * (point1.y - point3.y));
        float gamma = 1.0f - alpha - beta;

        return alpha > 0 && beta > 0 && gamma > 0;
    }
}