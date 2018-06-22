using UnityEngine;
using System.Collections;

public class QuitProgram: MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public void onMouseDown()
    {
        Application.Quit();
    }

	// Update is called once per frame
}
