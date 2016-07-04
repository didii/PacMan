using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class StateMap : MonoBehaviour {
    #region Fields


    //private string _json;

    #endregion

    #region Properties
    
    #endregion

    #region Constructor

    #endregion

    #region Methods

    public void StoreState() {

    }

    public void RestoreInitialState() {
        //JsonUtility.FromJsonOverwrite(_json, GetComponent<NodeMovement>());
    }
    #endregion
}
