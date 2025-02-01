using UnityEngine;
using UnityEngine.UI;
using System;

public class ChoiceManager : MonoBehaviour
{
    public GameObject choicePanel;
    public Button choiceButtonPrefab;

    public void ShowChoices(string[] choices, Action<int> onChoiceSelected)
    {
        choicePanel.SetActive(true);
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < choices.Length; i++)
        {
            var button = Instantiate(choiceButtonPrefab, choicePanel.transform);
            button.GetComponentInChildren<Text>().text = choices[i];
            int choiceIndex = i;
            button.onClick.AddListener(() => OnChoiceClicked(choiceIndex, onChoiceSelected));
        }
    }

    private void OnChoiceClicked(int choiceIndex, Action<int> onChoiceSelected)
    {
        choicePanel.SetActive(false);
        onChoiceSelected?.Invoke(choiceIndex);
    }
}
