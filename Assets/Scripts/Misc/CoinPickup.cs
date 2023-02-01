using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision!= null)
        {
            if (collision.CompareTag("Hero"))
            {
                AudioManager.instance.Play("coin_pickup");
                GameManager.instance.ExtraPickupCoin();
                GameManager.instance.RemoveCoinFromList(gameObject);
            }
        }
    }
}
