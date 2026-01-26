using UnityEngine;

public class KeepAwake : MonoBehaviour
{
    void Start()
    {
        // "sleepThreshold = 0" tells Unity to NEVER put this object to sleep
        // even if it stops moving completely.
        GetComponent<Rigidbody>().sleepThreshold = 0f;
    }
}