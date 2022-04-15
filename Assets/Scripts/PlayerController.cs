using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using QA;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI textField;
    private int lastKey;
    private bool inputActive = false;
    private Manager manager;

    private QuestionAnswers currentQuestion;

    void Start()
    {
        manager = FindObjectOfType<Manager>();
        Invoke("MakeInputActive", 0.5f);
    }

    void Update()
    {
        if (inputActive) 
            CheckKeyInput();    
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.type == EventType.KeyUp)
        {
            lastKey = (int)(char)e.keyCode;
        }
    

        
    }


    private void MakeInputActive()
    {
        inputActive = true;
    }
    private void CheckKeyInput()
    {
        if (lastKey != 0)
        {
            if ((lastKey >= 97 && lastKey <=122) || lastKey == 32 || lastKey == 46 || lastKey == 47
                || lastKey == 49)
            {
                textField.text += ((char)lastKey).ToString();
                lastKey = 0;
            }
            
            else if (lastKey == 8 && textField.text.Length > 0)
            {
                textField.text = textField.text.Remove(textField.text.Length - 1);
                lastKey = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            var input = textField.text;
            textField.text = "";
            manager.SetNextQuestion(input);
        }
    }

}
