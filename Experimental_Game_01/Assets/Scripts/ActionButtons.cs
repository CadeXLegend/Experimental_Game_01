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
    private Image actionButtonPositionerImg;
    private Color32 positionerColor;
    [SerializeField] private Button gather, investigate, talk, attack;
    private GameObject gatherNodeRef;
    private Agents.Agent agentDoingActionRef;
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
            GameActionsLogger.instance.LogAction($"<b>{agentDoingActionRef.name}</b> just <color=green>gathered</color> {gatherNodeID} from <b>{gatherNodeRef.name}</b>");
            Destroy(gatherNodeRef);
            gatherAmount = 0;
            gatherNodeID = null;
            gatherNodeRef = null;
            agentDoingActionRef = null;
            DisableActionButton(ActionType.Gather);
        });
        actionButtonPositionerImg = actionButtonPisitioner.GetComponent<Image>();
        positionerColor = actionButtonPositionerImg.color;
    }

    public void SetCursorVisibility(bool visibility)
    {
        actionButtonPisitioner.SetActive(visibility);
        IsActive = visibility;
        actionButtonPositionerImg.color = positionerColor;
    }

    public void EnableActionButton(ActionType _actionType)
    {
        switch (_actionType)
        {
            case ActionType.Attack:
                attack.gameObject.SetActive(true);
                actionButtonPositionerImg.color = new Color32(190, 0 , 0, positionerColor.a);
                break;
            case ActionType.Gather:
                gather.gameObject.SetActive(true);
                actionButtonPositionerImg.color = new Color32(positionerColor.r, positionerColor.g, 80, positionerColor.a);
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
        actionButtonPositionerImg.color = positionerColor;
    }

    public void SetGatherNodeAndAmount(string _gatherNodeID, int _gatherAmount)

    {
        gatherNodeID = _gatherNodeID;
        gatherAmount = _gatherAmount;
    }

    public void SetGatherNodeRef(GameObject _gatherNodeRef, Agents.Agent _agentDoingActionRef)
    {
        agentDoingActionRef = _agentDoingActionRef;
        gatherNodeRef = _gatherNodeRef;
    }

    public void SetActionButtonPosition(Vector2 position) => actionButtonPisitioner.transform.position = position;
}
