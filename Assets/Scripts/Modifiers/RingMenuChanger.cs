using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Modifier that allows to change a Ringmenus contents on selection
public class RingMenuChanger : Modifier
{
    //Menu thats contents should be changed
    public SideLR sideOfMenu;
    //new contents for the menu
    public RingMenuItem[] Items;

    public override void OnSelectedEnd()
    {
        //end any active modifiers
        RingMenu.GetRingMenu(sideOfMenu).EndCurrentItem();
    }

    public override void OnSelectedStart()
    {
        //change the items of the menu
        RingMenu menuToChange  = RingMenu.GetRingMenu(sideOfMenu);
        menuToChange.items = Items;
        menuToChange.SetupMenu();
    }

    public override void OnSelectedUpdate()
    {
    }

}
