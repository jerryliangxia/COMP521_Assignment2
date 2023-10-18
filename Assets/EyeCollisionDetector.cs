using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeCollisionDetector : MonoBehaviour
{
    public GameObject explosionEffectPrefab; // Assign this in the inspector
    public float radius = 0.18f; // Adjust as needed
    private GameObject[] fishEyes;

	private Vector2 velocity;
    private float gravity = 9.8f;
    private float lifeTime = 5f;
    
    public Vector2 point1 = new (1, 0.5f);
    public Vector2 point2 = new (0, 3);
    public Vector2 point3 = new (-1, 0.5f);

	public void SetInitialVelocity(Vector2 initialVelocity)
    {
        velocity = initialVelocity;
    }

    // Start is called before the first frame update
    void Start()
    {
        fishEyes = GameObject.FindGameObjectsWithTag("FishEye");
        StartCoroutine(UpdateFishEyePosition());
    }

    IEnumerator UpdateFishEyePosition()
    {
        float timer = 0;

        while (timer < lifeTime)
        {
            // Update position based on velocity
            transform.position += (Vector3)velocity * Time.deltaTime;

            // Update velocity based on gravity
            velocity.y -= gravity * Time.deltaTime;
            
            // Rotate FishEye according to its velocity
            var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Check if fishEye is within the pond regions
            if (IsInPondRegion(transform.position) ||
                IsInsideTriangle(new Vector2(transform.position.x, transform.position.y)) ||
                IsOutsideXRange(transform.position.x))
            {
                // Instantiate explosion effect at fishEye position
                Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Destroy FishEye after lifeTime seconds
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Update position based on velocity
        transform.position += (Vector3)velocity * Time.deltaTime;

        // Update velocity based on gravity
        velocity.y -= gravity * Time.deltaTime;

        foreach (var fishEye in fishEyes)
        {
            if (fishEye == null || fishEye.transform.position.y < 0)
            {
                continue;
            }
    
            if (fishEye != gameObject && IsCollidingWith(fishEye))
            {
                Debug.Log("Collision detected with " + fishEye.name);
            
                // Calculate new velocities after collision
                var otherDetector = fishEye.GetComponent<EyeCollisionDetector>();
                if (otherDetector != null)
                {
                    var otherVelocity = otherDetector.GetVelocity();
                    var newVelocity = velocity - 2 * (Vector2.Dot(velocity - otherVelocity, (Vector2)(transform.position - fishEye.transform.position))) / ((transform.position - fishEye.transform.position).sqrMagnitude) * (Vector2)(transform.position - fishEye.transform.position);
                    var newOtherVelocity = otherVelocity - 2 * (Vector2.Dot(otherVelocity - velocity, (Vector2)(fishEye.transform.position - transform.position))) / ((fishEye.transform.position - transform.position).sqrMagnitude) * (Vector2)(fishEye.transform.position - transform.position);

                    // Set new velocities
                    velocity = newVelocity;
                    otherDetector.SetVelocity(newOtherVelocity);
                }
            }
        }
    }
    
    private Vector2 GetVelocity()
    {
        return velocity;
    }

    private void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
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
    
    bool IsInsideTriangle(Vector2 point)
    {
        var alpha = ((point2.y - point3.y) * (point.x - point3.x) + (point3.x - point2.x) * (point.y - point3.y)) /
                      ((point2.y - point3.y) * (point1.x - point3.x) + (point3.x - point2.x) * (point1.y - point3.y));
        var beta = ((point3.y - point1.y) * (point.x - point3.x) + (point1.x - point3.x) * (point.y - point3.y)) /
                     ((point2.y - point3.y) * (point1.x - point3.x) + (point3.x - point2.x) * (point1.y - point3.y));
        var gamma = 1.0f - alpha - beta;

        return alpha > 0 && beta > 0 && gamma > 0;
    }

    bool IsCollidingWith(GameObject other)
    {
        var otherRadius = radius; // Adjust as needed
        var distance = Vector3.Distance(transform.position, other.transform.position);

        return distance < radius + otherRadius;
    }

    void SetColor(GameObject obj, Color color)
    {
        var spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}