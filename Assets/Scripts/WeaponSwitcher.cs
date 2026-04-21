using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Оружие")]
    public Weapon[] weapons;
    private int currentIndex = 0;

    void Start()
    {
        // Активируем только первое оружие, остальные скрываем
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == 0);
        }
    }

    void Update()
    {
        // Колёсико мыши (циклическое переключение)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int newIndex = currentIndex + (scroll > 0 ? 1 : -1);
            if (newIndex < 0) newIndex = weapons.Length - 1;
            if (newIndex >= weapons.Length) newIndex = 0;
            SwitchTo(newIndex);
        }

        // Клавиши 1, 2, 3
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchTo(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Length > 1) SwitchTo(1);
        if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Length > 2) SwitchTo(2);
    }

    void SwitchTo(int index)
    {
        if (index == currentIndex) return;

        // Отключаем текущее оружие
        weapons[currentIndex].gameObject.SetActive(false);
        // Включаем новое
        weapons[index].gameObject.SetActive(true);
        currentIndex = index;
    }
}