using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public GameObject pickupEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // 1. Tell Player they have the gun
            PlayerController.instance.hasBubbleGun = true;

            // 2. Show the Gun Model in the Player's Hand
            if (PlayerController.instance.bubbleGunModel != null)
            {
                PlayerController.instance.bubbleGunModel.SetActive(true);
            }

            // 3. Play Effect and Destroy this ground item
            if (pickupEffect != null) Instantiate(pickupEffect, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}