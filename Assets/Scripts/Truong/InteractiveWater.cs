using System;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;


[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer),typeof(EdgeCollider2D))]
[RequireComponent(typeof(WarterTriger))]
public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Mesh Generation")] [Range(2, 500)]
    public int NumOfXVertices = 70;

    public float Width = 10f;
    public float Height = 4f;
    public Material WaterMaterial;
    private const int NUM_OF_Y_VERTICES = 2;

    [Header("GIzmo")] public Color GizmoColor = Color.white;

    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private Vector3[] _vertices;

    private int[] _topVerticesIndex;
    
    private EdgeCollider2D _coll;

    private void Reset()
    {
        _coll = GetComponent<EdgeCollider2D>();
        _coll.isTrigger = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}