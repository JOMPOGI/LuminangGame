using UnityEngine;

/// <summary>
/// Makes the potato glow with emission AND adds a pulsing streak of light shooting upward.
/// Works in URP.
/// </summary>
public class PotatoGlow : MonoBehaviour
{
    [Header("Glow Color (HDR)")]
    [ColorUsage(true, true)]
    public Color glowColor = new Color(4f, 3.5f, 0.5f, 1f);

    [Header("Pulse")]
    public float pulseSpeed   = 3f;
    public float minIntensity = 1f;
    public float maxIntensity = 8f;

    [Header("Upward Streak")]
    public float streakHeight     = 1.5f;  // How tall the beam goes
    public float streakWidthBase  = 0.15f; // Width at the bottom
    public float streakWidthTip   = 0f;    // Width at the top (tapers to point)

    private Renderer    _renderer;
    private Material    _mat;
    private LineRenderer _line;
    private Material    _lineMat;

    void Start()
    {
        SetupEmission();
        SetupStreak();
    }

    void SetupEmission()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null) return;

        _mat = _renderer.material;
        _mat.EnableKeyword("_EMISSION");
        _mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
    }

    void SetupStreak()
    {
        // Create a child object to hold the line renderer
        GameObject streakGO = new GameObject("_Streak");
        streakGO.transform.SetParent(transform);
        streakGO.transform.localPosition = Vector3.zero;

        _line                  = streakGO.AddComponent<LineRenderer>();
        _line.positionCount    = 2;
        _line.useWorldSpace    = false;
        _line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _line.receiveShadows   = false;

        // Start at potato, shoot straight up
        _line.SetPosition(0, Vector3.zero);
        _line.SetPosition(1, Vector3.up * streakHeight);

        // Taper from wide at base to a point at the top
        _line.startWidth = streakWidthBase;
        _line.endWidth   = streakWidthTip;

        // URP Unlit material — guaranteed to work
        Shader urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");
        if (urpUnlit != null)
        {
            _lineMat = new Material(urpUnlit);
        }
        else
        {
            // Fallback
            _lineMat = new Material(Shader.Find("Unlit/Color"));
        }

        // Set to transparent/additive so it looks like light
        _lineMat.SetFloat("_Surface",  1);   // Transparent
        _lineMat.SetFloat("_Blend",    3);   // Additive
        _lineMat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _lineMat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
        _lineMat.SetColor("_BaseColor", new Color(1f, 0.95f, 0.3f, 0.8f));
        _lineMat.renderQueue = 3000;

        _line.material = _lineMat;
    }

    void Update()
    {
        float t         = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        // Pulse the potato glow
        if (_mat != null)
            _mat.SetColor("_EmissionColor", glowColor * intensity);

        // Pulse the streak width and alpha
        if (_line != null)
        {
            float width = Mathf.Lerp(0.02f, streakWidthBase, t);
            _line.startWidth = width;

            float alpha = Mathf.Lerp(0.1f, 0.85f, t);
            _line.startColor = new Color(1f, 0.95f, 0.3f, alpha);
            _line.endColor   = new Color(1f, 0.95f, 0.3f, 0f);
        }
    }

    void OnDestroy()
    {
        if (_mat != null)    Destroy(_mat);
        if (_lineMat != null) Destroy(_lineMat);
    }
}
