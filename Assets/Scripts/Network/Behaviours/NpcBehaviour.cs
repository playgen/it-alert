using PlayGen.ITAlert.Simulation.Contracts;
using UnityEngine;
using UnityEngine.UI;


// ReSharper disable once CheckNamespace
public class NpcBehaviour : EntityBehaviour<VirusState>
{
    //private Image _timerImage;

    ///private float _imageFillTimer;
    private bool _pulseDown = true;

    //private string _warningText;

    private SpriteRenderer _spriteRenderer;

    #region Initialization

    public void Start()
    {
        
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void OnInitialize()
    {
        //UIManager.DisplayWarning(_warningText);
    }

    #endregion

    #region Unity Update

    protected override void OnFixedUpdate()
    {
    }

    protected override void OnUpdate()
    {

    }

    #endregion

    #region State Update

    protected override void OnUpdatedState()
    {
        HandlePulse();
    }

    private void HandlePulse()
    {
        if (EntityState.Active)
        {
            if (_pulseDown)
            {
                _spriteRenderer.color -= new Color(0, 0, 0, 0.05f);
            }
            else
            {
                _spriteRenderer.color += new Color(0, 0, 0, 0.05f);
            }
            if (_spriteRenderer.color.a <= 0)
            {
                _pulseDown = false;
            }
            else if (_spriteRenderer.color.a >= 1)
            {
                _pulseDown = true;
            }
        }
        else
        {
            _pulseDown = false;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1);
        }
    }

    #endregion

}
