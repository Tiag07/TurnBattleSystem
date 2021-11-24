using UnityEngine;

[CreateAssetMenu(fileName = "New Fighter")]
public class FighterData : ScriptableObject
{
    public string baseName;
    public int baseHp;
    public int baseAttack;
    public int baseSpeed;
    public enum FighterClass
    {
        Warrior, Mage, Assassin, Tank
    }
    public FighterClass fighterClass;
    public GameObject fighterModelPrefab;
}
