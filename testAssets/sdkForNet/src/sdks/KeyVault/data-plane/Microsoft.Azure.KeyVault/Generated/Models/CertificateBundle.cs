// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.KeyVault.Models
{
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A certificate bundle consists of a certificate (X509) plus its
    /// attributes.
    /// </summary>
    public partial class CertificateBundle
    {
        /// <summary>
        /// Initializes a new instance of the CertificateBundle class.
        /// </summary>
        public CertificateBundle()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CertificateBundle class.
        /// </summary>
        /// <param name="id">The certificate id.</param>
        /// <param name="kid">The key id.</param>
        /// <param name="sid">The secret id.</param>
        /// <param name="x509Thumbprint">Thumbprint of the certificate.</param>
        /// <param name="policy">The management policy.</param>
        /// <param name="cer">CER contents of x509 certificate.</param>
        /// <param name="contentType">The content type of the secret.</param>
        /// <param name="attributes">The certificate attributes.</param>
        /// <param name="tags">Application specific metadata in the form of
        /// key-value pairs</param>
        public CertificateBundle(string id = default(string), string kid = default(string), string sid = default(string), byte[] x509Thumbprint = default(byte[]), CertificatePolicy policy = default(CertificatePolicy), byte[] cer = default(byte[]), string contentType = default(string), CertificateAttributes attributes = default(CertificateAttributes), IDictionary<string, string> tags = default(IDictionary<string, string>))
        {
            Id = id;
            Kid = kid;
            Sid = sid;
            X509Thumbprint = x509Thumbprint;
            Policy = policy;
            Cer = cer;
            ContentType = contentType;
            Attributes = attributes;
            Tags = tags;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets the certificate id.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        /// <summary>
        /// Gets the key id.
        /// </summary>
        [JsonProperty(PropertyName = "kid")]
        public string Kid { get; private set; }

        /// <summary>
        /// Gets the secret id.
        /// </summary>
        [JsonProperty(PropertyName = "sid")]
        public string Sid { get; private set; }

        /// <summary>
        /// Gets thumbprint of the certificate.
        /// </summary>
        [JsonConverter(typeof(Base64UrlJsonConverter))]
        [JsonProperty(PropertyName = "x5t")]
        public byte[] X509Thumbprint { get; private set; }

        /// <summary>
        /// Gets the management policy.
        /// </summary>
        [JsonProperty(PropertyName = "policy")]
        public CertificatePolicy Policy { get; private set; }

        /// <summary>
        /// Gets or sets CER contents of x509 certificate.
        /// </summary>
        [JsonProperty(PropertyName = "cer")]
        public byte[] Cer { get; set; }

        /// <summary>
        /// Gets or sets the content type of the secret.
        /// </summary>
        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the certificate attributes.
        /// </summary>
        [JsonProperty(PropertyName = "attributes")]
        public CertificateAttributes Attributes { get; set; }

        /// <summary>
        /// Gets or sets application specific metadata in the form of key-value
        /// pairs
        /// </summary>
        [JsonProperty(PropertyName = "tags")]
        public IDictionary<string, string> Tags { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Policy != null)
            {
                Policy.Validate();
            }
        }
    }
}
