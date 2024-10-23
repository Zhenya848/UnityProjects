using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    public int CollectedCoins { get; private set; }

    private void Start() => GameObject.Find("CoinSaver").GetComponent<CoinSaver>().Initialize(this);

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Coin"))
        {
            CollectedCoins++;
            Destroy(other.gameObject);
        }
    }
}
