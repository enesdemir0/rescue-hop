using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBattleController : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth;
    private int currentHealth;
    public Slider healthSlider;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public GameObject bossObject;
    public float waitBeforeSpawn;
    private int lastSpawn;
    public GameObject smokeEffect;

    [Header("Attack Settings")]
    public GameObject theShot;
    public Transform shotPoint;
    public float timeBetweenShots1, timeBetweenShots2, timeBetweenShots3;
    private float shotCounter;

    // Allows us to play dragon fire particles
    public ParticleSystem mouthFireEffect;

    [Header("Level Settings")]
    public GameObject levelExit;
    public string areaToUnlock;

    [Header("Components")]
    public Animator anim;

    // Logic flag to stop movement/shooting
    private bool isDeath;

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        // Ensure boss starts alive
        isDeath = false;

        if (smokeEffect != null)
        {
            Instantiate(smokeEffect, bossObject.transform.position, bossObject.transform.rotation);
            AudioManager.instance.PlaySFX(1);
        }


        shotCounter = timeBetweenShots1;
        AudioManager.instance.PlayMusic(0);
    }

    void Update()
    {
        if (bossObject.activeSelf)
        {
            // FIX 1: If dead, stop everything immediately.
            // This prevents him from rotating or shooting while the death animation plays.
            if (isDeath)
            {
                return;
            }

            // Look at player
            bossObject.transform.LookAt(PlayerController.instance.transform);
            bossObject.transform.rotation = Quaternion.Euler(0f, bossObject.transform.rotation.eulerAngles.y, 0f);

            // Attack Timer
            shotCounter -= Time.deltaTime;
            if (shotCounter <= 0)
            {
                // Start the smooth attack sequence
                StartCoroutine(ShootWithDelay());

                // Reset timer logic
                ReserShotCounter();
            }
        }
    }

    public void DamageBoss()
    {
        if (isDeath) return; // Don't take damage if already dying

        currentHealth--;

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            // FIX 2: Set Death Logic HERE.
            isDeath = true;

            // FIX 3: Use Trigger for Death to prevent looping
            if (anim != null)
            {
                anim.SetTrigger("die");
            }

            StartCoroutine(EndBattleCo());
        }
        else
        {
            StartCoroutine(SpawnCo());
        }

        healthSlider.value = currentHealth;
    }

    // This makes the shooting look much better (syncs with animation)
    IEnumerator ShootWithDelay()
    {
        // 1. Play Attack Animation
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }

        // 2. Play Fire Effect (if you have one assigned)
        if (mouthFireEffect != null)
        {
            mouthFireEffect.Play();
        }

        // 3. Wait for animation to open mouth (Adjust this number to fit your animation!)
        yield return new WaitForSeconds(0.5f);

        // 4. Spawn Bullet (Only if still alive and active)
        if (bossObject.activeSelf && !isDeath)
        {
            Instantiate(theShot, shotPoint.position, shotPoint.rotation);
        }
    }

    IEnumerator SpawnCo()
    {
        if (smokeEffect != null)
        {
            Instantiate(smokeEffect, bossObject.transform.position, bossObject.transform.rotation);
            AudioManager.instance.PlaySFX(1);
        }

        bossObject.SetActive(false);

        yield return new WaitForSeconds(waitBeforeSpawn);

        int poosSelecet = Random.Range(0, spawnPoints.Length);
        int tracker = 0;
        while (poosSelecet == lastSpawn && tracker < 100)
        {
            poosSelecet = Random.Range(0, spawnPoints.Length);
            tracker++;
        }
        lastSpawn = poosSelecet;

        bossObject.transform.position = spawnPoints[poosSelecet].position;
        bossObject.SetActive(true);

        if (smokeEffect != null)
        {
            Instantiate(smokeEffect, bossObject.transform.position, bossObject.transform.rotation);
            AudioManager.instance.PlaySFX(1);
        }
    }

    IEnumerator EndBattleCo()
    {
        // Wait for death animation to finish (e.g., 3 seconds)
        yield return new WaitForSeconds(3f);
        AudioManager.instance.PlaySFX(2);
        bossObject.SetActive(false);
        PlayerPrefs.SetInt(areaToUnlock + "_unlocked", 1);

        yield return new WaitForSeconds(1f);
        levelExit.SetActive(true);

        // Finally, turn off this controller
        gameObject.SetActive(false);
    }

    private void ReserShotCounter()
    {
        if (currentHealth > maxHealth * .5f)
        {
            shotCounter = timeBetweenShots1;
        }
        else if (currentHealth > maxHealth * .25f)
        {
            shotCounter = timeBetweenShots2;
        }
        else
        {
            shotCounter = timeBetweenShots3;
        }
    }
}