/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Eunge, Legal Name: Jian Liu, Email: eunge.liu@RapidWebDev.org

	The GNU Library General Public License (LGPL) used in RapidWebDev is 
	intended to guarantee your freedom to share and change free software - to 
	make sure the software is free for all its users.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Library General Public License (LGPL) for more details.

	You should have received a copy of the GNU Library General Public License (LGPL)
	along with this program.  
	If not, see http://www.rapidwebdev.org/Content/ByUniqueKey/OpenSourceLicense
 ****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using RapidWebDev.Common;
using RapidWebDev.ExtensionModel.Properties;

namespace RapidWebDev.ExtensionModel
{
    /// <summary>
    /// Hierarchy type property's metadata
    /// </summary>
    public partial class HierarchyFieldMetadata : IFieldMetadata
    {
		/// <summary>
		/// Gets or sets id.
		/// </summary>
		public Guid Id { get; set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        [XmlIgnore]
		public FieldType Type { get { return FieldType.Hierarchy; } }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlIgnore]
        public string Description { get; set; }

		/// <summary>
		/// True indicates the field is inherited from parent object metadata.
		/// </summary>
		[XmlIgnore]
		public bool Inherited { get; set; }

        /// <summary>
        /// Validate this field.
        /// </summary>
        /// <param name="value">Value of field</param>
        /// <exception cref="InvalidFieldValueException">This extension field's value is invalid</exception>
        public void Validate(IFieldValue value)
        {
            if (value == null)
            {
                if (this.IsRequired)
                    throw new InvalidFieldValueException(string.Format(Resources.FieldValueCannotBeNull, this.Name));
            }
            else
            {
                HierarchyNodeValueCollection selectedHierarchyNodes = value.Value as HierarchyNodeValueCollection;
                if ((selectedHierarchyNodes == null || selectedHierarchyNodes.Count == 0) && this.IsRequired)
                    throw new InvalidFieldValueException(string.Format(Resources.InvalidValueForSpecifiedField, this.Name));

                if (selectedHierarchyNodes != null && selectedHierarchyNodes.Count > 0)
                {
					if (selectedHierarchyNodes.Count > 1 && this.SelectionMode == SelectionModes.Single)
                        throw new InvalidFieldValueException(string.Format(Resources.OnlySupportSingleSelectForSpecifiedField, this.Name));
                }

                HashSet<string> hierarchyNodeValueHashSet = new HashSet<string>(GetAllDefinedHierarchyNodeValues(this.Node).Distinct());
                foreach (string hierarchyNodeValue in selectedHierarchyNodes)
                    if (!hierarchyNodeValueHashSet.Contains(hierarchyNodeValue))
                        throw new InvalidFieldValueException(string.Format(Resources.NoteValueNotDefinedInHierarchy, hierarchyNodeValue));
            }
        }

        /// <summary>
        /// Get default field value
        /// </summary>
        /// <returns>if no default value, return null</returns>
        public IFieldValue GetDefaultValue()
        {
            HierarchyNodeValueCollection selectedHierarchyNodes = new HierarchyNodeValueCollection();
            GetSelectedHierarchyNodes(selectedHierarchyNodes, this.Node);
            return selectedHierarchyNodes.FieldValue();
        }

		#region ICloneable Members

		object ICloneable.Clone()
		{
			return new HierarchyFieldMetadata
			{
				Id = this.Id,
				Priviledge = this.Priviledge,
				FieldGroup = this.FieldGroup,
				Description = this.Description,
				Inherited = this.Inherited,
				IsRequired = this.IsRequired,
				Name = this.Name,
				Node = this.Node,
				Ordinal = this.Ordinal,
				SelectionMode = this.SelectionMode
			};
		}

		#endregion

        private static void GetSelectedHierarchyNodes(HierarchyNodeValueCollection selectedHierarchyNodes, HierarchyNode[] hierarchyNodes)
        {
            if (hierarchyNodes == null) return;

            foreach (HierarchyNode hierarchyNode in hierarchyNodes)
            {
                if (hierarchyNode.Selected)
                    selectedHierarchyNodes.Add(hierarchyNode.Value);

                GetSelectedHierarchyNodes(selectedHierarchyNodes, hierarchyNode.Node);
            }
        }

        private static List<string> GetAllDefinedHierarchyNodeValues(HierarchyNode[] hierarchyNodes)
        {
            if (hierarchyNodes == null || hierarchyNodes.Length == 0)
                return new List<string>();

            List<string> hierarchyNodeValues = hierarchyNodes.Select(h => h.Value).ToList();
            foreach (HierarchyNode hierarchyNode in hierarchyNodes)
                hierarchyNodeValues.AddRange(GetAllDefinedHierarchyNodeValues(hierarchyNode.Node));

            return hierarchyNodeValues;
        }
	}
}
