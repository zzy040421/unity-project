using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDataManager : MonoBehaviour
{
    public List<CardData> cardDatas; // List of CardData instances
    public GameObject artCardPrefab; // Prefab for Art card fronts (red)
    public GameObject pictureCardPrefab; // Prefab for Picture card fronts (yellow)
    public GameObject soundCardPrefab; // Prefab for Sound card fronts (pink)
    public Transform cardContainer; // Parent container for the card fronts

    public void LoadCardDataFromFolders()
    {
        // Load card data from designated folders and populate cardDatas
        Debug.Log("Loading card data from folders...");
        foreach (var cardData in cardDatas)
        {
            Debug.Log($"Loaded CardData: {cardData.name}, CardType: {cardData.cardType}");
        }
    }

    public List<CardData> GetGroup(int groupIndex)
    {
        Debug.Log($"Getting group for index: {groupIndex}");
        var group = cardDatas.FindAll(card => (int)card.cardType == groupIndex);
        foreach (var cardData in group)
        {
            Debug.Log($"Group CardData: {cardData.name}, CardType: {cardData.cardType}");
        }
        return group;
    }

    public void AssignCardDataToCardFront(GameObject cardFront, CardData cardData)
    {
        Image cardFrontImage = cardFront.GetComponent<Image>();
        Image cardImage = cardFront.transform.Find("CardImage")?.GetComponent<Image>();
        Text cardText = cardFront.transform.Find("CardText")?.GetComponent<Text>();
        Button playButton = cardFront.transform.Find("PlayButton")?.GetComponent<Button>();

        Debug.Log($"Assigning data to card front. CardType: {cardData.cardType}, CardData: {cardData.name}");

        cardFrontImage.sprite = cardData.cardFront;

        if (cardData.cardType == CardData.CardType.Art || cardData.cardType == CardData.CardType.Picture)
        {
            if (cardImage != null && cardData.images.Count > 0)
            {
                int randomImageIndex = UnityEngine.Random.Range(0, cardData.images.Count);
                cardImage.sprite = cardData.images[randomImageIndex];
                cardImage.enabled = true;
            }

            if (cardText != null && cardData.questions.Count > 0)
            {
                cardText.text = cardData.questions[UnityEngine.Random.Range(0, cardData.questions.Count)];
                cardText.enabled = true;
            }

            if (playButton != null)
            {
                playButton.gameObject.SetActive(false); // Hide play button for Art and Picture card types
            }
        }
        else if (cardData.cardType == CardData.CardType.Sound)
        {
            if (cardImage != null)
            {
                cardImage.enabled = false; // Hide image for Sound card type
            }

            if (playButton != null)
            {
                playButton.gameObject.SetActive(true); // Show play button for Sound card type
                AudioSource audioSource = cardFront.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = cardFront.AddComponent<AudioSource>();
                }

                if (cardData.soundClips.Count > 0)
                {
                    int randomSoundIndex = UnityEngine.Random.Range(0, cardData.soundClips.Count);
                    audioSource.clip = cardData.soundClips[randomSoundIndex];
                }

                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(() => PlaySound(audioSource));
            }

            if (cardText != null && cardData.questions.Count > 0)
            {
                cardText.text = cardData.questions[UnityEngine.Random.Range(0, cardData.questions.Count)];
                cardText.enabled = true;
            }
        }
    }

    private void PlaySound(AudioSource audioSource)
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
