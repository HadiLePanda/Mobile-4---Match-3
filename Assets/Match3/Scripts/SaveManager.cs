using UnityEngine;

public struct UserData
{
    public int coins;
}

public class SaveManager : SingletonMonoBehaviour<SaveManager>
{
    public static readonly string PREF_COINS = "Coins";

    protected override bool IsDontDestroyOnLoad() => true;

    public UserData LoadUserData()
    {
        var data = new UserData
        {
            coins = LoadCoins(),
        };

        return data;
    }

    public void SaveUserData(UserData data)
    {
        SaveCoins(data.coins);

        PlayerPrefs.Save();
    }

    public int LoadCoins() => PlayerPrefs.GetInt(PREF_COINS, 0);
    public void SaveCoins(int value) => PlayerPrefs.SetInt(PREF_COINS, value);
}
