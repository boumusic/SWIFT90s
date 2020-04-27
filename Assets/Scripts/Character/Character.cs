using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody body;

    #region Movement

    #region 



    #endregion

    #region Input

    private float horizontal = 0;

    #endregion

    #endregion

    private void Update()
    {
        
    }

    #region Input

    public void InputHorizontal(float horizontal)
    {
        this.horizontal = horizontal;
    }

    #endregion

    #region Horizontal Movement

    private void ApplyVelocity()
    {

    }

    #endregion

    #region Cast



    #endregion
}
