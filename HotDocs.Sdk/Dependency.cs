/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Xml.Serialization;

namespace HotDocs.Sdk
{
    /// <summary>
	/// A helper class to contain information of another file a template depends upon.
	/// </summary>
	public class Dependency : IEquatable<Dependency>, IComparable<Dependency>
	{
		private string _fileName;
		private string _hintPath;
		private DependencyType _dependencyType;
		private string _key;

		internal Dependency(string fileName, string hintPath, DependencyType dependencyType)
		{
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");

			_fileName = fileName;
			_hintPath = hintPath;
			_dependencyType = dependencyType;
			_key = null;
		}

		internal Dependency(string fileName, DependencyType dependencyType)
			: this(fileName, null, dependencyType)
		{
		}

		internal Dependency()
		{
			_fileName = string.Empty;
			_hintPath = string.Empty;
			_dependencyType = DependencyType.NoDependency;
			_key = null;
		}

		/// <summary>
		/// The file name of the dependency.
		/// </summary>
		[XmlAttribute("fileName")]
		public string FileName
		{
			get { return _fileName; }
			set 
			{
				if (_fileName != value)
				{
					_fileName = value;
					InvalidateKey();
				}
			}
		}

		/// <summary>
		/// A file system folder that suggests where in the local file system the dependency may be found. For internal use only. 
		/// </summary>
		[XmlAttribute("hintPath")]
		public string HintPath
		{
			get { return _hintPath; }
			set
			{
				if (_hintPath != value)
				{
					_hintPath = value;
					InvalidateKey();
				}
			}
		}

		/// <summary>
		/// Returns true if HintPath should be serialized to XML. For internal use only. 
		/// </summary>
		/// <returns>true if the value should be serialized.</returns>
		[XmlIgnore]
		public bool HintPathSpecified
		{
			get { return !string.IsNullOrEmpty(HintPath); }
		}

		/// <summary>
		/// The type of dependency.
		/// </summary>
		[XmlAttribute("type")]
		public DependencyType DependencyType
		{
			get { return _dependencyType; }
			set
			{
				if (_dependencyType != value)
				{
					_dependencyType = value;
					InvalidateKey();
				}
			}
		}

		private string Key
		{
			get
			{
				if (_key == null)
				{
					_key = (FileName != null ? FileName.ToLower() : string.Empty) + "|" + (HintPath != null ? HintPath.ToLower() : string.Empty) + "|" + DependencyType.ToString();
				}
				return _key;
			}
		}

		private void InvalidateKey()
		{
			#if DEBUG
				if (_key != null)
				{
					//throw new InvalidOperationException("Potential problem when changing a property affecting the key and the instance is already in a container.");
				}
			#endif
			_key = null;
		}

		/// <summary>
		/// Make a copy.
		/// </summary>
		/// <returns>A copy of the current object.</returns>
		public Dependency Copy()
		{
			return new Dependency(FileName, HintPath, DependencyType);
		}

		/// <summary>
		/// Check if a dependency refers to a template
		/// </summary>
		/// <param name="dt">The type of dependency.</param>
		/// <returns>true if the dependency refers to a template.</returns>
		public static bool IsTemplateDependency(DependencyType dt)
		{
			return dt == DependencyType.TemplateInsert || dt == DependencyType.Assemble;
		}

		/// <summary>
		/// Check if two dependncies are equal. File names are compared ignoring case.
		/// </summary>
		/// <param name="obj">The other dependency to compare with.</param>
		/// <returns>true if both dependecies are equal.</returns>
		public override bool Equals(object obj)
		{
			return (obj != null) && (obj is Dependency) && Equals((Dependency)obj);
		}

		/// <summary>
		/// Get the hash code for all the dependency's data.
		/// </summary>
		/// <returns>The hash code of the dependency's data.</returns>
		public override int GetHashCode()
		{
			return Key.GetHashCode();
		}

		/// <summary>
		/// Convert all the dependency's data to a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("FileName: {0}  HintPath: {1}  DependencyType: {2}", FileName, HintPath, DependencyType);
		}

		#region IEquatable<Dependency> Members

		/// <summary>
		/// Check if two dependncies are equal. File names are compared ignoring case.
		/// </summary>
		/// <param name="other">The other dependency to compare with.</param>
		/// <returns>true if both dependecies are equal.</returns>
		public bool Equals(Dependency other)
		{
			return CompareTo(other) == 0;
		}

		#endregion

		#region IComparable<Dependency> Members

		/// <summary>
		/// Compare the current dependncy to another dependency. File names are compared ignoring case.
		/// </summary>
		/// <param name="other">The other dependency to compare with.</param>
		/// <returns>Zero if both dependecies are equal. Less than zero if the current dependency is less than the other dpendency. Greater than zero if the current dependency is greater than the other dpendency</returns>
		public int CompareTo(Dependency other)
		{
			return string.CompareOrdinal(other.Key, Key);
		}

		#endregion
	}

}
