using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Data")]
    public WeaponData data;          // Ссылка на ScriptableObject
    public Transform firePoint;      // Точка выстрела (опционально, если нет - будет камера)

    [Header("UI")]
    public Text ammoText;            // Поле UI для отображения патронов

    private int currentAmmo;
    private int currentReserve;
    private float nextFireTime;
    private bool isReloading;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        currentAmmo = data.magazineSize;
        currentReserve = data.totalReserve;
        UpdateUI();
    }

    void Update()
    {
        if (isReloading) return;

        // Стрельба (ЛКМ или пробел, зависит от Input Manager)
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + data.fireRate;
        }

        // Перезарядка (клавиша R)
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < data.magazineSize && currentReserve > 0)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        currentAmmo--;
        UpdateUI();

        // Звук выстрела
        if (data.shootSound != null)
            AudioSource.PlayClipAtPoint(data.shootSound, transform.position);

        // Визуальная вспышка (если назначен префаб)
        if (data.muzzleFlashPrefab != null)
            Instantiate(data.muzzleFlashPrefab, firePoint != null ? firePoint.position : transform.position, Quaternion.identity);

        // Расчёт направления с разбросом
        Vector3 direction = playerCamera.transform.forward;
        direction += Random.insideUnitSphere * data.spread;
        direction.Normalize();

        // Raycast (луч)
        if (Physics.Raycast(playerCamera.transform.position, direction, out RaycastHit hit, 100f))
        {
            Debug.DrawLine(playerCamera.transform.position, hit.point, Color.red, 2f); // Визуализация для отладки

            // Попытка нанести урон (если на объекте есть скрипт EnemyTarget)
            EnemyTarget enemy = hit.collider.GetComponent<EnemyTarget>();
            if (enemy != null)
            {
                enemy.TakeDamage(data.damage);
            }
        }

        // Отдача камеры
        playerCamera.transform.Rotate(-data.recoilForce, Random.Range(-1f, 1f), 0f);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        if (data.reloadSound != null)
            AudioSource.PlayClipAtPoint(data.reloadSound, transform.position);

        yield return new WaitForSeconds(2f); // Время перезарядки (позже вынесем в data)

        int needed = data.magazineSize - currentAmmo;
        int take = Mathf.Min(needed, currentReserve);
        currentAmmo += take;
        currentReserve -= take;

        isReloading = false;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo}/{currentReserve}";
    }
}