using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Raccoom.Windows.Forms.Design
{
    /// <summary>
    ///     Implements a custom type editor for enum's with FlagAttribute
    /// </summary>
    /// <remarks>
    ///     Copyright by Thierry Bouquain,
    ///     <a href="http://www.codeproject.com/cs/miscctrl/flagseditor.asp?target=FlagsEditor" target="_blank">
    ///         A flag editor
    ///         article on codeproject.com
    ///     </a>
    /// </remarks>
    public class FlagsEditor : UITypeEditor
    {
        private CheckedListBox _clb;

        private IWindowsFormsEditorService _edSvc;

        private bool _handleLostfocus;
        private ToolTip _tooltipControl;

        /// <summary>
        ///     Overrides the method used to provide basic behaviour for selecting editor.
        ///     Shows our custom control for editing the value.
        /// </summary>
        /// <param name="context">The context of the editing control</param>
        /// <param name="provider">A valid service provider</param>
        /// <param name="value">The current value of the object to edit</param>
        /// <returns>The new value of the object</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context?.Instance == null || provider == null)
            {
                return value;
            }

            _edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (_edSvc == null)
            {
                return value;
            }

            // Create a CheckedListBox and populate it with all the enum values
            _clb = new CheckedListBox();
            _clb.BorderStyle = BorderStyle.FixedSingle;
            _clb.CheckOnClick = true;
            _clb.MouseDown += OnMouseDown;
            _clb.MouseMove += OnMouseMoved;

            _tooltipControl = new ToolTip();
            _tooltipControl.ShowAlways = true;

            if (context.PropertyDescriptor == null)
            {
                return null;
            }

            foreach (string name in Enum.GetNames(context.PropertyDescriptor.PropertyType))
            {
                // Get the enum value
                object enumVal = Enum.Parse(context.PropertyDescriptor.PropertyType, name);
                // Get the int value 
                int intVal = (int)Convert.ChangeType(enumVal, typeof(int));

                // Get the description attribute for this field
                FieldInfo fi = context.PropertyDescriptor.PropertyType.GetField(name);
                DescriptionAttribute[] attrs =
                    (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                // Store the the description
                string tooltip = attrs.Length > 0 ? attrs[0].Description : string.Empty;

                // Get the int value of the current enum value (the one being edited)
                int intEdited = (int)Convert.ChangeType(value, typeof(int));

                // Creates a clbItem that stores the name, the int value and the tooltip
                ClbItem item = new ClbItem(enumVal.ToString(), intVal, tooltip);

                // Get the checkstate from the value being edited
                //bool checkedItem = (intEdited & intVal) > 0;
                bool checkedItem = (intEdited & intVal) == intVal;

                // Add the item with the right check state
                _clb.Items.Add(item, checkedItem);
            }

            // Show our CheckedListbox as a DropDownControl. 
            // This methods returns only when the dropdowncontrol is closed
            _edSvc.DropDownControl(_clb);

            // Get the sum of all checked flags
            int result = _clb.CheckedItems.Cast<ClbItem>().Aggregate(0, (current, obj) => current | obj.Value);

            // return the right enum value corresponding to the result
            return Enum.ToObject(context.PropertyDescriptor.PropertyType, result);
        }

        /// <summary>
        ///     Shows a dropdown icon in the property editor
        /// </summary>
        /// <param name="context">The context of the editing control</param>
        /// <returns>Returns <c>UITypeEditorEditStyle.DropDown</c></returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) =>
            UITypeEditorEditStyle.DropDown;

        /// <summary>
        ///     When got the focus, handle the lost focus event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (_handleLostfocus || !_clb.ClientRectangle.Contains(_clb.PointToClient(new Point(e.X, e.Y))))
            {
                return;
            }

            _clb.LostFocus += ValueChanged;
            _handleLostfocus = true;
        }

        /// <summary>
        ///     Occurs when the mouse is moved over the checkedlistbox.
        ///     Sets the tooltip of the item under the pointer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMoved(object sender, MouseEventArgs e)
        {
            int index = _clb.IndexFromPoint(e.X, e.Y);
            if (index >= 0)
            {
                _tooltipControl.SetToolTip(_clb, ((ClbItem)_clb.Items[index]).Tooltip);
            }
        }

        /// <summary>
        ///     Close the dropdowncontrol when the user has selected a value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValueChanged(object sender, EventArgs e) => _edSvc?.CloseDropDown();

        /// <summary>
        ///     Internal class used for storing custom data in listviewitems
        /// </summary>
        internal class ClbItem
        {
            private readonly string _str;

            /// <summary>
            ///     Creates a new instance of the <c>clbItem</c>
            /// </summary>
            /// <param name="str">
            ///     The string to display in the <c>ToString</c> method.
            ///     It will contains the name of the flag
            /// </param>
            /// <param name="value">The integer value of the flag</param>
            /// <param name="tooltip">The tooltip to display in the <see cref="CheckedListBox" /></param>
            public ClbItem(string str, int value, string tooltip)
            {
                _str = str;
                Value = value;
                Tooltip = tooltip;
            }

            /// <summary>
            ///     Gets the int value for this item
            /// </summary>
            public int Value { get; }

            /// <summary>
            ///     Gets the tooltip for this item
            /// </summary>
            public string Tooltip { get; }

            /// <summary>
            ///     Gets the name of this item
            /// </summary>
            /// <returns>The name passed in the constructor</returns>
            public override string ToString() => _str;
        }
    }
}
