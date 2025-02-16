using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InoriDock.WPF.Public.DockbarComponents
{
    public class DockList
    {
        //存放作为DockButton父容器的panel，用于与_dockList的一维层对照
        private List<Panel> _PanelList;
        //存放所有的DockButton，并根据其父容器panel存放
        private List<List<DockItem>> _dockList;

        public DockList(List<Panel> panelList,List<List<DockItem>> dockItemList)
        {
            _PanelList = panelList;
            _dockList = dockItemList;
        }
        
        /// <summary>
        /// 获取对应的DockButton
        /// </summary>
        /// <param name="panelIndex"></param>
        /// <param name="dockItemIndex"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public DockItem this[int panelIndex,int dockItemIndex]
        {
            get
            {
                if (panelIndex + 1 <= _dockList.Count)
                {
                    if (dockItemIndex + 1 <= _dockList[panelIndex].Count)
                    {
                        return _dockList[panelIndex][dockItemIndex];
                    }
                    else
                    {
                        // dockItemIndex 超出范围
                        throw new ArgumentOutOfRangeException(nameof(dockItemIndex), $"DockItem index {dockItemIndex} is out of range for panel index {panelIndex}.");
                    }
                }
                else
                {
                    // panelIndex 超出范围
                    throw new ArgumentOutOfRangeException(nameof(panelIndex), $"Panel index {panelIndex} is out of range.");
                }
            }
        }
        /// <summary>
        /// 获取一个Dock和及其下的所有DockButton
        /// </summary>
        /// <param name="panelIndex"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public (Panel,List<DockItem>) this[int panelIndex]
        {
            get
            {
                if (panelIndex + 1 <= _dockList.Count)
                {
                    return (_PanelList[panelIndex], _dockList[panelIndex]);
                }
                else
                {
                    // panelIndex 超出范围
                    throw new ArgumentOutOfRangeException(nameof(panelIndex), $"Panel index {panelIndex} is out of range.");
                }
            }
        }

    }
}
