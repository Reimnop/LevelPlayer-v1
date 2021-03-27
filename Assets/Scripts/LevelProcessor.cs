using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Entity
{
    public float StartTime;
    public float KillTime;

    public List<Entity> Children = new List<Entity>();

    public List<Keyframe> PositionKeyframes = new List<Keyframe>() { new Keyframe { Values = new float[] { 0, 0 } } };
    public List<Keyframe> ScaleKeyframes = new List<Keyframe>() { new Keyframe { Values = new float[] { 2, 2 } } };
    public List<Keyframe> RotationKeyframes = new List<Keyframe>() { new Keyframe { Values = new float[] { 0 } } };
    public List<Keyframe> ColorKeyframes = new List<Keyframe>() { new Keyframe { Values = new float[] { 1, 1, 1, 1 } } };
}

[Serializable]
public class Level
{
    public float Length;
    public List<Entity> SurfaceEntities = new List<Entity>();
}

public class LevelProcessor : MonoBehaviour
{
    public static float CurrentTime;

    public Level Level = new Level();

    public GameObject Prefab;

    public Queue<EntityData> SpawnQueue = new Queue<EntityData>();
    public EntityData NextEntity;

    void Start()
    {
#if UNITY_EDITOR
        //debug init

        for (int i = 0; i < 200; i++)
        {
            List<Keyframe> PositionKeyframes = new List<Keyframe>();
            for (int k = 0; k < 64; k++)
            {
                PositionKeyframes.Add(new Keyframe { Time = k, Easing = Easing.InOutSine, Values = new float[] { UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f) } });
            }

            Level.SurfaceEntities.Add(new Entity() { StartTime = 5.0f, KillTime = 64.0f, PositionKeyframes = PositionKeyframes });
        }

        for (int i = 0; i < 1000; i++)
        {
            List<Keyframe> PositionKeyframes = new List<Keyframe>();
            for (int k = 0; k < 5; k++)
            {
                PositionKeyframes.Add(new Keyframe { Time = k, Easing = Easing.InOutSine, Values = new float[] { UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-20f, 20f) } });
            }

            Level.SurfaceEntities.Add(new Entity() { StartTime = 10.0f, KillTime = 5.0f, PositionKeyframes = PositionKeyframes });
        }

        //----------
#endif

        List<EntityData> entities = new List<EntityData>();

        foreach (var en in Level.SurfaceEntities)
            RecursivelyLoadEntity(en, transform, entities);

        entities.Sort((x, y) => x.Entity.StartTime.CompareTo(y.Entity.StartTime));

        foreach (var en in entities)
            SpawnQueue.Enqueue(en);

        NextEntity = SpawnQueue.Dequeue();
    }

    private void RecursivelyLoadEntity(Entity en, Transform parent, List<EntityData> entities)
    {
        GameObject go = Instantiate(Prefab, parent);

        var ed = go.GetComponent<EntityData>();

        ed.Entity = en;

        ed.PositionSequence = new Sequence(en.PositionKeyframes.ToArray(), 2);
        ed.ScaleSequence = new Sequence(en.ScaleKeyframes.ToArray(), 2);
        ed.RotationSequence = new Sequence(en.RotationKeyframes.ToArray(), 1);
        ed.ColorSequence = new Sequence(en.ColorKeyframes.ToArray(), 4);

        entities.Add(ed);

        foreach (var child in en.Children)
            RecursivelyLoadEntity(child, go.transform, entities);
    }

    void Update()
    {
        CurrentTime += Time.deltaTime;

        CheckSpawn();
    }

    private void CheckSpawn()
    {
        if (SpawnQueue.Count > 0 || NextEntity != null)
        {
            EntityData en = NextEntity;

            if (CurrentTime > en.Entity.StartTime)
            {
                en.gameObject.SetActive(true);

                if (SpawnQueue.Count > 0)
                {
                    NextEntity = SpawnQueue.Dequeue();
                    CheckSpawn();
                }
            }
        }
    }
}
