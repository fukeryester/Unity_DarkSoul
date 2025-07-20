using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUserInput : MonoBehaviour
{
    [Header("===== Output Signals ======")]
    public float Dup;
    public float Dright;
    public float Dmag;
    public Vector3 Dvec;
    public float CUp;
    public float CRight;

    //1. press and hold
    public bool run;
    public bool defence;

    //2. press once to trigger
    public bool jump;
    protected bool lastjump;
    public bool roll;
    //public bool attack;
    public bool lockon;

    public bool lb;
    public bool lt;
    public bool rb;
    public bool rt;

    //3. press twice to trigger
    [Header("===== Others ======")]
    public bool inputEnabled = true;
    public float TargetDup;
    public float TargetDright;
    protected float DupVelocity;
    protected float DrightVelocity;

    protected Vector2 SquaretoCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2);
        return output;
    }

    protected void UpdateDmagDvec(float Dup, float Dright)
    {
        Vector2 D_fixed_signal = SquaretoCircle(new Vector2(Dright, Dup));
        Dmag = Mathf.Sqrt((D_fixed_signal.y * D_fixed_signal.y) + (D_fixed_signal.x * D_fixed_signal.x));
        Dvec = D_fixed_signal.y * transform.forward + D_fixed_signal.x * transform.right;
    }
}
