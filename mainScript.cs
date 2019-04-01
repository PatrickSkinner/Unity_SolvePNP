using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Vuforia;

public class mainScript : MonoBehaviour
{
    Vector2[] points = new Vector2[4];
    int i = 0;
    public GameObject ground;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Screen Size: " + Screen.width + "x" + Screen.height);

        OpenCVInterop.Init();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))

        {
            Vector2 mousePos = new Vector2();
            mousePos = Input.mousePosition;

            if(i < 4)
            {
                points[i] = mousePos;
                i++;
                Debug.Log(mousePos);
            }
            else if (i == 4)
            {
	    	// Let the user select points in clockwise order from top left, rather than the natural order of vertices in a quad
	    	Vector2[] reorderedPoints = { points[3], points[1], points[2], points[0] };
                points = reorderedPoints;
	    
                Vector3[] vertices = new Vector3[4];

                for (int i = 0; i < 4; i++) vertices[i] = ground.transform.TransformPoint(ground.GetComponent<MeshFilter>().mesh.vertices[i]); //Get world positions of vertices
                Debug.Log("Vertices: " + vertices[0] + ", " + vertices[1] + ", " + vertices[2] + ", " + vertices[3]);

                float[] rotationMatrix = new float[9];
                float[] translationMatrix = new float[3];
                Matrix4x4 transformationMatrix = new Matrix4x4();
                
                OpenCVInterop.ComputePNP(ref vertices, ref points, ref rotationMatrix, ref translationMatrix);

                transformationMatrix[0, 0] = rotationMatrix[0];
                transformationMatrix[0, 1] = rotationMatrix[1];
                transformationMatrix[0, 2] = rotationMatrix[2];
                transformationMatrix[0, 3] = translationMatrix[0];

                transformationMatrix[1, 0] = rotationMatrix[3];
                transformationMatrix[1, 1] = rotationMatrix[4];
                transformationMatrix[1, 2] = rotationMatrix[5];
                transformationMatrix[1, 3] = translationMatrix[1];

                transformationMatrix[2, 0] = rotationMatrix[6];
                transformationMatrix[2, 1] = rotationMatrix[7];
                transformationMatrix[2, 2] = rotationMatrix[8];
                transformationMatrix[2, 3] = translationMatrix[2];

                transformationMatrix[3, 3] = 1;

				//Convert from OpenCV to Unity coordinates
                Matrix4x4 invertYM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
                
                transformationMatrix = transformationMatrix * invertYM;
                //transformationMatrix = transformationMatrix * this.gameObject.transform.localToWorldMatrix;
                Debug.Log(transformationMatrix.ToString());

                ground.transform.position = transformationMatrix.MultiplyPoint3x4(ground.transform.position);
                ground.transform.rotation *= Quaternion.LookRotation(transformationMatrix.GetColumn(2), -transformationMatrix.GetColumn(1));

                ground.GetComponent<MeshRenderer>().enabled = true;


            }
        }
    }
}


internal static class OpenCVInterop
{
    [DllImport("OCVmatch")]
    internal static extern int Init();

    [DllImport("OCVmatch")]
    internal static extern void ComputePNP(ref Vector3[] op, ref Vector2[] ip, ref float[] rv, ref float[] tv);
}

