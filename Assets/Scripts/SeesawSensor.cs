using UnityEngine;

public class SeesawSensor : MonoBehaviour
{
    public SeesawController controller; // Drag the Pivot Parent here

    [Header("Which Platform is this?")]
    public bool isPlatformA; // CHECK for A, UNCHECK for B

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Parenting keeps player from sliding off while rotating
            other.transform.parent = controller.transform;

            if (isPlatformA) controller.playerOnA = true;
            else controller.playerOnB = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // Unparent player
            other.transform.parent = null;

            if (isPlatformA) controller.playerOnA = false;
            else controller.playerOnB = false;
        }
    }
}