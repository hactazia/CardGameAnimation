using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerStrategy : MonoBehaviour
{
    public enum Strategy { Economic, Aggressive }
    public Strategy currentStrategy;

    //private int resources = 0;
    //private int attackCount = 0;

    public int Resources {  get; private set; } = 0;
    public int AttackCount { get; private set; } = 0;

    // Determine the player's strategy
    public void UpdateStrategy() 
    {
        if (Resources > 10 && AttackCount < 5)
        {
            currentStrategy = Strategy.Economic;
        }

        else if (AttackCount > 5)
        {
            currentStrategy = Strategy.Aggressive;
        }
    }

    // Increase player resources
    public void AddResources(int amount)
    {
        Resources += amount;
        UpdateStrategy();
    }

    // record an attack
    public void RegisterAttack()
    {
        AttackCount++;
        UpdateStrategy();
    }

    // to show the strategy in the console 
    public void PrintStrategy()
    {
        Debug.Log($"{gameObject.name} - Stratégie : {currentStrategy} | Ressources : {Resources} | Attaques : {AttackCount}");
        //Debug.Log("Current Strategy: " + currentStrategy);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
