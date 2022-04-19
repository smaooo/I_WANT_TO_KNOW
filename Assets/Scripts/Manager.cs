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
    [SerializeField]
    private int pQ = -1;
    [SerializeField]
    private Category swears;
    [SerializeField]
    private QuestionAnswers disgustEnding;
    public Dictionary<int, List<WheelController>> currentWheels = new Dictionary<int, List<WheelController>>();

    private void Start()
    {
        if (pQ != -1)
            currentQuestion = questionsAnswers.Find(q => q.number == pQ);
        else
            currentQuestion = questionsAnswers[0];
        questionText.text = currentQuestion.questions[0].question;
        resEnding = questionsAnswers.Find(x => x.number == 3);
        for (int i = 0; i < wheelTracks.Count; i++)
        {
            currentWheels.Add(i, new List<WheelController>());

        }
        StartCoroutine(SpawnWheel());
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
            if (rand >= 0 && rand < 3)
            {
                w.GetComponent<Animator>().SetFloat("LeftRight", 1);
            }
            else
            {
                w.GetComponent<Animator>().SetFloat("LeftRight",-1);

            }
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
        questionText.text = "";
        foreach (var q in currentQuestion.questions)
        {
            questionText.text += q.question + "\n";
        }
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
        int sScore = 0;
        foreach (var s in swears.childs)
        {
            if (input.Contains(s.word))
            {
                sScore += s.score;
            }
        }
        sScore *= swears.scoreMultiplier;

        var x = scoring.Where(x => x.Value == 0);

        foreach (var s in scoring)
        {
            print(s.Key + " " + s.Value);
        }
        print("S: " + sScore);
        if (sScore > 0)
        {
            print("ASDAS");
            nextQuestion = disgustEnding;
            return nextQuestion;
        }
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
            if (ordered[0] < sScore)
            {
                nextQuestion = disgustEnding;
            }
            else
            {
                nextQuestion = currentQuestion.answers[ordered[0]].nextQuestion;

            }
            return nextQuestion;
        }
        
        return resEnding; 
        
    }
}
