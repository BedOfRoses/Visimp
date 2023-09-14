using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriangleSurface : MonoBehaviour
{

    private void FixedUpdate()
    {
        /*  Algoritme, som skal skje i hvert tidssteg
         *
         * 1. Identifiser hvilken trekant ballen er på (med barysentriske koordinater)
         *
         * 2. Beregn normalvektoren i kontaktpunktet med underlaget:
         *  N = v0v1 x v0v2 = (v1 - v0) x (v2 - v0)
         *
         * 3. Beregn akselerasjonsvektoren til kula etter ligning 8.12  
         *
         * 4. Oppdater ballens hastighet (ligning (8.14))
         * 5. Oppdater ballens posisjon (ligning(8.15))
         *
         * ---- Tror dette er valgfritt? ----
         * 6.(Beregn ballens rotasjonsvektor)
         * 7.(Beregn ballens rotasjon)
         * 
         */       
        
    }
}
