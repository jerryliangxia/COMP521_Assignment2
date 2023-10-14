using UnityEngine;

public class FishBody : MonoBehaviour
{
    public GameObject fishEye;
    public GameObject pointPrefab;
    public GameObject linePrefab;
    private GameObject[] points;
    private GameObject[] lines;

    private Vector3[] positions = new Vector3[9];
    
    public float fishSizeRatio = 1.0f;

    void Start()
    {
        int numVertices = 9;
        points = new GameObject[numVertices];
        lines = new GameObject[numVertices];
        // Vector3[] positions = new Vector3[numVertices];

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
        for (int i = 0; i < points.Length; i++)
        {
            // Update line positions
            LineRenderer lr = lines[i].GetComponent<LineRenderer>();
            lr.SetPosition(0, points[i].transform.position);
            lr.SetPosition(1, points[(i + 1) % points.Length].transform.position);
        }
    }
}