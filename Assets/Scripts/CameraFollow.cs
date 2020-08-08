using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float scaleMultiplier;
    public float posMultiplier;
    public float xPosOffset;
    public float yPosOffset;
    private float yPosStart;
    private float baseScale;
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        yPosStart = transform.position.y;
        _camera = GetComponent<Camera>();
        baseScale = _camera.orthographicSize;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.x = PlayerMovement.PlayerTransform.position.x + xPosOffset;
        if (PlayerMovement.PlayerTransform.position.y>yPosStart+yPosOffset)
        {
            float diff = PlayerMovement.PlayerTransform.position.y - (yPosOffset + yPosStart);
            newPos.y = posMultiplier * diff;
            _camera.orthographicSize = baseScale + diff * scaleMultiplier;
        }

        transform.position = newPos;
    }
}
