using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Allows to expose properties in the inspector
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ExposePropertyAttribute : Attribute {

}