using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class ScoutSystem : MonoBehaviour
{
    public GameObject playerInfoContainer; // panneau contenant les infos du joueur
    public GameObject playerInfoPrefab;
    public List<PlayerStrategy> players = new List<PlayerStrategy>();

    public void ScoutPlayers()
    {
        foreach (Transform child in playerInfoContainer.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var player in players)
        {
            GameObject newPlayerInfo = Instantiate(playerInfoPrefab, playerInfoContainer.transform);
            Text playerText = newPlayerInfo.GetComponent<Text>();

            string economyStrength = DetermineEconomicStrength(player);
            playerText.text = $"?? {player.name} ? Économie : {economyStrength} | Stratégie : {player.currentStrategy}";

            // Change la couleur du texte si le joueur devient agressif
            AnimateStrategyChange(playerText, player);
        }



         private string DetermineEconomicStrength(PlayerStrategy player)
    {
        if (player.Resources > 15) return "?? Forte";
        if (player.Resources < 5) return "? Faible";
        return "? Moyenne";
    }

    // Change la couleur du texte selon l'agressivité
    private void AnimateStrategyChange(Text playerText, PlayerStrategy player)
    {
        if (player.currentStrategy == PlayerStrategy.Strategy.Aggressive)
        {
            playerText.color = Color.Lerp(playerText.color, Color.red, Time.deltaTime * 2);
        }
        else
        {
            playerText.color = Color.Lerp(playerText.color, Color.white, Time.deltaTime * 2);
        }
    }

}

public class List<T>
{
}