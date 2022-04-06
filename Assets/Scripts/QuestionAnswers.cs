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
        public string answer;
        public string approach;
        public int relatedQuestionNum;
        public QuestionAnswers nextQuestion;
    }

    public int number;
    [TextArea]
    public string question;
    public List<QA> answers;
    public int endingNumber;
}



