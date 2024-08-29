using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScene : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> sprites = new List<Sprite>();
    public List<string> nameList = new List<string>();
    public bool isHuman;
    public bool isRobot;
    public int index = 0;
    public Image image;
    public Text text;
    public GameObject returnB;
    public GameObject nextB;
    void Start()
    {
        if(isHuman)
        {
            nameList = ClientContent.Instance().allName;
            nameList.Remove(PhotonNetwork.MasterClient.NickName);
            OnNext();
        }else if(isRobot)
        {
            nameList = ClientContent.Instance().allName;
            if (!nameList.Contains(PhotonNetwork.MasterClient.NickName))
                nameList.Add(PhotonNetwork.MasterClient.NickName);
            OnNextWto();
        }
    }
    public void OnNext()
    {
        text.text = nameList[index] + ",You Made it!";
        OnEnaOne(nameList[index]);
        if (index == nameList.Count-1)
            OnReturn();
        else
            index++;
    }
    public void OnNextWto()
    {
        text.text = nameList[index] + ",You Made it!";
        OnEnaTwo(nameList[index]);
        if (index == nameList.Count-1)
            OnReturn();
        else
            index++;
    }
    public void OnReturn()
    {
        returnB.SetActive(true);
        nextB.SetActive(false);
    }
    public void OnEnaOne(string na)
    {
        foreach(var x in RecordScoreScene.instance.scoreD[na])
        {
            Debug.Log(x.Key + x.Value);
        }
        if (RecordScoreScene.instance.scoreD[na][ScoreType.detail]==2)
        {
            image.sprite = sprites[0];
        }else if (RecordScoreScene.instance.scoreD[na][ScoreType.disclose] == 2)
        {
            image.sprite = sprites[1];
        }else if (RecordScoreScene.instance.scoreD[na][ScoreType.passion] == 2)
        {
            image.sprite = sprites[2];
        }else if (RecordScoreScene.instance.scoreD[na][ScoreType.detail] == 1 && RecordScoreScene.instance.scoreD[na][ScoreType.disclose]==1)
        {
            image.sprite = sprites[3];
        }
        else if (RecordScoreScene.instance.scoreD[na][ScoreType.detail] == 1 && RecordScoreScene.instance.scoreD[na][ScoreType.passion] == 1)
        {
            image.sprite = sprites[4];
        }
        else if (RecordScoreScene.instance.scoreD[na][ScoreType.passion] == 1 && RecordScoreScene.instance.scoreD[na][ScoreType.disclose] == 1)
        {
            image.sprite = sprites[5];
        }
    }
    public void OnEnaTwo(string na)
    {
        foreach (var x in RecordScoreScene.instance.scoreD[na])
        {
            Debug.Log(x.Key + x.Value);
        }
        if (RecordScoreScene.instance.scoreD[na][ScoreType.detail] == 2)
        {
            image.sprite = sprites[0];
        }
        else if (RecordScoreScene.instance.scoreD[na][ScoreType.disclose] == 2)
        {
            image.sprite = sprites[1];
        }
        else if (RecordScoreScene.instance.scoreD[na][ScoreType.passion] == 2)
        {
            image.sprite = sprites[2];
        }
        else if (RecordScoreScene.instance.scoreD[na][ScoreType.detail] == 1 && RecordScoreScene.instance.scoreD[na][ScoreType.disclose] == 1)
        {
            image.sprite = sprites[3];
        }
        else if (RecordScoreScene.instance.scoreD[na][ScoreType.detail] == 1 && RecordScoreScene.instance.scoreD[na][ScoreType.passion] == 1)
        {
            image.sprite = sprites[4];
        }
        else if (RecordScoreScene.instance.scoreD[na][ScoreType.passion] == 1 && RecordScoreScene.instance.scoreD[na][ScoreType.disclose] == 1)
        {
            image.sprite = sprites[5];
        }
    }
}
