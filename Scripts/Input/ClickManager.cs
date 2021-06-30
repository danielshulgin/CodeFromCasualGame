using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    public UnityEvent OnCarouselTouch;
    
    public UnityEvent OnCarouselEndTouch;
    
    public UnityEvent OnVasylTouch;
    
    public UnityEvent OnStartGameButtonTouch;
    
    public UnityEvent OnElkTouch;
    
    public UnityEvent OnDuckTouch;
    
    public MyTouchEvent OnSwipeRight;
    
    public MyTouchEvent OnSwipeLeft;
    
    public MyTouchEvent OnSwipe;
    
    public MyTouchEvent OnSwipeBegin;
    
    public MyTouchEvent OnSwipeEnd;
    
    public UnityEvent OnExitButtonClick;
    
    public Action<Vector2> OnStartClik;
    
    public UnityEvent OnEndClick;
    
    [SerializeField] private MainMenuUIState mainMenuUIState;
    
    [SerializeField] private float swipeThreshold = 20f;
    
    private Vector2 _fingerNow;
    
    private bool _swiping;
    
    private bool _menuMode;

    private string _touchStartTartgetTag = "";
    
    private Vector2 _fingerDown;

    public Vector2 FingerDown => _fingerDown;
    
    private float VerticalDelta => Mathf.Abs(_fingerNow.y - _fingerDown.y);
    
    private float HorizontalValDelta => Mathf.Abs(_fingerNow.x - _fingerDown.x);

    public bool Press { get; private set; }

    public Vector2 LastPosition { get; private set; }

    private Dictionary<InputTargetType, bool> _handleInputType;

    private void Awake()
    {
        LastPosition = Vector2.zero;
        _handleInputType = new Dictionary<InputTargetType, bool>
        {
            [InputTargetType.OnCarouselTouch] = true,
            [InputTargetType.OnVasylTouch] = true,
            [InputTargetType.OnCarouselEndTouch] = true,
            [InputTargetType.OnBulletTouch] = true,
            [InputTargetType.OnElkTouch] = true,
            [InputTargetType.OnDuckTouch] = true,
            [InputTargetType.OnSwipeRight] = true,
            [InputTargetType.OnSwipeLeft] = true,
            [InputTargetType.OnSwipe] = true,
            [InputTargetType.OnSwipeBegin] = true,
            [InputTargetType.OnSwipeEnd] = true,
            [InputTargetType.OnExitButtonClick] = true,
            [InputTargetType.OnEndClick] = true,
        };

        mainMenuUIState.OnEnter += HandleEnterMainMenu;
        mainMenuUIState.OnExit += HandleExitMainMenu;
    }

    void Update()
    {
#if UNITY_EDITOR
        CheckMouseTouch();
#else
        CheckFingerTouch();
#endif
        
        if (Input.GetKeyDown(KeyCode.Escape) && _handleInputType[InputTargetType.OnExitButtonClick])
        {
            OnExitButtonClick.Invoke();
        }
    }

    public void SetActiveMenuMode(bool active)
    {
        _menuMode = active;
    }
    
    private void CheckFingerTouch()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (IsPointerOverGameObject())
                    {
                        return;
                    }
                    Press = true;
                    CheckTouchDownTarget(touch.position);
                    _fingerDown = touch.position;
                    break;
                case TouchPhase.Moved:
                    break;
                case TouchPhase.Ended:
                    _fingerNow = touch.position;
                    CheckSwipe(touch);
                    Press = false;
                    CheckTouchUpTarget(touch.position);
                    if (_handleInputType[InputTargetType.OnEndClick])
                    {
                        Debug.Log("EndClick");
                        OnEndClick.Invoke();
                    }
                    if (_handleInputType[InputTargetType.OnCarouselEndTouch] && _touchStartTartgetTag == "Carousel")
                        OnCarouselEndTouch.Invoke();
                    break;
                case TouchPhase.Canceled:
                    _fingerNow = touch.position;
                    CheckSwipe(touch);
                    Press = false;
                    CheckTouchUpTarget(touch.position);
                    if (_handleInputType[InputTargetType.OnCarouselEndTouch] && _touchStartTartgetTag == "Carousel")
                        OnCarouselEndTouch.Invoke();
                    break;
                case TouchPhase.Stationary:
                    break;
            }

            if (Press)
            {
                CheckSwipe(touch);
            }
        }
    }

