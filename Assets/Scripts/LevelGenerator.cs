using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public int maxLevelParts;
    public float yPos;
    private GameObject[] levelParts;
    private float size;
    private Transform cameraTransform;
    private TrailRenderer mainTrial;
    private TrailRenderer seccondTrail;

    private float trailTime;
    // Start is called before the first frame update
    void Start()
    {
        mainTrial = PlayerMovement.PlayerTransform.gameObject.GetComponent<TrailRenderer>();
        seccondTrail = PlayerMovement.PlayerTransform.GetChild(0).gameObject.GetComponent<TrailRenderer>();
        cameraTransform = Camera.main.transform;
        MeshGenerator.Instance.previousEndingY = 0;
        Vector3 newPos=new Vector3(0,yPos,0);
        levelParts=new GameObject[maxLevelParts];
        levelParts[0] = MeshGenerator.Instance.Generate(Random.Range(0f, 1f), Random.Range(0f, 1f));
        levelParts[0].transform.position = newPos;
        size=levelParts[0].GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
        for (int i=1;i<maxLevelParts;i++)
        {
            levelParts[i] = MeshGenerator.Instance.Generate(Random.Range(0f, 1f), Random.Range(0f, 1f));
            newPos.x += size;
            levelParts[i].transform.position = newPos;
        }
    }

    private void Update()
    {
        if (PlayerMovement.PlayerTransform.position.x>levelParts[maxLevelParts-1].transform.position.x+size/2)
        {
            remakeLevel();
            Vector3 pos=PlayerMovement.PlayerTransform.position;
            pos.x -= (maxLevelParts - 1) * size;
            PlayerMovement.PlayerTransform.position = pos;
            cameraTransform.Translate(-1*(maxLevelParts-1)*size,0,0);
            //do stuff about trail
            trailTime = mainTrial.time;
            mainTrial.time = 0;
            mainTrial.enabled = false;
            seccondTrail.time = 0;
            seccondTrail.enabled = false;
            BgElement.skip = true;
            StartCoroutine(enableTrail());
        }
    }

    IEnumerator enableTrail()
    {
        yield return  new WaitForSeconds(0.1f);
        BgElement.skip = false;
        yield return new WaitForSeconds(trailTime);
        mainTrial.time = trailTime;
        mainTrial.enabled = true;
        seccondTrail.time = trailTime;
        seccondTrail.enabled = true;
    }

    public void remakeLevel()
    {
        levelParts[maxLevelParts-1].transform.Translate(-1*(maxLevelParts-1)*size,0,0);
        levelParts[0].transform.Translate((maxLevelParts-1)*size,0,0);
        GameObject tmpHolder=levelParts[maxLevelParts-1];
        for (int i = 1; i < maxLevelParts-1; i++)
        {
            levelParts[i] = MeshGenerator.Instance.Generate(Random.Range(0f, 1f), Random.Range(0f, 1f), levelParts[i]);
        }

        levelParts[maxLevelParts - 1] =
            MeshGenerator.Instance.Generate(Random.Range(0f, 1f), Random.Range(0f, 1f), levelParts[0]);
        levelParts[0] = tmpHolder;
    }
}
