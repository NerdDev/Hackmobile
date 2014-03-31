using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PrefabProbabilityContainer
{
    public float Multiplier = 1f;
    public bool Unique;
    public UnityEngine.Object Item;
}

//[CustomPropertyDrawer(typeof(PrefabProbabilityContainer))]
//public class PrefabProbabilityContainerDrawer : PropertyDrawer
//{
//    const int multWidth = 95;
//    const int uniqueWidth = 10;
//    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
//    {
//        SerializedProperty multiplier = prop.FindPropertyRelative("Multiplier");
//        SerializedProperty unique = prop.FindPropertyRelative("Unique");
//        //EditorGUI.BeginChangeCheck();
//        //multiplier.floatValue = EditorGUI.FloatField(new Rect(pos.x, pos.y, multWidth, pos.height), new GUIContent("Multiplier"), multiplier.floatValue);
//        //EditorGUI.EndChangeCheck();
//        //EditorGUI.BeginChangeCheck();
//        //unique.boolValue = EditorGUI.Toggle(new Rect(pos.x + multWidth, pos.y, uniqueWidth, pos.height), GUIContent.none, unique.boolValue);
//        //EditorGUI.EndChangeCheck();
//        //EditorGUI.Slider(pos, multiplier, 0f, 10f);
//        EditorGUI.indentLevel++;
//        EditorGUI.LabelField(new Rect(pos.x, pos.y, 100, pos.height), new GUIContent("Mult"));
//        //EditorGUI.TextArea
//        EditorGUI.PropertyField(new Rect(pos.x + 40, pos.y, multWidth, pos.height), multiplier, GUIContent.none);
//        EditorGUI.indentLevel--;
//    }
//    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    //{
//    //    //EditorGUI.LabelField(position, "TEST", EditorStyles.whiteMiniLabel);
//    //    PrefabProbabilityContainer myTarget = (PrefabProbabilityContainer)attribute;
//    //    Rect label1 = EditorGU
//    //    double test = (double)EditorGUI.FloatField(position, "Multiplier", 0f);
//    //    //myTarget.Unique = EditorGUILayout.Toggle("Unique", myTarget.Unique);
//    //    //UnityEngine.Object obj = myTarget.Item;
//    //    //Type t = obj.GetType();
//    //    //myTarget.Item = EditorGUILayout.ObjectField("Item", obj, t);
//    //    //label = EditorGUI.BeginProperty(position, label, property);
//    //    //Rect contentPosition = EditorGUI.PrefixLabel(position, label);
//    //    //if (position.height > 16f)
//    //    //{
//    //    //    position.height = 16f;
//    //    //    EditorGUI.indentLevel += 1;
//    //    //    contentPosition = EditorGUI.IndentedRect(position);
//    //    //    contentPosition.y += 18f;
//    //    //}
//    //    //contentPosition.width *= 0.75f;
//    //    //EditorGUI.indentLevel = 0;
//    //    //EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("position"), GUIContent.none);
//    //    //contentPosition.x += contentPosition.width;
//    //    //contentPosition.width /= 3f;
//    //    //EditorGUIUtility.labelWidth = 14f;
//    //    //EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("color"), new GUIContent("C"));
//    //    //EditorGUI.EndProperty();
//    //}
//}