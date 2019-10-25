using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void doAction();
    void Transition();

}
