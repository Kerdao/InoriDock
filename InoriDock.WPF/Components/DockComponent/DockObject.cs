﻿using InoriDock.WPF.Components.DockComponent.DockItems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InoriDock.WPF.Components.DockComponent
{
    public class DockObject
    {
        public DockObject(Panel DockOf)
        {
            _PanelOf = DockOf;
        }
        private readonly Panel _PanelOf;
        public List<DockItem> Children;
        private int _mouseOverIndex;
        private void SetDockItemStyle(int index, int Grade)
        {
            //因为index序列从0开始
            if (index < 0 || index + 1 > Children.Count)
            {
                //筛选器
                return;
            }

            Children[index].BouncingAnimation(Grade);



        }
        public int MouseOverIndex
        {
            get => _mouseOverIndex;
            set
            {
                if (value != -1)
                {
                    /*此处注释代码虽然解决多个Grade为1的问题，但是会有卡顿问题
                     * >1 if用于修复在没有触发DockMouseLeave事件情况下有多个Grade为1的item
                    if (Math.Abs(value - _mouseOverIndex) > 1 && value != -1 && _mouseOverIndex != -1)
                    {
                        //MessageBox.Show("c  new:"+  (int)e.NewValue +" old:"+ (int)e.OldValue);
                        SetDockItemStyle(_mouseOverIndex - 3, -1);
                        SetDockItemStyle(_mouseOverIndex - 2, -1);
                        SetDockItemStyle(_mouseOverIndex - 1, -1);
                        SetDockItemStyle(_mouseOverIndex, -1);
                        SetDockItemStyle(_mouseOverIndex + 1, -1);
                        SetDockItemStyle(_mouseOverIndex + 2, -1);
                        SetDockItemStyle(_mouseOverIndex + 3, -1);
                    }
                    */
                    SetDockItemStyle(value - 3, 4);
                    SetDockItemStyle(value - 2, 3);
                    SetDockItemStyle(value - 1, 2);
                    SetDockItemStyle(value, 1);
                    SetDockItemStyle(value + 1, 2);
                    SetDockItemStyle(value + 2, 3);
                    SetDockItemStyle(value + 3, 4);
                }
                else
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        SetDockItemStyle(i, -1);
                    }
                }
            }
        }
        private JsonSerializerSettings setting = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore, // 忽略 null 值
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // 忽略循环引用
        };

        public void Save()
        {
            var root = new JObject();
            var array = new JArray();
            foreach (DockItem child in Children)
            {
                array.Add(child.ToJObject());
            }
            root["Children"] = array;

            string json = JsonConvert.SerializeObject(root, Formatting.None);

            string path = @"config\DockLayout.json";
            string directory = Path.GetDirectoryName(path);

            Directory.CreateDirectory(directory);
            File.WriteAllText(path, json);
        }
        public void Load()
        {
            string path = @"config\DockLayout.json";
            if (!File.Exists(path))
            {
                return;
            }
            string json = File.ReadAllText(path);
            JObject root = JObject.Parse(json);
            JArray array = (JArray)root["Children"];
            foreach (var item in array)
            {
                DockItem dockItem = (DockItem)Activator.CreateInstance(Type.GetType(item["Type"].ToString()));
                dockItem.LoadFromJObject((JObject)item, _PanelOf);
                Children.Add(dockItem);
                _PanelOf.Children.Add(dockItem);
            }
        }
    }
}
