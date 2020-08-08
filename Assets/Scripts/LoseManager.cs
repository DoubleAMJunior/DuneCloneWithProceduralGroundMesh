using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseManager : MonoBehaviour
{
    
    public static bool lose;
    // Start is called before the first frame update
    private void Start()
    {
        lose = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (lose)
        {
            if (Input.anyKeyDown || Input.touchCount>0)
            {
                SceneManager.LoadScene(0);
            }
        }    
    }
}
