﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace BLL_Infrastructure
{
    public class EnumHelper
    {
        public static Type GetEnum(DependencyObject obj)
        {
            return (Type)obj.GetValue(EnumProperty);
        }
        public static void SetEnum(DependencyObject obj, string value)
        {
            obj.SetValue(EnumProperty, value);
        }
        // Using a DependencyProperty as the backing store for Enum.  
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnumProperty =
            DependencyProperty.RegisterAttached("Enum", typeof(Type),
            typeof(EnumHelper), new PropertyMetadata(null, OnEnumChanged));

        private static void OnEnumChanged(DependencyObject sender,
                          DependencyPropertyChangedEventArgs e)
        {
            var control = sender as ItemsControl;
            if (control != null)
            {
                if (e.NewValue != null)
                {
                    var _enum = Enum.GetValues(e.NewValue as Type);
                    control.ItemsSource = _enum;
                }
            }
        }
    }
}
