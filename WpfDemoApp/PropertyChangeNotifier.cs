using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace WpfDemoApp
{
    public sealed class PropertyChangeNotifier :
        DependencyObject,
        IDisposable
    {
        #region Member Variables

        private readonly WeakReference _propertySource;

        #endregion // Member Variables

        #region PropertySource

        public DependencyObject PropertySource
        {
            get
            {
                try
                {
                    // note, it is possible that accessing the target property
                    // will result in an exception so i’ve wrapped this check
                    // in a try catch
                    return _propertySource.IsAlive
                        ? _propertySource.Target as DependencyObject
                        : null;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion // PropertySource

        #region IDisposable Members

        public void Dispose()
        {
            BindingOperations.ClearBinding(this, ValueProperty);
        }

        #endregion

        #region Events

        public event EventHandler ValueChanged;

        #endregion // Events

        #region Constructor

        public PropertyChangeNotifier(DependencyObject propertySource, string path)
            : this(propertySource, new PropertyPath(path))
        {
        }

        public PropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property)
            : this(propertySource, new PropertyPath(property))
        {
        }

        public PropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
        {
            if (null == propertySource)
                throw new ArgumentNullException(nameof(propertySource));
            if (null == property)
                throw new ArgumentNullException(nameof(property));
            _propertySource = new WeakReference(propertySource);
            var binding = new Binding();
            binding.Path = property;
            binding.Mode = BindingMode.OneWay;
            binding.Source = propertySource;
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }

        #endregion // Constructor

        #region Value

        /// <summary>
        ///     Identifies the <see cref="Value" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(object), typeof(PropertyChangeNotifier), new FrameworkPropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var notifier = (PropertyChangeNotifier) d;
            if (null != notifier.ValueChanged)
                notifier.ValueChanged(notifier, EventArgs.Empty);
        }

        /// <summary>
        ///     Returns/sets the value of the property
        /// </summary>
        /// <seealso cref="ValueProperty" />
        [Description("Returns / sets the value of the property")]
        [Category("Behavior")]
        [Bindable(true)]
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion //Value
    }
}