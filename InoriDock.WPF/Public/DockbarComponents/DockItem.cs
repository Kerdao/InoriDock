using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

using met = InoriDock.WPF.Public.Methods.Methods;

namespace InoriDock.WPF.Public.DockbarComponents
{
    public class DockItem : Button
    {
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(DockItem), new PropertyMetadata(-1));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(DockItem));


        private ResourceDictionary _resourceDictionary;

        /// <summary>
        /// 目标路径
        /// </summary>
        public string TargetPath { get; set; }

        public DockItem()
        {
            // 创建一个新的ResourceDictionary并加载资源文件
            _resourceDictionary = Methods.Methods.GetResource("pack://application:,,,/Public/DockbarComponents/Animation/DockItemAnimation.xaml");

            this.Loaded += (sender, e) =>
            {
                UpdateIcon();
            };
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

        public void UpdateIndex()
        {
            if ((Methods.Methods.GetParent(this) is Panel panel) == false)
            {
                Index = -1;
                return;
            }

            int i = 0;
            DockList list = Dock.GetDockList;
            foreach (var item in list[Dock.GetPanclIndex(panel)].Item2)
            {
                if (this == item)
                {
                    this.Index = i;
                }
                i += 1;
            }
            return;
        }
        public void UpdateIcon()
        {
            if (TargetPath == null) return;
            Source = Methods.Methods.IconToBitmapSourceFromPath(TargetPath);
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