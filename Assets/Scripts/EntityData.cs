using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityData : MonoBehaviour
{
    public Entity Entity;

    public Sequence PositionSequence;
    public Sequence ScaleSequence;
    public Sequence RotationSequence;
    public Sequence ColorSequence;

    private Renderer rendererComponent;

    private void Start()
    {
        rendererComponent = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (LevelProcessor.CurrentTime > Entity.StartTime + Entity.KillTime)
        {
            Destroy(gameObject);
            return;
        }

        UpdateSequences();
        UpdateData();
    }

    private void UpdateSequences()
    {
        float localTime = LevelProcessor.CurrentTime - Entity.StartTime;

        PositionSequence.Update(localTime);
        ScaleSequence.Update(localTime);
        RotationSequence.Update(localTime);
        ColorSequence.Update(localTime);
    }

    private void UpdateData()
    {
        var pos = PositionSequence.GetValues();
        transform.localPosition = new Vector2(pos[0], pos[1]);

        var sca = ScaleSequence.GetValues();
        transform.localScale = new Vector2(sca[0], sca[1]);

        var rot = RotationSequence.GetValues();
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, rot[0]);

        var col = ColorSequence.GetValues();
        rendererComponent.material.color = new Color(col[0], col[1], col[2], col[3]);
    }
}
