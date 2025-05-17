namespace InoriDock.WPF
{
    public class Struct
    {
        public struct ShortcutDescription
        {
            /// <summary>
            /// 启动参数
            /// </summary>
            public string Arguments { get; set; }

            /// <summary>
            /// 描述文本
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// 快捷方式文件的完整路径（只读属性）
            /// </summary>
            public string FullName { get; set; }

            /// <summary>
            /// 快捷方式的热键组合，用于快速启动目标程序
            /// </summary>
            public string Hotkey { get; set; }

            /// <summary>
            /// 快捷方式的图标位置，格式为 "路径,索引"
            /// </summary>
            public string IconLocation { get; set; }

            /// <summary>
            /// 快捷方式的目标路径
            /// </summary>
            public string TargetPath { get; set; }

            /// <summary>
            /// 快捷方式启动时窗口的样式（如最大化、最小化等）
            /// </summary>
            public int WindowStyle { get; set; }

            /// <summary>
            /// 快捷方式的工作目录，即目标程序运行时的当前目录
            /// </summary>
            public string WorkingDirectory { get; set; }
        }
    }
}