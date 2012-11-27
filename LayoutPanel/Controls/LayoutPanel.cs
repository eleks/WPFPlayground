using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup.Primitives;

namespace Controls
{
    public class LayoutPanel : Panel
    {
        private Panel m_Panel;
        private static IList<DependencyPropertyDescriptor> ms_AttachedProperties; 

        public LayoutPanel()
        {
            m_Panel = LoadPanelTemplate(Panel);
            InitializeChildren();
        }

        public static readonly DependencyProperty PanelProperty =
            DependencyProperty.Register("Panel", typeof(ItemsPanelTemplate), typeof(LayoutPanel),
            new FrameworkPropertyMetadata(new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel))),
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsArrange,
                PanelChanged));

        private static void PanelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            LayoutPanel layoutPanel = sender as LayoutPanel;

            if (layoutPanel != null)
            {
                var newPanel = LoadPanelTemplate((ItemsPanelTemplate)args.NewValue);
                layoutPanel.m_Panel = newPanel;
                layoutPanel.InitializeChildren();
            }
        }

        public ItemsPanelTemplate Panel
        {
            get
            {
                return (ItemsPanelTemplate)GetValue(PanelProperty);
            }
            set
            {
                SetValue(PanelProperty, value);
            }
        }

        private static readonly DependencyPropertyKey ProxyUIElementPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("ProxyUIElement", typeof(UIElement), typeof(LayoutPanel),
                                                        new PropertyMetadata(null));

        private static readonly DependencyProperty ProxyUIElementProperty = ProxyUIElementPropertyKey.DependencyProperty;

        private static UIElement GetProxyUIElement(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(ProxyUIElementProperty);
        }

        private static void SetProxyUIElement(DependencyObject obj, UIElement element)
        {
            obj.SetValue(ProxyUIElementPropertyKey, element);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            PanelInternal.Measure(availableSize);
            return PanelInternal.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            PanelInternal.Arrange(new Rect(finalSize));
            foreach (ProxyUIElement child in PanelInternal.Children)
            {
                Point point = child.TranslatePoint(new Point(), PanelInternal);
                child.Element.Arrange(new Rect(point, child.RenderSize));
            }
            return PanelInternal.RenderSize;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if (visualAdded != null)
            {
                var props = GetAllAttachedProperties();
                foreach (var d in props)
                {
                    d.AddValueChanged(visualAdded, AttachedPropertyValueChanged);
                }

                var child = (UIElement) visualAdded;
                int index = Children.IndexOf(child);
                var proxyElement = CreateProxyUIElement(child);
                PanelInternal.Children.Insert(index, proxyElement);
            }
            if (visualRemoved != null)
            {
                var props = GetAllAttachedProperties();
                foreach (var d in props)
                {
                    d.RemoveValueChanged(visualRemoved, AttachedPropertyValueChanged);
                }
                var proxyElement = GetProxyUIElement(visualRemoved); 
                PanelInternal.Children.Remove(proxyElement);
                SetProxyUIElement(visualRemoved, null);
            }
        }

        private Panel PanelInternal
        {
            get { return m_Panel; }
        }

        private void InitializeChildren()
        {
            foreach (UIElement child in Children)
            {
                PanelInternal.Children.Add(CreateProxyUIElement(child));
            }
        }

        private ProxyUIElement CreateProxyUIElement(UIElement child)
        {
            var proxyElement = new ProxyUIElement(child);
            SetProxyUIElement(child, proxyElement);
            var props = GetAllAttachedProperties();
            foreach (var d in props)
            {
                d.SetValue(proxyElement, d.GetValue(child));
            }
            return proxyElement;
        }

        private static Panel LoadPanelTemplate(ItemsPanelTemplate panelTemplate)
        {
            var panel = (Panel)panelTemplate.LoadContent();
            panel.IsItemsHost = false; //set by ItemPanelTemplate, we need to reset it
            return panel;
        }

        private void AttachedPropertyValueChanged(object sender, EventArgs args)
        {
            var element = (UIElement) sender;
            var proxyElement = GetProxyUIElement(element);
            if (proxyElement != null)
            {
                var props = GetAllAttachedProperties();
                foreach (var d in props) //
                {
                    d.SetValue(proxyElement, d.GetValue(element));
                }
            }
        }

        private static List<DependencyProperty> GetAttachedProperties(Object element)
        {
            MarkupObject markupObject = MarkupWriter.GetMarkupObjectFor(element);
            return (from mp in markupObject.Properties
                    where mp.IsAttached
                    select mp.DependencyProperty).ToList();
        }
 
        private static IEnumerable<DependencyPropertyDescriptor> GetAllAttachedProperties()
        {
            if (ms_AttachedProperties != null)
            {
                return ms_AttachedProperties;
            }

            ms_AttachedProperties = new List<DependencyPropertyDescriptor>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assembly.GetTypes())
                {
                    foreach (FieldInfo fi in t.GetFields(BindingFlags.DeclaredOnly
                        | BindingFlags.Static | BindingFlags.Public))
                    {
                        if (fi.FieldType == typeof(DependencyProperty))
                        {
                            DependencyProperty dp = (DependencyProperty)fi.GetValue(null);
                            try
                            {
                                DependencyPropertyDescriptor dpd =
                                    DependencyPropertyDescriptor.FromProperty(dp, t);

                                if (dpd != null && dpd.IsAttached && !dpd.IsReadOnly)
                                {
                                    ms_AttachedProperties.Add(dpd);
                                }
                            }
                            catch (ArgumentException)
                            {
                                // there was a problem obtaining the
                                // DependencyPropertyDescriptor?
                            }
                        }
                    }
                }
            }

            return ms_AttachedProperties;
        }

        private class ProxyUIElement : FrameworkElement
        {
            private readonly UIElement m_Element;

            public ProxyUIElement(UIElement element)
            {
                m_Element = element;
            }

            public UIElement Element
            {
                get { return m_Element; }
            }

            protected override Size MeasureOverride(Size constraint)
            {
                Element.Measure(constraint);
                return Element.DesiredSize;
            }
        }
    }
}
