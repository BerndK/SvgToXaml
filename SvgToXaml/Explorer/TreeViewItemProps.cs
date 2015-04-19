using System.Windows;

namespace SvgToXaml.Explorer
{

	public static class TreeViewItemProps
	{
		public static bool GetIsRootLevel(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsRootLevelProperty);
		}

		public static void SetIsRootLevel(
			DependencyObject obj, bool value)
		{
			obj.SetValue(IsRootLevelProperty, value);
		}
		
		public static readonly DependencyProperty IsRootLevelProperty =
			DependencyProperty.RegisterAttached(
			"IsRootLevel", 
			typeof(bool), 
			typeof(TreeViewItemProps), 
			new UIPropertyMetadata(false));
	}

}