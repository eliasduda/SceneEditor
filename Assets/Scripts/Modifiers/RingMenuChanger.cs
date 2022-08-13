using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingMenuChanger : Modifier
{
    public SideLR sideOfMenu;
    public RingMenuItem[] Items;

    public override void OnSelectedEnd()
    {
        RingMenu.GetRingMenu(sideOfMenu).EndCurrentItem();
    }

    public override void OnSelectedStart()
    {
        RingMenu menuToChange  = RingMenu.GetRingMenu(sideOfMenu);
        menuToChange.items = Items;
        menuToChange.SetupMenu();
    }

    public override void OnSelectedUpdate()
    {
    }

}
