using UnityEngine;

/// <summary>
/// Контейнер данных для настройки оружия. Создаётся как ассет в папке _Data.
/// </summary>
[CreateAssetMenu(fileName = "WeaponData", menuName = "WeaponSystem/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Основные параметры")]
    public string weaponName = "Новое оружие";
    public float damage = 10f;
    public float fireRate = 0.2f;      // интервал между выстрелами (сек)
    public int magazineSize = 30;
    public int totalReserve = 90;      // общий запас патронов

    [Header("Точность и отдача")]
    public float spread = 0.5f;        // угловой разброс
    public float recoilForce = 2f;     // сила подброса прицела

    [Header("Аудио и визуал")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public GameObject muzzleFlashPrefab;
    public GameObject projectilePrefab; // опционально: физические пули
}
