using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class DisappearingBlock : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField, Min(0.1f)]
    private float visibleDuration = 3f;

    [SerializeField, Min(0.1f)]
    private float warningDuration = 1f;

    [SerializeField, Min(0.1f)]
    private float hiddenDuration = 2f;

    [SerializeField, Min(0f)]
    private float startDelay = 0f;

    [Header("Appearance")]
    [SerializeField] private Color warningColor = Color.red;

    private BoxCollider blockCollider;
    private MeshRenderer blockRenderer;
    private Material blockMaterial;
    private Color normalColor;

    private void Awake()
    {
        blockCollider = GetComponent<BoxCollider>();
        blockRenderer = GetComponent<MeshRenderer>();

        // Give this block its own material instance.
        blockMaterial = blockRenderer.material;
        normalColor = blockMaterial.color;
    }

    private void OnEnable()
    {
        ShowBlock();
        StartCoroutine(DisappearCycle());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ShowBlock();
    }

    private IEnumerator DisappearCycle()
    {
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        while (true)
        {
            ShowBlock();

            yield return new WaitForSeconds(visibleDuration);

            blockMaterial.color = warningColor;

            yield return new WaitForSeconds(warningDuration);

            blockRenderer.enabled = false;
            blockCollider.enabled = false;

            yield return new WaitForSeconds(hiddenDuration);
        }
    }

    private void ShowBlock()
    {
        blockRenderer.enabled = true;
        blockCollider.enabled = true;
        blockMaterial.color = normalColor;
    }

    private void OnDestroy()
    {
        if (blockMaterial != null)
        {
            Destroy(blockMaterial);
        }
    }
}
