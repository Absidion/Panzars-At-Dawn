using UnityEngine;
using System.Collections;

public class SharedVariables : MonoBehaviour {

    public string SelectedRole;
	// Use this for initialization
	void Start () {
        
    }

    void Awake()
    {
        Object.DontDestroyOnLoad(transform.gameObject);
    }
}
