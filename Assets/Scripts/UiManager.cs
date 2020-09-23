using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    
    [SerializeField] private Text levelText = default;
    [SerializeField] private Image progressFill = default;
    
    // Start is called before the first frame update
    private void Start()
    {
        GameManager.Instance.OnLevelChanged += LevelChanged;
        GameManager.Instance.OnProgressChanged += Progress;
    }

    private void Progress(int total, int collected)
    {
        if (collected > 0)
        {
            progressFill.fillAmount = (float) collected / total;
        }
        else
        {
            progressFill.fillAmount = 0.0f;
        }
        
        //Debug.Log("Total: " + total + "Collected: " + collected);
    }
    
    private void LevelChanged(int level)
    {
        levelText.text = "Level: " + (level + 1);
    }
}
