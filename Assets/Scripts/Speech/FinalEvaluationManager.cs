using UnityEngine;
using System.Collections.Generic;

public class FinalEvaluationManager : MonoBehaviour
{
    public static FinalEvaluationManager Instance { get; private set; }

    [Header("Evaluation State")]
    public bool isEvaluationMode = false;
    public string currentExpectedPhrase = "";
    public string currentPhaseId = "";
    
    [Header("Scores")]
    public float comprehensionScore;
    public float vocabularyScore;
    public float grammarScore;
    public float functionalScore;
    public float targetLanguageScore;
    public float confidenceScore;
    public float freeResponseScore;
    
    [Header("Result Nodes")]
    public DialogueNode advancedSpeakerNode;
    public DialogueNode proficientTravelerNode;
    public DialogueNode developingSpeakerNode;
    public DialogueNode beginningSpeakerNode;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// Triggered via a Dialogue Event (e.g., 'StartFinalEval_Phase2: naimbag a bigat')
    /// </summary>
    public void StartEvaluationPhase(string phaseAndPhrase)
    {
        isEvaluationMode = true;
        // Format is "PhaseX: actual phrase"
        string[] parts = phaseAndPhrase.Split(new[] { ':' }, 2);
        if (parts.Length > 1)
        {
            currentPhaseId = parts[0].Trim();
            currentExpectedPhrase = parts[1].Trim();
        }
        else
        {
            currentPhaseId = phaseAndPhrase;
            currentExpectedPhrase = phaseAndPhrase;
        }
        Debug.Log($"[FinalEvaluationManager] Entered Evaluation Mode. Target: '{currentExpectedPhrase}'");
    }

    public void EndEvaluationMode()
    {
        isEvaluationMode = false;
        currentExpectedPhrase = "";
        Debug.Log("[FinalEvaluationManager] Exited Evaluation Mode.");
    }

    public void RecordScore(string phaseAndPhrase, PhraseEvaluator.EvaluateResponse response)
    {
        if (response == null) return;
        
        string phase = "Phase1";
        if (phaseAndPhrase.StartsWith("Phase"))
        {
            phase = phaseAndPhrase.Split(':')[0];
        }

        Debug.Log($"[FinalEvaluationManager] Recorded Score for {phase}: Semantic={response.semantic_score}, Lexical={response.lexical_score}");
        
        // Accumulate scores based on the phase type (we can average them if there are multiple attempts)
        switch(phase)
        {
            case "Phase1": 
                comprehensionScore = response.semantic_score * 100f; // Could be just manual 100 or 0
                break;
            case "Phase2":
                vocabularyScore = response.semantic_score * 100f;
                break;
            case "Phase3":
                grammarScore = response.semantic_score * 100f; // Uses structural template score in backend normally
                break;
            case "Phase4":
                functionalScore = response.semantic_score * 100f;
                break;
            case "Phase5":
                grammarScore = (grammarScore + (response.semantic_score * 100f)) / 2f; // Average if multiple
                break;
            case "Phase6":
                freeResponseScore = response.semantic_score * 100f;
                break;
        }
        
        targetLanguageScore = (targetLanguageScore == 0 ? (response.score * 100f) : (targetLanguageScore + response.score * 100f) / 2f);
        confidenceScore = (confidenceScore == 0 ? (response.final_confidence * 100f) : (confidenceScore + response.final_confidence * 100f) / 2f);
    }

    /// <summary>
    /// Manually record comprehension score from a dialogue button choice.
    /// </summary>
    public void RecordComprehensionScore(float score)
    {
        comprehensionScore = score;
        Debug.Log($"[FinalEvaluationManager] Comprehension Score Recorded: {score}");
    }

    public void CompleteEvaluation()
    {
        float finalScore = 
            (comprehensionScore * 0.20f) + 
            (vocabularyScore * 0.15f) + 
            (grammarScore * 0.20f) + 
            (functionalScore * 0.20f) + 
            (targetLanguageScore * 0.10f) + 
            (confidenceScore * 0.05f) + 
            (freeResponseScore * 0.10f);

        Debug.Log($"[FinalEvaluationManager] Final Score: {finalScore}");

        DialogueNode nextNode = null;

        if (finalScore >= 90f) nextNode = advancedSpeakerNode;
        else if (finalScore >= 75f) nextNode = proficientTravelerNode;
        else if (finalScore >= 60f) nextNode = developingSpeakerNode;
        else nextNode = beginningSpeakerNode;

        if (finalScore >= 75f)
        {
            // Trigger Crystal Awakening
            ObjectiveManager.Instance.SetObjective("Ilocos Chapter Complete");
            // Also Journal update, etc. (Can be done via the node's end events)
        }

        EndEvaluationMode();
        
        // Inject the result node immediately
        if (nextNode != null && DialogueManager.Instance != null)
        {
            // We use ForceStartDialogue on Kalaw to inject this.
            // Or better yet, we just tell DialogueManager to process it.
            // But since we are mid-dialogue, we can just load the node.
            DialogueManager.Instance.StartDialogue(nextNode, null, null);
        }
    }
}
