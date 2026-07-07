using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject healthLifePrefab;
    public PlayerController currentPlayerHealth;
    List<HealthImgControll> life = new List<HealthImgControll>();

    private void OnEnable()
    {
        PlayerController.OnPlayerHit += DrawLife;
    }
    private void OnDisable()
    {
        PlayerController.OnPlayerHit -= DrawLife;
    }
    
    private void Start()
    {
        DrawLife();
    }

    public void DrawLife()
    {
        ClearLife();
        
        // determine how many life to make in total 
        // based off max Health
        float maxHealthRemainder = currentPlayerHealth.playerMaxHealth % 2; // odd or even
        int lifeToMake = (int)(currentPlayerHealth.playerMaxHealth / 2 + maxHealthRemainder);
        int totalLife = Mathf.CeilToInt(currentPlayerHealth.playerMaxHealth / 4f);
        for (int i = 0; i < totalLife; i++)
        {
            CreateEmptylife();
        }
       
        // 
        //for (int i = 0; i < lifeToMake; i++)
        //{
          //  CreateEmptylife(); // make total hearts needed
        //}

        for (int i = 0; i < life.Count; i++)
        {
            float lifeValue = currentPlayerHealth.playerHealth - (i * 4);
            int lifeStatusRemainder = (int)Mathf.Clamp(lifeValue, 0, 4);
            life[i].SetHealthImage((HealthStatus)lifeStatusRemainder);
        }
        
    }
    
    public void CreateEmptylife()
    {
        GameObject newLife = Instantiate(healthLifePrefab);
        newLife.transform.SetParent(transform);
        
        HealthImgControll healthcomponent = newLife.GetComponent<HealthImgControll>();
        healthcomponent.SetHealthImage(HealthStatus.Empty);
        life.Add(healthcomponent);
    }
    
    public void ClearLife()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        life = new List<HealthImgControll>();
        {
            
        }
    }
    
}
