using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Other
{
    public static class Utility
    {
        public static IEnumerator WaitForSeconds(float t, Action<object[]> action, object[] parameters = null,
                                                       bool isLoop = false, Func<bool> loopEndCondition = null)
        {
            do
            {
                yield return new WaitForSeconds(t);
                action(parameters);

            } while (isLoop && (loopEndCondition == null || !loopEndCondition()));

            yield break;
        }

        public static T GetNext<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
                return enumerator.Current;
            return default;
        }

        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T GetRandomEnumValue<T>()
        {
            var values = GetEnumValues<T>().ToList();

            return values[UnityEngine.Random.Range(0, values.Count)];
        }

        public static T GetRandomEnumValue<T>(List<T> excludesList)
        {
            var values = GetEnumValues<T>().ToList();

            values.RemoveAll(x => excludesList.Contains(x));

            return values[UnityEngine.Random.Range(0, values.Count)];
        }

    }
}