using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace SvgToXaml.ViewModels
{
    public abstract class ViewModelBase : BindableBase
    {
        protected static bool InDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        /// <summary>
        /// Führt die Action über den UI-Dispatcher aus.
        /// </summary>
        /// <param name="action">Auszuführende Aktion</param>
        public void InUi(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
        }

        /// <summary>
        /// Führt die Action über den UI-Dispatcher aus.
        /// </summary>
        /// <param name="priority">Priority</param>
        /// <param name="action">Auszuführende Aktion</param>
        public void InUi(DispatcherPriority priority, Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action, priority);
        }

        /// <summary>
        /// Sofern <paramref name="value" /> ungleich dem Wert in <paramref name="field"/> ist, wird das Member-Feld <paramref name="field"/> auf den Wert <paramref name="value" /> gesetzt und ein PropertyChanged-Event für die in <paramref name="propertyExpression" /> beschriebene Property ausgelöst.
        /// </summary>
        /// <typeparam name="T">Type-Parameter, wird inferiert und kann ignoriert werden</typeparam>
        /// <param name="field">Member-Variable, deren Wert gesetzt werden soll</param>
        /// <param name="value">der zu setzende neue Wert</param>
        /// <param name="propertyExpression">Lambda-Ausdruck, der die Property aufruft, deren Name in <see cref="BindableBase.PropertyChanged"/> übergeben werden soll. <example>Beispiel: <code>()=>Name</code></example></param> 
        /// <returns>Liefert <see cref="bool.True"/>, wenn der Wert des Feldes geändert wurde.</returns>
        protected bool SetProperty<T>(ref T field, T value, Expression<Func<T>> propertyExpression)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyExpression);

            return true;
        }

        protected new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }

    }
}
