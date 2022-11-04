using System.Collections.Generic;
using Rewired;
using UnityEngine;

namespace PlayerControl
{
    public class PlayerController : MonoBehaviour
    {
        public delegate void Axis2dInputHandler(float x, float y);
        public delegate void AxisInputHandler(float value);
        public delegate void ButtonDownInputHandler();
        public delegate void ButtonUpInputHandler();
        public delegate void ButtonInputHandler();

        private IPlayerControllable _target;
        private Player _player;

        private List<AxisInputBinding> _axisInputBindings;
        private List<Axis2dInputBinding> _axis2dInputBindings;
        private List<ButtonDownInputBinding> _buttonDownInputBindings;
        private List<ButtonDownInputBinding> _buttonRepeatInputBindings;
        private List<ButtonUpInputBinding> _buttonUpInputBindings;
        private List<ButtonInputBinding> _buttonInputBindings;

        private static PlayerController _instance;

        public static PlayerController Instance
        {
            get
            {
                return _instance;
            }
        }
        
        public IPlayerControllable Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (_target != null)
                {
                    DeregisterTarget();
                }
                
                _target = value;
                if (_target != null)
                {
                    RegisterTarget();
                }
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError($"Cannot have more than one instance of {nameof(PlayerController)}; Destroying this one");
                Destroy(gameObject);
                return;
            }

            _instance = this;
            
            _axisInputBindings = new List<AxisInputBinding>();
            _axis2dInputBindings = new List<Axis2dInputBinding>();
            _buttonDownInputBindings = new List<ButtonDownInputBinding>();
            _buttonUpInputBindings = new List<ButtonUpInputBinding>();
            _buttonInputBindings = new List<ButtonInputBinding>();
        }

        private void Start()
        {
            _player = ReInput.players.GetPlayer(0);
        }

        public void Update()
        {
            ProcessInputs();
        }

        private void ProcessInputs()
        {
            if (Target == null)
            {
                return;
            }
            
            ProcessAxisInputs();
            ProcessAxis2dInputs();
            ProcessButtonInputs();
            ProcessButtonDownInputs();
            ProcessButtonRepeatInputs();
            ProcessButtonUpInputs();
        }

        private void ProcessButtonUpInputs()
        {
            foreach (var buttonUpInputBinding in _buttonUpInputBindings)
            {
                if (_player.GetButtonUp(buttonUpInputBinding.ButtonName))
                {
                    buttonUpInputBinding.Handler?.Invoke();
                }
            }
        }

        private void ProcessButtonDownInputs()
        {
            foreach (var buttonDownInputBinding in _buttonDownInputBindings)
            {
                if (_player.GetButtonDown(buttonDownInputBinding.ButtonName))
                {
                    buttonDownInputBinding.Handler?.Invoke();
                }
            }
        }
        
        private void ProcessButtonRepeatInputs()
        {
            foreach (var buttonRepeatInputBinding in _buttonRepeatInputBindings)
            {
                if (_player.GetButtonRepeating(buttonRepeatInputBinding.ButtonName))
                {
                    buttonRepeatInputBinding.Handler?.Invoke();
                }
            }
        }

        private void ProcessButtonInputs()
        {
            foreach (var buttonInputBinding in _buttonInputBindings)
            {
                if (_player.GetButton(buttonInputBinding.ButtonName))
                {
                    buttonInputBinding.Handler?.Invoke();
                }
            }
        }

        private void ProcessAxisInputs()
        {
            foreach (var axisInputBinding in _axisInputBindings)
            {
                var value = _player.GetAxis(axisInputBinding.AxisName);
                if (Mathf.Abs(value) > float.Epsilon)
                {
                    axisInputBinding.Handler?.Invoke(value);
                }
            }
        }
        
        private void ProcessAxis2dInputs()
        {
            foreach (var axis2dInputBinding in _axis2dInputBindings)
            {
                var axis2d = _player.GetAxis2D(axis2dInputBinding.AxisXName, axis2dInputBinding.AxisYName);
                if (axis2d.sqrMagnitude > 0.0f)
                {
                    axis2dInputBinding.Handler?.Invoke(axis2d.x, axis2d.y);
                }
            }
        }

        public void AddAxisInputHandler(string axis, AxisInputHandler handler)
        {
            _axisInputBindings.Add(new AxisInputBinding()
            {
                AxisName = axis,
                Handler = handler
            });
        }
        
        public void AddAxis2dInputHandler(string axisX, string axisY, Axis2dInputHandler handler)
        {
            _axis2dInputBindings.Add(new Axis2dInputBinding()
            {
                AxisXName = axisX,
                AxisYName = axisY,
                Handler = handler
            });
        }

        public void AddButtonDownInputHandler(string buttonName, ButtonDownInputHandler buttonDownInputHandler)
        {
            _buttonDownInputBindings.Add(new ButtonDownInputBinding()
            {
                ButtonName = buttonName,
                Handler = buttonDownInputHandler 
            });
        }
        
        public void AddButtonRepeatInputHandler(string buttonName, ButtonDownInputHandler buttonRepeatInputHandler)
        {
            _buttonRepeatInputBindings.Add(new ButtonDownInputBinding()
            {
                ButtonName = buttonName,
                Handler = buttonRepeatInputHandler 
            });
        }
        
        public void AddButtonUpInputHandler(string buttonName, ButtonUpInputHandler buttonUpInputHandler)
        {
            _buttonUpInputBindings.Add(new ButtonUpInputBinding()
            {
                ButtonName = buttonName,
                Handler = buttonUpInputHandler 
            });
        }
        
        public void AddButtonInputHandler(string buttonName, ButtonInputHandler buttonInputHandler)
        {
            _buttonInputBindings.Add(new ButtonInputBinding()
            {
                ButtonName = buttonName,
                Handler = buttonInputHandler 
            });
        }
        
        private void RegisterTarget()
        {
            _target.OnPlayerControllerPossess(this);
        }
        
        private void DeregisterTarget()
        {
            if (_target != null)
            {
                _target.OnPlayerControllerRelease(this);
            }
            
            _axisInputBindings.Clear();
            _axis2dInputBindings.Clear();
            _buttonUpInputBindings.Clear();
            _buttonDownInputBindings.Clear();
            _buttonRepeatInputBindings.Clear();
            _buttonInputBindings.Clear();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
