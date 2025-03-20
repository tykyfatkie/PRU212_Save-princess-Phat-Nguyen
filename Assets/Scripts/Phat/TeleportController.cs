using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeleportController : MonoBehaviour
{

    [SerializeField] GameObject Cong;


    void Start()
    {
        
    }

    void Update()
    {
        if (Cong != null)
        {
            transform.position = Cong.GetComponent<CongDichChuyen>().GetDiemDichChuyenDen().position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CongDichChuyen")) 
        {
            Cong = collision.gameObject; 
        }
    }   
            
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("CongDichChuyen")) 
        {
            Cong = null;
        }
    }

}
