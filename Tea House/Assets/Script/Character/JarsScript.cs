using System.Collections.Generic;
using UnityEngine;

public class JarsScript : MonoBehaviour
{
    public static JarsScript instance;
    private void Awake()
    {
        instance = this;
    }

    List<WorkBenchStorageScript> enfants;
    private void Start()
    {

        enfants = new List<WorkBenchStorageScript>();

        foreach (Transform enfant in transform)
        {
            enfants.Add(enfant.GetComponent<WorkBenchStorageScript>());
        }
    }

    public void UpdateJars()
    {
        foreach (WorkBenchStorageScript enfant in enfants)
        {
            enfant.CheckIngredient();
        }
    }
}
