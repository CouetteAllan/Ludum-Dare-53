using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveBackground
{
    public void Move(float speed);
    public static bool CanMove { get; set; }
}
