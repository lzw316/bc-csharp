namespace Org.BouncyCastle.Bcpg
{
    /// <summary>Basic PGP signature subpacket tag types.</summary>
    public enum SignatureSubpacketTag
    {
        CreationTime = 2,						// signature creation time
        ExpireTime = 3,							// signature expiration time
        Exportable = 4,							// exportable certification
        TrustSig = 5,							// trust signature
        RegExp = 6,								// regular expression
        Revocable = 7,							// revocable
        KeyExpireTime = 9,						// key expiration time
        Placeholder = 10,						// placeholder for backward compatibility
        PreferredSymmetricAlgorithms = 11,		// preferred symmetric algorithms
        RevocationKey = 12,						// revocation key
        IssuerKeyId = 16,						// issuer key ID
        NotationData = 20,						// notation data
        PreferredHashAlgorithms = 21,			// preferred hash algorithms
        PreferredCompressionAlgorithms = 22,	// preferred compression algorithms
        KeyServerPreferences = 23,				// key server preferences
        PreferredKeyServer = 24,				// preferred key server
        PrimaryUserId = 25,						// primary user id
        PolicyUrl = 26,							// policy URL
        KeyFlags = 27,							// key flags
        SignerUserId = 28,						// signer's user id
        RevocationReason = 29,                  // reason for revocation
        Features = 30,                          // features
        SignatureTarget = 31,                   // signature target
        EmbeddedSignature = 32,					// embedded signature
        IssuerFingerprint = 33,                 // issuer key fingerprint
        //PreferredAeadAlgorithms = 34,         // RESERVED since crypto-refresh-05
        IntendedRecipientFingerprint = 35,      // intended recipient fingerprint
        AttestedCertifications = 37,            // attested certifications (RESERVED)
        KeyBlock = 38,                          // Key Block (RESERVED)
        PreferredAeadAlgorithms = 39,           // preferred AEAD algorithms
    }
}
