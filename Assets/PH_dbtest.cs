using UnityEngine;
using System.Collections;

[System.Obsolete("Functional and implemented in other classes.")]
public class PH_dbtest : MonoBehaviour {
    public string perksURL = "https://raw.githubusercontent.com/FrontierDev/UnityDatabase/master/Database%20Project/Assets/StreamingAssets/Perks.json";

    IEnumerator Start() {
        WWW perksJSON = new WWW(perksURL);
        yield return perksJSON;
        Debug.Log(perksJSON.text);
    }
}
