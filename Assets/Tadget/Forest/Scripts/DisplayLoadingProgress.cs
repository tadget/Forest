using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayLoadingProgress : MonoBehaviour
{
    private TextMeshProUGUI LoadingText;

	void Start ()
	{
	    LoadingText = GetComponent<TextMeshProUGUI>();

	    if (LoadingText == null)
	        Destroy(gameObject);
	}
	
	void Update ()
	{
	    LoadingText.text = SceneLoaderAsync.Instance.LoadingProgress.ToString("F0") + "%";
	}
}
