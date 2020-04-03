using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Yamed.Registry.DragAndDrop
{
    /// <summary>
    /// Опции для источника перетаскивания
    /// </summary>
    public struct DragOptions
    {
        /// <summary>
        /// Переносимые данные
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// Визуальное представление во время перетаскивания (курсор)
        /// </summary>
        //public FrameworkElement Visual { get; set; }
        public Lazy<FrameworkElement> VisualFactory { get; set; }
        /// <summary>
        /// Изменение расположения Template в зависимости от точки касания
        /// </summary>
        public bool IsDragOffsetEnabled { get; set; }
        /// <summary>
        /// Явное смещение расположения Template
        /// </summary>
        public Point VisualOffset { get; set; }
        /// <summary>
        /// Метод, вызываемый после начала перетаскивания
        /// </summary>
        public Action DragStarted { get; set; }
        /// <summary>
        /// Метод, вызываемый в конца перетаскивания
        /// </summary>
        public Action DragEnded { get; set; }
        /// <summary>
        /// Метод, вызываемый при отмене перетаскивания
        /// </summary>
        public Action DragCancelled { get; set; }
    }

    public class MyDragDropEventArgs
    {
        public FrameworkElement DragSource { get; set; }
        public object Data { get; set; }
        public DragEventArgs DragArgs { get; set; }
    }

    public struct DropOptions
    {
        public Action<MyDragDropEventArgs> DragEnter { get; set; }
        public Action<MyDragDropEventArgs> DragOver { get; set; }
        public Action<MyDragDropEventArgs> DragLeave { get; set; }
        public Action<MyDragDropEventArgs> Dropped { get; set; }
    }

    public class DragManager
    {
        private static DragManager instance;
        private Dictionary<FrameworkElement, DragOptions> sources = new Dictionary<FrameworkElement, DragOptions>();
        private Dictionary<FrameworkElement, DropOptions> targets = new Dictionary<FrameworkElement, DropOptions>();
        private List<FrameworkElement> excepts = new List<FrameworkElement>();

        private Point initialMousePosition;                         //Первоначальная позиция мыши (нужна для оперделения координат для отрисовки)
        private DraggedAdorner draggedAdorner;                      //Рисунок, который прикрепляется к указателю мыши
        private Window topWindow;                                   //Главное окно, для определения координат (наверно заменю на adornerCanvas)


        //private IDragSource draggingElement;                        //Перетаскиваемый элемент
        //private FrameworkElement draggingVisual;                    //Его рисунок, который возле мыши

        private FrameworkElement draggingElement;
        private Point draggingOffset = new Point(0, 0);

        //
        //  Функции добавления источников и целей


        /// <summary>
        /// Добавляет источник, который можно перетаскивать
        /// </summary>
        /// <param name="source">Какой-нибудь контрол</param>
        public void AddDragSource(FrameworkElement source, DragOptions options)
        {

            if (!sources.ContainsKey(source))
            {
                this.sources.Add(source, options);
                source.MouseLeftButtonDown += new MouseButtonEventHandler(this.DragSource_MouseLeftButtonDown);
                source.MouseLeftButtonUp += new MouseButtonEventHandler(this.DragSource_MouseLeftButtonUp);
                source.MouseMove += new MouseEventHandler(this.DragSource_MouseMove);
            }
        }

        /// <summary>
        /// Добавляет приёмник перетаскиваемых данных
        /// </summary>
        /// <param name="target">Какой-нибудь контрол</param>
        public void AddDropTarget(FrameworkElement target, DropOptions options)
        {
            if (!this.targets.ContainsKey(target))
            {
                var td = target;
                td.AllowDrop = true;
                td.DragEnter += new DragEventHandler(DropTarget_DragEnter);
                td.DragOver += new DragEventHandler(DropTarget_DragOver);
                td.DragLeave += new DragEventHandler(DropTarget_DragLeave);
                td.Drop += new DragEventHandler(DropTarget_Drop);
                this.targets.Add(target, options);
            }
        }

        /// <summary>
        /// Противоположность DropTarget, запрещает перетаскивание на себя
        /// </summary>
        /// <param name="fe">Какой-нибудь контрол</param>
        public void AddExceptDropTarget(FrameworkElement fe)
        {
            fe.AllowDrop = true;        //При false не будет отображаться перетаскиваемый элемент
            fe.DragEnter += new DragEventHandler(ExceptTarget_DragEnter);
            fe.DragOver += new DragEventHandler(ExceptTarget_DragOver);
            this.excepts.Add(fe);
        }



        //
        // События drag and drop для источников и целей

        //
        //Цели-исключения
        private void ExceptTarget_DragEnter(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(this.topWindow));
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void ExceptTarget_DragOver(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(this.topWindow));
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        //
        // События элементов-источников

        private void DragSource_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            this.topWindow = Window.GetWindow(element);
            this.initialMousePosition = e.GetPosition(this.topWindow);

            this.draggingElement = element;
            this.draggingOffset = new Point(0, 0);
            if (sources[draggingElement].IsDragOffsetEnabled)
                this.draggingOffset = e.GetPosition(this.draggingElement as UIElement);
        }

        private void DragSource_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CancelDrag();
        }

        private void DragSource_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.draggingElement != null)
            {
                //Функция определения перетаскиваемого расстояния
                Func<Point, Point, bool> IsMovementBigEnough = (initialMousePosition, currentPosition) =>
                    (Math.Abs(currentPosition.X - initialMousePosition.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPosition.Y - initialMousePosition.Y) >= SystemParameters.MinimumVerticalDragDistance);

                if (IsMovementBigEnough(this.initialMousePosition, e.GetPosition(this.topWindow)))
                {
                    if (sources[draggingElement].DragStarted != null) sources[draggingElement].DragStarted();
                    this.initialMousePosition = e.GetPosition(this.topWindow);

                    //События главного окна для отображения рисунка возле указателя мыши
                    bool previousAllowDrop = this.topWindow.AllowDrop;
                    this.topWindow.AllowDrop = true;
                    this.topWindow.DragEnter += TopWindow_DragEnter;
                    this.topWindow.DragOver += TopWindow_DragOver;
                    this.topWindow.DragLeave += TopWindow_DragLeave;

                    //Можно сделать так, чтобы указатель мыши был всегда одинаковым
                    //this.draggingElement.GiveFeedback += (s, args) =>
                    //{
                    //    args.UseDefaultCursors = false;
                    //    args.Handled = true;
                    //    Mouse.SetCursor(Cursors.Arrow);
                    //};

                    DataObject data = new DataObject("Object", sources[this.draggingElement].Data);
                    DragDropEffects effects = DragDrop.DoDragDrop(this.draggingElement, data, DragDropEffects.Move);

                    //Если не найдена ни одна цель, отменяем перетаскивание
                    if (effects == DragDropEffects.None)
                        CancelDrag();

                    //Отменяем события окна
                    this.topWindow.AllowDrop = previousAllowDrop;
                    this.topWindow.DragEnter -= TopWindow_DragEnter;
                    this.topWindow.DragOver -= TopWindow_DragOver;
                    this.topWindow.DragLeave -= TopWindow_DragLeave;
                    RemoveDraggedAdorner();


                    this.draggingElement = null;
                }
            }
        }

        //
        // События для целевых элементов

        private MyDragDropEventArgs CreateMyDragDropEventArgs(DragEventArgs e)
        {
            return new MyDragDropEventArgs { DragSource = this.draggingElement, Data = e.Data.GetData("Object"), DragArgs = e };
        }

        private void DropTarget_DragEnter(object sender, DragEventArgs e)
        {
            //В этой функции показывает рамку при движении над листбоксом
            ShowDraggedAdorner(e.GetPosition(this.topWindow));

            var t = sender as FrameworkElement;
            if (targets[t].DragEnter != null)
                targets[t].DragEnter(CreateMyDragDropEventArgs(e));
            e.Handled = true;
        }

        private void DropTarget_DragOver(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(this.topWindow));

            //var s = e.Data.GetData("FrameworkElement") as FrameworkElement;
            var t = sender as FrameworkElement;
            if (targets[t].DragOver != null)
                targets[t].DragOver(CreateMyDragDropEventArgs(e));
            e.Handled = true;
        }

        private void DropTarget_DragLeave(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(this.topWindow));
            //var s = e.Data.GetData("FrameworkElement") as FrameworkElement;
            var t = sender as FrameworkElement;
            if (targets[t].DragLeave != null)
                targets[t].DragLeave(CreateMyDragDropEventArgs(e));
            e.Handled = true;
        }

        void DropTarget_Drop(object sender, DragEventArgs e)
        {
            var s = this.draggingElement;
            var t = sender as FrameworkElement;

            if (targets[t].DragLeave != null) targets[t].DragLeave(CreateMyDragDropEventArgs(e));
            if (targets[t].Dropped != null) targets[t].Dropped(CreateMyDragDropEventArgs(e));
            if (sources[s].DragEnded != null) sources[s].DragEnded();
            

            e.Handled = true;
        }

        //
        // События окна

        private void TopWindow_DragEnter(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(sender as IInputElement));
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void TopWindow_DragOver(object sender, DragEventArgs e)
        {
            ShowDraggedAdorner(e.GetPosition(sender as IInputElement));
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void TopWindow_DragLeave(object sender, DragEventArgs e)
        {
            RemoveDraggedAdorner();
            e.Handled = true;
        }

        //
        // Adorners

        private void ShowDraggedAdorner(Point currentPosition)
        {
            var visual = sources[this.draggingElement].VisualFactory.Value;

            if(this.AdornerCanvas.Children.Count == 0)
                this.AdornerCanvas.Children.Add(visual);

            if (sources[draggingElement].IsDragOffsetEnabled)
            {
                Canvas.SetLeft(visual, currentPosition.X - draggingOffset.X + sources[draggingElement].VisualOffset.X);
                Canvas.SetTop(visual, currentPosition.Y - draggingOffset.Y + sources[draggingElement].VisualOffset.Y);
            }
            else
            {
                Canvas.SetLeft(visual, currentPosition.X + sources[draggingElement].VisualOffset.X);
                Canvas.SetTop(visual, currentPosition.Y + sources[draggingElement].VisualOffset.Y);
            }
        }

        private void RemoveDraggedAdorner()
        {
            this.AdornerCanvas.Children.Clear();
        }


        /// <summary>
        /// Функция отмены перетаскивания
        /// </summary>
        public void CancelDrag()
        {
            if (this.draggingElement != null)
            {
                if (sources[this.draggingElement].DragCancelled != null)
                    sources[this.draggingElement].DragCancelled();
                this.draggingElement = null;
            }
        }


        //
        //Функции удаления источников и целей

        public void RemoveDragSource(FrameworkElement source)
        {
            if (this.sources.Remove(source))
            {
                source.MouseLeftButtonDown -= new MouseButtonEventHandler(this.DragSource_MouseLeftButtonDown);
                source.MouseLeftButtonUp -= new MouseButtonEventHandler(this.DragSource_MouseLeftButtonUp);
                source.MouseMove -= new MouseEventHandler(this.DragSource_MouseMove);
            }
        }

        public void RemoveDropTarget(FrameworkElement target)
        {
            if (targets.Remove(target))
            {
                var td = target;
                //td.AllowDrop = false;
                td.DragEnter -= new DragEventHandler(DropTarget_DragEnter);
                td.DragOver -= new DragEventHandler(DropTarget_DragOver);
                td.DragLeave -= new DragEventHandler(DropTarget_DragLeave);
                td.Drop -= new DragEventHandler(DropTarget_Drop);
            }
        }

        public void RemoveExceptDropTarget(FrameworkElement fe)
        {
            if (this.excepts.Remove(fe))
            {
                fe.DragEnter -= new DragEventHandler(ExceptTarget_DragEnter);
                fe.DragOver -= new DragEventHandler(ExceptTarget_DragOver);
            }
        }

        //
        //Свойства

        /// <summary>
        /// Единственный эеземпляр класса
        /// </summary>
        public static DragManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DragManager();
                }
                return instance;
            }
        }

        private Canvas _adornerCanvas;

        public Canvas AdornerCanvas
        {
            get 
            {
                if (this._adornerCanvas == null)
                {
                    var panel = Application.Current.MainWindow.Content as Panel;
                    if (panel != null)
                    {
                        _adornerCanvas = new Canvas { IsHitTestVisible = false };
                        panel.Children.Add(_adornerCanvas);
                    }
                }

                return _adornerCanvas; 
            }
            set { _adornerCanvas = value; }
        }
        

    }
}
