using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace InoriDock.Public.DockbarComponents
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

        private ResourceDictionary _resourceDictionary;

        public DockItem()
        {
            this.Loaded += (sender, e) =>
            {
                // 创建一个新的ResourceDictionary并加载资源文件
                _resourceDictionary = Methods.GetResource("pack://application:,,,/Public/DockbarComponents/Animation/DockItemAnimation.xaml");
            };
        }

        public void UpdateIndex()
        {
            Index = Methods.GetControlIndexThanParent(this);
        }

        public void UpdateStyle(int Grade)
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
                    moveAdded = moveAdded*0.5;
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
        private void StartAnimation(string AnimationName ,PropertyPath property, double To)
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
    }
}
