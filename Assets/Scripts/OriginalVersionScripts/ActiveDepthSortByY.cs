using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveDepthSortByY : MonoBehaviour {

    float halfWidth;
    float verticalOffset;
    public float floorHeight;

    // Use this for initialization
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        verticalOffset = spriteRenderer.bounds.size.y * 0.5f;
        halfWidth = spriteRenderer.bounds.size.x * 0.5f;
    }

    void Update()
    {
        transform.position = new Vector3
            (
                 transform.position.x,
                 transform.position.y,
                 (transform.position.y - verticalOffset + floorHeight)
             );
    }

    void OnDrawGizmos()
    {
        Vector3 floorHeightPos = new Vector3(transform.position.x, transform.position.y - verticalOffset + floorHeight, transform.position.z);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(floorHeightPos + Vector3.left * halfWidth, floorHeightPos + Vector3.right * halfWidth);
    }
}