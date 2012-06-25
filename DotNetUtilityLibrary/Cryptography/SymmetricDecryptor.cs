﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace DotNetUtilityLibrary.Cryptography
{
	public class SymmetricDecryptor : IDisposable
	{
		#region Properties

		private MemoryStream mEncryptedDataStream;
		private SymmetricAlgorithm mAlgorithm;
		private ICryptoTransform mTransform;

		private byte[] _mEncryptedDataByteArray;
		private byte[] mEncryptedDataByteArray
		{
			get
			{
				if (_mEncryptedDataByteArray == null)
				{
					_mEncryptedDataByteArray = mEncryptedDataStream.ToArray();
				}
				return _mEncryptedDataByteArray;
			}
		}

		#endregion Properties

		#region Constructors

		private SymmetricDecryptor(SymmetricAlgorithm algorithm,
			byte[] key,
			byte[] iv)
		{
			mAlgorithm = algorithm;
			algorithm.Key = key;
			algorithm.IV = iv;
			mTransform = mAlgorithm.CreateDecryptor(key, iv);
		}

		public SymmetricDecryptor(SymmetricAlgorithm algorithm,
			MemoryStream rawData,
			byte[] key,
			byte[] iv)
			: this(algorithm, key, iv)
		{
			mEncryptedDataStream = rawData;
		}

		public SymmetricDecryptor(SymmetricAlgorithm algorithm,
			byte[] rawData,
			byte[] key,
			byte[] iv)
			: this(algorithm, key, iv)
		{
			mEncryptedDataStream = new MemoryStream(rawData);
			_mEncryptedDataByteArray = rawData;
		}

		public SymmetricDecryptor(SymmetricAlgorithm algorithm,
			string rawData,
			byte[] key,
			byte[] iv)
			: this(algorithm, ConvertHelper.StringToBytes(rawData), key, iv)
		{
		}

		#endregion Constructors

		#region Exposed Methods

		public string GetDecryptedString()
		{
			return ConvertHelper.BytesToString(GetDecryptedBytes());
		}

		public byte[] GetDecryptedBytes()
		{
			return GetInternalDecryptedStream().ToArray();
		}

		public MemoryStream GetDecryptedStream()
		{
			MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(
				mEncryptedDataStream, mTransform, CryptoStreamMode.Read))
			{
				StreamHelper.CopyStream(cryptoStream, memoryStream);
				MemoryStream result = new MemoryStream();
				StreamHelper.CopyStream(memoryStream, result);
				return result;
			}
		}

		private MemoryStream GetInternalDecryptedStream()
		{
			MemoryStream memoryStream = new MemoryStream();
			using (CryptoStream cryptoStream = new CryptoStream(
				mEncryptedDataStream, mTransform, CryptoStreamMode.Read))
			{
				StreamHelper.CopyStream(cryptoStream, memoryStream);
				return memoryStream;
			}
		}

		#endregion Exposed Methods

		#region IDisposable

		void IDisposable.Dispose()
		{
			if (mEncryptedDataStream != null)
				mEncryptedDataStream.Dispose();
			if (mAlgorithm != null)
				mAlgorithm.Dispose();
			if (mTransform != null)
				mTransform.Dispose();
		}

		#endregion IDisposable
	}
}
