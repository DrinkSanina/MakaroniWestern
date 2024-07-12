using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEPress : MonoBehaviour
{
    public float fillAmount = 1f;
    private float timeTreshold = 0;
    public bool respect;
    private bool sthap;
    [Range(0f,1f)]
    public float speed = 0.01f;
    private TextMeshProUGUI ButtonText;
    public List<string> KbrdBtns;
    private string CurrentBtn;
    private Vector4 Color;
    

    // Start is called before the first frame update
    void Start()
    {
        ButtonText = GetComponentInChildren<TextMeshProUGUI>();
        sthap = false;
        RandomDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(CurrentBtn))
        {
            sthap = true;
            Color = new Color(0, 1, 0, 1);
            gameObject.GetComponent<Image>().color = Color;
            respect = true;
            Destroy(this.gameObject, 0.7f);
        }
        else if(AnyKey())
        {
            sthap = true;
            Color = new Vector4(1, 0, 0, 1);
            gameObject.GetComponent<Image>().color = Color;
            respect = false;
            Destroy(this.gameObject, 0.7f);
        }

        timeTreshold += Time.deltaTime;

        if (timeTreshold>0.1f&&!sthap)
        {
            timeTreshold = 0;
            fillAmount -= speed;
        }
        
        if(fillAmount < 0)
        {
            fillAmount = 0;
        }

        GetComponent<Image>().fillAmount = fillAmount;    
    }

    void RandomDisplay()
    {      
        int index = Random.Range(0, KbrdBtns.Count);
        CurrentBtn = KbrdBtns[index];
        ButtonText.text = CurrentBtn.ToUpper();
    }

    bool AnyKey()
    {
        foreach(string code in KbrdBtns)
        {
            if(Input.GetKeyDown(code)&&code!=CurrentBtn)
            {
                return true;
            }
        }
        return false;
    }
}

