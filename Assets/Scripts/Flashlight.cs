using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject spotLight;
    private bool isOn = false;

    void Start()
    {
        spotLight.SetActive(false);
    }

    void Update()
    {
        if (!isOn)
        {
            // Fenerin �����n� a�ma
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                spotLight.SetActive(true);
                isOn = true;
            }
        }
        else
        {
            // Fenerin �����n� kapatma
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                spotLight.SetActive(false);
                isOn = false;
            }
        }
    }
}
