using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField]
    SpeechPopup speechPopup = null;

    [SerializeField]
    DialogueWriter dialogueWriter = null;

    [SerializeField]
    bool scaleToScreen = true;

    [SerializeField]
    float dialogueWaitTime = 3.0f;

    [SerializeField]
    bool m_debugTest = false;

    string m_nextDialogue = "";

    RectTransform m_rectTransform;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();

        speechPopup.onPopupEnter += StartDialogue;
        dialogueWriter.onFinished += CloseDialogue;

        float newScale = (Screen.height / 6.0f) / m_rectTransform.sizeDelta.y;
        m_rectTransform.localScale *= newScale;

        m_rectTransform.position = new Vector3(newScale * m_rectTransform.sizeDelta.x * 1.0f, 0.0f, 0.0f);
    }

    private void Update()
    {
        if(m_debugTest)
        {
            BeginNewDialogue("hello and welcome, I am John John Bubby.");
            m_debugTest = false;
        }
    }

    public void BeginNewDialogue(string dialogue)
    {
        m_nextDialogue = dialogue;
        dialogueWriter.ClearDialogue();
        speechPopup.BeginPopup();
    }

    public void StartDialogue()
    {
        dialogueWriter.StartNewDialogue(m_nextDialogue);
    }

    public void CloseDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(DoCloseDialogue());
    }

    IEnumerator DoCloseDialogue()
    {
        yield return new WaitForSeconds(dialogueWaitTime);
        speechPopup.EndPopup();
    }
}
