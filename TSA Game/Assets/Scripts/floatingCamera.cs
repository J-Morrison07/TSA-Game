using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingCamera : MonoBehaviour
{
    public GameObject p1;
    public GameObject p2;
    public float camMultiplier = 1;
    public float cameraSpeed = 0.01f;
    private Vector3 p1Pos;
    private Vector3 p2Pos;
    private Vector3 cameraPos;
    private Vector3 cameraRot;
    public float zOffset = 0;
    // Start is called before the first frame update

    private Vector3 normalize(Vector3 inVector){
        float length = Mathf.Sqrt(inVector.x * inVector.x + inVector.y * inVector.y + inVector.z * inVector.z);
        Vector3 final;
        final.x = inVector.x / length;
        final.y = inVector.y / length;
        final.z = inVector.z / length;
        return final;
    }

    private float length(Vector3 inVector){
        return Mathf.Sqrt(inVector.x * inVector.x + inVector.y * inVector.y + inVector.z * inVector.z);
    }

    Vector3 lookAt(Vector3 origin, Vector3 target){
        Vector3 final;
        final = origin - target;
        final = normalize(final);
        return final;
    }

    void Start()
    {
        p1Pos = p1.transform.position;
        p2Pos = p2.transform.position;
        cameraPos = (p1Pos + p2Pos) / 2;
        cameraPos = cameraPos * camMultiplier;
        cameraPos.z = -5 - Mathf.Sqrt((p1Pos.x - p2Pos.x) * (p1Pos.x - p2Pos.x) + (p1Pos.y - p2Pos.y) * (p1Pos.y - p2Pos.y));
        cameraPos.y = cameraPos.y + 3;
        cameraPos.z = cameraPos.z - zOffset;
        transform.position = cameraPos;
        cameraRot = (p1Pos + p2Pos) / 2 - transform.position;
        transform.rotation = Quaternion.LookRotation(cameraRot, Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        p1Pos = p1.transform.position;
        p2Pos = p2.transform.position;
        cameraPos = (p1Pos + p2Pos) / 2;
        cameraPos = cameraPos * camMultiplier;
        cameraPos.z = -5 - Mathf.Sqrt((p1Pos.x - p2Pos.x) * (p1Pos.x - p2Pos.x) + (p1Pos.y - p2Pos.y) * (p1Pos.y - p2Pos.y));
        cameraPos.y = cameraPos.y + 3;
        cameraPos.z = cameraPos.z - zOffset;
        transform.position = Vector3.Lerp(transform.position, cameraPos, cameraSpeed);
        cameraRot = (p1Pos + p2Pos) / 2 - transform.position;
        transform.rotation = Quaternion.LookRotation(cameraRot, Vector3.up);
    }
}
