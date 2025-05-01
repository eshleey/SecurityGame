using UnityEngine;
using System.Collections;

public class CopAttack : MonoBehaviour
{
    private GameObject cop;
    private Animator copAnimation;

    public float rayDistance = 1f;
    public LayerMask npcLayer;
    private Camera mainCamera;

    public int loseScore = 0;
    public GameObject inventoryPanel;
    public float attackCooldown = 0.5f;
    private bool canAttack = true;

    // Yeni değişken: 'R' tuşuna basılmadan önce geçen süreyi kontrol etmek için
    public float resetCooldown = 2f; // Reset için cooldown süresi
    private bool isResetCooldownActive = false;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Panel açıkken animasyonu oynatmamayı sağla
        if (inventoryPanel != null && inventoryPanel.activeSelf)
        {
            return; // Eğer panel açıksa, animasyon başlamasın
        }

        // Animasyon çalarken panelin açılmasını engelle
        if (copAnimation != null && copAnimation.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            return; // Eğer "Attack" animasyonu çalıyorsa, panel açılmasına izin verme
        }

        // Her frame’de aktif cop'u bul
        FindActiveCop();

        // Eğer mouse'a tıklanmışsa ve cop animasyonu varsa, Attack animasyonunu tetikle
        if (Input.GetMouseButtonDown(0) && canAttack && copAnimation != null)
        {
            StartCoroutine(AttackRoutine());
        }

        // 'R' tuşuna basıldığında eşyayı sıfırla ve cooldown aktifse, sadece 1 kez basılabilir
        if (Input.GetKeyDown(KeyCode.R) && !isResetCooldownActive && !IsAttackAnimationPlaying())
        {
            ResetItem();
            StartCoroutine(ResetCooldownRoutine());
        }
    }

    void FindActiveCop()
    {
        // "Cop" tag'li aktif objeyi sahnede bul
        GameObject[] cops = GameObject.FindGameObjectsWithTag("Cop");
        foreach (GameObject obj in cops)
        {
            if (obj.activeInHierarchy)
            {
                if (cop == null || cop != obj) // Eğer yeni bir cop objesi aktifse, işlemi güncelle
                {
                    cop = obj;
                    copAnimation = cop.GetComponent<Animator>();
                }
                return;
            }
        }

        // Cop deaktifse, cop objesini null yap
        cop = null;
        copAnimation = null;
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;

        // Eğer Attack animasyonu çalmıyorsa, başlat
        if (copAnimation != null && !copAnimation.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            Debug.Log("Attack Trigger Tetiklendi");
            copAnimation.SetTrigger("Attack");
        }

        PerformRaycast();

        // Attack animasyonu bitene kadar bekleyelim (0.5 saniye gibi)
        yield return new WaitForSeconds(0.5f); // Burada animasyon süresi kadar bekleyebilirsin

        // Attack sonrası cooldown bekleyelim
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    void PerformRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, rayDistance, npcLayer))
        {
            if (hit.collider.CompareTag("NPC"))
            {
                NPCData npcData = hit.collider.GetComponent<NPCData>();
                if (npcData != null)
                {
                    if (!npcData.hasIllegalItem)
                    {
                        loseScore++;
                        Debug.Log("Skor arttı: " + loseScore);
                    }

                    npcData.ResetData();

                    NPCBehavior npcBehavior = hit.collider.GetComponent<NPCBehavior>();
                    if (npcBehavior != null)
                        npcBehavior.ForceExit();
                }
            }
        }
    }

    // 'R' tuşuna basıldığında her şeyi sıfırlayan fonksiyon
    void ResetItem()
    {
        Debug.Log("Skor sıfırlandı.");

        // Eğer cop objesi ve animasyonu varsa, animasyonu sıfırla
        if (cop != null && copAnimation != null)
        {
            canAttack = true;
            copAnimation.ResetTrigger("Attack"); // Attack trigger'ını sıfırla
            copAnimation.Play("Idle", 0, 0f); // Animasyonu baştan başlat
        }

        // Eğer inventoryPanel varsa, onu kapat
        if (inventoryPanel != null && inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
        }

        // NPC'lerin verilerini sıfırlama işlemi, gerekirse burada eklenebilir
        Debug.Log("Eşyalar sıfırlandı, animasyonlar ve panel sıfırlandı.");
    }

    // Attack animasyonunun oynayıp oynamadığını kontrol eden fonksiyon
    bool IsAttackAnimationPlaying()
    {
        // Eğer animasyon "Attack" ise, tuşa basılmasını engelle
        return copAnimation != null && copAnimation.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    }

    // 'R' tuşuna basılmadan önce geçen süreyi kontrol eden coroutine
    IEnumerator ResetCooldownRoutine()
    {
        isResetCooldownActive = true;
        yield return new WaitForSeconds(resetCooldown); // Belirtilen süre kadar bekle
        isResetCooldownActive = false; // Cooldown bitince, tekrar 'R' tuşuna basılabilir
    }
}
