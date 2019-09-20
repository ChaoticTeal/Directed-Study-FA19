using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzle
{
    void AppendSolution(char entry);
    void AppendSwitchList(Switch addedSwitch);
}
