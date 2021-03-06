﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UIBlocks.MaterialDesignTheme
{
    public class SmartHint:Control
    {
        public const string ContentStatesGroupName = "ContentStates";
        public const string ContentEmptyName = "ContentEmpty";
        public const string ContentNotEmptyName = "ContentNotEmpty";

        static SmartHint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SmartHint),new FrameworkPropertyMetadata(typeof(SmartHint)));
        }

        public SmartHint()
        {
            IsHitTestVisible = false;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        #region HintProxy,依赖属性，以代理设计模式来控制对象对SmarHint的访问。
        public static readonly DependencyProperty HintProxyProperty = DependencyProperty.Register(
    "HintProxy", typeof(IHintProxy), typeof(SmartHint), new PropertyMetadata(default(IHintProxy), HintProxyPropertyChangedCallback));
        public IHintProxy HintProxy
        {
            get { return (IHintProxy)GetValue(HintProxyProperty); }
            set { SetValue(HintProxyProperty, value); }
        }
        private static void HintProxyPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var smartHint = dependencyObject as SmartHint;
            if (smartHint == null) return;

            var hintProxy = dependencyPropertyChangedEventArgs.OldValue as IHintProxy;

            if (hintProxy != null)
            {
                hintProxy.IsVisibleChanged -= smartHint.OnHintProxyIsVisibleChanged;
                hintProxy.ContentChanged -= smartHint.OnHintProxyContentChanged;
                hintProxy.Loaded -= smartHint.OnHintProxyContentChanged;
                hintProxy.Dispose();
            }

            hintProxy = dependencyPropertyChangedEventArgs.NewValue as IHintProxy;
            if (hintProxy != null)
            {
                hintProxy.IsVisibleChanged += smartHint.OnHintProxyIsVisibleChanged;
                hintProxy.ContentChanged += smartHint.OnHintProxyContentChanged;
                hintProxy.Loaded += smartHint.OnHintProxyContentChanged;
                smartHint.RefreshState(false);
            }
        }
        #endregion

        #region Hint,依赖属性，提示内容。

        public static readonly DependencyProperty HintProperty = DependencyProperty.Register(
            nameof(Hint), typeof(object), typeof(SmartHint), new PropertyMetadata(null));

        public object Hint
        {
            get { return GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        #endregion

        #region IsContentNullOrEmpty，依赖属性，内容是否为空。
        public static readonly DependencyProperty IsContentNullOrEmptyProperty = DependencyProperty.Register(
            nameof(IsContentNullOrEmpty), typeof(bool), typeof(SmartHint), new PropertyMetadata(default(bool)));
        public bool IsContentNullOrEmpty
        {
            get { return (bool)GetValue(IsContentNullOrEmptyProperty); }
            set { SetValue(IsContentNullOrEmptyProperty, value); }
        }
        #endregion

        #region UseFloating，依赖属性，是否使用飘浮特效。
        public static readonly DependencyProperty UseFloatingProperty = DependencyProperty.Register(
            nameof(UseFloating), typeof(bool), typeof(SmartHint), new PropertyMetadata(false));
        public bool UseFloating
        {
            get { return (bool)GetValue(UseFloatingProperty); }
            set { SetValue(UseFloatingProperty, value); }
        }
        #endregion

        #region FloatingScale & FloatingOffset 浮动后缩放大小和偏移位置。
        public static readonly DependencyProperty FloatingScaleProperty = DependencyProperty.Register(
            nameof(FloatingScale), typeof(double), typeof(SmartHint), new PropertyMetadata(.74));
        public double FloatingScale
        {
            get { return (double)GetValue(FloatingScaleProperty); }
            set { SetValue(FloatingScaleProperty, value); }
        }
        public static readonly DependencyProperty FloatingOffsetProperty = DependencyProperty.Register(
            nameof(FloatingOffset), typeof(Point), typeof(SmartHint), new PropertyMetadata(new Point(1, -16)));
        public Point FloatingOffset
        {
            get { return (Point)GetValue(FloatingOffsetProperty); }
            set { SetValue(FloatingOffsetProperty, value); }
        }
        #endregion

        #region HintOpacity，Hint透明度。
        public static readonly DependencyProperty HintOpacityProperty = DependencyProperty.Register(
            nameof(HintOpacity), typeof(double), typeof(SmartHint), new PropertyMetadata(.46));
        public double HintOpacity
        {
            get { return (double)GetValue(HintOpacityProperty); }
            set { SetValue(HintOpacityProperty, value); }
        }
        #endregion

        private void RefreshState(bool useTransitions)
        {
            var proxy = HintProxy;

            if (proxy == null) return;
            if (!proxy.IsVisible) return;

            var action = new Action(() =>
            {
                var state = String.IsNullOrEmpty((proxy.Content ?? String.Empty).ToString()) ? ContentEmptyName:ContentNotEmptyName;
                VisualStateManager.GoToState(this, state, useTransitions);
            });

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                action();
            }
            else
            {
                Dispatcher.BeginInvoke(action);
            }
        }

        protected virtual void OnHintProxyIsVisibleChanged(object sender, EventArgs e)
        {
            RefreshState(false);
        }

        private void HintProxySetStateOnLoaded(object sender, EventArgs e)
        {
            RefreshState(false);
            HintProxy.Loaded -= HintProxySetStateOnLoaded;
        }

        protected virtual void OnHintProxyContentChanged(object sender, EventArgs e)
        {
            IsContentNullOrEmpty = string.IsNullOrEmpty((HintProxy.Content ?? "").ToString());

            if (HintProxy.IsLoaded)
            {
                RefreshState(true);
            }
            else
            {
                HintProxy.Loaded += HintProxySetStateOnLoaded;
            }
        }
    }
}
