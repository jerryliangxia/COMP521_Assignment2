using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeCollisionDetector : MonoBehaviour
{
    public GameObject explosionEffectPrefab; // Assign this in the inspector
    public float radius = 0.18f; // Adjust as needed
    private GameObject[] fishEyes;

    // Start is called before the first frame update
    void Start()
    {
        fishEyes = GameObject.FindGameObjectsWithTag("FishEye");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject fishEye in fishEyes)
        {
            if (fishEye == null || fishEye.transform.position.y < 0)
            {
                continue;
            }
        
            if (fishEye != gameObject && IsCollidingWith(fishEye))
            {
                Debug.Log("Collision detected with " + fishEye.name);
                
                // Instantiate explosion effect at collision point
                Instantiate(explosionEffectPrefab, fishEye.transform.position, Quaternion.identity);
                
                // Destroy both FishEye objects
                Destroy(gameObject);
                Destroy(fishEye);
                
                // Exit the loop since this object no longer exists
                break;
            }
        }
    }

    bool IsCollidingWith(GameObject other)
    {
        float otherRadius = radius; // Adjust as needed
        float distance = Vector3.Distance(transform.position, other.transform.position);

        return distance < radius + otherRadius;
    }

    void SetColor(GameObject obj, Color color)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}