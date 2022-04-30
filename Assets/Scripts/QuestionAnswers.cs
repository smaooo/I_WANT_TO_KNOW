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

    public enum Zoom { None, In, Out, InX2, InX3, InX4, SlowZoomInFull, OutX2, OutFull, PondZoom}
    [System.Serializable]
    public struct FaceExpression
    {
        public Sprite face;
        public Sprite eyes;
        public Sprite brows;
        public Sprite mouth;
        public Sprite additional;
    }
    [System.Serializable]
    public struct FaceAnimation
    {
        public AnimationClip eyes;
        public int eyesLayer;
        public bool reverseEyes;
        public AnimationClip face;
        public int faceLayer;
        public bool reverseFace;
        public AnimationClip additional;
        public int additionalLayer;
    }
    [System.Serializable]
    public struct QuestionSprite
    {
        [TextArea]
        public string question;
        public FaceExpression sprite;
        public FaceAnimation animation;
        public Zoom zoom;
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



