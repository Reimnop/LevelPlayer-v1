using UnityEngine;

[System.Serializable]
public struct Keyframe
{
    public float Time;
    public float[] Values;
    public Easing Easing;
}

public sealed class Sequence
{
    private Keyframe[] keyframes;
    private float[] currentValues;

    private int count;

    public Sequence(Keyframe[] keyframes, int count)
    {
        this.keyframes = keyframes;
        this.count = count;

        currentValues = new float[count];
    }

    public float[] GetValues()
        => currentValues;

    public void Update(float time)
    {
        if (keyframes.Length == 1)
        {
            DeepCopyArray(keyframes[0].Values);
            return;
        }

        if (time <= keyframes[0].Time) 
        {
            DeepCopyArray(keyframes[0].Values);
            return;
        }

        if (time >= keyframes[keyframes.Length - 1].Time)
        {
            DeepCopyArray(keyframes[keyframes.Length - 1].Values);
            return;
        }

        var pair = FindClosestPair(time);

        Keyframe start = pair.Item1;
        Keyframe end = pair.Item2;

        float length = end.Time - start.Time;
        float t = (time - start.Time) / length;

        float easedT = Ease.EaseLookup[end.Easing](t);

        for (int i = 0; i < count; i++)
        {
            currentValues[i] = Mathf.Lerp(start.Values[i], end.Values[i], easedT);
        }
    }

    private void DeepCopyArray(float[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
            currentValues[i] = arr[i];
    }

    private (Keyframe, Keyframe) FindClosestPair(float time)
    {
        int mid = keyframes.Length / 2;

        if (time >= keyframes[mid - 1].Time && time < keyframes[mid].Time)
        {
            return (keyframes[mid - 1], keyframes[mid]);
        }

        if (time >= keyframes[mid].Time)
        {
            while (time >= keyframes[mid].Time)
                mid++;
        }
        else
        {
            while (time < keyframes[mid - 1].Time)
                mid--;
        }

        return (keyframes[mid - 1], keyframes[mid]);
    }
}
