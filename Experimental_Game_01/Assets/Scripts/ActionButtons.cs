using Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtons : MonoBehaviour
{
    public enum ActionType
    {
        Gather, Investigate, Talk, Attack
    };

    [SerializeField] private GameObject actionButtonPisitioner;
    [SerializeField] private Button gather, investigate, talk, attack;
    private GameObject gatherNodeRef;
    private Agent.Agent agentDoingActionRef;
    private string gatherNodeID;
    private int gatherAmount;
    public bool IsActive { get; private set; }

    #region Singleton
    public static ActionButtons instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log($"More than one {instance} instance found!");
            Destroy(this);
            return;
        }
        instance = this;
    }
    #endregion
    // Start is called before the first frame update
    private void Start()
    {
        gather.onClick.AddListener(
        () =>
        {
            Interaction.Gather(gatherNodeID, gatherAmount);
            agentDoingActionRef.ProcessAction();
            Destroy(gatherNodeRef);
            gatherAmount = 0;
            gatherNodeID = null;
            gatherNodeRef = null;
            agentDoingActionRef = null;
            DisableActionButton(ActionType.Gather);
        });
    }

    public void SetCursorVisibility(bool visibility)
    {
        actionButtonPisitioner.SetActive(visibility);
        IsActive = visibility;
    }

    public void EnableActionButton(ActionType _actionType)
    {
        switch (_actionType)
        {
            case ActionType.Attack:
                attack.gameObject.SetActive(true);
                break;
            case ActionType.Gather:
                gather.gameObject.SetActive(true);
                break;
            case ActionType.Investigate:
                investigate.gameObject.SetActive(true);
                break;
            case ActionType.Talk:
                talk.gameObject.SetActive(true);
                break;
        };
        IsActive = true;
    }

    public void DisableActionButton(ActionType _actionType)
    {
        switch (_actionType)
        {
            case ActionType.Attack:
                attack.gameObject.SetActive(false);
                break;
            case ActionType.Gather:
                gather.gameObject.SetActive(false);
                break;
            case ActionType.Investigate:
                investigate.gameObject.SetActive(false);
                break;
            case ActionType.Talk:
                talk.gameObject.SetActive(false);
                break;
        };
        IsActive = false;
    }

    public void SetGatherNodeAndAmount(string _gatherNodeID, int _gatherAmount)

    {
        gatherNodeID = _gatherNodeID;
        gatherAmount = _gatherAmount;
    }

    public void SetGatherNodeRef(GameObject _gatherNodeRef, Agent.Agent _agentDoingActionRef)
    {
        agentDoingActionRef = _agentDoingActionRef;
        gatherNodeRef = _gatherNodeRef;
    }

    public void SetActionButtonPosition(Vector2 position) => actionButtonPisitioner.transform.position = position;
}
