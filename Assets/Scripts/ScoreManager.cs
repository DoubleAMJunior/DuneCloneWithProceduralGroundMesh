using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Transform[] scoreLines;
    public Text scoreLabel;
    private float score;
    private int currentPlace;
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        scoreLabel.text = score.ToString();
        _animator = scoreLabel.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (LoseManager.lose)
        {
            scoreLabel.text = "Final Score:" + score;
            _animator.SetTrigger("lose");
        }
        for (int i=(scoreLines.Length-1);i>=0;i--)
        {
            if (PlayerMovement.PlayerTransform.position.y>scoreLines[i].position.y)
            {
                print("in");
                if (currentPlace<i+1)
                {
                    currentPlace = i+1;
                    addScore(i+1);    
                }
                else
                {
                    currentPlace = i + 1;
                }
                
                break;
            }

            if (i==0)
            {
                currentPlace = i;
            }
        }
        
    }

    public void addScore(int s)
    {
        score += s;
        scoreLabel.text = score.ToString();
        _animator.SetTrigger("score");
    }
}
