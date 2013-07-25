namespace Raven.Abstractions.Util.Encryptors
{
	public interface IEncryptor
	{
		IHashEncryptor Hash { get; }

		ISymmetricalEncryptor Symmetrical { get; }
	}

	public interface IHashEncryptor
	{
		int StorageHashSize { get; }

		byte[] ComputeForStorage(byte[] bytes);

		byte[] Compute(byte[] bytes);
	}

	public interface ISymmetricalEncryptor
	{
		byte[] Encrypt();

		byte[] Decrypt();
	}
}