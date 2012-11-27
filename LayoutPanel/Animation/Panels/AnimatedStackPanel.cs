using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Bornander.UI.Animations.Animators;

namespace Bornander.UI.Animations.Panels
{
    public class AnimatedStackPanel : StackPanel
    {
        private DispatcherTimer animationTimer;
        private DateTime lastArrange = DateTime.MinValue;
        private bool m_RunAnimation;

        public IArrangeAnimator Animator { get; set; }

        public AnimatedStackPanel()
            : this(new FractionDistanceAnimator(0.25), TimeSpan.FromSeconds(0.05))
        {
            Loaded += PanelLoaded;
        }

        private void PanelLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
                                                  {
                                                      m_RunAnimation = true;
                                                  }), DispatcherPriority.Background);
        }

        public AnimatedStackPanel(IArrangeAnimator animator, TimeSpan animationInterval)
        {
            animationTimer = AnimationBase.CreateAnimationTimer(this, animationInterval);
            Animator = animator;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            finalSize = base.ArrangeOverride(finalSize);

            if (m_RunAnimation && !IsArrangeValid)
            {
                AnimationBase.Arrange(Math.Max(0, (DateTime.Now - lastArrange).TotalSeconds), this, Children, Animator);
            }
            lastArrange = DateTime.Now;

            return finalSize;
        }
    }
}
