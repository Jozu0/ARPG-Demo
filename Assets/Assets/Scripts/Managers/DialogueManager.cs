using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;


public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueIndicator;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f;

    [Header("Input Settings")]
    public InputActionReference continueDialogueAction;

    private string[] currentLines;
    private int currentLineIndex;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private bool isDialogueActive;

    // Singleton pattern
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;



        // S'assurer que le dialogue est désactivé au démarrage
        dialoguePanel.SetActive(false);
        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        // Configurer l'action d'input pour continuer le dialogue
        if (continueDialogueAction != null)
        {
            continueDialogueAction.action.started += OnContinueDialogueInput;
        }
    }


    private void OnEnable()
    {
        // Activer l'action d'input
        if (continueDialogueAction != null)
        {
            continueDialogueAction.action.Enable();
        }
    }


    private void OnDisable()
    {
        // Désactiver l'action d'input
        if (continueDialogueAction != null)
        {
            continueDialogueAction.action.Disable();
        }
    }


    private void OnDestroy()
    {
        // Nettoyer les abonnements d'événements
        if (continueDialogueAction != null)
        {
            continueDialogueAction.action.started -= OnContinueDialogueInput;
        }
    }


    // Cette méthode est appelée quand l'action de continuer le dialogue est déclenchée
    private void OnContinueDialogueInput(InputAction.CallbackContext context)
    {
        if (!isDialogueActive)
        {
            return;
        }

        if (isTyping)
        {
            // Si le texte est en train d'être tapé, le compléter immédiatement
            CompleteTyping();

        }
        else
        {
            // Sinon, passer à la ligne suivante
            DisplayNextLine();
        }
    }

    public void StartDialogue(string speakerName, string[] lines)
    {
        // Réinitialiser les variables de dialogue
        currentLines = lines;
        currentLineIndex = 0;
        isDialogueActive = true;

        dialoguePanel.SetActive(true);
        nameText.text = speakerName;
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        // If we're already typing, stop the previous coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Check if out of lines
        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        // Disable continue indicator
        if (continueIndicator)
        {
            continueIndicator.SetActive(false);
        }

        // Start new typing coroutine
        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentLineIndex]));
        currentLineIndex++;
        Debug.Log(currentLineIndex);
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        // Ensure typing speed is reasonable
        float safeTypingSpeed = Mathf.Clamp(typingSpeed, 0.01f, 1f);

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(safeTypingSpeed);
        }

        isTyping = false;

        // Activate continue indicator
        if (continueIndicator)
        {
            continueIndicator.SetActive(true);
        }
    }
    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = currentLines[currentLineIndex - 1];
        isTyping = false;

        // Activer l'indicateur de continuation
        if (continueIndicator)
        {
            continueIndicator.SetActive(true);
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }

    // Méthode publique pour vérifier si un dialogue est en cours
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}
