using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Method = InoriDock.WPF.Methods;
namespace InoriDock.WPF.Components.DockComponent.DockItems
{
    //此类定义了DockItem的基础行为和属性
    public abstract class DockItemBase : Button
    {
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(DockItemBase));
        public Panel DockOf { get; set; }

        private ResourceDictionary _resourceDictionary;

        public DockItemBase()
        {
            _resourceDictionary = Method.GetResourceDictionary("/InoriDock.WPF;component/Components/DockComponent/Animation/DockItemAnimation.xaml");
            MouseEnter += OnDockItemMouseEnter;
            MouseLeave += OnDockItemMouseLeave;
        }
        private static void OnDockItemMouseEnter(object sender, MouseEventArgs e)
        {
            var item = (DockItem)sender;
            Dock.GetDockObject(item.DockOf).MouseOverIndex = Dock.GetDockObject(item.DockOf).Children.IndexOf(item);

            e.Handled = true;
        }
        private static void OnDockItemMouseLeave(object sender, MouseEventArgs e)
        {
            //暂且不写
            e.Handled = true;
        }

        private void StartAnimation(string AnimationName, double To)
        {
            // 获取Storyboard
            Storyboard sb = (Storyboard)_resourceDictionary[AnimationName];

            // 创建一个新的Storyboard
            Storyboard newSb = new Storyboard();

            // 创建一个新的Animation并克隆
            DoubleAnimation newAnimation = (DoubleAnimation)sb.Children[0].Clone();

            newAnimation.To = To;

            Storyboard.SetTarget(newAnimation, this);

            newSb.Children.Add(newAnimation);
            // 开始动画
            newSb.Begin();
        }
        private void StartAnimation(string AnimationName, PropertyPath property, double To)
        {
            // 获取Storyboard
            Storyboard sb = (Storyboard)_resourceDictionary[AnimationName];

            // 创建一个新的Storyboard
            Storyboard newSb = new Storyboard();

            // 创建一个新的Animation并克隆
            DoubleAnimation newAnimation = (DoubleAnimation)sb.Children[0].Clone();

            newAnimation.To = To;

            Storyboard.SetTarget(newAnimation, this);
            Storyboard.SetTargetProperty(newAnimation, property);

            newSb.Children.Add(newAnimation);
            // 开始动画
            newSb.Begin();
        }

        //鼠标over或在旁边的动画
        public void BouncingAnimation(int Grade)
        {
            double MoveOriginal = 0;
            double WidthOriginal = 80;
            double HeightOriginal = 80;

            double moveAdded = -35;
            double widthAdded = 120;
            double heightAdded = 120;

            switch (Grade)
            {
                case 1:

                    break;
                case 2:
                    moveAdded = moveAdded * 0.5;
                    widthAdded = 88;
                    heightAdded = 88;
                    break;
                case 3:
                    moveAdded = moveAdded * 0.2;
                    widthAdded = 82;
                    heightAdded = 82;
                    break;
                case 4:
                    moveAdded = MoveOriginal;
                    widthAdded = WidthOriginal;
                    heightAdded = HeightOriginal;
                    break;
                case -1:
                    moveAdded = MoveOriginal;
                    widthAdded = WidthOriginal;
                    heightAdded = HeightOriginal;
                    break;
                default:
                    moveAdded = MoveOriginal;
                    widthAdded = WidthOriginal;
                    heightAdded = HeightOriginal;
                    break;
            }

            //修改高度
            this.Width = widthAdded;
            this.Height = heightAdded;

            StartAnimation("MoveUp", moveAdded);
            StartAnimation("SizeUpdate", new PropertyPath("(FrameworkElement.Width)"), widthAdded);
            StartAnimation("SizeUpdate", new PropertyPath("(FrameworkElement.Height)"), heightAdded);
        }
    }
}
