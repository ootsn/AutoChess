using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessMove : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 dragOrigin; // ��갴��ʱ������λ��
    private Vector3 offset; // ��갴��ʱ�����λ��������λ�õ�ƫ��
    private bool inHexGrid = false;
    private int posIndex;
    private ChessControl controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosIndex(int posIndex)
    {
        this.posIndex = posIndex;
    }

    void clearLastPosition()
    {
        if (inHexGrid)
        {
            controller.hexGridAvailable[posIndex] = true;
        }
        else
        {
            controller.reserveSeatAvailable[posIndex] = true;
        }
    }

    // ����갴��ʱ����
    void OnMouseDown()
    {
        isDragging = true;

        controller.hexGrid.SetActive(true);

        dragOrigin = transform.position;
    }

    // ������϶�ʱ����
    void OnMouseDrag()
    {
        if (isDragging)
        {
            // ��ȡ���������ռ��е�λ��
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
                Vector3 palneNormal = controller.checkerboard.transform.up;
                Vector3 planePoint = new Vector3(0, 0, 0);
                Vector3 linePoint = ray.origin;
                Vector3 lineDir = ray.direction;
                float a = Vector3.Dot((planePoint - linePoint), palneNormal);
                float b = Vector3.Dot(lineDir, palneNormal);
                if (a != 0 && b != 0)
                {
                    Vector3 pos = a / b * lineDir + linePoint;
                    Debug.DrawLine(pos, new Vector3(pos.x, pos.y + 30, pos.z), Color.blue);

                    float checkerboardScaleX = controller.checkerboard.transform.localScale.x;
                    float checkerboardScaleZ = controller.checkerboard.transform.localScale.z;
                    float leftX = controller.checkerboard.transform.position.x - checkerboardScaleX * 5;
                    float rightX = controller.checkerboard.transform.position.x + checkerboardScaleX * 5;
                    float leftZ = controller.checkerboard.transform.position.z - checkerboardScaleZ * 5;
                    float rightZ = controller.checkerboard.transform.position.z + checkerboardScaleZ * 5;

                    if (pos.x < leftX) pos.x = leftX;
                    else if (pos.x > rightX) pos.x = rightX;
                    if (pos.z < leftZ) pos.z = leftZ;
                    else if (pos.z > rightZ) pos.z = rightZ;

                    this.gameObject.transform.position = pos;
                }
            }
        }
    }

    private float GetNearestCoordinate(Vector3 pos, List<Vector3> positions, out Vector3 result, out int posIndex)
    {
        result = positions[0];
        float distance = Vector3.Distance(pos, result);

        posIndex = 0;
        for (int i = 1; i < positions.Count; i++)
        {
            float tempDist = Vector3.Distance(pos, positions[i]);
            if (tempDist < distance)
            {
                posIndex = i;
                result = positions[i];
                distance = tempDist;
            }
        }

        return distance;
    }

    // ������ͷ�ʱ����
    void OnMouseUp()
    {
        Vector3 pos1, pos2;
        int posIndex1, posIndex2;
        if (GetNearestCoordinate(transform.position, controller.hexGridCoordinate, out pos1, out posIndex1) < GetNearestCoordinate(transform.position, controller.reserveSeatCoordinate, out pos2, out posIndex2))
        {
            if (controller.hexGridAvailable[posIndex1])
            {
                clearLastPosition();
                transform.position = pos1;
                controller.hexGridAvailable[posIndex1] = false;
                inHexGrid = true;
                posIndex = posIndex1;
            }
            else
            {
                transform.position = dragOrigin;
            }
        }
        else
        {
            if (controller.reserveSeatAvailable[posIndex2])
            {
                clearLastPosition();
                transform.position = pos2;
                controller.reserveSeatAvailable[posIndex2] = false;
                inHexGrid = false;
                posIndex = posIndex2;
            }
            else
            {
                transform.position = dragOrigin;
            }
        }

        isDragging = false;

        controller.hexGrid.SetActive(false);
    }

    public void SetController(ChessControl controller)
    {
        this.controller = controller;
    }
}