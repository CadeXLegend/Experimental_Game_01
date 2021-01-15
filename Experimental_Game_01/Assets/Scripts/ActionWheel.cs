using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionWheel : MonoBehaviour
{
    [SerializeField] private GameObject actionWheel;
    [SerializeField] private Button gather, investigate, talk, attack;
    private GameObject gatherNodeRef;
    private Agent.Agent agentDoingActionRef;
    private string gatherNodeID;
    private int gatherAmount;
    public bool IsActive { get; private set; }
    #region Singleton
    public static ActionWheel instance;
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
    void Start()
    {
        actionWheel.SetActive(false);
        IsActive = false;
        gather.onClick.AddListener(
            () => 
            {
                Debug.Log("click fired!");
                Interaction.Gather(gatherNodeID, gatherAmount);
                agentDoingActionRef.ProcessAction();
                Destroy(gatherNodeRef);
                gatherAmount = 0;
                gatherNodeID = null;
                gatherNodeRef = null;
                agentDoingActionRef = null;
            });
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

    public void EnableActionWheel()
    {
        actionWheel.SetActive(true);
        IsActive = true;
    }
    public void DisableActionWheel()
    {
        actionWheel.SetActive(false);
        IsActive = false;
    }

    public void SetActionWheelPosition(Vector2 position) => actionWheel.transform.position = position;
}
