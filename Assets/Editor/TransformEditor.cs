using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

[CustomEditor(typeof(Transform))]
public class TransformEditor : Editor
{
    public void DrawABetterInspector(Transform t)
    {
        EditorGUI.indentLevel = 0;
        Vector3 position = EditorGUILayout.Vector3Field("Position", t.localPosition);
        Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
        Vector3 scale = EditorGUILayout.Vector3Field("Scale", t.localScale);

        if (GUI.changed)
        {
            Undo.RecordObject(t, "Transform Change");

            t.localPosition = FixIfNaN(position);
            t.localEulerAngles = FixIfNaN(eulerAngles);
            t.localScale = FixIfNaN(scale);
        }
    }

    private Vector3 FixIfNaN(Vector3 v)
    {
        if (float.IsNaN(v.x))
        {
            v.x = 0.0f;
        }
        if (float.IsNaN(v.y))
        {
            v.y = 0.0f;
        }
        if (float.IsNaN(v.z))
        {
            v.z = 0.0f;
        }
        return v;
    }

    public override void OnInspectorGUI()
    {
        Transform t = (Transform)target;

        DrawABetterInspector(t);

        if (GUILayout.Button("Save"))
        {
            SaveData(t.gameObject);
        }

        if (GUILayout.Button("Load"))
        {
            LoadData(t.gameObject);
        }

    }

    string GetInstanceFileName(GameObject baseObject)
    {
        string filePath = System.IO.Path.Combine(Application.dataPath, "Temp", baseObject.name + "_" + baseObject.GetInstanceID() + "_keepTransform.txt");
        Debug.Log("file path: " + filePath);
        return filePath;
    }

    public void SaveData(GameObject baseObject)
    {
        List<float> data = new List<float>();

        data.Add(baseObject.transform.localPosition.x);
        data.Add(baseObject.transform.localPosition.y);
        data.Add(baseObject.transform.localPosition.z);

        data.Add(baseObject.transform.localRotation.eulerAngles.x);
        data.Add(baseObject.transform.localRotation.eulerAngles.y);
        data.Add(baseObject.transform.localRotation.eulerAngles.z);

        data.Add(baseObject.transform.localScale.x);
        data.Add(baseObject.transform.localScale.y);
        data.Add(baseObject.transform.localScale.z);

        System.IO.File.WriteAllBytes(GetInstanceFileName(baseObject), FloatListToByteArray(data));

        Debug.Log("floatList: " + string.Join(", ", data.Select(f => f.ToString())));
    }

    public void LoadData(GameObject baseObject)
    {
        try
        {
            string filePath = GetInstanceFileName(baseObject);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            List<float> data = ByteArrayToFloatList(bytes);
            if (data.Count > 0)
            {
                baseObject.transform.localPosition = new Vector3(data[0], data[1], data[2]);
                baseObject.transform.localRotation = Quaternion.Euler(data[3], data[4], data[5]);
                baseObject.transform.localScale = new Vector3(data[6], data[7], data[8]);
                System.IO.File.Delete(filePath);
                System.IO.File.Delete(filePath + ".meta");

            }

            Debug.Log("floatList: " + string.Join(", ", data.Select(f => f.ToString())));
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private byte[] FloatListToByteArray(List<float> floatList)
    {
        // 计算总字节数
        int totalBytes = floatList.Count * sizeof(float);
        byte[] byteArray = new byte[totalBytes];

        // 使用 Buffer.BlockCopy 将每个 float 复制到 byte 数组中
        int offset = 0;
        foreach (float f in floatList)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(f), 0, byteArray, offset, sizeof(float));
            offset += sizeof(float);
        }

        return byteArray;
    }

    private List<float> ByteArrayToFloatList(byte[] byteArray)
    {
        List<float> floatList = new List<float>();

        // 确保字节数组长度是 4 的倍数（每个 float 4 个字节）
        if (byteArray.Length % sizeof(float) != 0)
        {
            throw new ArgumentException("Byte array length is not a multiple of 4.");
        }

        // 使用 BitConverter.ToSingle 解析每个 float
        for (int i = 0; i < byteArray.Length; i += sizeof(float))
        {
            // 注意：BitConverter.ToSingle 假设字节数组是以小端序（least significant byte first）存储的
            // 如果你的字节数组是以大端序（most significant byte first）存储的，你需要先反转这四个字节的顺序
            float f = BitConverter.ToSingle(byteArray, i);
            floatList.Add(f);
        }

        return floatList;
    }
}