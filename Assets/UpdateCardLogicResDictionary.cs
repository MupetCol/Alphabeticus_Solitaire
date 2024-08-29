using SimpleSolitaire.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCardLogicResDictionary : MonoBehaviour
{
    public CardLogic logic;

    // Update is called once per frame
    void Update()
    {
        logic.InitializeSpacesDictionary();
    }
}
