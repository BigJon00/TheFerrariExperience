using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivalCarController : MonoBehaviour
{
    public float maxSpeed = 20f;
    public float acceleration = 5f;
    public float steeringSpeed = 2f;
    public float waypointThreshold = 5f;
    private int currentWaypointIndex = 0;
    public Transform[] waypoints; 
    public float stoppingDistance = 0.5f;
 
    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;
        direction.y = 0; // Keep movement on the horizontal plane

        // Move towards the waypoint
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, acceleration * Time.deltaTime);

        // Make the NPC look at the waypoint (optional)
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Check if reached the current waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < stoppingDistance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0; // Loop back to the beginning
                // Or stop movement: enabled = false;
            }
        }
    }
}
