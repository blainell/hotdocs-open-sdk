/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System.Linq;
using System.Text;
using System.Runtime.Serialization;

[assembly: ContractNamespace("http://hotdocs.com/contracts/", ClrNamespace = "HotDocs.Sdk.Server.Contracts")]

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
	/// This class is used for sending and receiving files, such as answers, templates, and documents.
	/// </summary>
	[DataContract]
	public class BinaryObject
	{
		/// <summary>
		/// The name of the file.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string FileName;
		/// <summary>
		/// The encoding used for the data.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string DataEncoding;
		/// <summary>
		/// The format of the file.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public OutputFormat Format;
		/// <summary>
		/// The bytes that make up the file.
		/// </summary>
		[DataMember(IsRequired = true)]
		public byte[] Data;
	}
}
