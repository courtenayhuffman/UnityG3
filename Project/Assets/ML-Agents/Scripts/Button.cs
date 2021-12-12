using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    // Start is called before the first frame update

    /*
    public event EventHandler Use;
    [SerializeField] private Material green;
    [SerializeField] private Material darkGreen;

    private MeshRenderer buttonMeshRender;
    private Transform buttonTransform;
    private bool canUseButton;

    private void Awake()
    {

      
        buttonTransform = transform.Find("a");
        buttonMeshRender = buttonTransform.GetComponent<MeshRenderer>();
        canUseButton = true;
    }

    private void Start()
    {
        ResetButton();
    }


    public bool CanUseButton() {
        return canUseButton;
    }


    public void UseButton() {
        if (canUseButton) {
            buttonMeshRender.material = darkGreen;
            buttonTransform.localScale = new Vector3(.5f, .2f, .5f);
            canUseButton = false;

            Use?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ResetButton() {
        buttonMeshRender.material = green;
        buttonTransform.localScale = new Vector3(.5f, .5f, .5f);

        transform.localPosition = new Vector3(UnityEngine.Random.Range(-2f, +2f), 0, UnityEngine.Random.Range(-0.6f, +0.6f));
        canUseButton = true;
    }
    */
    // Update is called once per frame

}
