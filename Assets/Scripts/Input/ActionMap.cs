using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

public class ActionMap<T> {

    #region Fields

    private Dictionary<T, Action> _dictionary;
    #endregion

    #region Properties
    public ActionMap() {
        _dictionary = new Dictionary<T, Action>();
    }
    #endregion

    #region Constructor

    #endregion

    #region Methods
    // alternative of operator[]
    public bool IsActive(T ID) {
        return this[ID].IsActive();
    }
    #endregion

    #region Operators
    // operator[] to access dictionary
    public Action this[T ID] {
        get {
            return _dictionary.ContainsKey(ID) ? _dictionary[ID] : null;
        }
        set {
            if (!_dictionary.ContainsKey(ID))
                _dictionary.Add(ID, value);
            else
                _dictionary[ID] = value;
        }
    }
    #endregion
}
