using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionSequence : MonoBehaviour
{
    public List<ASAction> sequence = new List<ASAction>();
    private int sequenceIndex;

    private Coroutine delayedAction;

    void Start()
    {
        CheckSequence();
    }

    private void OnEnable()
    {
        T3ButtonPuzzleManager.onPuzzleComplete += OnPuzzleComplete;
        DialogueManager.onDialogueComplete += OnDialogueComplete;
    }

    private void OnDisable()
    {
        T3ButtonPuzzleManager.onPuzzleComplete -= OnPuzzleComplete;
        DialogueManager.onDialogueComplete -= OnDialogueComplete;
    }

    void Update()
    {
        
    }

    private void CheckSequence()
    {
        if (sequenceIndex >= sequence.Count)
        {
            Debug.Log("Sequence complete!");
            return;
        }

        switch (sequence[sequenceIndex].trigger)
        {
            case ASAction.Trigger.None:
                InvokeASAction(sequence[sequenceIndex]);
                sequenceIndex += 1;
                CheckSequence();
                break;
            case ASAction.Trigger.Delay:
                if (delayedAction == null)
                {
                    float delay = sequence[sequenceIndex].delayDuration;
                    delayedAction = StartCoroutine(InvokeEventAfterDelay(delay));
                }
                break;
        }
    }

    private void InvokeASAction(ASAction action)
    {
        if (action.onTrigger != null)
            action.onTrigger.Invoke();
        if (action.onTriggerDialogue != null)
            action.onTriggerDialogue.Invoke(action.dialogueParams);
    }

    private IEnumerator InvokeEventAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        InvokeASAction(sequence[sequenceIndex]);
        sequenceIndex += 1;
        CheckSequence();

        delayedAction = null;
    }

    private void OnPuzzleComplete(object sender, System.EventArgs e)
    {
        if (sequence[sequenceIndex].trigger == ASAction.Trigger.OnPuzzleComplete)
        {
            InvokeASAction(sequence[sequenceIndex]);
            sequenceIndex += 1;
            CheckSequence();
        }
    }

    private void OnDialogueComplete(object sender, System.EventArgs e)
    {
        if (sequence[sequenceIndex].trigger == ASAction.Trigger.OnDialogueComplete)
        {
            InvokeASAction(sequence[sequenceIndex]);
            sequenceIndex += 1;
            CheckSequence();
        }
    }


    public void CompleteTrial()
    {
        SceneManager.LoadScene(0);
    }

    public void DebugLogString(string s)
    {
        Debug.Log(s);
    }
}
