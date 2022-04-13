using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CatChild
{
    public string word;
    public int score;
}

public enum CategoryType { INTENTION, WORD }
[CreateAssetMenu(fileName = "New Category", menuName = "Category")]
public class Category : ScriptableObject
{
    //public CategoryType type;
    public int score;
    public string category;
    public string simCategory;
    public List<CatChild> childs;
}

