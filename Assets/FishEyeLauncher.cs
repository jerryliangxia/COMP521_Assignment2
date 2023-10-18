using System.Collections;
using UnityEngine;

public class FishEyeLauncher : MonoBehaviour
{
    public GameObject fishEyePrefab; // Assign this in the inspector
    public Vector3 launchPosition1 = new (-7.5f, 0.5f, 0); // Adjust in the inspector
    public Vector3 launchPosition2 = new (7.5f, 0.5f, 0); // Adjust in the inspector
    private float launchSpeed = 8f; // Adjust as needed

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
        var fishEye = Instantiate(fishEyePrefab, launchPosition, Quaternion.identity);

        // Randomize angle and launch speed
        var randomAngle = angle + Random.Range(-10.0f, 10.0f);
        var randomSpeed = launchSpeed + Random.Range(-1.0f, 5.0f);

        // Calculate initial velocity
        var velocity = new Vector2(Mathf.Cos(Mathf.Deg2Rad * randomAngle), Mathf.Sin(Mathf.Deg2Rad * randomAngle)) * randomSpeed;

        // Pass the initial velocity to the EyeCollisionDetector component
        var detector = fishEye.GetComponent<EyeCollisionDetector>();
        if (detector != null)
        {
            detector.SetInitialVelocity(velocity);
        }
    }
}