using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Bornander.UI.Animations.Animators;

namespace Bornander.UI.Animations.Panels
{
    public class AnimatedGrid : Grid
    {
        private DispatcherTimer animationTimer;
        private DateTime lastArrange = DateTime.MinValue;

        public IArrangeAnimator Animator { get; set; }

        public AnimatedGrid()
            : this(new FractionDistanceAnimator(0.25), TimeSpan.FromSeconds(0.05))
        {
        }

        public AnimatedGrid(IArrangeAnimator animator, TimeSpan animationInterval)
        {
            animationTimer = AnimationBase.CreateAnimationTimer(this, animationInterval);
            Animator = animator;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Debug.WriteLine(IsLoaded);
            finalSize = base.ArrangeOverride(finalSize);
            
            AnimationBase.Arrange(Math.Max(0, (DateTime.Now - lastArrange).TotalSeconds), this, Children, Animator);
            lastArrange = DateTime.Now;

            return finalSize;
        }
    }
}
