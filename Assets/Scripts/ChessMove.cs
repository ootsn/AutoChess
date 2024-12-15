using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessMove : MonoBehaviour
{
    //做出售功能，临时替代棋子的费用，之后完善棋子属性后记得替换变量
    private readonly int a = 10;

    private bool isDragging = false;
    private Vector3 dragOrigin; // 鼠标按下时的物体位置
    private bool inHexGrid = false;
    private int posIndex;
    private ChessControl controller;
    private ChessShop shop;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = shop.mainCamera;
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

    // 当鼠标按下时调用
    void OnMouseDown()
    {
        isDragging = true;

        //controller.hexGrid.SetActive(true);
        controller.hexGrid.ActivateMyHexGrid();

        dragOrigin = transform.position;

        shop.DisplaySellingInterface(a);
    }

    // 当鼠标拖动时调用
    void OnMouseDrag()
    {
        if (isDragging)
        {
            // 获取鼠标在世界空间中的位置
            Vector3 mousePosition = Input.mousePosition;
            //Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit))
            //{
            //    //Debug.DrawLine(mainCamera.transform.position, hit.point, Color.magenta);
            //    this.gameObject.transform.position = hit.point;
            //    //Debug.DrawRay(ray.origin, ray.direction);
            //}
            //else
            //{
            //    Vector3 palneNormal = controller.checkerboard.transform.up;
            //    Vector3 planePoint = new Vector3(0, 0, 0);
            //    Vector3 linePoint = ray.origin;
            //    Vector3 lineDir = ray.direction;
            //    float a = Vector3.Dot((planePoint - linePoint), palneNormal);
            //    float b = Vector3.Dot(lineDir, palneNormal);
            //    if (a != 0 && b != 0)
            //    {
            //        Vector3 pos = a / b * lineDir + linePoint;
            //        Debug.DrawLine(pos, new Vector3(pos.x, pos.y + 30, pos.z), Color.blue);

            //        float checkerboardScaleX = controller.checkerboard.transform.localScale.x;
            //        float checkerboardScaleZ = controller.checkerboard.transform.localScale.z;
            //        float leftX = controller.checkerboard.transform.position.x - checkerboardScaleX * 5;
            //        float rightX = controller.checkerboard.transform.position.x + checkerboardScaleX * 5;
            //        float leftZ = controller.checkerboard.transform.position.z - checkerboardScaleZ * 5;
            //        float rightZ = controller.checkerboard.transform.position.z + checkerboardScaleZ * 5;

            //        if (pos.x < leftX) pos.x = leftX;
            //        else if (pos.x > rightX) pos.x = rightX;
            //        if (pos.z < leftZ) pos.z = leftZ;
            //        else if (pos.z > rightZ) pos.z = rightZ;

            //        this.gameObject.transform.position = pos;
            //    }
            //}

            Vector3 palneNormal = controller.checkerboard.transform.up;
            Vector3 planePoint = new Vector3(0, 0, 0);
            Vector3 linePoint = ray.origin;
            Vector3 lineDir = ray.direction;
            float a = Vector3.Dot((planePoint - linePoint), palneNormal);
            float b = Vector3.Dot(lineDir, palneNormal);
            if (a != 0 && b != 0)
            {
                Vector3 pos = a / b * lineDir + linePoint;
                //Debug.DrawLine(pos, new Vector3(pos.x, pos.y + 30, pos.z), Color.blue);

                float checkerboardScaleX = controller.checkerboard.transform.localScale.x;
                float checkerboardScaleZ = controller.checkerboard.transform.localScale.z;
                float leftX = controller.checkerboard.transform.position.x - checkerboardScaleX * 5f;
                float rightX = controller.checkerboard.transform.position.x + checkerboardScaleX * 5f;
                float leftZ = controller.checkerboard.transform.position.z - checkerboardScaleZ * 5f - 5f;
                float rightZ = controller.checkerboard.transform.position.z + checkerboardScaleZ * 5f;

                if (pos.x < leftX) pos.x = leftX;
                else if (pos.x > rightX) pos.x = rightX;
                if (pos.z < leftZ) pos.z = leftZ;
                else if (pos.z > rightZ) pos.z = rightZ;

                this.gameObject.transform.position = pos;
            }
        }
    }

    private float GetNearestCoordinate(Vector3 pos, Vector3[] positions, out Vector3 result, out int posIndex)
    {
        result = positions[0];
        float distance = Vector3.Distance(pos, result);

        posIndex = 0;
        for (int i = 1; i < positions.Length; i++)
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

    // 当鼠标释放时调用
    void OnMouseUp()
    {
        if (!shop.Sell(this.gameObject, a)) 
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
        }

        isDragging = false;
        //controller.hexGrid.SetActive(false);
        controller.hexGrid.DeactivateMyHexGrid();

        shop.DisplayPurchaseInterface();
    }

    public void SetController(ChessControl controller)
    {
        this.controller = controller;
    }

    public void SetShop(ChessShop shop)
    {
        this.shop = shop;
    }
}
