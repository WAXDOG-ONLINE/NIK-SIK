using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour {
    [SerializeField]
    private ActionTable actionTable;
    // Start is called before the first frame update
    public void activate() {
        actionTable.chooseAction();
    }


}
