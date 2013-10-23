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
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;

	/// <summary>Metadata definition for the DSP. This also implements the <see cref="IDataServiceMetadataProvider"/>.</summary>
	public class DSPMetadata : IDataServiceMetadataProvider
	{
		/// <summary>List of resource sets. Dictionary where key is the name of the resource set and value is the resource set itself.</summary>
		/// <remarks>Note that we store this such that we can quickly lookup a resource set based on its name.</remarks>
		private Dictionary<string, ResourceSet> resourceSets;

		/// <summary>List of resource types. Dictionary where key is the full name of the resource type and value is the resource type itself.</summary>
		/// <remarks>Note that we store this such that we can quickly lookup a resource type based on its name.</remarks>
		private Dictionary<string, ResourceType> resourceTypes;

		private Dictionary<string, ServiceOperation> serviceOperations;

		/// <summary>Name of the container to report.</summary>
		private string containerName;

		/// <summary>Namespace name.</summary>
		private string namespaceName;

		private ReaderWriterLockSlim readerWriterLock;

		/// <summary>Creates new empty metadata definition.</summary>
		/// <param name="containerName">Name of the container to report.</param>
		/// <param name="namespaceName">Namespace name.</param>
		/// <param name="readerWriterLock">Reader writer lock.</param>
		public DSPMetadata(string containerName, string namespaceName, ReaderWriterLockSlim readerWriterLock)
		{
			this.resourceSets = new Dictionary<string, ResourceSet>();
			this.resourceTypes = new Dictionary<string, ResourceType>();
			this.serviceOperations = new Dictionary<string, ServiceOperation>();
			this.containerName = containerName;
			this.namespaceName = namespaceName;
			this.readerWriterLock = readerWriterLock;
		}

		/// <summary>Adds a new entity type (without any properties).</summary>
		/// <param name="name">The name of the type.</param>
		/// <returns>The newly created resource type.</returns>
		public ResourceType AddEntityType(string name)
		{
			ResourceType resourceType = new ResourceType(typeof(DSPResource), ResourceTypeKind.EntityType, null, this.namespaceName, name, false);
			resourceType.CanReflectOnInstanceType = false;
			resourceType.CustomState = new ResourceTypeAnnotation();
			readerWriterLock.EnterWriteLock();
			try
			{
				this.resourceTypes.Add(resourceType.FullName, resourceType);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
			return resourceType;
		}

		/// <summary>Adds a new complex type (without any properties).</summary>
		/// <param name="name">The name of the type.</param>
		/// <returns>The newly created resource type.</returns>
		public ResourceType AddComplexType(string name)
		{
			ResourceType resourceType = new ResourceType(typeof(DSPResource), ResourceTypeKind.ComplexType, null, this.namespaceName, name, false);
			resourceType.CanReflectOnInstanceType = false;
			readerWriterLock.EnterWriteLock();
			try
			{
				this.resourceTypes.Add(resourceType.FullName, resourceType);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
			return resourceType;
		}

		/// <summary>
		/// Removes the specified resource type.
		/// </summary>
		/// <param name="name">The name of the resource type to remove.</param>
		public void RemoveResourceType(string name)
		{
			readerWriterLock.EnterWriteLock();
			try
			{
				this.resourceTypes.Remove(name);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
		}

		/// <summary>Adds a key property to the specified <paramref name="resourceType"/>.</summary>
		/// <param name="resourceType">The resource type to add the property to.</param>
		/// <param name="name">The name of the property to add.</param>
		/// <param name="propertyType">The CLR type of the property to add. This can be only a primitive type.</param>
		public void AddKeyProperty(ResourceType resourceType, string name, Type propertyType)
		{
			this.AddPrimitiveProperty(resourceType, name, propertyType, true);
		}

		/// <summary>Adds a primitive property to the specified <paramref name="resourceType"/>.</summary>
		/// <param name="resourceType">The resource type to add the property to.</param>
		/// <param name="name">The name of the property to add.</param>
		/// <param name="propertyType">The CLR type of the property to add. This can be only a primitive type.</param>
		public void AddPrimitiveProperty(ResourceType resourceType, string name, Type propertyType)
		{
			this.AddPrimitiveProperty(resourceType, name, propertyType, false);
		}

		/// <summary>Adds a key property to the specified <paramref name="resourceType"/>.</summary>
		/// <param name="resourceType">The resource type to add the property to.</param>
		/// <param name="name">The name of the property to add.</param>
		/// <param name="propertyType">The CLR type of the property to add. This can be only a primitive type.</param>
		/// <param name="isKey">true if the property should be a key property.</param>
		private void AddPrimitiveProperty(ResourceType resourceType, string name, Type propertyType, bool isKey)
		{
			ResourceType type = ResourceType.GetPrimitiveResourceType(propertyType);
			ResourcePropertyKind kind = ResourcePropertyKind.Primitive;
			if (isKey)
			{
				kind |= ResourcePropertyKind.Key;
			}

			ResourceProperty property = new ResourceProperty(name, kind, type);
			property.CanReflectOnInstanceTypeProperty = false;
			readerWriterLock.EnterWriteLock();
			try
			{
				resourceType.AddProperty(property);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
		}

		/// <summary>Adds a complex property to the specified <paramref name="resourceType"/>.</summary>
		/// <param name="resourceType">The resource type to add the property to.</param>
		/// <param name="name">The name of the property to add.</param>
		/// <param name="complexType">Complex type to use for the property.</param>
		public void AddComplexProperty(ResourceType resourceType, string name, ResourceType complexType)
		{
			if (complexType.ResourceTypeKind != ResourceTypeKind.ComplexType)
			{
				throw new ArgumentException("The specified type for the complex property is not a complex type.");
			}

			ResourceProperty property = new ResourceProperty(name, ResourcePropertyKind.ComplexType, complexType);
			property.CanReflectOnInstanceTypeProperty = false;
			readerWriterLock.EnterWriteLock();
			try
			{
				resourceType.AddProperty(property);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
		}

		///// <summary>Adds a resource reference property to the specified <paramref name="resourceType"/>.</summary>
		///// <param name="resourceType">The resource type to add the property to.</param>
		///// <param name="name">The name of the property to add.</param>
		///// <param name="targetResourceSet">The resource set the resource reference property points to.</param>
		///// <remarks>This creates a property pointing to a single resource in the target resource set.</remarks>
		//public void AddResourceReferenceProperty(ResourceType resourceType, string name, ResourceSet targetResourceSet)
		//{
		//    AddReferenceProperty(resourceType, name, targetResourceSet, false);
		//}

		///// <summary>Adds a resource set reference property to the specified <paramref name="resourceType"/>.</summary>
		///// <param name="resourceType">The resource type to add the property to.</param>
		///// <param name="name">The name of the property to add.</param>
		///// <param name="targetResourceSet">The resource set the resource reference property points to.</param>
		///// <remarks>This creates a property pointing to multiple resources in the target resource set.</remarks>
		//public void AddResourceSetReferenceProperty(ResourceType resourceType, string name, ResourceSet targetResourceSet)
		//{
		//    AddReferenceProperty(resourceType, name, targetResourceSet, true);
		//}

		///// <summary>Helper method to add a reference property.</summary>
		///// <param name="resourceType">The resource type to add the property to.</param>
		///// <param name="name">The name of the property to add.</param>
		///// <param name="targetResourceSet">The resource set the resource reference property points to.</param>
		///// <param name="resourceSetReference">true if the property should be a resource set reference, false if it should be resource reference.</param>
		//private void AddReferenceProperty(ResourceType resourceType, string name, ResourceSet targetResourceSet, bool resourceSetReference)
		//{
		//    ResourceProperty property = new ResourceProperty(
		//        name,
		//        resourceSetReference ? ResourcePropertyKind.ResourceSetReference : ResourcePropertyKind.ResourceReference,
		//        targetResourceSet.ResourceType);
		//    property.CanReflectOnInstanceTypeProperty = false;
		//    resourceType.AddProperty(property);

		//    // We don't support type inheritance so the property can only point to the base resource type of the target resource set
		//    // We also don't support MEST, that is having two resource sets with the same resource type, so we can determine
		//    //   the resource set from the resource type. That also means that the property can never point to different resource sets
		//    //   so we can precreate the ResourceAssociationSet for this property right here as we have all the information.
		//    property.CustomState = new ResourcePropertyAnnotation()
		//    {
		//        ResourceAssociationSet = new ResourceAssociationSet(
		//            resourceType.Name + "_" + name + "_" + targetResourceSet.Name,
		//            new ResourceAssociationSetEnd(resourceType.GetAnnotation().ResourceSet, resourceType, property),
		//            new ResourceAssociationSetEnd(targetResourceSet, targetResourceSet.ResourceType, null))
		//    };
		//}

		/// <summary>Adds a resource set to the metadata definition.</summary>
		/// <param name="name">The name of the resource set to add.</param>
		/// <param name="entityType">The type of entities in the resource set.</param>
		/// <returns>The newly created resource set.</returns>
		public ResourceSet AddResourceSet(string name, ResourceType entityType)
		{
			if (entityType.ResourceTypeKind != ResourceTypeKind.EntityType)
			{
				throw new ArgumentException("The resource type specified as the base type of a resource set is not an entity type.");
			}

			ResourceSet resourceSet = new ResourceSet(name, entityType);
			entityType.GetAnnotation().ResourceSet = resourceSet;
			readerWriterLock.EnterWriteLock();
			try
			{
				this.resourceSets.Add(name, resourceSet);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
			return resourceSet;
		}

		/// <summary>
		/// Removes the specified resource set.
		/// </summary>
		/// <param name="name">The name of the resource set to remove.</param>
		public void RemoveResourceSet(string name)
		{
			readerWriterLock.EnterWriteLock();
			try
			{
				this.resourceSets.Remove(name);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
		}

		/// <summary>
		/// Adds a new service operation.
		/// </summary>
		/// <param name="name">The name of the service operation.</param>
		/// <param name="resultKind">The kind of service operation.</param>
		/// <param name="resultType">The type of service operation.</param>
		/// <param name="resultSet">Information about the resource.</param>
		/// <param name="method">The method of the service operation.</param>
		/// <param name="parameters">Parameters for the service operation.</param>
		/// <returns>The newly added service operation.</returns>
		public ServiceOperation AddServiceOperation(string name, ServiceOperationResultKind resultKind, ResourceType resultType, ResourceSet resultSet,
			string method, IEnumerable<ServiceOperationParameter> parameters)
		{
			ServiceOperation serviceOperation = new ServiceOperation(name, resultKind, resultType, resultSet, method, parameters);
			serviceOperation.SetReadOnly();
			readerWriterLock.EnterWriteLock();
			try
			{
				this.serviceOperations.Add(name, serviceOperation);
			}
			finally
			{
				readerWriterLock.ExitWriteLock();
			}
			return serviceOperation;
		}

		///// <summary>Marks the metadata as read-only.</summary>
		//internal void SetReadOnly()
		//{
		//    foreach (var type in this.resourceTypes.Values)
		//    {
		//        type.SetReadOnly();
		//    }

		//    foreach (var set in this.resourceSets.Values)
		//    {
		//        set.SetReadOnly();
		//    }
		//}

		#region IDataServiceMetadataProvider Members

		/// <summary>Returns the name of the container. This value is used for example when a proxy is generated by VS through Add Service Reference.
		/// The main context class generated will have the ContainerName.</summary>
		public string ContainerName
		{
			get { return this.containerName; }
		}

		/// <summary>The namespace name for the container. This is used in the $metadata response.</summary>
		public string ContainerNamespace
		{
			get { return this.namespaceName; }
		}

		/// <summary>Returns list of all types derived (directly or indirectly) from the specified <c>resourceType</c>.</summary>
		/// <param name="resourceType">The resource type to determine derived types for.</param>
		/// <returns>List of derived types.</returns>
		/// <remarks>Note that this method will get called even if the HasDerivedTypes returns false.
		/// The implementation should be reasonably fast as it can be called to process a query request. (Aside from being called for the $metadata processing).</remarks>
		public System.Collections.Generic.IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
		{
			// We don't support type inheritance yet
			return new ResourceType[0];
		}

		/// <summary>
		/// Gets the ResourceAssociationSet instance when given the source association end.
		/// </summary>
		/// <param name="resourceSet">Resource set of the source association end.</param>
		/// <param name="resourceType">Resource type of the source association end.</param>
		/// <param name="resourceProperty">Resource property of the source association end.</param>
		/// <returns>ResourceAssociationSet instance.</returns>
		/// <remarks>This method returns a ResourceAssociationSet representing a reference which is specified
		/// by the <paramref name="resourceProperty"/> on the <paramref name="resourceType"/> for instances in the <paramref name="resourceSet"/>.</remarks>
		public ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
		{
			throw new NotImplementedException("Resource association sets are not yet supported.");

			//// We have the resource association set precreated on the property annotation, so no need to compute anything in here
			//ResourceAssociationSet resourceAssociationSet = resourceProperty.GetAnnotation().ResourceAssociationSet;

			//// Just few verification to show what is expected of the returned resource association set.
			//Debug.Assert(resourceAssociationSet.End1.ResourceSet == resourceSet, "The precreated resource association set doesn't match the specified resource set.");
			//Debug.Assert(resourceAssociationSet.End1.ResourceType == resourceType, "The precreated resource association set doesn't match the specified resource type.");
			//Debug.Assert(resourceAssociationSet.End1.ResourceProperty == resourceProperty, "The precreated resource association set doesn't match its resource property.");

			//return resourceAssociationSet;
		}

		/// <summary>Returns true if the specified type has some derived types.</summary>
		/// <param name="resourceType">The resource type to inspect.</param>
		/// <returns>true if the specified type has derived types.</returns>
		/// <remarks>The implementation should be fast as it will get called during normal request processing.</remarks>
		public bool HasDerivedTypes(ResourceType resourceType)
		{
			return false;
		}

		/// <summary>Returns all resource sets.</summary>
		/// <remarks>The implementation doesn't need to be fast as this will only be called for the $metadata and service document requests.</remarks>
		public System.Collections.Generic.IEnumerable<ResourceSet> ResourceSets
		{
			get
			{
				IEnumerable<ResourceSet> resourceSets;
				readerWriterLock.EnterReadLock();
				try
				{
					resourceSets = this.resourceSets.Values;
				}
				finally
				{
					readerWriterLock.ExitReadLock();
				}
				return resourceSets;
			}
		}

		/// <summary>Returns all service operations.</summary>
		/// <remarks>The implementation doesn't need to be fast as this will only be called for the $metadata requests.</remarks>
		public System.Collections.Generic.IEnumerable<ServiceOperation> ServiceOperations
		{
			get
			{
				IEnumerable<ServiceOperation> serviceOperations;
				readerWriterLock.EnterReadLock();
				try
				{
					serviceOperations = this.serviceOperations.Values;
				}
				finally
				{
					readerWriterLock.ExitReadLock();
				}
				return serviceOperations;
			}
		}

		/// <summary>Returnes a resource set specified by its name.</summary>
		/// <param name="name">The name of the resource set find.</param>
		/// <param name="resourceSet">The resource set instance found.</param>
		/// <returns>true if the resource set was found or false otherwise.</returns>
		/// <remarks>The implementation of this method should be very fast as it will get called for almost every request. It should also be fast
		/// for non-existing resource sets to avoid possible DoS attacks on the service.</remarks>
		public bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
		{
			bool found;
			readerWriterLock.EnterReadLock();
			try
			{
				found = this.resourceSets.TryGetValue(name, out resourceSet); ;
			}
			finally
			{
				readerWriterLock.ExitReadLock();
			}
			return found;
		}

		/// <summary>Returnes a resource type specified by its name.</summary>
		/// <param name="name">The full name of the resource type (including its namespace).</param>
		/// <param name="resourceType">The resource type instance found.</param>
		/// <returns>true if the resource type was found or false otherwise.</returns>
		/// <remarks>The implementation of this method should be very fast as it will get called for many requests. It should also be fast
		/// for non-existing resource types to avoid possible DoS attacks on the service.</remarks>
		public bool TryResolveResourceType(string name, out ResourceType resourceType)
		{
			bool found;
			readerWriterLock.EnterReadLock();
			try
			{
				found = this.resourceTypes.TryGetValue(name, out resourceType);
			}
			finally
			{
				readerWriterLock.ExitReadLock();
			}
			return found;
		}

		/// <summary>Returns a service operation specified by its name.</summary>
		/// <param name="name">The name of the service operation to find.</param>
		/// <param name="serviceOperation">The service operation instance found.</param>
		/// <returns>true if the service operation was found or false otherwise.</returns>
		/// <remarks>The implementation of this method should be very fast as it will get called for many requests. It should also be fast
		/// for non-existing service operations to avoid possible DoS attacks on the service.</remarks>
		public bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
		{
			bool found;
			readerWriterLock.EnterReadLock();
			try
			{
				found = this.serviceOperations.TryGetValue(name, out serviceOperation);
			}
			finally
			{
				readerWriterLock.ExitReadLock();
			}
			return found;
		}

		/// <summary>Returns all resource types.</summary>
		/// <remarks>The implementation doesn't need to be fast as this will only be called for the $metadata requests.</remarks>
		public System.Collections.Generic.IEnumerable<ResourceType> Types
		{
			get
			{
				IEnumerable<ResourceType> resourceTypes;
				readerWriterLock.EnterReadLock();
				try
				{
					resourceTypes = this.resourceTypes.Values;
				}
				finally
				{
					readerWriterLock.ExitReadLock();
				}
				return resourceTypes;
			}
		}

		#endregion
	}
}