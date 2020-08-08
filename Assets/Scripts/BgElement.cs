using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgElement : MonoBehaviour
{
    public static bool skip;
    public float relativeSpeed;
    private float screenOffset=60f;
    private float startY;
    private float prevXPos;
    private Transform parentTransform;

    // Start is called before the first frame update
    void Start()
    {
        parentTransform = transform.parent;
        startY = transform.position.y;
        prevXPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (skip)
        {
            prevXPos = parentTransform.position.x;
            return;
        }
        Vector3 newPos = transform.position;

        if (newPos.x<parentTransform.position.x-screenOffset)
        {
            transform.Translate(2*screenOffset,0,0);
            newPos= transform.position;
        }
        newPos.y = startY;
        float delta = parentTransform.position.x - prevXPos;
        delta *= relativeSpeed;
        newPos.x -= delta;
        transform.position = newPos;
        prevXPos = parentTransform.position.x;
    }
}
