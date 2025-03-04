using UnityEngine;
using UnityEngine.UI;

public class EconomyBar : MonoBehaviour
{
    [SerializeField] Slider economySlider;
    private float maxEconomy = 100f;
    private float currentEconomy = 50f;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        economySlider.maxValue = maxEconomy;
        UpdateEconomyBar();
    }

    public void ModifyEconomy(float amount)
    {
        currentEconomy += amount;
        currentEconomy = Mathf.Clamp(currentEconomy, 0, maxEconomy);
        UpdateEconomyBar();
    }
    
    // Update is called once per frame
    private void UpdateEconomyBar() 
    {
       economySlider.value = currentEconomy;
    }



    
    
}
