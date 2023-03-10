using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUnlockingManager : MonoBehaviour
{
   static public bool[] unlocked = new bool[10]{ true,true,false,false,false,false,false,false,false,false};
   int i;
   public void unlock(int i)
   {
      unlocked[i+1]=true;
   }
}
