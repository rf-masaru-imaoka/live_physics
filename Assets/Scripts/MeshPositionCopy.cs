using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshPositionCopy : MonoBehaviour
{
    public SkinnedMeshRenderer renderer;

    public Cloth cloth;

    public GameObject skirtRoot;


    public List<int> vertexIndecies = new List<int>();
    public List<string> boneIndeciesName = new List<string>();

    public Vector3[] vertices;

    [Range(0,24)]
    public int SelectMeshIndex;


    public class BoneStruct
    {
        // GameObject
        public GameObject gameObject;
        public Vector3 originalPosition;
        // 初期ボーンの向き
        public Vector3 boneAxis;
        // 初期ボーンの長さ
        public float boneLength;
        // 計算後位置
        public Vector3 calcPosition;
        public Vector3 tempPosition;
        // 親BoneStruct
        public BoneStruct child;
        // クロスメッシュ頂点位置
        public Vector3 calcClothPosition;

        // メッシュのインデックス
        public int meshIndex;

        // ボーンの初期姿勢
        public Quaternion localRotation;


        public BoneStruct(GameObject go)
        {
            gameObject = go;
            originalPosition = gameObject.transform.position;
            localRotation = gameObject.transform.localRotation;

            Transform child = null;
            foreach (Transform ch in go.transform)
            {
                child = ch;
            }
            if ( child != null )
            {
                boneAxis = (child.transform.position - go.transform.position).normalized;
                boneLength = Vector3.Distance(go.transform.position, child.position);
            }
        }
    };

    private List<BoneStruct> boneStructs = new List<BoneStruct>();

    private Quaternion localRotation;


    private void Awake()
    {
    }

    void Start()
    {
        vertices = cloth.vertices;

        // ボーンのジョイントとの距離を調べて最も近い頂点をジョイントに反映する

        // 1.全ボーン取得
        GetChildren(skirtRoot, null, ref boneStructs);
        foreach (BoneStruct bs in boneStructs)
        {
            //Debug.Log(bs.gameObject.name);
        }

        // 2.全ボーンと全頂点との距離計算
        for (int i = 0; i < boneStructs.Count; i++)
        {
            int nearestVertexIndex = -1;
            float nearestVertexLength = 1000000.0f;
            for (int j = 0; j < vertices.Length; j++)
            {

                Vector3 v = skirtRoot.transform.TransformPoint(vertices[j]);
                float length = Vector3.Distance(boneStructs[i].gameObject.transform.position, v);
                if (length < nearestVertexLength)
                {
                    nearestVertexLength = length;
                    nearestVertexIndex = j;
                }
            }
            vertexIndecies.Add(nearestVertexIndex);
            boneIndeciesName.Add(boneStructs[nearestVertexIndex].gameObject.name);
            boneStructs[i].meshIndex = nearestVertexIndex;
        }

    }

    private void GetChildren(GameObject go, BoneStruct parent, ref List<BoneStruct> boneStructs)
    {
        Transform children = go.GetComponentInChildren<Transform>();
        if (children.childCount == 0)
        {
            return;
        }
        foreach (Transform tf in children)
        {
            BoneStruct bs = null;
            if (tf.name.Contains("P_Cos_") == true)
            {
                bs = new BoneStruct(tf.gameObject);
                if ( parent != null )
                {
                    parent.child = bs;
                }
                boneStructs.Add(bs);
                GetChildren(tf.gameObject, bs, ref boneStructs);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y,transform.position.z + 0.002f);
    }

    private void LateUpdate()
    {
        vertices = cloth.vertices;
        // 頂点コピー（メッシュ頂点⇒ボーン座標）
        for (int i = 0; i < boneStructs.Count; i++)
        {
            int vertexIndex = boneStructs[i].meshIndex;
            Vector3 v = vertices[vertexIndex];
            boneStructs[i].calcClothPosition = skirtRoot.transform.TransformPoint(v);

        }
        //return;
        // ボーン座標から姿勢計算（親から子へ辿る）
        for (int i = 0; i < boneStructs.Count; i++)
        {
            //if (i > 2*2) continue;
            //if (i % 3 == 0) continue;
            //if (i % 3 == 1) continue;

            BoneStruct self = boneStructs[i];
            BoneStruct child = boneStructs[i].child;
            Vector3 selfPosition = self.gameObject.transform.position;
            if (child == null) continue;

            // 姿勢を初期化
            self.gameObject.transform.localRotation = boneStructs[i].localRotation;


            // 正規化（元のボーンの長さにする）
            if (self.boneLength != 0)
            {
                child.tempPosition = ((child.calcClothPosition - selfPosition).normalized * self.boneLength) + self.calcClothPosition;// selfPosition;
            }

            // 位置から回転を求める
            Vector3 aimVector = self.gameObject.transform.TransformDirection(self.boneAxis);
            Quaternion aimRotation = Quaternion.FromToRotation(aimVector, child.tempPosition - selfPosition);
            self.gameObject.transform.rotation = aimRotation * self.gameObject.transform.rotation;
        }
    }

    Vector3 offset = new Vector3(0.001f, 0.001f, 0.001f);

    void OnDrawGizmosSelected()
    {
        {
            Vector3 v0 = vertices[SelectMeshIndex];
            Vector3 v1 = skirtRoot.transform.TransformPoint(v0);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(v1+offset * 3, 0.01f);
        }

        // ボーンの位置表示
        for (int i = 0; i < boneStructs.Count; i++)
        {
            if (i%3 == 0) continue;
            Gizmos.color = Color.white;
            if (boneStructs[i].child != null)
            {
                // オリジナルボーン
                Gizmos.color = Color.white;
                Vector3 pos0 = boneStructs[i].gameObject.transform.position;
                Vector3 pos1 = boneStructs[i].gameObject.transform.position + boneStructs[i].boneAxis * boneStructs[i].boneLength;
                Gizmos.DrawLine(pos0, pos1);
                Gizmos.color = Color.black;
                // parent
                Gizmos.DrawSphere(boneStructs[i].gameObject.transform.position, 0.01f);
                // parent original
                //Gizmos.DrawSphere(boneStructs[i].originalPosition - offset, 0.01f);
                // child
                Gizmos.DrawSphere(boneStructs[i].child.gameObject.transform.position, 0.01f);
                // child original
                //Gizmos.DrawSphere(boneStructs[i].child.originalPosition, 0.01f);
                // child 計算結果（正規化）
                Gizmos.color = Color.grey;
                Gizmos.DrawSphere(boneStructs[i].child.tempPosition+offset*2, 0.01f);
                // child mesh vertex
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(boneStructs[i].child.calcClothPosition + offset * 2, 0.01f);
            }
        }


        return;

        // そもそも頂点とボーンの繋がりあっている？
        for (int i = 0; i < boneStructs.Count; i++)
        {
            if (i != 0) continue;
            int vertexIndex = boneStructs[i].meshIndex;
            Vector3 v0 = vertices[vertexIndex];
            Vector3 v1 = skirtRoot.transform.TransformPoint(v0);
            boneStructs[i].calcClothPosition = v1;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(v0, 0.01f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(v1+offset, 0.01f);
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(boneStructs[i].gameObject.transform.position, 0.01f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(boneStructs[i].child.gameObject.transform.position, 0.01f);
        }

        return;





        // Cloth計算結果表示
        vertices = cloth.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(vertices[i], 0.01f);
        }
        // Hipsの姿勢でWorldに変換
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];
            Vector3 v1 = skirtRoot.transform.TransformPoint(v);
            Gizmos.color = Color.blue;
            //Gizmos.DrawSphere(v1, 0.01f);
        }

        for (int i = 0; i < boneStructs.Count; i++)
        {
            //if (i != 1) continue;

            BoneStruct self = boneStructs[i];
            BoneStruct child = boneStructs[i].child;
            Vector3 v0 = self.gameObject.transform.position;
            //Vector3 v1 = skirtRoot.transform.TransformPoint(v0);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(v0, 0.01f);
            //Gizmos.DrawSphere(child.calcClothPosition, 0.01f);

        }


        return;
        for (int i = 0; i < boneStructs.Count; i++)
        {
            if (i != 1) continue;

            //Gizmos.DrawSphere(boneStructs[i].calcPosition, 0.01f);
            if (boneStructs[i].child == null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(boneStructs[i].gameObject.transform.position, 0.01f);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(boneStructs[i].gameObject.transform.position+ offset, 0.01f);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(boneStructs[i].calcPosition+ offset, 0.01f);
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(boneStructs[i].calcClothPosition+ offset, 0.01f);
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(boneStructs[i].child.calcClothPosition+ offset, 0.01f);
            }
        }
    }
}
