using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [Header("Data")]
    public WeaponData data;
    public Transform firePoint;

    [Header("UI")]
    public Text ammoText;

    [Header("Отдача")]
    public float recoilRecoverySpeed = 5f; // Скорость возврата прицела

    private int currentAmmo;
    private int currentReserve;
    private float nextFireTime;
    private bool isReloading;
    private Camera playerCamera;

    // Переменные для контроля отдачи
    private Vector3 cameraBaseRotation;
    private float currentRecoil = 0f;

    void Start()
    {
        playerCamera = Camera.main;
        cameraBaseRotation = playerCamera.transform.localEulerAngles; // Запоминаем стартовый угол
        currentAmmo = data.magazineSize;
        currentReserve = data.totalReserve;
        UpdateUI();
    }

    void Update()
    {
        if (isReloading) return;

        // Стрельба
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + data.fireRate;
        }

        // Перезарядка
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < data.magazineSize && currentReserve > 0)
        {
            StartCoroutine(Reload());
        }

        // Возврат отдачи (плавное опускание прицела)
        if (currentRecoil > 0.01f)
        {
            currentRecoil = Mathf.Lerp(currentRecoil, 0f, Time.deltaTime * recoilRecoverySpeed);

            // Применяем только отклонение по X, не сбрасывая поворот игрока
            Vector3 targetRot = cameraBaseRotation;
            targetRot.x -= currentRecoil;
            playerCamera.transform.localEulerAngles = targetRot;
        }
        else if (currentRecoil > 0f)
        {
            currentRecoil = 0f;
        }
    }

    void Shoot()
    {
        currentAmmo--;
        UpdateUI();

        // Звук выстрела
        if (data.shootSound != null)
            AudioSource.PlayClipAtPoint(data.shootSound, transform.position);

        // Вспышка
        if (data.muzzleFlashPrefab != null)
            Instantiate(data.muzzleFlashPrefab, firePoint != null ? firePoint.position : transform.position, Quaternion.identity);

        // Логика дробовика: если имя "Shotgun", выпускаем 8 лучей, иначе 1
        int pellets = (data.weaponName == "Shotgun") ? 8 : 1;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = playerCamera.transform.forward;
            direction += Random.insideUnitSphere * data.spread;
            direction.Normalize();

            if (Physics.Raycast(playerCamera.transform.position, direction, out RaycastHit hit, 100f))
            {
                Debug.DrawLine(playerCamera.transform.position, hit.point, Color.red, 2f);

                EnemyTarget enemy = hit.collider.GetComponent<EnemyTarget>();
                if (enemy != null)
                {
                    enemy.TakeDamage(data.damage);
                }
            }
        }

        // Отдача
        currentRecoil += data.recoilForce;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        if (data.reloadSound != null)
            AudioSource.PlayClipAtPoint(data.reloadSound, transform.position);

        yield return new WaitForSeconds(2f);

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

    // Фикс зависания UI и перезарядки при смене оружия
    private void OnDisable()
    {
        StopAllCoroutines();
        isReloading = false;
    }

    private void OnEnable()
    {
        UpdateUI();
    }
}