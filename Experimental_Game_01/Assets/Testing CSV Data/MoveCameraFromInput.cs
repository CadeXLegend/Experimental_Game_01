using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraFromInput : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;
    Vector3 movingDir
    {
        get
        {
            Vector3 dir;
            float xAxis = Input.GetAxis("Horizontal");
            float yAxis = Input.GetAxis("Vertical");

            if (xAxis != 0)
                dir = xAxis > 0 ? transform.right : -transform.right;
            else if (yAxis != 0)
                dir = yAxis > 0 ? transform.forward : -transform.forward;
            else
                dir = Vector3.zero;

            return dir;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            speed *= 2;

        transform.position = Vector3.Lerp(transform.position, transform.position + movingDir, Time.deltaTime * speed);
    }
}
