using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    public Transform player;
    public Transform player2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.pos.y = player.position.y + player2.position.y / 2;
        transform.pos.z = player.position.x + player2.position.x / 2;
    }
}
