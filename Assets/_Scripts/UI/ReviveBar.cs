using UnityEngine;

[ExecuteInEditMode]
public class ReviveBar : MonoBehaviour
{
    [SerializeField, Range(0,1)] float _reviveNormalized;

    [SerializeField] Color _fillColor;
    [SerializeField] Color _emptyColor;

    [SerializeField, Range(-0.1f,1)] float _lowHealthThreshold; // 0f   보다 작으면 반짝임 X 
    [SerializeField, Range(0,1.1f)] float _highHealthThreshold; // 1.0f 보다 크면 반짝임 X

    [Header("Wave")]
    [SerializeField, Range(0,0.1f)] float _fillWaveAmplitude;
    [SerializeField, Range(0,100f)] float _fillWaveFrequency;
    [SerializeField, Range(0, 1f)] float _fillWaveSpeed;

    [Header("Background")]
    [SerializeField] Color _backgroundColor;

    [Header("Border")]
    [SerializeField, Range(0, 0.15f)] float _borderWidth;
    [SerializeField] Color _borderColor;

    [Header("Enable")]
    [SerializeField]bool _enable = true;

    MaterialPropertyBlock _propBlock;


    /// <summary>
    /// Revive value between 0 and 1
    /// </summary>
    public float ReviveNormalized
    {
        get { return _reviveNormalized; }
        set
        {
            value = Mathf.Clamp(value, 0, 1);
            if (value == _reviveNormalized) return;

            _reviveNormalized = value;
            UpdateMaterialProperties();
        }
    }

    public bool Enable
    {
        set { _enable = value; }
    }

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        UpdateMaterialProperties();
    }

    void UpdateMaterialProperties()
    {
        if (_propBlock == null) return;

        if( TryGetComponent(out Renderer renderer) ) 
        {
            renderer.GetPropertyBlock(_propBlock);

            _propBlock.SetFloat("_healthNormalized", _reviveNormalized); // Shader 에서 _healthNormalized로 쓰임

            if( _reviveNormalized > 0.99f && _enable ){
                _propBlock.SetColor("_fillColor", _fillColor);
            }else{
                _propBlock.SetColor("_fillColor", _emptyColor);
            }

            _propBlock.SetFloat("_lowLifeThreshold", _lowHealthThreshold);
            _propBlock.SetFloat("_highLifeThreshold", _highHealthThreshold);

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
}
