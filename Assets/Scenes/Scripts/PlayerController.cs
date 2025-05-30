using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 initialPosition;
    
    private void Awake()
    {
        // Store the starting position when the game begins
        initialPosition = transform.position;
    }

    private void OnEnable()
    {
        // Reset position whenever the player is enabled (e.g., after scene reload)
        transform.position = initialPosition;
    }
}