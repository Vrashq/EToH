using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace PanzerNoob
{
    public static class ListExtensions
    {
        public static T Last<T> (this List<T> list) 
        {
            return (T)list[list.Count-1];
        }

        public static T Random<T> (this List<T> list) 
        {
            return (T)list[UnityRandom.Range(0,list.Count-1)];
        }

        public static T First<T> (this List<T> list)
        {
            return (T)list[0];
        }
        
        public static List<T> Swap<T>(this List<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }
    }

    public static class ArrayExtensions 
    {
        public static T Last<T> (this T[] list) 
        {
            return list[list.Length-1];
        }

        public static T Random<T> (this T[] list) 
        {
            return (T)list[UnityRandom.Range(0,list.Length-1)];
        }

        public static T First<T> (this T[] list)
        {
            return list[0];
        }
    }

    public static class VectorExtensions
    {
        public static Vector3 Reverse (this Vector3 vec) {
            return vec * -1;
        }

        public static Vector3 SetX (this Vector3 vec, float x) {
            vec.x = x;
            return vec;
        }

        public static Vector3 SetY (this Vector3 vec, float y) {
            vec.y = y;
            return vec;
        }

        public static Vector3 SetZ (this Vector3 vec, float z) {
            vec.z = z;
            return vec;
        }

        public static float BiggerValue (this Vector3 vec) {
            return Mathf.Max(vec.x, vec.y, vec.z);
        }

        public static float SmallerValue (this Vector3 vec) {
            return Mathf.Min(vec.x, vec.y, vec.z);
        }
    }

    public static class ObjectExtensions 
    {
        public static bool IsArray(this object o)
        {
            return o.GetType().IsArray;
        }

        public static bool IsList(this object o)
        {
            return o is IList && o.GetType().IsGenericType;
        }

        public static bool IsEmpty(this object o)
        {
            return o == null;
        }
    }

    public static class TypeExtensions 
    {
        public static bool IsList(this Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }
    }

    public static class FloatExtensions 
    {
        public static float AbsModulo (this float value, float modulo) {
            return Mathf.Abs(value % modulo);
        }
    }
}
