using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherView : MonoBehaviour
{
    [SerializeField] GameClearView clearView;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(10);
        clearView.Show();

    }

}
