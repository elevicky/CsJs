﻿using System;
using System.Collections.Generic;
using System.Html;
using MorseCode.CsJs.Common.Observable;

namespace MorseCode.CsJs.UI.Controls
{
    public abstract class PlaceHolderCompositeControlBase : CompositeControlBase
    {
        private Element _tempElement;

        protected override sealed void CreateElements()
        {
            _tempElement = Document.CreateElement("div");
        }

        protected override sealed Element GetChildElementContainer()
        {
            return Parent == null ? _tempElement : Parent.GetChildElementContainerInternal();
        }

        protected override sealed IEnumerable<Element> GetRootElements()
        {
            return null;
        }

        public override CompositeControlBase Parent
        {
            get { return base.Parent; }
            set
            {
                EnsureChildControlsCreated();

                Element oldContainer = GetChildElementContainerInternal();

                base.Parent = value;

                Element container = GetChildElementContainerInternal();

                if (ReferenceEquals(oldContainer, container))
                {
                    return;
                }

                foreach (ControlBase control in Controls)
                {
                    IEnumerable<Element> children = control.GetRootElementsInternal();
                    foreach (Element child in children)
                    {
                        container.AppendChild(child);
                        if (oldContainer.Contains(child))
                        {
                            oldContainer.RemoveChild(child);
                        }
                    }
                }
            }
        }
    }

    public abstract class PlaceHolderCompositeControlBase<T> : PlaceHolderCompositeControlBase
    {
        public void BindDataContext<TDataContext>(IReadableObservableProperty<TDataContext> dataContext, Func<TDataContext, T> getDataContext)
        {
            EnsureChildControlsCreated();

            BindControls(new ReadOnlyProperty<T>(getDataContext(dataContext.Value)));
        }

        public void BindDataContext<TDataContext>(IReadableObservableProperty<TDataContext> dataContext, Func<TDataContext, IReadableObservableProperty<T>> getDataContext)
        {
            EnsureChildControlsCreated();

            ObservableProperty<T> thisDataContext = new ObservableProperty<T>(getDataContext(dataContext.Value).Value);

            EventHandler updateControlEventHandler = null;
            CreateOneWayBinding(
                dataContext,
                d =>
                {
                    Action updateControl = () => thisDataContext.Value = getDataContext(d).Value;
                    updateControlEventHandler = (sender, args) => updateControl();
                    getDataContext(d).Changed += updateControlEventHandler;
                    updateControl();
                },
                d => getDataContext(d).Changed -= updateControlEventHandler);

            BindControls(thisDataContext);
        }

        protected abstract void BindControls(IReadableObservableProperty<T> dataContext);
    }
}