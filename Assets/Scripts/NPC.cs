using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public enum IterTypes
    {
        Say,
        Quest,
        Sell,
        Buy,
        Sell_Buy
    }

    public string Name;
    public float CharacterSize;
    public Vector3 TextPosition;
    public IterTypes TypeOfIteraction;
    

    private GameObject _text;
    // Use this for initialization

    void Start () {
        _text = new GameObject("NPC_name");
        _text.gameObject.AddComponent<TextMesh>();
        _text.GetComponent<TextMesh>().text = Name;
        _text.GetComponent<TextMesh>().offsetZ = -10;
        _text.GetComponent<TextMesh>().characterSize = CharacterSize;
        _text.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
        _text.transform.parent = this.transform;
        _text.transform.localScale = Vector3.one;
        _text.transform.localPosition = TextPosition;
        _text.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    
    public void Interaction()
    {
        if (TypeOfIteraction == IterTypes.Buy)
        {
            
        }
        else if (TypeOfIteraction == IterTypes.Quest)
        {

        }
        else if (TypeOfIteraction == IterTypes.Say)
        {

        }
        else if (TypeOfIteraction == IterTypes.Sell)
        {

        }
        else if (TypeOfIteraction == IterTypes.Sell_Buy)
        {

        }
    }

    void OnMouseEnter()
    {
        _text.gameObject.SetActive(true);
    }

    void OnMouseExit()
    {
        _text.gameObject.SetActive(false);
    }
}
