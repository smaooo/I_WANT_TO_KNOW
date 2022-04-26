using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using SpellingCorrector;
using DataCollector;
using System;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    [System.Serializable]
    public struct FaceStructure
    {
        public SpriteRenderer face;
        public SpriteRenderer eyes;
        public SpriteRenderer brows;
        public SpriteRenderer mouth;
        public SpriteRenderer additional;
    }

    public struct ScoringToData
    {
        public float cat1;
        public float cat2;
        public float cat3;
        public float cat4;
        public float catS;
    }
    [System.Serializable]
    public struct ZoomPack
    {
        public Vector3 position;
        public float speedDivider;
    }
    [System.Serializable]
    public struct Zooming
    {
        public ZoomPack zoomIn;
        public ZoomPack zoomOut;
        public ZoomPack zoomInX2;
        public ZoomPack zoomInX3;
        public ZoomPack zoomInX4;
        public ZoomPack SlowZoomIn;
        public ZoomPack zoomOutX2;
        public ZoomPack zoomOutFull;
        public Vector3 GetPostion(QuestionAnswers.Zoom zoom) => zoom switch
        {
            QuestionAnswers.Zoom.In => zoomIn.position,
            QuestionAnswers.Zoom.InX2 => zoomInX2.position,
            QuestionAnswers.Zoom.InX3 => zoomInX3.position,
            QuestionAnswers.Zoom.InX4 => zoomInX4.position,
            QuestionAnswers.Zoom.Out => zoomOut.position,
            QuestionAnswers.Zoom.SlowZoomInFull => SlowZoomIn.position,
            QuestionAnswers.Zoom.OutX2 => zoomOutX2.position,
            QuestionAnswers.Zoom.OutFull => zoomOutFull.position,
            _ => Vector3.zero

        };
        public float GetSpeedDivider(QuestionAnswers.Zoom zoom) => zoom switch
        {
            QuestionAnswers.Zoom.In => zoomIn.speedDivider,
            QuestionAnswers.Zoom.InX2 => zoomInX2.speedDivider,
            QuestionAnswers.Zoom.InX3 => zoomInX3.speedDivider,
            QuestionAnswers.Zoom.InX4 => zoomInX4.speedDivider,
            QuestionAnswers.Zoom.Out => zoomOut.speedDivider,
            QuestionAnswers.Zoom.SlowZoomInFull => SlowZoomIn.speedDivider,
            QuestionAnswers.Zoom.OutX2 => zoomOutX2.speedDivider,
            QuestionAnswers.Zoom.OutFull => zoomOutFull.speedDivider,
            _ => 1
        };
    }
   
    [SerializeField]
    private List<QuestionAnswers> questionsAnswers;
    [HideInInspector]
    public QuestionAnswers currentQuestion;
    [SerializeField]
    private TextMeshProUGUI questionText;
    [SerializeField]
    private GameObject inputText;
    [SerializeField]
    private GameObject playerInputPack;
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

    [SerializeField]
    private RawImage fade;

    private Spelling spelling;
    [SerializeField]
    private TextAsset textFile;
    private Collector collector;
    [SerializeField]
    private bool collectData = false;
    public enum State { Walking, Conversation}
    [SerializeField]
    private State state = State.Conversation;
    [SerializeField]
    private GameObject convoCam;
    [SerializeReference]
    private GameObject facePack;
    [SerializeField]
    private FaceStructure faceStructure;
    private PlayerController player;
    [SerializeField]
    private Zooming zooming;
    [HideInInspector]
    public string num15;
    private bool printing = false;
    private bool printWhole = false;
    private int preveQuestionNumber = -2;
    [SerializeField]
    private List<SpriteRenderer> stones;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        player.state = state;
        if (collectData)
        {
            collector = new Collector();

        }
        spelling = new Spelling(textFile);

        if (ConstData.Act1LastColor != null)
            fade.color = ConstData.Act1LastColor.GetPixel((int)(Screen.width / 2), (int)(Screen.height / 2));
        else
            fade.color = new Color(1, 1, 1, 1);

        StartCoroutine(FadeIn());

        
        resEnding = questionsAnswers.Find(x => x.number == 3);
        for (int i = 0; i < wheelTracks.Count; i++)
        {
            currentWheels.Add(i, new List<WheelController>());

        }
        StartCoroutine(SpawnWheel());
    }

    private IEnumerator FadeIn()
    {
        float timer = 0;

        while(fade.color.a != 0)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / 500;

            fade.color = Color.Lerp(fade.color, new Color(1, 1, 1, 0), timer);
        }
    }
    private IEnumerator SpawnWheel(int track = -1)
    {
        //yield return new WaitForSeconds(Random.Range(0.5f,1.5f));
        
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
            w.transform.position = t.position + new Vector3(0,0.2f,0);
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
        if (state == State.Conversation && printing && Input.GetKeyDown(KeyCode.Space))
        {
            printWhole = true;
        }
        //UpdateCurrentWheels();

        if (state == State.Conversation && !convoCam.activeSelf)
        {
            SetupConversation();
        }

        CheckSortingOrder();
    }
    private void CheckSortingOrder()
    {
        foreach (var s in stones)
        {
            float dot = Vector3.Dot(player.transform.forward, (s.transform.position - player.transform.position).normalized);
            if (dot < 0)
            {
                s.sortingOrder = 200 +(int)(Vector3.Distance(player.transform.position, s.transform.position));
            }
        }
    }
    private void SetupConversation()
    {

        Camera.main.gameObject.SetActive(false);
        convoCam.SetActive(true);
        questionText.gameObject.SetActive(true);
        inputText.SetActive(true);
        facePack.SetActive(true);
        if (pQ != -1)
            currentQuestion = questionsAnswers.Find(q => q.number == pQ);
        else
            currentQuestion = questionsAnswers[0];
        
        StartCoroutine(ManageQuestion(currentQuestion));
    }

    private void ChangeSprites(QuestionAnswers.QuestionSprite question)
    {
        var expression = question.sprite;
        if (expression.face != null)
            faceStructure.face.sprite = expression.face;
        if (expression.eyes != null)
            faceStructure.eyes.sprite = expression.eyes;
        if (expression.brows != null)
            faceStructure.brows.sprite = expression.brows;
        if (expression.mouth != null)
            faceStructure.mouth.sprite = expression.mouth;
        if (expression.additional != null)
            faceStructure.additional.sprite = expression.additional;
    }
    private IEnumerator PrintText(TextMeshProUGUI textField, string text, float waitTime = 0f)
    {
        printing = true;
        if (!convoCam.activeSelf)
        {
            yield return new WaitUntil(() => convoCam.activeSelf);
        }

        yield return new WaitForSeconds(waitTime);
        textField.text = "";
        bool meta = false;
        for (int i = 0; i < text.Length; i++)
        {
            var t = text[i];
            if (t == '<')
            {
                var x = text.Substring(i, 10);

                meta = true;
                if (x.Contains("30"))
                    textField.text += "<size=30>";

                else if (x.Contains("24"))
                    textField.text += "<size=24>";
                continue;
            }
            if (t == '>')
            {
                meta = false;
                continue;
            }
            if (meta) continue;
            if (printWhole) break;
            textField.text += t;
            yield return new WaitForSeconds(0.05f);
        }
        //foreach (var t in text)
        //{
        //    if (t == '<')
        //    {
        //        var x = text.Substring(text.IndexOf(t), text.IndexOf(t) + 10);

        //        meta = true;
        //        if (x.Contains("30"))
        //            textField.text += "<size=30>";

        //        else if (x.Contains("24"))
        //            textField.text += "<size=24>";
        //        print(x);
        //        continue;
        //    }
        //    if (t == '>')
        //    {
        //        meta = false;
        //        continue;
        //    }
        //    if (meta) continue;
        //    if (printWhole) break;
        //    textField.text += t;
        //    yield return new WaitForSeconds(0.05f);
        //}
        textField.text = text;
        printWhole = false;
        printing = false;
    }

    private IEnumerator ZoomCamera(Vector3 position,float divider, bool slowZoom = false)
    {
        float timer = 0;
        while (Camera.main.transform.position != position)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / divider;

            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, position, timer);
        }
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

        if (toRemove.Count > 0)
        {
            foreach (var w in toRemove)
            {
                currentWheels[w.Key].RemoveAll(x => w.Value.Contains(x));
            }

        }
    }

    private IEnumerator ManageQuestion(QuestionAnswers question)
    {
        var animator = faceStructure.face.GetComponent<Animator>();
        foreach (var q in question.questions)
        {
            print(question.questions.IndexOf(q));
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Empty")
                && Mathf.Abs(animator.GetCurrentAnimatorStateInfo(0).normalizedTime) < 0.9f)
            {
                yield return new WaitUntil(() => Mathf.Abs(animator.GetCurrentAnimatorStateInfo(0).normalizedTime) >= 0.9f);
            }
            else
            {
                if (!animator.GetCurrentAnimatorStateInfo(1).IsName("Empty")
                    && Mathf.Abs(animator.GetCurrentAnimatorStateInfo(1).normalizedTime) < 0.9f)
                {
                    yield return new WaitUntil(() => Mathf.Abs(animator.GetCurrentAnimatorStateInfo(1).normalizedTime) >= 0.9);
                }
                else
                {
                    if (!animator.GetCurrentAnimatorStateInfo(2).IsName("Empty")
                        && Mathf.Abs(animator.GetCurrentAnimatorStateInfo(2).normalizedTime) < 0.9f)
                    {
                        yield return new WaitUntil(() => Mathf.Abs(animator.GetCurrentAnimatorStateInfo(2).normalizedTime) >= 0.9);


                    }

                }

            }

            ChangeSprites(q);
            if (q.zoom != QuestionAnswers.Zoom.None)
                StartCoroutine(ZoomCamera(zooming.GetPostion(q.zoom), zooming.GetSpeedDivider(q.zoom)));
            PlayFaceAnimations(q);
            if (q.question != "")
            {
                string output = q.question;
                if (question.number == 15)
                {

                    if (question.questions.IndexOf(q) == 1 || question.questions.IndexOf(q) == 3
                        || question.questions.IndexOf(q) == 5)
                    {
                        yield return StartCoroutine(PrintText(player.textField, output));
                        yield return new WaitForSeconds(1);
                        player.textField.text = "";
                        playerInputPack.SetActive(false);
                    }
                    else
                    {
                        yield return StartCoroutine(PrintText(questionText, output));

                    }

                }
                else
                    yield return StartCoroutine(PrintText(questionText, output));


            }

            if (question.number == 15)
            {
                if (question.questions.IndexOf(q) == 0 || question.questions.IndexOf(q) == 2 
                    || question.questions.IndexOf(q) == 4)
                {
                    playerInputPack.SetActive(true);
                    player.inputActive = true;
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
                }
            }
            yield return new WaitForSeconds(1.5f);

            
        }
        if (currentQuestion.redirect != null)
        {
            inputText.GetComponent<TextMeshProUGUI>().text = "";
            preveQuestionNumber = currentQuestion.number;
            currentQuestion = currentQuestion.redirect;
            StartCoroutine(ManageQuestion(currentQuestion));
        }
        else
        {

            player.inputActive = true;
            playerInputPack.SetActive(true);
            inputText.GetComponent<TextMeshProUGUI>().text = "";

        }
    }

    private void PlayFaceAnimations(QuestionAnswers.QuestionSprite question)
    {
        var anims = question.animation;
        var animator = faceStructure.face.GetComponent<Animator>();
        if (anims.face != null)
        {
            if (anims.reverseFace)
            {
                animator.Play(anims.face.name+"R", anims.faceLayer);
            }
            else
            {
                animator.Play(anims.face.name, anims.faceLayer);
            }
  

        }

        if (anims.eyes != null)
        {
            if (anims.reverseEyes)
            {
                animator.Play(anims.eyes.name + "R", anims.eyesLayer);
            }
            else
            {
                animator.Play(anims.eyes.name, anims.eyesLayer);
            }
           
        }

        if (anims.additional != null)
        {
            animator.Play(anims.additional.name, anims.additionalLayer);

        }
    }
    public void SetNextQuestion(string input)
    {
        preveQuestionNumber = currentQuestion.number;
        currentQuestion = DetectNextQuestion(input, currentQuestion);
        print(currentQuestion.number);
        StartCoroutine(ManageQuestion(currentQuestion));
     
        //print(currentQuestion);
    }

    public QuestionAnswers DetectNextQuestion(string input, QuestionAnswers currentQuestion)
    {
        
        int prevQuestion = currentQuestion.number;
        string prevInput = input;
        string correctInput = "";

        foreach (string i in input.Split(' '))
        {
            correctInput += " " + spelling.Correct(i);
        }

        
        QuestionAnswers nextQuestion;
        var wCheck = CheckInput(input, currentQuestion);
        var check = CheckInput(correctInput, currentQuestion);
        var scoring = new Dictionary<int, int>();
        int sScore;
        if (wCheck.Item1.Values.Sum() >= check.Item1.Values.Sum())
        {
            scoring = wCheck.Item1;
            sScore = wCheck.Item2;
        }
        else
        {
            scoring = check.Item1;
            sScore = check.Item2;
        }

        var x = scoring.Where(x => x.Value == 0);
        var sToD = new ScoringToData
        {
            cat1 = scoring.ContainsKey(0) ? scoring[0] : -1,
            cat2 = scoring.ContainsKey(1) ? scoring[1] : -1,
            cat3 = scoring.ContainsKey(2) ? scoring[2] : -1,
            cat4 = scoring.ContainsKey(3) ? scoring[3] : -1,
            catS = sScore
        };
        foreach (var s in scoring)
        {
            print(s.Key + " " + s.Value);
        }
        if (sScore > 0)
        {
            nextQuestion = disgustEnding;
            if (collectData)
                collector.AddInput(prevInput, input, nextQuestion.number, prevQuestion, sToD);
            return nextQuestion;
        }
        if (x.Count() == scoring.Count)
        {
            var n = currentQuestion.answers.Where(x => x.categories.Contains(noneCat));
            if (n.Count() > 0)
            {
                nextQuestion = n.ToList()[0].nextQuestion;
                if (collectData)
                    collector.AddInput(prevInput, input, nextQuestion.number, prevQuestion, sToD);
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
            if (collectData)
                collector.AddInput(prevInput, input, nextQuestion.number, prevQuestion, sToD);
            return nextQuestion;
        }
        if (collectData)
            collector.AddInput(prevInput, input, resEnding.number, prevQuestion, sToD);
        return resEnding; 
        
    }


    private Tuple<Dictionary<int,int>,int> CheckInput(string input, QuestionAnswers currentQuestion)
    {
        var scoring = new Dictionary<int, int>();
        int max = 0;
        var array = input.Split(' ');
        if (currentQuestion.number == 12 && preveQuestionNumber == 7)
        {
            max = currentQuestion.answers.Count;
        }
        else if (currentQuestion.number == 12 && preveQuestionNumber != 7)
        {
            max = 3;
        }
        else
        {
            max = currentQuestion.answers.Count;
        }
        for (int i = 0; i < max; i++)
        {
            var currentAnswer = currentQuestion.answers[i];
            foreach (var cat in currentAnswer.categories)
            {
                
                int currentScore = 0;
                if (array.Contains(cat.word.word))
                {
                    currentScore += cat.word.score;
                        
                    
                }
                foreach (var v in cat.word.variations)
                {
                    if (array.Contains(v))
                    {
                        currentScore += cat.word.score;
                    }
                }
                if (cat.simCategory != "")
                {
                    if (array.Contains(cat.simCategory))
                        currentScore += cat.word.score;
                    
                }
                foreach (var child in cat.childs)
                {

                    if (child.numRepeat > 1)
                    {
                        var rep = input.Split(' ');
                        var match = rep.Where(x => x == child.word);
                       
                        if (match.Count() >= child.numRepeat && match.Count() < child.maxRepeat)
                        {
                            currentScore += child.score;
                        }
                    }
                    else
                    {
                        if (array.Contains(child.word))
                        {
                            currentScore += child.score;

                        }
                        foreach (var v in child.variations)
                        {
                            if (array.Contains(v))
                            {
                                currentScore += child.score;
                                print(v);  

                            }
                        }
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

        var result = new Tuple<Dictionary<int, int>, int>(scoring, sScore);

        return result;

        
    }
}
