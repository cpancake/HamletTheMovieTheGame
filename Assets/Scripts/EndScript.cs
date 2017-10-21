using UnityEngine;
using System.Collections;

public class EndScript : MonoBehaviour
{
	public int NumHit;
	public int TotalNotes;
	public int Score;

	public void OnGUI()
	{
		GUI.Box(new Rect(100, 100, Screen.width - 200, Screen.height - 200), 
		        "You hit " + NumHit + " things out of " + TotalNotes + " things.\n" +
		        "Your score was " + Score + ".\n" +
		        "Press R to restart or Escape to quit."
		);
	}

	public void Update()
	{
		if(Input.GetKey(KeyCode.Escape))
			Application.Quit();
		if(Input.GetKeyDown(KeyCode.R))
			Application.LoadLevel(Application.loadedLevel);
	}
}

