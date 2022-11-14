using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Transform player;

    public float shiftAmount;

    private Vector2 moveDirection;
    private Vector3 pos;

    private void Update()
    {
        pos = player.position;
        pos.y += 2f;
        pos.z = -10;

        GetInput();
        MoveCam();        
    }

    private void GetInput()
    {
        moveDirection.x = Input.GetAxis("HorizontalArrow") * shiftAmount;
        moveDirection.y = Input.GetAxis("Vertical") * shiftAmount;

        pos.x += moveDirection.x;
        pos.y += moveDirection.y;
    }

    private void MoveCam()
    {
        transform.position = pos;
    }

}
