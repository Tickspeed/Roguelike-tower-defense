using UnityEngine;

public class TowerPlacing : MonoBehaviour
{
    public static TowerPlacing main;

    [Header("refrences")]
    [SerializeField] private GameObject[] towerPrefabs;

    private int selectedTower = 0;

    private void Awake() 
    {
        main = this;
    }

    public GameObject GetSelectedTower() 
    {
        return towerPrefabs[selectedTower];
    }
}
