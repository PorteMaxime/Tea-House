using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Game", menuName = "Game/ingredientData")]
public class IngredientData : ScriptableObject
{
    public string Name;
    public GameObject Model;
    public int Warmth;
    public int Freshness;
    public int Relaxation;
    public int Energy;
    public int Prestige;

    public List<IngredientData> Ingredients;
}
