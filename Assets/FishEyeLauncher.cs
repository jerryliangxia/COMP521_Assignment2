using System.Collections;
using UnityEngine;

public class FishEyeLauncher : MonoBehaviour
{
    public GameObject fishEyePrefab; // Assign this in the inspector
    public Vector3 launchPosition1 = new Vector3(-7.5f, 0.5f, 0); // Adjust in the inspector
    public Vector3 launchPosition2 = new Vector3(7.5f, 0.5f, 0); // Adjust in the inspector
    private float launchSpeed = 10f; // Adjust as needed
    private float gravity = 9.8f;
    private float lifeTime = 5f; // Adjust as needed

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

    IEnumerator UpdateFishEyePosition(GameObject fishEye, Vector2 velocity)
    {
        float timer = 0;

        while (timer < lifeTime)
        {
            // Update position based on velocity
            fishEye.transform.position += (Vector3)velocity * Time.deltaTime;

            // Update velocity based on gravity
            velocity.y -= gravity * Time.deltaTime;

            timer += Time.deltaTime;
            yield return null;
        }

        // Destroy FishEye after lifeTime seconds
        Destroy(fishEye);
    }
}