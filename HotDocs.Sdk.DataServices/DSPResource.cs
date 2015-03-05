//*********************************************************
//
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the Microsoft Public License.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

namespace HotDocs.Sdk.DataServices
{
	using System;
	using System.Collections.Generic;
	using System.Data.Services.Providers;
	using System.Threading;

	/// <summary>Class which represents a single resource instance.</summary>
	/// <remarks>Uses a property bag to store properties of the resource.</remarks>
	public class DSPResource
	{
		/// <summary>The bag of properties. Dictionary where key is the property name and value is the value of the property.</summary>
		private Dictionary<string, object> properties;

		/// <summary>The resource type of the resource.</summary>
		private ResourceType resourceType;

		private ReaderWriterLockSlim readerWriterLock;

		/// <summary>The resource type of the resource.</summary>
		public ResourceType ResourceType { get { return this.resourceType; } }

		/// <summary>Constructor, creates a new resource (all properties are empty).</summary>
		/// <param name="resourceType">The type of the resource to create.</param>
		/// <param name="readerWriterLock">The reader writer lock.</param>
		public DSPResource(ResourceType resourceType, ReaderWriterLockSlim readerWriterLock)
		{
			this.properties = new Dictionary<string, object>();
			this.resourceType = resourceType;
			this.readerWriterLock = readerWriterLock;
		}

		/// <summary>Returns a value of the specified property.</summary> 
		/// <param name="propertyName">The name of the property to return.</param>
		/// <returns>The value of the specified property or null if there's no such property defined yet.</returns>
		public object GetValue(string propertyName)
		{
			object value;
			readerWriterLock.EnterReadLock();
			try
			{
				if (!this.properties.TryGetValue(propertyName, out value))
				{
					value = null;
				}
			}
			finally
			{
				readerWriterLock.ExitReadLock();
			}
			return value;
		}

		/// <summary>Sets a value of the specified property.</summary>
		/// <param name="propertyName">The name of the property to set.</param>
		/// <param name="value">The value to set the property to.</param>
		/// <remarks>Note that this method will define the property if it doesn't exist yet. If it does exist, it will overwrite its value.</remarks>
		public void SetValue(string propertyName, object value)
		{
			readerWriterLock.EnterWriteLock();
			try
			{
				this.properties[propertyName] = value;
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
		}
	}
}
