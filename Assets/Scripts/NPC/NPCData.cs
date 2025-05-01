using System.Collections;
using UnityEngine;

public class NPCData : MonoBehaviour
{
    public bool rohan;
    public bool hasIllegalItem;
    public float confessChance; // �tiraf etme ihtimali (0.0 - 1.0 aras� de�er)
    public bool G�rg�z;
    public bool isScanned;
    
    private void Start()
    {
        ResetData();
        
    }

    public void ResetData()
    {
        isScanned = false;
        rohan = false;
        // 20% �ansla illegal item var
        hasIllegalItem = Random.value < 0.20f;

        G�rg�z = Random.value < 0.10f;

        // �tiraf etme ihtimali: 30% ile 80% aras�nda rastgele ayarlan�yor
        confessChance = Random.Range(0.3f, 0.8f);
        
    }
}
