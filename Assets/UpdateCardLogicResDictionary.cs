using SimpleSolitaire.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCardLogicResDictionary : MonoBehaviour
{
    public CardLogic logic;
    public HintManager hintManager;

	private float screenWidth, screenHeight;

	// Update is called once per frame
	private void Start()
	{
		screenHeight = Screen.height;
		screenWidth = Screen.width;
		
	}

	private void Update()
	{
		logic.InitializeSpacesDictionary();
		//hintManager.UpdateAvailableForDragCards();
	}
}
