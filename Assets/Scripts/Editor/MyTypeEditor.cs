#if false
using UnityEditor;

// The class
public class MyType {}

// Ability to expose properties of above class
[CustomEditor(typeof(MyType))]
public class MyTypeEditor : Editor {
    #region Fields
    IntersectionNode m_Instance;
    PropertyField[] m_fields;
    #endregion

    #region Events
    public void OnEnable() {
        m_Instance = target as IntersectionNode;
        m_fields = ExposeProperties.GetProperties(m_Instance);
    }

    public override void OnInspectorGUI() {
        if (m_Instance == null)
            return;

        this.DrawDefaultInspector();
        ExposeProperties.Expose(m_fields);
    }
    #endregion
}
#endif