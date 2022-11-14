using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWeapon : MonoBehaviour
{
    private bool canPickUp = false;
    private Transform player;

    private void Update()
    {
        if (!canPickUp) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            //go through child or player, if there is another weapon (not "Indicator") then remove that
            for(int i = 0; i < player.childCount; i++)
            {
                if (player.GetChild(i).name == "Indicator") continue;
                player.GetChild(i).gameObject.SetActive(false);
            }

            GetComponent<Weapon>().enabled = true;
            transform.parent = player;
            transform.localPosition = Vector3.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        canPickUp = true;
        player = collision.transform;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) canPickUp = false;
    }
}
