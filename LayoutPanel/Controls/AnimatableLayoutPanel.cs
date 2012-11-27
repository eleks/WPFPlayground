using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Bornander.UI.Animations;
using Bornander.UI.Animations.Animators;

namespace Controls
{
    public class AnimatedLayoutPanel : LayoutPanel
    {
        private DispatcherTimer animationTimer;
        private DateTime lastArrange = DateTime.MinValue;

        public IArrangeAnimator Animator { get; set; }

        public static readonly DependencyProperty IsAnimatedProperty = DependencyProperty.Register("IsAnimated", 
            typeof (bool), typeof (AnimatedLayoutPanel), new PropertyMetadata(true));

        public AnimatedLayoutPanel()
            : this(new FractionDistanceAnimator(0.25), TimeSpan.FromSeconds(0.05))
        {
        }

        public AnimatedLayoutPanel(IArrangeAnimator animator, TimeSpan animationInterval)
        {
            animationTimer = AnimationBase.CreateAnimationTimer(this, animationInterval);
            Animator = animator;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            finalSize = base.ArrangeOverride(finalSize);

            if (IsAnimated)
            {
                AnimationBase.Arrange(Math.Max(0, (DateTime.Now - lastArrange).TotalSeconds), this, Children, Animator);
            }
            lastArrange = DateTime.Now;

            return finalSize;
        }

        public bool IsAnimated
        {
            get { return (bool) GetValue(IsAnimatedProperty); }
            set { SetValue(IsAnimatedProperty, value); }
        }
    }
}
