using Events.Scriptables;
using Player;
using TMPro;
using UnityEngine;

public class NameInput : MonoBehaviour
{
    [SerializeField] private PlayerName playerName;
    [SerializeField] private GameObject nameInputPanel;
    [SerializeField] private GameObject namePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    private TMP_InputField _inputField;
    [SerializeField] private GameObject deselectDefaultObject;
    [SerializeField] private GameObjectEventChannelSO selectObject;

    private void OnEnable()
    {
        _inputField ??= GetComponent<TMP_InputField>();
        if (playerName.playerName != string.Empty)
        {
            nameInputPanel.SetActive(false);
            namePanel.SetActive(true);
            nameText.text = playerName.playerName;
        }
    }

    private void OnDisable()
    {
        selectObject?.RaiseEvent(deselectDefaultObject);
    }

    public void OnInputFieldEndEdit(string input)
    {
        if (input.Length == 0)
            return;

        nameInputPanel.SetActive(false);
        namePanel.SetActive(true);
        playerName.playerName = _inputField.text;
        playerName.isInitialized = false;
        nameText.text = _inputField.text;
    }

    public void Reset()
    {
        playerName.playerName = string.Empty;
        namePanel.SetActive(false);
        nameInputPanel.SetActive(true);
        selectObject?.RaiseEvent(gameObject);
    }
}