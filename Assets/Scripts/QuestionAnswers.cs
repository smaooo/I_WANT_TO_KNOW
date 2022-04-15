using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Question Answer", menuName = "Question Answer")]
public class QuestionAnswers : ScriptableObject
{
    [System.Serializable]
    public struct QA
    {
        public List<Category> categories;
        public List<Category> helpers;
        public int relatedQuestionNum;
        public QuestionAnswers nextQuestion;
    }

    [System.Serializable]
    public struct FaceExpression
    {
        public Sprite face;
        public Sprite eyes;
        public Sprite brows;
        public Sprite mouth;
    }
    [System.Serializable]
    public struct QuestionSprite
    {
        [TextArea]
        public string question;
        public FaceExpression sprite;
    }
    [Header("Questions & Answers")]
    public int number;
    public List<QuestionSprite> questions;
    public List<QA> answers;
    [Header("Redirection")]
    public QuestionAnswers redirect;
    [Header("Ending")]
    public bool nextIsEnding;
    public int ending;
    public bool isEnding;
}



