using System.Collections;
using UnityEngine; 
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;

    private CameraController cam;

    [HideInInspector]
    public float levelTimer;
    public bool reloadOnRespawn = false;

    private void Awake()
    {
        instance = this;
        currentCrystals = PlayerPrefs.GetInt("Crystals");
        currentCoins = PlayerPrefs.GetInt("Coins");
       
    }

    public float waitBeforeRespawing;

    [HideInInspector]
    public bool respawing;

    private PlayerController player;
    [HideInInspector]
    public Vector3 respawnPoint;


    public int currentCoins, currentCrystals;
    public int cointTreshold = 5;

    public float waitToEndLevel = 1;
    [HideInInspector]
    public bool levelComplete;


    void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        respawnPoint = player.transform.position;
        cam = FindAnyObjectByType<CameraController>();

        UIController.instance.coinText.text = currentCoins.ToString();
        UIController.instance.crystalText.text = currentCrystals.ToString();
    }


    
    void Update()
    {
        if (!levelComplete)
        {
            levelTimer += Time.deltaTime;
            UIController.instance.timeText.text = levelTimer.ToString("0");
        }
        else
        {
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.Euler(0f, 180f, 0f), 10f * Time.deltaTime);
        }



#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerPrefs.DeleteAll();
        }
#endif


    }

    public void Respawn()
    {
        if (!respawing)
        {
            respawing = true;
            StartCoroutine(RespawnCo());

        }
    }


    public IEnumerator RespawnCo()
    {
        player.gameObject.SetActive(false);
        UIController.instance.FadeToBlack();
        AudioManager.instance.PlaySFX(10);

        yield return new WaitForSeconds(waitBeforeRespawing);
        if(reloadOnRespawn)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            player.transform.position = respawnPoint;

            cam.SnapToTarget();

            player.gameObject.SetActive(true);

            respawing = false;
            UIController.instance.FadeFromBlack();
            PlayerHealthController.instance.FillHealth();
        }

        

    }

    public void GetCoin()
    {
        currentCoins++;

        if(currentCoins >= cointTreshold)
        {
            GetCrystal();
            currentCoins -= cointTreshold;
        }

        UIController.instance.coinText.text = currentCoins.ToString();
        PlayerPrefs.SetInt("Coins", currentCoins);
    }

    public void GetCrystal()
    {
        currentCrystals++;
        UIController.instance.crystalText.text = currentCrystals.ToString();
        PlayerPrefs.SetInt("Crystals", currentCrystals);
    }

    public void EndLevel(string nextLevel)
    {
        StartCoroutine(EndLevelCo(nextLevel));
    }


    IEnumerator EndLevelCo(string nextLevel)
    {
        levelComplete = true;
        player.anim.SetTrigger("endLevel");
        yield return new WaitForSeconds(waitToEndLevel - 1f);
        UIController.instance.FadeToBlack();
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(nextLevel);

    }


}
