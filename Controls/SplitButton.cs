using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Confabulation.Controls
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Confabulation.Controls"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Confabulation.Controls;assembly=Confabulation.Controls"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <MyNamespace:SplitButton/>
	///
	/// </summary>
	[TemplatePart(Name = "PART_DropDown", Type = typeof(ToggleButton))]
	[ContentProperty("Items")]
	[DefaultProperty("Items")]
	public class SplitButton : Button
	{
		public static readonly DependencyProperty IsContextMenuOpenProperty =
			DependencyProperty.Register("IsContextMenuOpen",
			typeof(bool),
			typeof(SplitButton),
			new FrameworkPropertyMetadata(false));

		public bool IsContextMenuOpen
		{
			get
			{
				return (bool)GetValue(IsContextMenuOpenProperty);
			}
			set
			{
				SetValue(IsContextMenuOpenProperty, value);
			}
		}

		static SplitButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
		}

		public SplitButton()
		{
			ContextMenuOpening += OnContextMenuOpening;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			ButtonBase dropDown = Template.FindName("PART_DropDown", this) as ButtonBase;

			if (dropDown != null)
			{
				dropDown.Click += OnDropDownClick;
				dropDown.ContextMenuOpening += OnContextMenuOpening;

				Binding binding = new Binding("ContextMenu.IsOpen");
				binding.Source = this;
				SetBinding(IsContextMenuOpenProperty, binding);
			}

		}

		void OnDropDownClick(object sender, RoutedEventArgs e)
		{
			if (ContextMenu == null || ContextMenu.HasItems == false)
				return;

			ContextMenu.PlacementTarget = this;
			ContextMenu.IsOpen = true;
			ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;

			e.Handled = true;
		}

		void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			e.Handled = true;
		}

		public ItemCollection Items
		{
			get
			{
				if (ContextMenu == null)
					ContextMenu = new ContextMenu();

				return ContextMenu.Items;
			}
		}
	}
}