#if UNITY_EDITOR
    private void CheckMouseTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverGameObject())
            {
                return;
            }
            
            Press = true;
            _fingerNow = _fingerDown = Input.mousePosition;
            var touch = new Touch();
            LastPosition = Input.mousePosition;
            touch.deltaPosition = (Vector2) Input.mousePosition - LastPosition;
            touch.position = _fingerNow;
            touch.phase = TouchPhase.Began;
            
            CheckTouchDownTarget(Input.mousePosition);
            CheckSwipe(touch);
            LastPosition = Input.mousePosition;
        }
        else if (Press && (Mathf.Abs(Input.GetAxisRaw("Mouse X")) > float.Epsilon
                           || Mathf.Abs(Input.GetAxisRaw("Mouse Y")) > float.Epsilon))
        {
            _fingerNow = Input.mousePosition;
            var touch = new Touch();
            touch.deltaPosition = (Vector2) Input.mousePosition - LastPosition;
            touch.position = _fingerNow;
            touch.phase = TouchPhase.Moved;
            
            CheckSwipe(touch);
            LastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_handleInputType[InputTargetType.OnEndClick])
                OnEndClick.Invoke();
            _fingerNow = Input.mousePosition;
            if (_handleInputType[InputTargetType.OnCarouselEndTouch] && _touchStartTartgetTag == "Carousel")
                OnCarouselEndTouch.Invoke();
            Press = false;
            var touch = new Touch();
            touch.deltaPosition = (Vector2) Input.mousePosition - LastPosition;
            touch.position = _fingerNow;
            touch.phase = TouchPhase.Ended;
            
            CheckTouchUpTarget(Input.mousePosition);
            CheckSwipe(touch);
        }
    }
#endif

    private void CheckTouchDownTarget(Vector2 screenPosition)
    {
        OnStartClik?.Invoke(screenPosition);
        _touchStartTartgetTag = "";
        var hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(screenPosition), Vector2.zero);
        
        if (hit.collider != null)
        {
            _touchStartTartgetTag = hit.collider.tag;
            if (_menuMode)
                return;
            
            if ( hit.collider.name == "Carousel" && _handleInputType[InputTargetType.OnCarouselTouch])
            {
                OnCarouselTouch.Invoke();
            }
        }
        else
        {
            _fingerDown = screenPosition;
            _fingerNow = screenPosition;
        }
    }
    
    private void CheckTouchUpTarget(Vector2 screenPosition)
    {
        var hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(screenPosition), Vector2.zero);
        
        if (hit.collider == null || hit.collider.tag != _touchStartTartgetTag) 
            return;

        if (_menuMode)
        {
            if (hit.collider.gameObject.CompareTag("StartGameButton") && _handleInputType[InputTargetType.OnBulletTouch])
            {
                OnStartGameButtonTouch.Invoke();
            }
            else if (hit.collider.gameObject.CompareTag("Elk") && _handleInputType[InputTargetType.OnElkTouch])
            {
                OnElkTouch.Invoke();
            }
            else if (hit.collider.gameObject.CompareTag("Duck") && _handleInputType[InputTargetType.OnDuckTouch])
            {
                OnDuckTouch.Invoke();
            }
        }
        else
        {
            if (hit.collider.gameObject.CompareTag("Elk") && _handleInputType[InputTargetType.OnVasylTouch])
            {
                OnVasylTouch.Invoke();
            }
        }
    }

    private void CheckSwipe(Touch touch)
    {
        _fingerNow = touch.position;
        if (!_menuMode || (HorizontalValDelta > swipeThreshold) && HorizontalValDelta > VerticalDelta)
        {
            if (!_swiping)
            {
                if (_handleInputType[InputTargetType.OnSwipeBegin])
                    OnSwipeBegin.Invoke(touch);
                if (_handleInputType[InputTargetType.OnSwipe])
                    OnSwipe.Invoke(touch);
                //if(touch.phase == TouchPhase.Began)
                    _swiping = true;
            }
            else
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary )
                {
                    if (_handleInputType[InputTargetType.OnSwipe])
                        OnSwipe.Invoke(touch);
                    if (touch.deltaPosition.x > 0 && _handleInputType[InputTargetType.OnSwipeRight])
                    {
                        OnSwipeRight.Invoke(touch);
                    }
                    else if (touch.deltaPosition.x < 0 && _handleInputType[InputTargetType.OnSwipeLeft])
                    {
                        OnSwipeLeft.Invoke(touch);
                    }
                }
                else
                {
                    if (_handleInputType[InputTargetType.OnSwipeEnd])
                    {
                        OnSwipeEnd.Invoke(touch);
                    }
                    _swiping = false;
                }
            }
        }
    }

    public void SetHandleInputTypes(Dictionary<InputTargetType, bool> handleInputTypes)
    {
        foreach (var type in handleInputTypes.Keys)
        {
            _handleInputType[type] = handleInputTypes[type];
        }
    }
    
    public void SetHandleInputTypes(InputTargetType inputType, bool handle)
    {
        _handleInputType[inputType] = handle;
    }

    public static bool IsPointerOverGameObject()
    {
        // Check mouse
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
 
        // Check touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                return true;
            }
        }
             
        return false;
    }

    private void HandleEnterMainMenu()
    {
        _menuMode = true;
    }
    
    private void HandleExitMainMenu()
    {
        _menuMode = false;
    }
}

public enum InputTargetType
{
    OnCarouselTouch,
    OnVasylTouch,
    OnCarouselEndTouch,
    OnBulletTouch,
    OnElkTouch,
    OnDuckTouch,
    OnSwipeRight,
    OnSwipeLeft,
    OnSwipe,
    OnSwipeBegin,
    OnSwipeEnd,
    OnExitButtonClick,
    OnEndClick
}