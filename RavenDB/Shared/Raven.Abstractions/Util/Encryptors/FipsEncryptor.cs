// -----------------------------------------------------------------------
//  <copyright file="FipsEncryptor.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------
namespace Raven.Abstractions.Util.Encryptors
{
	using System.Security.Cryptography;

	public class FipsEncryptor : IEncryptor
	{
		public FipsEncryptor()
		{
			Hash = new FipsHashEncryptor();
			Symmetrical = new FipsSymmetricalEncryptor();
		}

		public IHashEncryptor Hash { get; private set; }

		public ISymmetricalEncryptor Symmetrical { get; private set; }

		private class FipsHashEncryptor : HashEncryptorBase, IHashEncryptor
		{
			public int StorageHashSize
			{
				get
				{
					return 20;
				}
			}

			public byte[] ComputeForStorage(byte[] bytes)
			{
				return ComputeHash(SHA1.Create(), bytes);
			}

			public byte[] Compute(byte[] bytes)
			{
				return ComputeHash(SHA1.Create(), bytes);
			}
		}

		private class FipsSymmetricalEncryptor : ISymmetricalEncryptor
		{
			public byte[] Encrypt()
			{
				throw new System.NotImplementedException();
			}

			public byte[] Decrypt()
			{
				throw new System.NotImplementedException();
			}
		}
	}
}