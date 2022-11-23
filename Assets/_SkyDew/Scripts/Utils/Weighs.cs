using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Калькулятор весов
/// </summary>
public static class Weights
{

    /// <summary>
    /// Получить ячейку из списка ячеек на основе весов
    /// </summary>
    public static WeightNode<T> GetWeightNode<T>(IList<WeightNode<T>> list) => BaseSerch(list);

    /// <summary>
    /// Получить элемент из списка ячеек на основе весов
    /// </summary>
    public static T GetWeightObject<T>(IList<WeightNode<T>> list) => BaseSerch(list).value;

    /// <summary>
    /// Получить элемент из перечня ячеек на основе весов
    /// </summary>
    public static T GetWeightObject<T>(params WeightNode<T>[] array)  => BaseSerch(array).value;

    /// <summary>
    /// Получить элемент из списка на основе весов
    /// </summary>
    public static T GetWeightObject<T>(IList<T> list) where T : IHasWeight => BaseSerch(list);

    /// <summary>
    /// Получить элемент из перечня на основе весов
    /// </summary>
    public static T GetWeightObject<T>(params T[] array) where T : IHasWeight => BaseSerch(array);

    static T BaseSerch<T>(IList<T> list) where T : IHasWeight
    {
        var sum = list.Sum(x => x.Weight);
        var rnd = Random.Range(0, sum);
        float th = 0;
        for (int i = 0; i < list.Count; i++)
        {
            th += list[i].Weight;
            if (rnd < th)
                return list[i];
        }
        throw new System.Exception();
    }
}

[Serializable]
public class WeightNode<T> : IHasWeight
{
    public T value;
    public float weight;

    public WeightNode(T value, float weight)
    {
        this.value = value;
        this.weight = weight;
    }

    public float Weight => weight;
}

/// <summary>
/// Объект имеет вес для выборки
/// </summary>
public interface IHasWeight
{
    float Weight { get; }
}