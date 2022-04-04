﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Text))]
public class DialogueWriter : MonoBehaviour
{
    [SerializeField]
    float m_secondsPerChar = 0.2f;

    public Action onFinished = null;

    Text textBox;

    private void Awake()
    {
        textBox = this.GetComponent<Text>();
        ClearDialogue();
    }
    
    public void ClearDialogue()
    {
        textBox.text = "";
    }

    public void StartNewDialogue(string dialogue)
    {
        StartCoroutine(RunDialogueStream(dialogue));
    }

    public IEnumerator RunDialogueStream(string dialogue)
    {
        textBox.text = "" + dialogue[0];

        int currentCharIndex = 0;
        int totalCharacters = dialogue.Length;

        while(currentCharIndex < totalCharacters)
        {
            yield return new WaitForSeconds(m_secondsPerChar);

            textBox.text += dialogue[currentCharIndex];
            currentCharIndex++;
        }

        onFinished?.Invoke();
    }
}