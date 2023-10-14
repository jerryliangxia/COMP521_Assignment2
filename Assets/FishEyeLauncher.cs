using System.Collections;
using UnityEngine;

public class FishEyeLauncher : MonoBehaviour
{
    public GameObject fishEyePrefab; // Assign this in the inspector
    public GameObject explosionEffectPrefab;
    public Vector3 launchPosition1 = new Vector3(-7.5f, 0.5f, 0); // Adjust in the inspector
    public Vector3 launchPosition2 = new Vector3(7.5f, 0.5f, 0); // Adjust in the inspector
    private float launchSpeed = 10f; // Adjust as needed
    private float gravity = 9.8f;
    private float lifeTime = 5f; // Adjust as needed
    
    public Vector2 point1 = new Vector2(1, 0.5f);
    public Vector2 point2 = new Vector2(0, 3);
    public Vector2 point3 = new Vector2(-1, 0.5f);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LaunchFishEye(45, launchPosition1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LaunchFishEye(-225, launchPosition2);
        }
    }

    void LaunchFishEye(float angle, Vector3 launchPosition)
    {
        // Instantiate FishEye prefab at the specified launch position
        GameObject fishEye = Instantiate(fishEyePrefab, launchPosition, Quaternion.identity);

        // Randomize angle and launch speed
        float randomAngle = angle + Random.Range(-10.0f, 10.0f); // Adjust range as needed
        float randomSpeed = launchSpeed + Random.Range(-1.0f, 5.0f); // Adjust range as needed

        // Calculate initial velocity
        Vector2 velocity = new Vector2(Mathf.Cos(Mathf.Deg2Rad * randomAngle), Mathf.Sin(Mathf.Deg2Rad * randomAngle)) * randomSpeed;

        // Start coroutine to update FishEye position
        StartCoroutine(UpdateFishEyePosition(fishEye, velocity));
    }
    
    bool IsInPondRegion(Vector3 position)
    {
        return (position.x > 3 && position.x < 7.5 && position.y < 0.5) ||
               (position.x > -7.5 && position.x < -3 && position.y < 0.5);
    }

    bool IsOutsideXRange(float x)
    {
        return x < -10.5 || x > 10.5;
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

    IEnumerator UpdateFishEyePosition(GameObject fishEye, Vector2 velocity)
    {
        float timer = 0;

        while (timer < lifeTime)
        {
            // Check if fishEye has been destroyed
            if (fishEye == null)
            {
                yield break;
            }

            // Update position based on velocity
            fishEye.transform.position += (Vector3)velocity * Time.deltaTime;

            // Check if fishEye is within the pond regions
            if (IsInPondRegion(fishEye.transform.position) ||
                IsInsideTriangle(new Vector2(fishEye.transform.position.x, fishEye.transform.position.y), point1, point2, point3) ||
                IsOutsideXRange(fishEye.transform.position.x))
            {
                // Instantiate explosion effect at fishEye position
                Instantiate(explosionEffectPrefab, fishEye.transform.position, Quaternion.identity);
                Destroy(fishEye);
                yield break;
            }

            // Update velocity based on gravity
            velocity.y -= gravity * Time.deltaTime;

            timer += Time.deltaTime;
            yield return null;
        }

        // Check if fishEye has been destroyed
        if (fishEye != null)
        {
            // Destroy FishEye after lifeTime seconds
            Destroy(fishEye);
        }
    }
}