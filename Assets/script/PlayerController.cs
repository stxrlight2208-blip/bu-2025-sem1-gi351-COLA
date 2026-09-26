using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Shooting Settings")]
    public Transform firePoint;     // จุดที่กระสุนจะออกจากตัวละคร (เช่น ปลายกระบอกปืน)
    public GameObject bulletPrefab; // Prefab ของกระสุน
    public float bulletForce = 20f;
    public float fireRate = 0.15f;  // ความเร็วในการยิงอัตโนมัติ

    private Rigidbody2D rb;
    private Camera mainCam;
    private Vector2 moveInput;
    private Vector2 mousePos;
    private float nextFireTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    void Update()
    {
        // 1. รับค่าการเคลื่อนที่ (WASD)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized; // ทำให้น้ำหนักการเดินเฉียงเท่ากับการเดินตรง

        // 2. รับตำแหน่งเมาส์บนหน้าจอ (World Position)
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // 3. ระบบยิง (คลิกซ้ายค้างเพื่อยิงรัว)
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void FixedUpdate()
    {
        // คำนวณการเคลื่อนที่ด้วย Rigidbody
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);

        // คำนวณมุมหันหน้าไปทางเมาส์
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            
            if (bulletRb != null)
            {
                bulletRb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
            }
        }
    }
}