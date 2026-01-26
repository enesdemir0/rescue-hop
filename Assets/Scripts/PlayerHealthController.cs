using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Health")]
    public int maxHealt;
    private int currentHealth;

    [Header("Settings")]
    public float invincibilityLength = 0.6f;
    public float flashTime = 0.05f;

    [Header("Visuals")]
    public GameObject[] modelDisplay;
    public Material damageMaterial;

    private float invincCounter;
    private float flashCounter;

    private List<Renderer> modelRenderers = new List<Renderer>();
    private List<Material[]> originalMaterials = new List<Material[]>();

    void Start()
    {
        // 1. Setup Renderers and Save Originals
        foreach (GameObject piece in modelDisplay)
        {
            Renderer rend = piece.GetComponent<Renderer>();
            if (rend != null)
            {
                modelRenderers.Add(rend);
                originalMaterials.Add(rend.materials);
            }
        }

        FillHealth();
    }

    void Update()
    {
        if (invincCounter > 0)
        {
            invincCounter -= Time.deltaTime;
            flashCounter -= Time.deltaTime;

            if (flashCounter <= 0)
            {
                flashCounter = flashTime;
                foreach (GameObject piece in modelDisplay)
                {
                    piece.SetActive(!piece.activeSelf);
                }
            }

            if (invincCounter <= 0)
            {
                // Animation Finished: Reset to Normal
                ResetVisuals();
            }
        }
    }

    public void DamagePlayer()
    {
        if (invincCounter <= 0)
        {
            // 1. Subtract Health FIRST
            currentHealth--;
            UIController.instance.UpdateHealthDisplay(currentHealth);

            // 2. Check if Dead
            if (currentHealth <= 0)
            {
                // PLAYER DIED
                // Do NOT start invincibility or red flash.
                // Just respawn immediately.
                invincCounter = 0;
                LevelManager.instance.Respawn();

            }
            else
            {
                AudioManager.instance.PlaySFX(12);
                // PLAYER SURVIVED
                // Now we can start the Red Flash animation
                invincCounter = invincibilityLength;
                flashCounter = 0;

                // Apply Red Material to EVERYTHING
                if (damageMaterial != null)
                {
                    for (int i = 0; i < modelRenderers.Count; i++)
                    {
                        Renderer rend = modelRenderers[i];
                        Material[] redMats = new Material[rend.materials.Length];
                        for (int x = 0; x < redMats.Length; x++)
                        {
                            redMats[x] = damageMaterial;
                        }
                        rend.materials = redMats;
                    }
                }
            }
        }
    }

    public void FillHealth()
    {
        currentHealth = maxHealt;
        UIController.instance.UpdateHealthDisplay(currentHealth);

        // Safety: When we refill health (respawn), force visuals to look normal
        // This prevents spawning as a red ghost
        ResetVisuals();
    }

    // Helper function to make the player look normal again
    private void ResetVisuals()
    {
        for (int i = 0; i < modelRenderers.Count; i++)
        {
            // Make visible
            modelRenderers[i].gameObject.SetActive(true);

            // Restore original materials
            if (i < originalMaterials.Count)
            {
                modelRenderers[i].materials = originalMaterials[i];
            }
        }
    }
}