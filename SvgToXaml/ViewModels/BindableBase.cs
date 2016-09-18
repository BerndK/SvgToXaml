using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SvgToXaml.ViewModels
{
    /// <summary>
    /// Implementation of <see cref="T:System.ComponentModel.INotifyPropertyChanged"/> to simplify models.
    /// 
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        ///             notifies listeners only when necessary.
        /// 
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam><param name="storage">Reference to a property with both getter and setter.</param><param name="value">Desired value for the property.</param><param name="propertyName">Name of the property used to notify listeners. This
        ///             value is optional and can be provided automatically when invoked from compilers that
        ///             support CallerMemberName.</param>
        /// <returns>
        /// True if the value was changed, false if the existing value matched the
        ///             desired value.
        /// </returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// 
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        ///             value is optional and can be provided automatically when invoked from compilers
        ///             that support <see cref="T:System.Runtime.CompilerServices.CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler changedEventHandler = PropertyChanged;
            changedEventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the property that has a new value</typeparam><param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            OnPropertyChanged(ExtractPropertyName(propertyExpression));
        }

        /// <summary>
        /// Extracts the property name from a property expression.
        /// 
        /// </summary>
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam><param name="propertyExpression">The property expression (e.g. p =&gt; p.PropertyName)</param>
        /// <returns>
        /// The name of the property.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown if the <paramref name="propertyExpression"/> is null.</exception><exception cref="T:System.ArgumentException">Thrown when the expression is:<br/>
        ///                 Not a <see cref="T:System.Linq.Expressions.MemberExpression"/><br/>
        ///                 The <see cref="T:System.Linq.Expressions.MemberExpression"/> does not represent a property.<br/>
        ///                 Or, the property is static.
        ///             </exception>
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));
            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("PropertySupport NotMemberAccessExpression", nameof(propertyExpression));
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("PropertySupport ExpressionNotProperty", nameof(propertyExpression));
            if (propertyInfo.GetMethod.IsStatic)
                throw new ArgumentException("PropertySupport StaticExpression", nameof(propertyExpression));
            return memberExpression.Member.Name;
        }

    }
}
