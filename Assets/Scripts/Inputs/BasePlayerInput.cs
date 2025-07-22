using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class BasePlayerInput : MonoBehaviour
{
    PlayerControls inputControls;

    [HideInInspector] public Vector2 mouseInput;
    [HideInInspector] public Vector2 movementVector;

    float scrollValue;

    [HideInInspector] public ThirdPersonMovement movement;

    private void OnDisable()
    {
        //Cleaning up :)
        if(inputControls != null)
            inputControls.Disable();
    }

    private void Awake()
    {
        movement = GetComponentInChildren<ThirdPersonMovement>();
        InitializeInputs();
    }

    private void InitializeInputs()
    {
        inputControls = new PlayerControls();
        inputControls.Enable();

        inputControls.Master.Movement.performed += ctx => movementVector = ctx.ReadValue<Vector2>();

        inputControls.Master.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        inputControls.Master.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();

        inputControls.Master.Confirm.performed += ctx => HandleConfirm(); 

        inputControls.Master.LeftPrimary.performed += ctx => HandleLeftPrimary();
        inputControls.Master.RightPrimary.performed += ctx => HandleRightPrimary();

        inputControls.Master.LeftSecondary.performed += ctx => HandleLeftSecondary(true);
        inputControls.Master.LeftSecondary.canceled += ctx => HandleLeftSecondary(false);

        inputControls.Master.RightSecondary.performed += ctx => HandleRightSecondary(true);
        inputControls.Master.RightSecondary.canceled += ctx => HandleRightSecondary(false);

        inputControls.Master.South.performed += ctx => HandleSouthButton();
        inputControls.Master.West.performed += ctx => HandleWestButton();
        inputControls.Master.North.performed += ctx => HandleNorthButton();
        inputControls.Master.East.performed += ctx => HandleEastButton();

        inputControls.Master.Left.performed += ctx => Left();
        inputControls.Master.Right.performed += ctx => Right();
        inputControls.Master.Up.performed += ctx => Up();
        inputControls.Master.Down.performed += ctx => Down();

        inputControls.Master.Scroll.performed += ctx => scrollValue = ctx.ReadValue<float>();

    }

    #region Interaction Keys
    public virtual void HandleSouthButton()
    {
       
    }

    public virtual void HandleWestButton()
    {
       
    }

    public virtual void HandleNorthButton()
    {

    }

    public virtual void HandleEastButton()
    {

    }
    #endregion

    #region Scroll

    void HandleScroll()
    {
        if (scrollValue > 0)
        {
            Scroll(-1);
            scrollValue = 0;
        }
        else if(scrollValue < 0)
        {
            Scroll(1);
            scrollValue = 0;
        }
    }

    public virtual void Scroll(int direction) { }
    #endregion

    #region Directions
    public virtual void Left() { }
    public virtual void Right() { }
    public virtual void Up() { }
    public virtual void Down() 
    {
        
    }
    #endregion

    #region Triggers/Mouse

    public virtual void HandleLeftSecondary(bool state) 
    {
        //left bumper
    }

    public virtual void HandleRightSecondary(bool state)
    {
        //Ctrl or right bumper
    }

    public virtual void HandleLeftPrimary()
    {
        //Left click or left trigger
    }

    public virtual void HandleRightPrimary()
    {
        //Right click or right trigger
    }
    #endregion

    public virtual void HandleConfirm()
    {

    }

}
