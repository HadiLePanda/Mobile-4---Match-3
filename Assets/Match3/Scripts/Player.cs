using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    private UserData userData = new();
    public UserData UserData => userData;

    protected override bool IsDontDestroyOnLoad() => true;

    public void LoadUserData(UserData userData)
    {
        this.userData = userData;
    }

    public void AddCoins(int amount)
    {
        int newCoins = userData.coins + amount;
        SetCoins(newCoins);
    }
    public void RemoveCoins(int amount)
    {
        if (amount > userData.coins)
            return;

        int newCoins = userData.coins - amount;
        SetCoins(newCoins);
    }

    private void SetCoins(int amount)
    {
        userData.coins = Mathf.Max(0, amount);
    }
}
