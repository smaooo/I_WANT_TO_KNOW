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
    [SerializeField]
    private GameObject wheelPrefab;
    [SerializeField]
    private List<Transform> wheelTracks;
    private Dictionary<int, List<WheelController>> currentWheels = new Dictionary<int, List<WheelController>>();

    private void Start()
    {
        currentQuestion = questionsAnswers[0];
        questionText.text = currentQuestion.questions[0].question;
        resEnding = questionsAnswers.Find(x => x.number == 3);
        for (int i = 0; i < wheelTracks.Count; i++)
        {
            currentWheels.Add(i, new List<WheelController>());
            StartCoroutine(SpawnWheel(i));

        }
    }

    private IEnumerator SpawnWheel(int track = -1)
    {
        yield return new WaitForSeconds(Random.Range(0.5f,1.5f));
        
        int rand;
        if (track == -1)
        {
            rand = Random.Range(0, wheelTracks.Count);
        }
        else
            rand = track;

        bool canSpawn = true;
        foreach (var cW in currentWheels[rand])
        {
            
            if (Vector3.Distance(wheelTracks[rand].transform.position,cW.transform.position) < 3)
            {
                canSpawn = false;
                break;
            }
            
        }

        if (canSpawn)
        {
            var t = wheelTracks[rand];
            var w = Instantiate(wheelPrefab);
            w.GetComponent<WheelController>().currenTrack = rand;
            w.GetComponent<WheelController>().tracks = wheelTracks;
            w.transform.position = t.position;
            w.transform.SetParent(GameObject.FindGameObjectWithTag("Island").transform);
            currentWheels[rand].Add(w.GetComponent<WheelController>());

        }
        yield return new WaitForSeconds(Random.Range(0.5f, 3f));
        StartCoroutine(SpawnWheel());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            currentQuestion = questionsAnswers[0];
            FindObjectOfType<PlayerController>().textField.text = "";
        }

        UpdateCurrentWheels();
    }

    private void UpdateCurrentWheels()
    {
        Dictionary<int, List<WheelController>> toRemove = new Dictionary<int, List<WheelController>>();
        for (int i = 0; i < wheelTracks.Count; i++)
        {
            toRemove.Add(i, new List<WheelController>());
        }
        foreach (var t in currentWheels)
        {
            foreach (var w in t.Value)
            {
                if (w == null)
                {
                    toRemove[t.Key].Add(w);
                }
            }
        }

        foreach (var w in toRemove)
        {
            currentWheels[w.Key].RemoveAll(x => w.Value.Contains(x));
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
