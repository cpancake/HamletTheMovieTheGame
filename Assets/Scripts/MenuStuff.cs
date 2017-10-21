using UnityEngine;
using System.Collections;

public class MenuStuff : MonoBehaviour
{
	private bool _loading = false;
	private AsyncOperation _loadingOp;

	void Start ()
	{
	
	}

	void OnGUI()
	{
		if(_loading)
		{
			GUI.Label(new Rect(0, 0, 100, 50), "Loading: " + Mathf.Floor(_loadingOp.progress * 100) + "%");
		}
	}

	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Return))
		{
			StartCoroutine("Load");
		}
	}

	IEnumerator Load()
	{
		_loadingOp = Application.LoadLevelAsync(1);
		_loading = true;
		yield return _loadingOp;
	}
}

