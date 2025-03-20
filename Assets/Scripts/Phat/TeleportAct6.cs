using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongDichChuyen : MonoBehaviour
{
    [SerializeField] Transform diemDichChuyenDen; 

    public Transform GetDiemDichChuyenDen() 
    {
        return diemDichChuyenDen; 
    }
}
