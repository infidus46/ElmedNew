/*
 *This work is being distributed under the The Code Project Open License (CPOL) (http://www.codeproject.com/info/cpol10.aspx) along with all restrictions placed therein.
 *Attribution:
 *1. Source Article: http://www.codeproject.com/Articles/618014/Automatic-Scrolling-During-Drag-operation
 *   Author: Amit Mittal (http://www.codeproject.com/Members/Amit_Mittal)
 */

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Yamed.Registry.DragAndDrop
{
    public class AutoDragScrollingProvider
    {
        public static readonly DependencyProperty EnableAutomaticScrollingProperty =
            DependencyProperty.RegisterAttached("EnableAutomaticScrolling", typeof(bool), typeof(AutoDragScrollingProvider), new PropertyMetadata(default(bool), OnEnableAutomaticScrollingChange));

        public static bool GetEnableAutomaticScrolling(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableAutomaticScrollingProperty);
        }

        public static void SetEnableAutomaticScrolling(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableAutomaticScrollingProperty, value);
        }

        public static readonly DependencyProperty LineSizeForVerticalChangeProperty =
            DependencyProperty.RegisterAttached("LineSizeForVerticalChange", typeof(double), typeof(AutoDragScrollingProvider), new PropertyMetadata(double.NaN));

        public static double GetLineSizeForVerticalChange(DependencyObject obj)
        {
            return (double)obj.GetValue(LineSizeForVerticalChangeProperty);
        }

        public static void SetLineSizeForVerticalChange(DependencyObject obj, double value)
        {
            obj.SetValue(LineSizeForVerticalChangeProperty, value);
        }

        public static readonly DependencyProperty LineSizeForHorizontalChangeProperty =
            DependencyProperty.RegisterAttached("LineSizeForHorizontalChange", typeof(double), typeof(AutoDragScrollingProvider), new PropertyMetadata(double.NaN));

        public static double GetLineSizeForHorizontalChange(DependencyObject obj)
        {
            return (double)obj.GetValue(LineSizeForHorizontalChangeProperty);
        }

        public static void SetLineSizeForHorizontalChange(DependencyObject obj, double value)
        {
            obj.SetValue(LineSizeForHorizontalChangeProperty, value);
        }

        private static readonly DependencyProperty _autoDragScrollingProviderProperty =
            DependencyProperty.RegisterAttached("_AutoDragScrollingProvider", typeof(AutoDragScrollingProvider), typeof(AutoDragScrollingProvider), new PropertyMetadata(default(AutoDragScrollingProvider)));

        private static void OnEnableAutomaticScrollingChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = d as UIElement;
            if (uiElement != null)
            {
                if (true.Equals(e.NewValue))
                {
                    Debug.Assert(uiElement.GetValue(_autoDragScrollingProviderProperty) == null);
                    var scrollingManager = new AutoDragScrollingProvider();
                    uiElement.PreviewDragEnter += scrollingManager.ResetTotalDelta;
                    uiElement.PreviewDragLeave += scrollingManager.ResetTotalDelta;
                    uiElement.PreviewDragEnter += scrollingManager.HandlePreviewDragMove;
                    uiElement.PreviewDragOver += scrollingManager.HandlePreviewDragMove;
                    uiElement.SetValue(_autoDragScrollingProviderProperty, scrollingManager);
                }
                else
                {
                    var scrollingManager = uiElement.GetValue(_autoDragScrollingProviderProperty) as AutoDragScrollingProvider;
                    if (scrollingManager != null)
                    {
                        uiElement.PreviewDragEnter -= scrollingManager.ResetTotalDelta;
                        uiElement.PreviewDragLeave -= scrollingManager.ResetTotalDelta;
                        uiElement.PreviewDragEnter -= scrollingManager.HandlePreviewDragMove;
                        uiElement.PreviewDragOver -= scrollingManager.HandlePreviewDragMove;
                        uiElement.SetValue(_autoDragScrollingProviderProperty, null);
                    }
                }
            }
        }

        private double _totalVerticalDelta;
        private double _totalHorizontalDelta;
        private void ResetTotalDelta(object sender, DragEventArgs e)
        {
            _totalVerticalDelta = 0;
            _totalHorizontalDelta = 0;
        }

        //this caching is to avid some redundant steps but may prove troublesome 
        //if parent tree is dynamic which in most cases is not the case.
        private IScrollInfo _scrollInfo;
        private void HandlePreviewDragMove(object sender, DragEventArgs e)
        {
            if (_scrollInfo == null)
            {
                ScrollViewer parentScrollViewer = null;
                var parent = e.OriginalSource as DependencyObject;
                while (parent != null)
                {
                    parentScrollViewer = parent as ScrollViewer;
                    if (parentScrollViewer != null)
                    {
                        break;
                    }

                    parent = VisualTreeHelper.GetParent(parent);
                }

                if (parentScrollViewer != null)
                {
                    parent = e.OriginalSource as DependencyObject;
                    while (parent != null)
                    {
                        _scrollInfo = parent as IScrollInfo;
                        if (_scrollInfo != null && _scrollInfo.ScrollOwner == parentScrollViewer)
                        {
                            break;
                        }
                        _scrollInfo = null;
                        parent = VisualTreeHelper.GetParent(parent);
                    }
                }

                Panel scrollablePanel = _scrollInfo as Panel;
                if (scrollablePanel != null)
                {
                    Orientation? orientation = null;
                    //Special case handling as stack panel does not support 'smooth scrolling' or in other words fractional offset changes
                    if (scrollablePanel is StackPanel)
                    {
                        orientation = (Orientation)scrollablePanel.GetValue(StackPanel.OrientationProperty);
                    }
                    //VirtualizingStackPanel behaves like StackPanel only if it is not hosted in an ItemsControl
                    else if (scrollablePanel is VirtualizingStackPanel && !scrollablePanel.IsItemsHost)
                    {
                        orientation = (Orientation)scrollablePanel.GetValue(VirtualizingStackPanel.OrientationProperty);
                    }

                    if (orientation != null)
                    {
                        if (orientation == Orientation.Horizontal)
                        {
                            SetLineSizeForHorizontalChange(scrollablePanel, 1.0);
                        }
                        else
                        {
                            SetLineSizeForVerticalChange(scrollablePanel, 1.0);
                        }
                    }
                }
            }

            UIElement scrollable = _scrollInfo as UIElement;
            if (scrollable != null)
            {
                var mousePos = e.GetPosition(scrollable);

                if (_scrollInfo.CanHorizontallyScroll)
                {
                    var avgWidth = scrollable.RenderSize.Width / _scrollInfo.ViewportWidth; //translate to pixels
                    if (mousePos.X < 20)
                    {
                        var delta = (mousePos.X - 20) / avgWidth; //translate back to original unit
                        var lineSize = GetLineSizeForHorizontalChange(scrollable);
                        if (!double.IsNaN(lineSize))
                        {
                            _totalHorizontalDelta += delta;
                            if ((0 - _totalHorizontalDelta) >= lineSize) //since delta is negative here
                            {
                                _scrollInfo.LineLeft();
                                _totalHorizontalDelta = 0d;
                            }
                        }
                        else
                        {
                            _scrollInfo.SetHorizontalOffset(_scrollInfo.HorizontalOffset + delta);
                        }
                    }
                    else if (mousePos.X > (scrollable.RenderSize.Width - 20))
                    {
                        var delta = (mousePos.X + 20 - scrollable.RenderSize.Width) / avgWidth; //translate back to original unit
                        var lineSize = GetLineSizeForHorizontalChange(scrollable);
                        if (!double.IsNaN(lineSize))
                        {
                            _totalHorizontalDelta += delta;
                            if (_totalHorizontalDelta >= lineSize)
                            {
                                _scrollInfo.LineRight();
                                _totalHorizontalDelta = 0d;
                            }
                        }
                        else
                        {
                            _scrollInfo.SetHorizontalOffset(_scrollInfo.HorizontalOffset + delta);
                        }
                    }
                }

                if (_scrollInfo.CanVerticallyScroll)
                {
                    var avgHeight = scrollable.RenderSize.Height / _scrollInfo.ViewportHeight; //translate to pixels
                    if (mousePos.Y < 20)
                    {
                        var delta = (mousePos.Y - 20) / avgHeight; //translate back to original unit
                        var lineSize = GetLineSizeForVerticalChange(scrollable);
                        if (!double.IsNaN(lineSize))
                        {
                            _totalVerticalDelta += delta;
                            if ((0 - _totalVerticalDelta) >= lineSize) //since delta is negative here
                            {
                                _scrollInfo.LineUp();
                                _totalVerticalDelta = 0d;
                            }
                        }
                        else
                        {
                            _scrollInfo.SetVerticalOffset(_scrollInfo.VerticalOffset + delta);
                        }
                    }
                    else if (mousePos.Y > (scrollable.RenderSize.Height - 20))
                    {
                        var delta = (mousePos.Y + 20 - scrollable.RenderSize.Height) / avgHeight; //translate back to original unit
                        var lineSize = GetLineSizeForVerticalChange(scrollable);
                        if (!double.IsNaN(lineSize))
                        {
                            _totalVerticalDelta += delta;
                            if (_totalVerticalDelta >= lineSize)
                            {
                                _scrollInfo.LineDown();
                                _totalVerticalDelta = 0d;
                            }
                        }
                        else
                        {
                            _scrollInfo.SetVerticalOffset(_scrollInfo.VerticalOffset + delta);
                        }
                    }
                }
            }
        }
    }
}
