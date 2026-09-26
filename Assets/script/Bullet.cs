using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float destroyTime = 2f;
    public int damage = 10; // <<-- เพิ่มบรรทัดนี้เข้าไป

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // เมื่อกระสุนชนศัตรู
        if (hitInfo.CompareTag("Enemy"))
        {
            // สามารถส่งค่า damage ไปทำความเสียหายใส่ศัตรูได้ที่นี่
            Destroy(gameObject);
        }
        else if (hitInfo.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
