using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Bence's Game Kit
namespace BGK.Utility
{
    public static class ObjectManagement
    {
        public static Object FindObjectWithInstanceID(int instanceID)
        {
            #pragma warning disable CS0618
            Object[] objs = Object.FindObjectsOfTypeIncludingAssets(typeof(Object));
            #pragma warning restore CS0618

            foreach (Object obj in objs)
            {
                if (obj.GetInstanceID() == instanceID)
                {
                    return obj;
                }
            }

            return null;
        }
    }

    public enum Order
    {
        Asc,
        Desc
    }

    public static class ArrayManagement
    {
        public static void Add<T>(this T[] array, T value)
        {
            T[] replace = new T[array.Length + 1];

            for (int i = 0; i < array.Length; i++)
            {
                replace[i] = array[i];
            }

            replace[array.Length] = value;

            array = replace;
        }

        public static void Add<T>(this T[,] array, T[] value)
        {
            int length = array.GetLength(1);

            if (length < value.Length)
            {
                length = value.Length;
            }

            T[,] replace = new T[array.GetLength(0) + 1, length];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int l = 0; l < array.GetLength(1); l++)
                {
                    replace[i, l] = array[i, l];
                }
            }

            for (int i = 0; i < value.Length; i++)
            {
                replace[array.GetLength(0), i] = value[i];
            }

            array = replace;
        }

        public static void Replace<T>(this T[] array, T value, T replaceValue)
        {
            int ind = array.LinearSearch(value);

            if (ind == -1)
            {
                return;
            }

            array[ind] = replaceValue;
        }

        public static void ReplaceAll<T>(this T[] array, T value, T replaceValue)
        {
            while (true)
            {
                int ind = array.LinearSearch(value);

                if (ind == -1)
                {
                    return;
                }

                array[ind] = replaceValue;
            }
        }

        public static void Remove<T>(this T[] array, T value)
        {
            int ind = array.LinearSearch(value);

            if (ind == -1)
            {
                return;
            }

            T[] replace = new T[array.Length - 1];

            for (int i = 0; i < replace.Length; i++)
            {
                if (i < ind)
                {
                    replace[i] = array[i];
                }
                else
                {
                    replace[i] = array[i + 1];
                }
            }

            array = replace;
        }

        public static void RemoveAll<T>(this T[] array, T value)
        {
            while (true)
            {
                int ind = array.LinearSearch(value);

                if (ind == -1)
                {
                    return;
                }

                T[] replace = new T[array.Length - 1];

                for (int i = 0; i < replace.Length; i++)
                {
                    if (i < ind)
                    {
                        replace[i] = array[i];
                    }
                    else
                    {
                        replace[i] = array[i + 1];
                    }
                }

                array = replace;
            }
        }

        public static T[][] ConvertToArrayArray<T>(this T[,] array)
        {
            T[][] converted = new T[array.GetLength(0)][];

            for (int i = 0; i < converted.Length; i++)
            {
                converted[i] = new T[array.GetLength(1)];
            }

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int l = 0; l < array.GetLength(1); l++)
                {
                    converted[i][l] = array[i, l];
                }
            }

            return converted;
        }

        public static T[,] ConvertToMultidimensional<T>(this T[][] array, bool skipEmpty)
        {
            int length = 0;
            int maxLength = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                {
                    length++;

                    if (array[i].Length > maxLength)
                    {
                        maxLength = array[i].Length;
                    }
                }
            }

            T[,] converted;

            if (skipEmpty)
            {
                converted = new T[length, maxLength];
            }
            else
            {
                converted = new T[array.Length, maxLength];
            }

            int ind = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                {
                    for (int l = 0; l < array[i].Length; l++)
                    {
                        converted[ind, l] = array[i][l];
                    }

                    ind++;
                }
                else
                {
                    if (!skipEmpty)
                    {
                        ind++;
                    }
                }
            }

            return converted;
        }

        public static T[,] ConvertToMultidimensional<T>(this T[][] array)
        {
            return array.ConvertToMultidimensional(true);
        }
    }

    public static class Sort
    {
        public static T[] BubbleSort<T>(this T[] array, Order ord) where T : System.IComparable<T>
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    bool b;

                    if (ord == Order.Asc)
                    {
                        b = array[j].CompareTo(array[j + 1]) > 0;
                    }
                    else
                    {
                        b = array[j].CompareTo(array[j + 1]) < 0;
                    }

                    if (b)
                    {
                        T tempVar = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = tempVar;
                    }
                }
            }
            return array;
        }

        public static T[] InsertionSort<T>(this T[] array, Order ord) where T : System.IComparable<T>
        {
            for (int i = 1; i < array.Length; i++)
            {
                T key = array[i];
                int flag = 0;
                for (int j = i - 1; j >= 0 && flag != 1;)
                {
                    bool b;

                    if (ord == Order.Desc)
                    {
                        b = key.CompareTo(array[j]) > 0;
                    }
                    else
                    {
                        b = key.CompareTo(array[j]) < 0;
                    }

                    if (b)
                    {
                        array[j + 1] = array[j];
                        j--;
                        array[j + 1] = key;
                    }
                    else flag = 1;
                }
            }
            return array;
        }

        public static T[] SelectionSort<T>(this T[] array, Order ord) where T : System.IComparable<T>
        {
            int arrayLength = array.Length;
            for (int i = 0; i < arrayLength - 1; i++)
            {
                int smallestVal = i;

                for (int j = i + 1; j < arrayLength; j++)
                {
                    bool b;

                    if (ord == Order.Desc)
                    {
                        b = array[j].CompareTo(array[smallestVal]) > 0;
                    }
                    else
                    {
                        b = array[j].CompareTo(array[smallestVal]) < 0;
                    }

                    if (b)
                    {
                        smallestVal = j;
                    }
                }
                T tempVar = array[smallestVal];
                array[smallestVal] = array[i];
                array[i] = tempVar;
            }
            return array;
        }

        public static T[] BubbleSort<T>(this T[] array) where T : System.IComparable<T>
        {
            return BubbleSort(array, Order.Asc);
        }

        public static T[] InsertionSort<T>(this T[] array) where T : System.IComparable<T>
        {
            return InsertionSort(array, Order.Asc);
        }

        public static T[] SelectionSort<T>(this T[] array) where T : System.IComparable<T>
        {
            return SelectionSort(array, Order.Asc);
        }
    }

    public static class SearchSorted
    {
        public static int BinarySearch<T>(this T[] array, T value, Order ord) where T : System.IComparable<T>
        {
            if (array == null || array.Length == 0)
            {
                return -1;
            }

            int left = 0;
            int right = array.Length - 1;

            while (true)
            {
                if (right < left)
                {
                    return -1;
                }

                int mid = (left + right) / 2;

                if (ord == Order.Asc)
                {
                    if (value.CompareTo(array[mid]) > 0)
                    {
                        left = mid + 1;
                    }
                    else if (value.CompareTo(array[mid]) < 0)
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        return mid;
                    }
                }
                else
                {
                    if (value.CompareTo(array[mid]) < 0)
                    {
                        left = mid + 1;
                    }
                    else if (value.CompareTo(array[mid]) > 0)
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        return mid;
                    }
                }
            }
        }
    }

    public static class SearchUnsorted
    {
        public static int LinearSearch<T>(this T[] array, T value)
        {
            if (array == null)
            {
                return -1;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(array[i], value))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
