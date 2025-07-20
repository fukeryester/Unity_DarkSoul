using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : IUserInput
{
    //Variable
    [Header("===== Key Inputs ======")]
    public string keyUp = "w";
    public string keyDown = "s";
    public string keyLeft = "a";
    public string keyRight = "d";

    public string keyRun = "left shift";
    public string keyJump = "f";
    public string keyRoll = "space";
    public string keyLeftMouse = "mouse 0";
    public string keyRightMouse = "mouse 1";
    public string keyPlusInput = "left alt";
    

    [Header("===== Camera Inputs =====")]
    public string keyCUp = "up";
    public string keyCDown = "down";
    public string keyCLeft = "left";
    public string keyCRight = "right";
    public string keyLockOn = "o";
    public bool mouseEnabled = true;
    public float mouseSensitibityX = 1.0f;
    public float mouseSensitibityY = 1.0f;


    // Update is called once per frame
    void Update()
    {
        if (mouseEnabled)
        {
            CUp = Input.GetAxis("Mouse Y") * 2f * mouseSensitibityY;
            CRight = Input.GetAxis("Mouse X") * 2f * mouseSensitibityX;
        }
        else
        {
            CUp = (Input.GetKey(keyCUp) ? 1f : 0) - (Input.GetKey(keyCDown) ? 1f : 0);
            CRight = (Input.GetKey(keyCRight) ? 1f : 0) - (Input.GetKey(keyCLeft) ? 1f : 0);
        }

        TargetDup = (Input.GetKey(keyUp) ? 1f : 0) - (Input.GetKey(keyDown) ? 1f : 0);
        TargetDright = (Input.GetKey(keyRight) ? 1f : 0) - (Input.GetKey(keyLeft) ? 1f : 0);

        if (inputEnabled == false)
        {
            TargetDup = 0;
            TargetDright = 0;
        }

        Dup = Mathf.SmoothDamp(Dup, TargetDup, ref DupVelocity, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, TargetDright, ref DrightVelocity, 0.1f);

        UpdateDmagDvec(Dup, Dright);

        if(inputEnabled == true)
        {
            //run input
            run = Input.GetKey(keyRun);

            //jump input (=GetKeyDown)
            bool newjump = Input.GetKey(keyJump);
            if (newjump != lastjump && newjump == true)
            {
                jump = true;
            }
            else
            {
                jump = false;
            }
            lastjump = newjump;
        }

        //attack input or plus attack input
        rb = Input.GetKeyDown(keyLeftMouse);
        rt = Input.GetKeyDown(keyLeftMouse) && Input.GetKey(keyPlusInput);
        lb = Input.GetKeyDown(keyRightMouse);
        lt = Input.GetKeyDown(keyRightMouse) && Input.GetKey(keyPlusInput);

        //roll input
        roll = Input.GetKeyDown(keyRoll);

        //defence input
        defence = Input.GetKey(keyRightMouse);

        //camera lock input
        lockon = Input.GetKeyDown(keyLockOn);
    }

}