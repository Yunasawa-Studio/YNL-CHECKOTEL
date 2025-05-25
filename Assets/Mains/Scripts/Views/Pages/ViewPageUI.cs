using UnityEngine;
using UnityEngine.UIElements;
using YNL.Utilities.Extensions;
using YNL.Utilities.UIToolkits;

namespace YNL.Checkotel
{
    public abstract class ViewPageUI : MonoBehaviour
    {
        [SerializeField] private bool _hideOnAwake = true;
        [SerializeField] protected bool _isPopupPage = false;
        public VisualElement Root;

        private bool _isDisplayThisTime = false;

        private void Awake()
        {
            Root = GetComponent<UIDocument>().rootVisualElement;
            Root.SetTransitionProperty("translate");
            Root.RegisterCallback<TransitionEndEvent>(OnTransitionEnded);

            VirtualAwake();

            Collect();

            if (!_isPopupPage)
            {
                Root.SetTranslate(_hideOnAwake ? 100 : 0, 0, true);
                Root.SetTransitionDuration(0.2f);
            }
            else
            {
                OnPageOpened(false);
            }

            _isDisplayThisTime = _hideOnAwake;

            Marker.OnDatabaseSerializationDone += OnDatabaseSerializationDone;
        }

        private void OnDestroy()
        {
            Marker.OnDatabaseSerializationDone -= OnDatabaseSerializationDone;
        }

        protected virtual void VirtualAwake() { }

        protected virtual void Collect() { }
        protected virtual void Initialize() { }
        protected virtual void Refresh() { }

        public void DisplayView(bool display, bool needRefresh = true)
        {
            Root.SetDisplay(DisplayStyle.Flex);
            Root.SetTranslate(display ? 0 : -100, 0, true);

            _isDisplayThisTime = display;

            if (Main.View != null) Main.View.IsAbleToMovePage = false;

            if (display && needRefresh)
            {
                Refresh();
            }
        }

        public virtual void OnPageOpened(bool isOpen, bool needRefresh = true)
        {
            Root.SetTranslate(0, isOpen ? 0 : 100, true);

            if (isOpen && needRefresh)
            {
                Refresh();
            }
        }

        private void OnDatabaseSerializationDone()
        {
            Initialize();
            Refresh();
        }

        private void OnTransitionEnded(TransitionEndEvent evt)
        {
            if (_isDisplayThisTime) return;

            Root.SetDisplay(DisplayStyle.None);
            Root.SetTranslate(100, 0, true);

            _isDisplayThisTime = true;

            if (Main.View != null)
            {
                Main.View.IsAbleToMovePage = true;
            }
        }
    }
}