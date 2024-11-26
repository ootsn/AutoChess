using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class test : MonoBehaviour
{
    public GameObject hexGrid;
    public GameObject reserveSeat;

    // Start is called before the first frame update
    void Start()
    {
        //double radians = (Camera.main.transform.eulerAngles.x * Math.PI) / 180;
        //sinx = (float)Math.Sin(radians);

        for (int i = 0; i < hexGrid.transform.childCount; i++)
        {
            placePoints.Add(hexGrid.transform.GetChild(i).position);
        }

        for (int i = 0; i < reserveSeat.transform.childCount; i++)
        {
            placePoints.Add(reserveSeat.transform.GetChild(i).position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        
    }

    private bool isDragging = false;
    private Vector3 dragOrigin; // 鼠标按下时的物体位置
    private Vector3 offset; // 鼠标按下时的鼠标位置与物体位置的偏移
    private List<Vector3> placePoints = new List<Vector3>();
    //private float sinx;

    // 当鼠标按下时调用
    void OnMouseDown()
    {
        isDragging = true;

        hexGrid.SetActive(true);
    }

    // 当鼠标拖动时调用
    void OnMouseDrag()
    {
        if (isDragging)
        {
            // 获取鼠标在世界空间中的位置
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.magenta);
                this.gameObject.transform.position = hit.point;
                Debug.DrawRay(ray.origin, ray.direction);
            }
            else
            {
                GameObject checkerboard = GameObject.Find("checkerboard");
                Vector3 palneNormal = checkerboard.transform.up;
                Vector3 planePoint = new Vector3(0, 0, 0);
                Vector3 linePoint = ray.origin;
                Vector3 lineDir = ray.direction;
                float a = Vector3.Dot((planePoint - linePoint), palneNormal);
                float b = Vector3.Dot(lineDir, palneNormal);
                if (a != 0 && b != 0)
                {
                    Vector3 pos = a / b * lineDir + linePoint;
                    Debug.DrawLine(pos, new Vector3(pos.x, pos.y + 30, pos.z), Color.blue);

                    float checkerboardScaleX = checkerboard.transform.localScale.x;
                    float checkerboardScaleZ = checkerboard.transform.localScale.z;
                    float leftX = checkerboard.transform.position.x - checkerboardScaleX * 5;
                    float rightX = checkerboard.transform.position.x + checkerboardScaleX * 5;
                    float leftZ = checkerboard.transform.position.z - checkerboardScaleZ * 5;
                    float rightZ = checkerboard.transform.position.z + checkerboardScaleZ * 5;
                    
                    if (pos.x < leftX) pos.x = leftX;
                    else if (pos.x > rightX) pos.x = rightX;
                    if (pos.z < leftZ) pos.z = leftZ;
                    else if (pos.z > rightZ) pos.z = rightZ;

                    this.gameObject.transform.position = pos;
                }
            }
        }
    }

    private Vector3 GetNearestPlace(Vector3 pos)
    {
        Vector3 res = placePoints[0];
        float distance = Vector3.Distance(pos, res);

        for (int i = 1; i < placePoints.Count; i++)
        {
            float tempDist = Vector3.Distance(pos, placePoints[i]);
            if (tempDist < distance)
            {
                res = placePoints[i];
                distance = tempDist;
            }
        }

        return res;
    }

    // 当鼠标释放时调用
    void OnMouseUp()
    {
        transform.position = GetNearestPlace(transform.position);
        isDragging = false;

        hexGrid.SetActive(false);
    }
}
