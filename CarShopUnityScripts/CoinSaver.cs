using UnityEngine;

public class CoinSaver : MonoBehaviour
{
    private CoinCollector _coinCollector;
    private const string MONEY_TAG = "Money";

    public void Initialize(CoinCollector coinCollector) => _coinCollector = coinCollector;

    public void SaveChanges()
    {
        var money = PlayerPrefs.GetInt(MONEY_TAG);

        money += _coinCollector.CollectedCoins;
        PlayerPrefs.SetInt(MONEY_TAG, money);
    }
}
