using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongStateSub : MonoBehaviour
{
    [SerializeField] private Button soundSwitchBtn;

    private void Start()
    {
        soundSwitchBtn.onClick.AddListener(delegate { SongStateHandler.instance.SwitchState(); });
    }
}
