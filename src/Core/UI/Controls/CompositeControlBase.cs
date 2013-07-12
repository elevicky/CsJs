﻿using System.Collections.Generic;
using System.Html;
using System.Linq;

namespace MorseCode.CsJs.UI.Controls
{
    public abstract class CompositeControlBase : ControlBase, ICompositeControl
    {
        private readonly ControlCollection _controls;

        private bool _childControlsCreated;

        protected CompositeControlBase()
        {
            _controls = new ControlCollection(this);
            _controls.ControlAdded += (sender, args) => ChangeControl(args.Control, true);
            _controls.ControlRemoved += (sender, args) => ChangeControl(args.Control, false);
            _controls.ControlsReset += (sender, args) =>
                {
                    args.OldControls.ForEach(c => ChangeControl(c, false));
                    args.NewControls.ForEach(c => ChangeControl(c, true));
                };
        }

        private void ChangeControl(ControlBase control, bool add)
        {
            Element container = GetChildElementContainerInternal();
            IEnumerable<Element> rootElements = control.GetRootElementsInternal();
            if (rootElements != null)
            {
                foreach (Element element in rootElements)
                {
                    if (add)
                    {
                        container.AppendChild(element);
                    }
                    else
                    {
                        container.RemoveChild(element);
                    }
                }
            }
        }

        protected IEnumerable<IControl> Controls
        {
            get { return _controls; }
        }

        public override IEnumerable<Element> GetRootElementsInternal()
        {
            EnsureChildControlsCreated();

            return base.GetRootElementsInternal();
        }

        protected void EnsureChildControlsCreated()
        {
            EnsureElementsCreated();

            if (!_childControlsCreated)
            {
                CreateChildControls(_controls);
                _childControlsCreated = true;
            }
        }

        protected abstract void CreateChildControls(ControlCollection controls);

        internal Element GetChildElementContainerInternal()
        {
            EnsureElementsCreated();
            return GetChildElementContainer();
        }

        protected abstract Element GetChildElementContainer();

        protected override void OnDispose()
        {
            base.OnDispose();

            List<ControlBase> controls = _controls.ToList();
            _controls.Clear();
            foreach (ControlBase control in controls)
            {
                control.Dispose();
            }
        }
    }
}