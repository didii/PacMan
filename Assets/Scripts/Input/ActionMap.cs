using System.Collections.Generic;

/// <summary>
/// Maps a key of type <see cref="T"/> with an action of type <see cref="Action"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class ActionMap<T> {

    #region Fields
    /// <summary>
    /// The dictionary holding the keys and actions.
    /// </summary>
    private readonly Dictionary<T, Action> _dictionary = new Dictionary<T, Action>();
    #endregion

    #region Methods
    /// <summary>
    /// Alternative to operator[]
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public bool IsActive(T ID) {
        return this[ID].IsActive();
    }
    #endregion

    #region Operators
    /// <summary>
    /// Operator[] to access dictionary. Automatically adds the key-value pair to the dictionary if not yet present.
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
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
