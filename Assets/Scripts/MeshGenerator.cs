using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator 
{
    private static MeshGenerator _instance;

    public static MeshGenerator Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance= new MeshGenerator();
            }
            return _instance;
        }

    }

    public float previousEndingY=0;
    
    private GameObject templateObject=null;
    private const float minYPos=-3;
    
    private int maxSteps=500;
    private float stepSize=0.05f;
    private float preConnectionSize=1;
    private float widthMultiplier=10;
    private float heightMultiplier=10;
    
    private Vector3[] meshVerticies;
    private int[] meshTris;
    private Vector3[] meshNormals;
    private Vector2[] meshUv;
    private Vector2[] colldierPoints;

    private delegate float GenerationFunction(float seed,float t);
    
    public GameObject Generate(float seed,float functionSeed,GameObject pooledObject=null)
    {
        GameObject generated;
        if (pooledObject is null)
        {
            if (templateObject is null)
            {
                templateObject=Resources.Load<GameObject>("GroundTemplate");
            }

            generated = GameObject.Instantiate(templateObject, Vector3.zero, Quaternion.identity);
        }
        else
        {
            generated = pooledObject;
        }

        GenerationFunction generationFunction;
        if (functionSeed<0.5f)
        {
           generationFunction=new GenerationFunction(PerlinGenerator);
        }
        else
        {
            generationFunction=new GenerationFunction(sinGenerator);
        }

        int numConnectionNodes=(int)(preConnectionSize/stepSize);
        meshVerticies=new Vector3[(maxSteps+numConnectionNodes) * 2]; 
        meshTris=new int[((maxSteps+numConnectionNodes) * 2 - 2)*3];
        meshNormals =new Vector3[(maxSteps+numConnectionNodes) * 2];
        meshUv=new Vector2[(maxSteps+numConnectionNodes) * 2];
        colldierPoints =new Vector2[maxSteps+numConnectionNodes];

        int triCounter=0;
        float x = 0;
        Vector3 basePoint=new Vector3(x*widthMultiplier,minYPos,0);
        Vector3 point= new Vector3(x*widthMultiplier,
           generationFunction(seed,x)*heightMultiplier ,
            0);
        x = -1 * preConnectionSize;
        
        //this iteration is to connect last mesh generated last point smoothly to the current mesh being generated
        for (int i=0;i<numConnectionNodes;i++)
        {
            float connectingPointY = Mathf.Lerp(previousEndingY, point.y, (float)i / numConnectionNodes);
            Vector3 connectionBase=new Vector3(x*widthMultiplier,minYPos,0);
            Vector3 connectionPoint=new Vector3(x*widthMultiplier,connectingPointY
            ,0);
            meshVerticies[i * 2] = connectionBase;
            meshVerticies[i * 2 + 1] = connectionPoint;
            meshNormals[i * 2] = Vector3.forward;
            meshNormals[i * 2 + 1]=Vector3.forward;
            meshUv[i * 2] = new Vector2(i/numConnectionNodes+maxSteps,0);
            meshUv[i * 2 + 1]=new Vector2(i/numConnectionNodes+maxSteps,1);
            colldierPoints[i] = new Vector2(connectionPoint.x,connectionPoint.y);
            x += stepSize;
        }
        //this iteration is used to generate the new mesh 
        for (int i=0;i<maxSteps;i++)
        {
            meshVerticies[(numConnectionNodes+i) * 2] = basePoint;
            meshVerticies[(numConnectionNodes+i) * 2 + 1] = point;
            meshNormals[(numConnectionNodes+i) * 2] = Vector3.forward;
            meshNormals[(numConnectionNodes+i) * 2 + 1]=Vector3.forward;
            meshUv[(numConnectionNodes+i) * 2] = new Vector2((numConnectionNodes+i)/numConnectionNodes+maxSteps,0);
            meshUv[(numConnectionNodes+i) * 2 + 1]=new Vector2((numConnectionNodes+i)/numConnectionNodes+maxSteps,1);
            colldierPoints[numConnectionNodes+i]=new Vector2(point.x,point.y);
            x += stepSize;
            basePoint=new Vector3(x*widthMultiplier,minYPos,0);
            point= new Vector3(x*widthMultiplier,
                generationFunction(seed,x) *heightMultiplier,
                0);
            
        }

        for (int i=0;i<(maxSteps+numConnectionNodes) * 2 - 2;i++)
        {
            if (i%2==0)
            {
                meshTris[triCounter] = i;
                meshTris[triCounter + 1] = i + 1;
                meshTris[triCounter + 2] = i + 2;
            }
            else
            {
                meshTris[triCounter] = i + 2;
                meshTris[triCounter + 1] = i + 1;
                meshTris[triCounter + 2] = i;
            }

            triCounter += 3;
        }
        
        Mesh mesh=new Mesh();
        mesh.vertices = meshVerticies;
        mesh.normals = meshNormals;
        mesh.triangles = meshTris;
        mesh.uv = meshUv;
        generated.GetComponent<MeshFilter>().sharedMesh = mesh;
        generated.GetComponent<EdgeCollider2D>().points = colldierPoints;
        LineRenderer lr = generated.GetComponent<LineRenderer>();
        lr.positionCount = maxSteps + numConnectionNodes;
        for (int i = 0; i < maxSteps+numConnectionNodes; i++)
        {
            lr.SetPosition(i,colldierPoints[i]);
        }
        previousEndingY = meshVerticies[(maxSteps + numConnectionNodes) * 2-1].y;
        return generated;
    }    

    private float PerlinGenerator(float seed,float t)    
    {
        return Mathf.PerlinNoise(t, seed);
    }

    private float sinGenerator(float f, float t)
    {
        if (f < 0.3f)
            f = 0.3f;
        
        return Mathf.Sin(2f * f * Mathf.PI * t)*0.2f+0.3f;
    }
}
