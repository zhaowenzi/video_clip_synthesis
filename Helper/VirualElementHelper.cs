using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace 素材合成.Helper
{
    public static class VirualElementHelper
    {
        /// <summary>
        /// 通过查找obj父元素 获取相应的类型的控件 
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="obj">当前控件</param>
        /// <param name="name"> 控件名字 可为空</param>
        /// <returns></returns>
        public static T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// 通过查找obj子元素 获取相应的类型的控件 
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="obj">当前控件</param>
        /// <param name="name"> 控件名字 可为空</param>
        /// <returns></returns>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    child = GetChildObject<T>(child,name);
                    if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                    {
                        return (T)child;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 通过查找obj子元素 获取相应的类型的控件 
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="obj">当前控件</param>
        /// <param name="name"> 控件名字 可为空</param>
        /// <returns></returns>
        public static List<T> GetChildObjectList<T>(DependencyObject obj,string name) where T : FrameworkElement
        {

            try
            {
                List<T> TList = new List<T> { };
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                    {
                        TList.Add((T)child);
                        List<T> childOfChildren = GetChildObjectList<T>(child, name);
                        if (childOfChildren != null)
                        {
                            TList.AddRange(childOfChildren);
                        }
                    }
                    else
                    {
                        List<T> childOfChildren = GetChildObjectList<T>(child, name);
                        if (childOfChildren != null)
                        {
                            TList.AddRange(childOfChildren);
                        }
                    }
                }
                return TList;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                return null;
            }
        }
    }
}
