using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
public class ManaBar : MonoBehaviour, IShakable
{
    [SerializeField, Range(0,1)] float _manaNormalized;
    [SerializeField, Range(0,1)] float _lowManaThreshold;
    [SerializeField, Range(0,1)] float _highManaThreshold;

    [Header("Wave")]
    [SerializeField, Range(0,0.1f)] float _fillWaveAmplitude;
    [SerializeField, Range(0,100f)] float _fillWaveFrequency;
    [SerializeField, Range(0, 1f)] float _fillWaveSpeed;

    [Header("Background")]
    [SerializeField] Color _backgroundColor;

    [Header("Border")]
    [SerializeField, Range(0, 0.15f)] float _borderWidth;
    [SerializeField] Color _borderColor;

    MaterialPropertyBlock _propBlock;
    
    private Vector3 _parentOriginalPosition;
    private RectTransform _parentRectTransform;

    /// <summary>
    /// Mana value between 0 and 1
    /// </summary>
    public float ManaNormalized
    {
        get { return _manaNormalized; }
        set
        {
            value = Mathf.Clamp(value, 0, 1);
            if (value == _manaNormalized) return;

            _manaNormalized = value;
            UpdateMaterialProperties();
        }
    }

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        UpdateMaterialProperties();
        
        _parentOriginalPosition = transform.parent.localPosition;
        _parentRectTransform = transform.parent.GetComponent<RectTransform>();
    }

    void UpdateMaterialProperties()
    {
        if (_propBlock == null) return;

        if( TryGetComponent(out Renderer renderer) ) 
        {
            renderer.GetPropertyBlock(_propBlock);

            _propBlock.SetFloat("_healthNormalized", _manaNormalized); // Shader 에서 _healthNormalized로 쓰임

            _propBlock.SetFloat("_lowLifeThreshold", _lowManaThreshold);
            _propBlock.SetFloat("_highLifeThreshold", _highManaThreshold);

            _propBlock.SetFloat("_waveAmp", _fillWaveAmplitude);
            _propBlock.SetFloat("_waveFreq", _fillWaveFrequency);
            _propBlock.SetFloat("_waveSpeed", _fillWaveSpeed);

            _propBlock.SetColor("_backgroundColor", _backgroundColor);
            _propBlock.SetFloat("_borderWidth", _borderWidth);
            _propBlock.SetColor("_borderColor", _borderColor);

            renderer.SetPropertyBlock(_propBlock);
        }
    }

    void OnValidate()
    {
        UpdateMaterialProperties();
    }
    
    public void Shake(float power = 1f, float duration = 0.5f, int shakeVibrato = 20, float shakeRandomness = 0f)
    {
        transform.parent.localPosition = _parentOriginalPosition;
        _parentRectTransform.DOKill();
        _parentRectTransform.DOShakePosition(duration, new Vector3(power, power / 4f, 0f), shakeVibrato, shakeRandomness).SetLoops(1, LoopType.Restart)
            .OnComplete(() => _parentRectTransform.localPosition = _parentOriginalPosition);
    }
}
