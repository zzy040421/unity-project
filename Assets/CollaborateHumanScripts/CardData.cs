using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "Card Data", order = 51)]
public class CardData : ScriptableObject
{
    public enum CardType { Art, Picture, Sound }

    public CardType cardType;
    public Sprite cardFront;
    public List<Sprite> images = new List<Sprite>();
    public List<string> questions = new List<string>();
    public List<AudioClip> soundClips = new List<AudioClip>(); 
}
