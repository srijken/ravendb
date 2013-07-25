namespace Raven.Abstractions.Util.Encryptors
{
	using System.Security.Cryptography;

	internal abstract class HashEncryptorBase
	{
		public byte[] ComputeHash(HashAlgorithm algorithm, byte[] bytes)
		{
			using (algorithm)
			{
				return algorithm.ComputeHash(bytes);
			}
		}
	}
}