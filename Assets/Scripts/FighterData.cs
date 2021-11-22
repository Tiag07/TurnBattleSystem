using UnityEngine;

[CreateAssetMenu(fileName = "New Fighter")]
public class FighterData : ScriptableObject
{
    public string fighterName;
    public int hp;
    public int attack;
    public int speed;
    public enum FighterClass
    {
        Warrior, Mage, Assassin, Tank
    }
    public FighterClass fighterClass;
    public GameObject fighterModelPrefab;
}
