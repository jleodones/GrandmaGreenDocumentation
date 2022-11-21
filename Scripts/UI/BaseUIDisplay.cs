using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI
{
    [System.Flags]
    public enum UITag
    {
        None = 0,
        HUD = 1,
        INVENTORY = 2,
        TOOLS = 4,
        SHOPPING = 8,

        ALL = UITag.HUD | UITag.INVENTORY | UITag.TOOLS | UITag.SHOPPING,
    }


    [RequireComponent(typeof(UIDocument))]
    public class BaseUIDisplay : MonoBehaviour
    {

        [field: Header("Display References")]
        [field: SerializeField] public virtual UIDocument UIDocument { get; protected set; }

        [field: Header("Display Settings")]
        [field: SerializeField] public virtual UITag PanelTags { get; protected set; }

        [field: Header("Display Variables")]
        [field: SerializeField] public virtual BaseUIController Controller { get; protected set; }
        [field: SerializeField] public virtual bool DisplayOpen { get; protected set; } = false;
        [field: SerializeField] public virtual bool DisplayEnabled { get; protected set; } = true;
        [field: SerializeField] public virtual bool DisplayInteractive { get; protected set; } = true;

        public event System.Action onPanelOpened;
        public event System.Action onPanelClosed;

        public event System.Action onPanelEnabled;
        public event System.Action onPanelDisabled;

        VisualElement m_root;

        #region Unity Methods

        void Start()
        {
            InitalizePanel();
        }

        protected virtual void OnValidate()
        {
            UIDocument ??= GetComponent<UIDocument>();
        }

        void OnDestroy()
        {

        }

        #endregion


        #region Public Methods
        public virtual void InitalizePanel()
        {

            m_root = UIDocument.rootVisualElement;

            //TODO: Initalize controller here
        }

        [ContextMenu ("Open")]
        public void OpenPanel()
        {
            if (!DisplayEnabled) return;

            DisplayOpen = true;

            PanelOpenLogic();

            UIDisplayTracker.AddPanel(this);

            onPanelOpened?.Invoke();
        }

        [ContextMenu ("Close")]
        public void ClosePanel()
        {
            if (!DisplayOpen) return;

            DisplayOpen = false;

            PanelCloseLogic();

            UIDisplayTracker.RemovePanel(this);

            onPanelClosed?.Invoke();
        }

        public void EnablePanel()
        {
            if (DisplayEnabled) return;

            DisplayEnabled = true;

            onPanelEnabled?.Invoke();
        }

        public void DisablePanel()
        {
            if (!DisplayEnabled) return;

            DisplayEnabled = false;

            onPanelDisabled?.Invoke();
        }

        public void EnableInteraction()
        {
            if (DisplayInteractive) return;

            DisplayInteractive = true;

            InteractionEnabledLogic();
        }

        public void DisableInteraction()
        {
            if (!DisplayInteractive) return;

            DisplayInteractive = false;

            InteractionDisabledLogic();
        }

        #endregion

        #region Protected Methods


        protected virtual void PanelOpenLogic()
        {
            //TODO: open panel
        }

        protected virtual void PanelCloseLogic()
        {
            //TODO: close panel
        }

        protected virtual void InteractionEnabledLogic()
        {
            //TODO: enable controller interaction
        }

        protected virtual void InteractionDisabledLogic()
        {
            //TODO: disable controller interaction
        }

        #endregion


    }
}
