using UnityEngine;

public class PlayerAimAndWeapon : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Aiming Setup")]
    public Transform handPivot;       // ลาก HandLeft_WeaponPivot มาใส่ที่นี่
    public SpriteRenderer bodySprite; // ลาก Body (SpriteRenderer) มาใส่ที่นี่

    [Header("Weapon Settings")]
    public Transform firePoint;     // ตำแหน่งปลายปืน
    public GameObject bulletPrefab; // Prefab ของกระสุน
    public float fireRate = 0.15f;   // ความเร็วในการยิง (วินาที/นัด)
    public float bulletForce = 20f;  // ความเร็วพุ่งของกระสุน
    public int damage = 10;          // ความเสียหาย

    private Rigidbody2D rb;
    private Camera mainCam;
    private Vector2 moveInput;
    private Vector2 mousePos;
    private float nextFireTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;

        if (rb != null)
        {
            rb.freezeRotation = true; // ป้องกันไม่ให้ตัวละครหมุนติ้วด้วยระบบ Physics
        }

        // ปรับให้สไปรต์ปืนและมือลอยอยู่หน้าตัวละครเสมอ
        SetHandSortingOrder();
    }

    void Update()
    {
        // 1. รับค่าการเดิน (WASD / Arrow Keys)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // 2. รับตำแหน่งเมาส์
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // 3. ปุ่มยิงปืน (คลิกซ้ายค้าง)
        HandleShooting();
    }

    void FixedUpdate()
    {
        // คำนวณการเดิน
        rb.linearVelocity = moveInput.normalized * moveSpeed;

        // คำนวณการหมุนมือเล็งตามเมาส์
        HandleAiming();
    }
void HandleAiming()
    {
        if (handPivot == null || bodySprite == null) return;

        // คำนวณทิศทางและมุมจาก Pivot ไปยังตำแหน่งเมาส์
        Vector2 aimDir = mousePos - (Vector2)handPivot.position;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        // หมุน handPivot ตามมุมที่คำนวณได้เสมอ
        handPivot.rotation = Quaternion.Euler(0f, 0f, angle);

        // --- ส่วนที่แก้ไขเพื่อแก้ปัญหากลับหัว ---

        if (aimDir.x < 0) // เมื่อหันเมาส์ไปทางซ้าย
        {
            bodySprite.flipX = true; // หันลำตัวไปทางซ้าย

            // พลิก Scale แนวแกน Y ของมือ/ปืน ให้เป็น -1 เพื่อแก้ปืนกลับหัว
            handPivot.localScale = new Vector3(1f, -1f, 1f);
        }
        else // เมื่อหันเมาส์ไปทางขวา (รวมถึงด้านบน/ล่างที่เยื้องขวา)
        {
            bodySprite.flipX = false; // หันลำตัวไปทางขวา

            // กลับ Scale Y เป็น 1 ตามปกติ
            handPivot.localScale = new Vector3(1f, 1f, 1f);
        }
        // ------------------------------------
    }

    void HandleShooting()
    {
        // ยิงปืนเมื่อกดคลิกซ้าย
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // สร้างกระสุน
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // ใส่ความแรงให้กระสุน
        Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(firePoint.right * bulletForce, ForceMode2D.Impulse);
        }

        // ส่งค่า Damage ให้กระสุน (ถ้ามีสคริปต์ Bullet)
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = damage;
        }
    }

    void SetHandSortingOrder()
    {
        // กำหนดให้สไปรต์ใน handPivot ทั้งหมดมี Order in Layer สูงกว่าตัวละครหลัก
        if (handPivot != null && bodySprite != null)
        {
            int targetOrder = bodySprite.sortingOrder + 1;
            SpriteRenderer[] handSprites = handPivot.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sr in handSprites)
            {
                sr.sortingLayerID = bodySprite.sortingLayerID;
                sr.sortingOrder = targetOrder;
            }
        }
    }
}