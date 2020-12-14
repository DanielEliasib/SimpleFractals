using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.VFX;

public class OptionsController : MonoBehaviour
{
    [SerializeField] private RectTransform _Panel;
    private bool _Show, _Moving;

    [SerializeField] private VisualEffect _EffectFractal;
    [SerializeField] private VisualEffect _ExactFractal;

    Vector3 target;

    private void Awake()
    {
        var pos = _Panel.position;
        pos.x = -400;
        _Panel.position = pos;

        target = pos;

        _Show = false;
        _Moving = false;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _Show = !_Show;
            _Moving = true;
            SetPosition(_Show);
        }

        // Probably there is a WAY better of doing this but it works ¯\_(ツ)_/¯
        if (_Moving)
        {
            if (Mathf.Abs(target.x - _Panel.position.x) >= 1)
            {
                _Panel.position = Vector3.Lerp(_Panel.position, target, Time.deltaTime * 10);
            }
            else
            {
                _Panel.position = target;
                _Moving = false;
            }
        }
       
    }

    public void SetExactPointSize(string size)
    {
        float val = float.Parse(size);
        _ExactFractal.SetFloat("size", val);
        _ExactFractal.Reinit();
    }

    public void SetExactnPoints(string size)
    {
        int val = int.Parse(size);
        _ExactFractal.SetInt("nPoint", val);
        _ExactFractal.Reinit();
    }

    public void SetEffectnPoints(string size)
    {
        int val = int.Parse(size);
        _EffectFractal.SetInt("nPoint", val);
    }

    public void ToggleEffect()
    {
        _EffectFractal.enabled = !_EffectFractal.enabled;
    }

    public void ToggleExact()
    {
        _ExactFractal.enabled = !_ExactFractal.enabled;
    }

    private void SetPosition(bool show)
    {
        var pos = _Panel.position;

        pos.x = show? 10: -400;
        
        target = pos;
    }
}
