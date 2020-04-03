using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Yamed.Registry.DragAndDrop
{
    public class DraggedAdorner : Adorner
    {
        private FrameworkElement visual;
        private double left;
        private double top;
        private AdornerLayer adornerLayer;

        public DraggedAdorner(FrameworkElement visual, UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement)
        {
            this.adornerLayer = adornerLayer;
            this.visual = visual;

            this.adornerLayer.Add(this);
        }

        public void SetPosition(double left, double top)
        {
            // -1 and +13 align the dragged adorner with the dashed rectangle that shows up
            // near the mouse cursor when dragging.
            this.left = left;
            this.top = top;
            if (this.adornerLayer != null)
            {
                this.adornerLayer.Update(this.AdornedElement);
            }
        }

        //protected override Size MeasureOverride(Size constraint)
        //{
        //    this.contentPresenter.Measure(constraint);
        //    return this.contentPresenter.DesiredSize;
        //}

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.visual.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visual;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.left, this.top));

            return result;
        }

        public void Detach()
        {
            this.adornerLayer.Remove(this);
        }
        }
}
