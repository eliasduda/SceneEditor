using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Kalaha : MonoBehaviour
{
    public bool ComputerBeginnt = true;
    public bool höchterGewinn = true;
    public TextMeshProUGUI Anzeige;
    int[] wirFelder = new int[16], gegnerFelder = new int[16];

    bool gegnerIstdran = false;
    int enemyStart = -1;
    bool awaitDirection = false;
    private KeyCode[] keyCodes = {
        KeyCode.Alpha0,
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };


    // Start is called before the first frame update
    void Start()
    {
        StartAuftellung(wirFelder, false);
        StartAuftellung(gegnerFelder, true);
        PrintFiels();
        if (ComputerBeginnt)
        {
            gegnerIstdran = false;
        }
        else
        {
            gegnerIstdran = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gegnerIstdran)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GegnerComputerZug();
            }
            /*
            if (!awaitDirection)
            {
                for (int i = 0; i < keyCodes.Length; i++)
                {
                    if (Input.GetKeyDown(keyCodes[i]))
                    {
                        if (enemyStart == -1)
                        {
                            enemyStart = i * 10;
                            Debug.Log("erste zahl = " + i);
                        }
                        else
                        {
                            enemyStart += i;
                            if (enemyStart > 0 && enemyStart < wirFelder.Length && gegnerFelder[enemyStart] > 1)
                            {
                                Debug.Log("start = " + enemyStart);
                                awaitDirection = true;
                            }
                            else
                            {
                                Debug.Log("probiers nochmal");
                                enemyStart = -1;
                            }
                        }
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.U))
                {
                    GegnerFährt(enemyStart, true, true);
                    enemyStart = -1;
                    awaitDirection = false;
                    gegnerIstdran = false;
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    GegnerFährt(enemyStart, false, true);
                    enemyStart = -1;
                    awaitDirection = false;
                    gegnerIstdran = false;
                }
            }*/
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ComputerZug();
            }
        }
 
    } 


    void ComputerZug()
    {
        Zug weg = bestweg();
        if (weg.start != -1)
        {
            WirFahren( weg.start, weg.uzs, true);

            if (WinCheck())
            {
                Debug.Log("Compuer hat gewonnen !!!");
            }
            else
            {
                gegnerIstdran = true;
            }
        }
        else
        {
            Debug.Log("Compuer hat verloren");
        }

    }

    void GegnerComputerZug()
    {
        Zug weg = bestwegGegner(gegnerFelder, wirFelder);
        if (weg.start != -1)
        {
            GegnerFährt(weg.start, weg.uzs, true);

            if (WinCheck())
            {
                Debug.Log("Compuer hat gewonnen !!!");
            }
            else
            {
                gegnerIstdran = false;
            }
        }
        else
        {
            Debug.Log("Gegner hat verloren");
        }

    }

    Zug bestweg()
    {
        int besteStelle = -1;
        int maximalerGewinn = -1000;
        bool bestUZS = false;

        for (int u = 0 ; u < 16; u+= 1)
        {

            bool uzs = false;
            if (wirFelder[u] == 0 || wirFelder[u] == 1)
            {
                continue;
            }

            int iuzs = WirFahren(u, true);
            int guzs = WirFahren(u, false);


            int gewinn = Mathf.Max(iuzs,guzs);
            if(gewinn == iuzs)
            {
                uzs = true;
            }
            else
            {
                uzs = false;
            }

            if (gewinn > maximalerGewinn)
            {
                maximalerGewinn = gewinn;
                besteStelle = u;
                bestUZS = uzs;
            }
        }

        return new Zug(besteStelle, maximalerGewinn, bestUZS);
    }

    Zug bestwegGegner(int[] gegnerAnfang, int[] wirAnfang)
    {
        int besteStelle = -1;
        int maximalerGewinn = -1;
        bool bestUZS = false;

        for (int u = 0; u < 16; u += 1)
        {

            bool uzs = false;
            if (wirFelder[u] == 0 || wirFelder[u] == 1)
            {
                continue;
            }

            int iuzs = GegnerFährt(u, true, false, gegnerAnfang, wirAnfang);
            int guzs = GegnerFährt(u, false, false, gegnerAnfang, wirAnfang);


            int gewinn = Mathf.Max(iuzs, guzs);
            if (gewinn == iuzs)
            {
                uzs = true;
            }
            else
            {
                uzs = false;
            }

            if (gewinn > maximalerGewinn)
            {
                maximalerGewinn = gewinn;
                besteStelle = u;
                bestUZS = uzs;
            }
        }

        return new Zug(besteStelle, maximalerGewinn, bestUZS);
    }

    int WirFahren(int startPosition, bool uzs, bool wirklichFahren = false)
    {
        int gewonnen = 0;
        int[] wirKopie, gegnerKopie;   
        gewonnen += WirFahren(startPosition, uzs, wirklichFahren,wirFelder, gegnerFelder, out wirKopie,out gegnerKopie);
        if (!wirklichFahren && !höchterGewinn) {
            gewonnen -= bestwegGegner(gegnerKopie, wirKopie).gewinn;
        }
        return gewonnen;
    }

    int WirFahren(int startPosition, bool uzs, bool wirklichFahren,int[]ourInput, int[]enemyInput, out int[] ourOutcome, out int[] enemyOucome)
    {
        int[] wirKopie = CloneArray(ourInput);
        int[] gegnerKopie = CloneArray(enemyInput);
        bool leerFeld = false;
        int jetztPosition = startPosition;
        int gewonneneSteine = 0;

        while(leerFeld == false)
        {
            int steine = wirKopie[jetztPosition];

            int nextPosition = uzs ? jetztPosition + steine : jetztPosition - steine;
            nextPosition = loop(nextPosition);

            wirKopie[jetztPosition] = 0;

            if (!uzs)
            {
                for (int i = jetztPosition - 1, st = steine; st > 0; i--, st--)
                {
                    wirKopie[loop(i)]++;
                }
            }
            else
            {
                for (int i = jetztPosition + 1, st = steine; st > 0; i++, st--)
                {
                    wirKopie[loop(i)]++;
                }
            }

            jetztPosition = nextPosition;


            if (wirKopie[jetztPosition] == 0 || wirKopie[jetztPosition] == 1)
            {
                leerFeld = true;
                break;
            }
            else if (jetztPosition < 8 && gegnerKopie[jetztPosition] > 0)
            {
                int genommeneSteine = (gegnerKopie[jetztPosition] + gegnerKopie[15 - jetztPosition]);
                gewonneneSteine += genommeneSteine;
                wirKopie[jetztPosition] += genommeneSteine;
                gegnerKopie[jetztPosition] = 0;
                gegnerKopie[15 - jetztPosition] = 0;

            }

        }

        if (wirklichFahren)
        {
            wirFelder = wirKopie;
            gegnerFelder = gegnerKopie;

            PrintFiels();

            string richtung = uzs ? "im Uhrzeigersinn" : "gegen den Uhrzeigersinn";
            Debug.Log("Computer startet von " + startPosition + " " + richtung + " und macht " + gewonneneSteine + " Steine gewinn!");
        }

        ourOutcome = wirKopie;
        enemyOucome = gegnerKopie;

        return gewonneneSteine;
    }

    int GegnerFährt(int startPosition, bool uzs, bool wirklichFahren = false)
    {
        int gewonnen = 0;
        gewonnen += GegnerFährt(startPosition, uzs, wirklichFahren, gegnerFelder, wirFelder);
        return gewonnen;
    }

    int GegnerFährt(int startPosition, bool uzs, bool wirklichFahren, int[] enemyInput, int[] ourInput)
    {
        int[] wirKopie = CloneArray(enemyInput);
        int[] gegnerKopie = CloneArray(ourInput);
        bool leerFeld = false;
        int jetztPosition = startPosition;
        int gewonneneSteine = 0;

        while (leerFeld == false)
        {
            int steine = wirKopie[jetztPosition];

            int nextPosition = uzs ? jetztPosition - steine : jetztPosition + steine;
            nextPosition = loop(nextPosition);

            wirKopie[jetztPosition] = 0;

            if (uzs)
            {
                for (int i = jetztPosition - 1, st = steine; st > 0; i--, st--)
                {
                    wirKopie[loop(i)]++;
                }
            }
            else
            {
                for (int i = jetztPosition + 1, st = steine; st > 0; i++, st--)
                {
                    wirKopie[loop(i)]++;
                }
            }

            jetztPosition = nextPosition;


            if (wirKopie[jetztPosition] == 0 || wirKopie[jetztPosition] == 1)
            {
                leerFeld = true;
                break;
            }
            else if (jetztPosition < 8 && gegnerKopie[jetztPosition] > 0)
            {
                int genommeneSteine = (gegnerKopie[jetztPosition] + gegnerKopie[15 - jetztPosition]);
                gewonneneSteine += genommeneSteine;
                wirKopie[jetztPosition] += genommeneSteine;
                gegnerKopie[jetztPosition] = 0;
                gegnerKopie[15 - jetztPosition] = 0;

            }

        }

        if (wirklichFahren)
        {
            wirFelder = gegnerKopie;
            gegnerFelder = wirKopie;

            PrintFiels();

            string richtung = uzs ? "im Uhrzeigersinn" : "gegen den Uhrzeigersinn";
            Debug.Log("Gegner startet von " + startPosition + " " + richtung + " und macht " + gewonneneSteine + " Steine gewinn!");
        }

        wirKopie = gegnerKopie;
        gegnerKopie = wirKopie;

        return gewonneneSteine;
    }

    void StartAuftellung(int[] felder, bool gegner)
    {
        for (int i = 0; i < felder.Length; i++)
        {
            if ((!gegner && (i > 7 || i <= 3)) || gegner && i > 3)
            {
                felder[i] = 2;
            }
            else
            {
                felder[i] = 0;
            }
        }
    }

    public static int loop(int zahl, int loopCount = 16)
    {
        int num = zahl;
        if (num >= loopCount)
        {
            num -= loopCount;
        }
        else if (num < 0)
        {
            num += loopCount;
        }
        if(num >= loopCount || num < 0)
        {
            num = loop(num, loopCount);
        }
        return num;
    }

    public static int[] CloneArray(int[] toClone)
    {
        int[] clone = new int[toClone.Length]; 
        for (int i = 0; i < toClone.Length; i++)
        {
            clone[i] = toClone[i];
        }
        return clone;
    }

    private bool WinCheck()
    {
        bool won = true;
        for(int i = 0; i < gegnerFelder.Length; i++)
        {
            if(gegnerFelder[i] > 1)
            {
                won = false;
            }
        }
        return won;
    }

    public void PrintFiels()
    {
        string text = "";
        text+="GEGNER:\n";
        for (int i = gegnerFelder.Length - 1; i >= 8; i--)
        {
            text += "[" + gegnerFelder[i] + "] ";
        }
        text += "\n";
        for (int i = 0; i < 8; i++)
        {
            text += "[" + gegnerFelder[i] + "] ";
        }
        text += "\n";
        text += "Computer:\n";
        for (int i = 0; i < 8; i++)
        {
            text += "[" + wirFelder[i] + "] ";
        }
        text += "\n";
        for (int i = wirFelder.Length - 1; i >= 8; i--)
        {
            text += "[" + wirFelder[i] + "] ";
        }

        Anzeige.text = text;
    }
}

public class Zug
{
    public int start;
    public bool uzs;
    public int gewinn;

    public Zug(int start, int gewinn, bool uzs)
    {
        this.start = start;
        this.gewinn = gewinn;
        this.uzs = uzs;
    }
}
