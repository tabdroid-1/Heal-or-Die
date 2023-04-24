using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [Header("Essentials")]
    [Space]
    [SerializeField] private Transform player;
    [Space]
    [Header("Properties")]
    [Space]
    [SerializeField] private float arriveTime;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float maxXPosition;
    [SerializeField] private float minXPosition;
    [SerializeField] private float maxYPosition;
    [SerializeField] private float minYPosition;
    [Space]
    [Header("State")]
    [Space]
    public bool attached;

    private void FixedUpdate()
    {

        Vector3 target = Vector3.zero;

        float targetX = 0;
        float targetY = 0;

        if (player.position.x > minXPosition && player.position.x < maxXPosition)
        {
            targetX = player.position.x;
        }
        else if (player.position.x > maxXPosition)
        {
            targetX = maxXPosition;
        }
        else if (player.position.x < minXPosition)
        {
            targetX = minXPosition;
        }

        if (player.position.y > minYPosition && player.position.y < maxYPosition)
        {
            targetY = player.position.y;
        }
        else if (player.position.y > maxYPosition)
        {
            targetY = maxYPosition;
        }
        else if (player.position.y < minYPosition)
        {
            targetY = minYPosition;
        }

        target = new Vector3(targetX - offset.x, targetY - offset.y, player.position.z - offset.z);

        if (!attached)
        {
            transform.position = Vector3.Lerp(transform.position, target, arriveTime);
//            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(playerSight, 0, 0), playerSightArriveTime);
        }
    }
}
