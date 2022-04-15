using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private List<QuestionAnswers> questionsAnswers;
    
    private QuestionAnswers currentQuestion;
    [SerializeField]
    private TextMeshProUGUI questionText;
    private QuestionAnswers resEnding;
    [SerializeField]
    private Category noneCat;


    private void Start()
    {
        currentQuestion = questionsAnswers[0];
        questionText.text = currentQuestion.questions[0].question;
        resEnding = questionsAnswers.Find(x => x.number == 3);
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            currentQuestion = questionsAnswers[0];
            FindObjectOfType<PlayerController>().textField.text = "";
        }
    }
    public void SetNextQuestion(string input)
    {
        
        currentQuestion = DetectNextQuestion(input, currentQuestion);
        questionText.text = currentQuestion.questions[0].question;
        print(currentQuestion);
    }

    public QuestionAnswers DetectNextQuestion(string input, QuestionAnswers currentQuestion)
    {
        QuestionAnswers nextQuestion;

        Dictionary<int, int> scoring = new Dictionary<int, int>();
        for (int i = 0; i < currentQuestion.answers.Count; i++)
        {
            var currentAnswer = currentQuestion.answers[i];
            foreach (var cat in currentAnswer.categories)
            {
                int currentScore = 0;
                if (input.Contains(cat.word.word))
                    currentScore += cat.word.score;
                if (cat.simCategory != "")
                    if (input.Contains(cat.simCategory))
                        currentScore += cat.word.score;
                foreach (var child in cat.childs)
                {
                    if (input.Contains(child.word))
                        currentScore += child.score;
                    foreach (var v in child.variations)
                    {
                        if (input.Contains(v))
                            currentScore += child.score;
                    }
                }
                currentScore *= cat.scoreMultiplier;

                if (scoring.ContainsKey(i))
                    scoring[i] += currentScore;
                else
                    scoring.Add(i, currentScore);
                
            }


        }

        var x = scoring.Where(x => x.Value == 0);
        if (x.Count() == scoring.Count)
        {
            var n = currentQuestion.answers.Where(x => x.categories.Contains(noneCat));
            if (n.Count() > 0)
            {
                nextQuestion = n.ToList()[0].nextQuestion;
                return nextQuestion;
            }
        }
        else
        {
            var ordered = (from v in scoring orderby v.Value descending select v.Key).ToList();
            nextQuestion = currentQuestion.answers[ordered[0]].nextQuestion;
            return nextQuestion;
        }

        return resEnding; 
        
    }
}
