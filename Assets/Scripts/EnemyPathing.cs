using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    // Config Parameters

    // State
    WaveConfig _waveConfig;
    List<Transform> _waypoints;
    int wayPointIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        _waypoints = _waveConfig.GetWayPoints();
        transform.position = _waypoints[wayPointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void SetWaveConfig(WaveConfig waveConfig)
    {
        _waveConfig = waveConfig;
    }

    private void Move()
    {
        if (wayPointIndex <= _waypoints.Count - 1)
        {
            var targetPosition = _waypoints[wayPointIndex].transform.position;
            var movementThisFrame = _waveConfig.GetMoveSpeed() * Time.deltaTime;
            transform.position = Vector2.MoveTowards
                (transform.position, targetPosition, movementThisFrame);

            if (transform.position == targetPosition)
            {
                wayPointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
