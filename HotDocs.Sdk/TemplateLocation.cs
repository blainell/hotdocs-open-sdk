/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML comments where missing.
//TODO: Add method parameter validation.
//TODO: Add appropriate unit tests.

using System;
using System.Collections.Concurrent;

namespace HotDocs.Sdk
{
	public abstract class TemplateLocation
	{
		/// <summary>
		/// Return a copy of this object.
		/// </summary>
		/// <returns></returns>
		//TODO: Consider using ICloneable.
		public abstract TemplateLocation Duplicate();
		public abstract System.IO.Stream GetFile(string fileName);
		/// <summary>
		/// Return the directory for a template. In a system where all template file names are unique, one directory
		/// may be used for all templates. Generally, however, each main template (e.g. a package main template) should
		/// reside in a separate directory. Each dependent template (e.g. a package template that is not the
		/// main template) should reside in the same directory as its main template. Hence, typically, one ITemplateLocation
		/// object would exist per main template.
		/// 
		/// An ITemplateLocation object may be implemented by the host application, and may represent templates
		/// stored on the file system, in a DMS, in some other database, etc. However, when an actual template file path
		/// is needed, the ITemplateLocation implementation is expected to provide a full path to the template, and
		/// ensure that the template and all of its dependencies exist. The directory where the template exists should
		/// survive the serialization and deserialization of this object.
		/// </summary>
		/// <returns>The directory for the template.</returns>
		public abstract string GetTemplateDirectory();
		//TODO: Fix obsolete XML comment.
		/// <summary>
		/// Return a string that can be used to initialize a new, uninitialized TemplateLocation
		/// object of the same type as this one. Deserialize the content with DeserializeContent.
		/// </summary>
		/// <returns></returns>
		protected abstract string SerializeContent();
		/// <summary>
		/// Initialize a TemplateLocation object from a string created by SerializeContent.
		/// </summary>
		/// <param name="templateLocator"></param>
		/// <returns></returns>
		protected abstract void DeserializeContent(string content);
		/// <summary>
		/// Return an unencrypted locator object.
		/// </summary>
		/// <returns></returns>
		public string CreateLocator()
		{
			string stamp = GetType().ToString();
			return Util.EncryptString(stamp + "|" + SerializeContent());
		}
		/// <summary>
		/// Get an updated file name for a template. Return true if the file name needed updating.
		/// If this method returns true, then fileName contains the updated file name. This method
		/// should be overridden for file storage systems where the template is stored in a database
		/// such that a file name is created on demand.
		/// </summary>
		/// <param name="template"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public virtual bool GetUpdatedFileName(Template template, out string fileName)
		{
			fileName = "";
			return false;
		}
		//TODO: Remove this.
		/// <summary>
		/// A key identifying the template -- such as a DMS file key.
		/// </summary>
		public string Key
		{
			get {return _key;}
			protected set {_key = value;}
		}
		public static TemplateLocation Locate(string encodedLocator)
		{
			string locator = Util.DecryptString(encodedLocator);
			int stampLen = locator.IndexOf('|');
			if (stampLen == -1)
				return null;
			string stamp = locator.Substring(0, stampLen);
			string content = locator.Substring(stampLen + 1, locator.Length - (stampLen + 1));

			foreach (Type type in _registeredTypes)
			{
				if (stamp == type.ToString())
				{
					object obj = System.Runtime.Serialization.FormatterServices.GetSafeUninitializedObject(type);
					TemplateLocation templateLocation = obj as TemplateLocation;
					if (templateLocation == null)
						throw new Exception("Invalid template location.");

					templateLocation.DeserializeContent(content);
					return templateLocation;
				}
			}
			throw new Exception("The type " + stamp + " is not registered as a TemplateLocation. Call TemplateLocation.RegisterLocation at application start-up.");
		}

		/// <summary>
		/// Call this method to register a type derived from TemplateLocation. All TemplateLocation derivatives must
		/// be registered before use in order for Template.Locate and TemplateLocation.Locate reconstitute
		/// template location information. This method should only be called at application start-up.
		/// </summary>
		/// <param name="type"></param>
		public static void RegisterLocation(Type type)
		{
			//Validate the type.
			Type baseType = type.BaseType;
			while (baseType != null && baseType != typeof(TemplateLocation))
				baseType = baseType.BaseType;
			if (baseType != typeof(TemplateLocation))
				throw new Exception("The registered location must be of type TemplateLocation.");

			_registeredTypes.Enqueue(type);
		}

		private static ConcurrentQueue<Type> _registeredTypes = new ConcurrentQueue<Type>();
		private string _key = "";//TODO: Remove this.
	}
}
