using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CatChild
{
    public string word;
    public int score;
    public bool root;
    public List<string> variations;
}

public enum CategoryType { INTENTION, WORD }
[CreateAssetMenu(fileName = "New Category", menuName = "Category")]
public class Category : ScriptableObject
{
    
    public CatChild word;
    public int scoreMultiplier;
    public string simCategory;
    public List<CatChild> childs;
}

