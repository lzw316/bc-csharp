using System;
using System.Collections.Generic;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Bsi;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Eac;
using Org.BouncyCastle.Asn1.EdEC;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Rosstandart;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Security
{
    /// <summary>
    ///  Signer Utility class contains methods that can not be specifically grouped into other classes.
    /// </summary>
    public static class SignerUtilities
    {
        private static readonly Dictionary<string, string> AlgorithmMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<DerObjectIdentifier, string> AlgorithmOidMap =
            new Dictionary<DerObjectIdentifier, string>();
        private static readonly HashSet<string> NoRandom = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, DerObjectIdentifier> Oids =
            new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);

        static SignerUtilities()
        {
            AlgorithmMap["MD2WITHRSA"] = "MD2withRSA";
            AlgorithmMap["MD2WITHRSAENCRYPTION"] = "MD2withRSA";
            AlgorithmOidMap[PkcsObjectIdentifiers.MD2WithRsaEncryption] = "MD2withRSA";

            AlgorithmMap["MD4WITHRSA"] = "MD4withRSA";
            AlgorithmMap["MD4WITHRSAENCRYPTION"] = "MD4withRSA";
            AlgorithmOidMap[PkcsObjectIdentifiers.MD4WithRsaEncryption] = "MD4withRSA";
            AlgorithmOidMap[OiwObjectIdentifiers.MD4WithRsa] = "MD4withRSA";
            AlgorithmOidMap[OiwObjectIdentifiers.MD4WithRsaEncryption] = "MD4withRSA";

			AlgorithmMap["MD5WITHRSA"] = "MD5withRSA";
            AlgorithmMap["MD5WITHRSAENCRYPTION"] = "MD5withRSA";
            AlgorithmOidMap[PkcsObjectIdentifiers.MD5WithRsaEncryption] = "MD5withRSA";
            AlgorithmOidMap[OiwObjectIdentifiers.MD5WithRsa] = "MD5withRSA";

            AlgorithmMap["SHA1WITHRSA"] = "SHA-1withRSA";
            AlgorithmMap["SHA-1WITHRSA"] = "SHA-1withRSA";
            AlgorithmMap["SHA1WITHRSAENCRYPTION"] = "SHA-1withRSA";
            AlgorithmMap["SHA-1WITHRSAENCRYPTION"] = "SHA-1withRSA";
            AlgorithmOidMap[PkcsObjectIdentifiers.Sha1WithRsaEncryption] = "SHA-1withRSA";
            AlgorithmOidMap[OiwObjectIdentifiers.Sha1WithRsa] = "SHA-1withRSA";

            AlgorithmMap["SHA224WITHRSA"] = "SHA-224withRSA";
            AlgorithmMap["SHA-224WITHRSA"] = "SHA-224withRSA";
            AlgorithmMap["SHA224WITHRSAENCRYPTION"] = "SHA-224withRSA";
            AlgorithmMap["SHA-224WITHRSAENCRYPTION"] = "SHA-224withRSA";
            AlgorithmOidMap[PkcsObjectIdentifiers.Sha224WithRsaEncryption] = "SHA-224withRSA";

            AlgorithmMap["SHA256WITHRSA"] = "SHA-256withRSA";
            AlgorithmMap["SHA-256WITHRSA"] = "SHA-256withRSA";
            AlgorithmMap["SHA256WITHRSAENCRYPTION"] = "SHA-256withRSA";
            AlgorithmMap["SHA-256WITHRSAENCRYPTION"] = "SHA-256withRSA";
            AlgorithmOidMap[PkcsObjectIdentifiers.Sha256WithRsaEncryption] = "SHA-256withRSA";

            AlgorithmMap["SHA384WITHRSA"] = "SHA-384withRSA";
            AlgorithmMap["SHA-384WITHRSA"] = "SHA-384withRSA";
            AlgorithmMap["SHA384WITHRSAENCRYPTION"] = "SHA-384withRSA";
            AlgorithmMap["SHA-384WITHRSAENCRYPTION"] = "SHA-384withRSA";
            AlgorithmOidMap[PkcsObjectIdentifiers.Sha384WithRsaEncryption] = "SHA-384withRSA";

            AlgorithmMap["SHA512WITHRSA"] = "SHA-512withRSA";
            AlgorithmMap["SHA-512WITHRSA"] = "SHA-512withRSA";
            AlgorithmMap["SHA512WITHRSAENCRYPTION"] = "SHA-512withRSA";
            AlgorithmMap["SHA-512WITHRSAENCRYPTION"] = "SHA-512withRSA";
            AlgorithmOidMap[PkcsObjectIdentifiers.Sha512WithRsaEncryption] = "SHA-512withRSA";

            AlgorithmMap["SHA512(224)WITHRSA"] = "SHA-512(224)withRSA";
            AlgorithmMap["SHA-512(224)WITHRSA"] = "SHA-512(224)withRSA";
            AlgorithmMap["SHA512(224)WITHRSAENCRYPTION"] = "SHA-512(224)withRSA";
            AlgorithmMap["SHA-512(224)WITHRSAENCRYPTION"] = "SHA-512(224)withRSA";
            AlgorithmOidMap[PkcsObjectIdentifiers.Sha512_224WithRSAEncryption] = "SHA-512(224)withRSA";

            AlgorithmMap["SHA512(256)WITHRSA"] = "SHA-512(256)withRSA";
            AlgorithmMap["SHA-512(256)WITHRSA"] = "SHA-512(256)withRSA";
            AlgorithmMap["SHA512(256)WITHRSAENCRYPTION"] = "SHA-512(256)withRSA";
            AlgorithmMap["SHA-512(256)WITHRSAENCRYPTION"] = "SHA-512(256)withRSA";
            AlgorithmOidMap[PkcsObjectIdentifiers.Sha512_256WithRSAEncryption] = "SHA-512(256)withRSA";

            AlgorithmMap["SHA3-224WITHRSA"] = "SHA3-224withRSA";
            AlgorithmMap["SHA3-224WITHRSAENCRYPTION"] = "SHA3-224withRSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_224] = "SHA3-224withRSA";
            AlgorithmMap["SHA3-256WITHRSA"] = "SHA3-256withRSA";
            AlgorithmMap["SHA3-256WITHRSAENCRYPTION"] = "SHA3-256withRSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_256] = "SHA3-256withRSA";
            AlgorithmMap["SHA3-384WITHRSA"] = "SHA3-384withRSA";
            AlgorithmMap["SHA3-384WITHRSAENCRYPTION"] = "SHA3-384withRSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_384] = "SHA3-384withRSA";
            AlgorithmMap["SHA3-512WITHRSA"] = "SHA3-512withRSA";
            AlgorithmMap["SHA3-512WITHRSAENCRYPTION"] = "SHA3-512withRSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_512] = "SHA3-512withRSA";

            AlgorithmMap["PSSWITHRSA"] = "PSSwithRSA";
            AlgorithmMap["RSASSA-PSS"] = "PSSwithRSA";
            AlgorithmOidMap[PkcsObjectIdentifiers.IdRsassaPss] = "PSSwithRSA";
            AlgorithmMap["RSAPSS"] = "PSSwithRSA";

            AlgorithmMap["SHA1WITHRSAANDMGF1"] = "SHA-1withRSAandMGF1";
            AlgorithmMap["SHA-1WITHRSAANDMGF1"] = "SHA-1withRSAandMGF1";
            AlgorithmMap["SHA1WITHRSA/PSS"] = "SHA-1withRSAandMGF1";
            AlgorithmMap["SHA-1WITHRSA/PSS"] = "SHA-1withRSAandMGF1";
            AlgorithmMap["SHA1WITHRSASSA-PSS"] = "SHA-1withRSAandMGF1";
            AlgorithmMap["SHA-1WITHRSASSA-PSS"] = "SHA-1withRSAandMGF1";

            AlgorithmMap["SHA224WITHRSAANDMGF1"] = "SHA-224withRSAandMGF1";
            AlgorithmMap["SHA-224WITHRSAANDMGF1"] = "SHA-224withRSAandMGF1";
            AlgorithmMap["SHA224WITHRSA/PSS"] = "SHA-224withRSAandMGF1";
            AlgorithmMap["SHA-224WITHRSA/PSS"] = "SHA-224withRSAandMGF1";
            AlgorithmMap["SHA224WITHRSASSA-PSS"] = "SHA-224withRSAandMGF1";
            AlgorithmMap["SHA-224WITHRSASSA-PSS"] = "SHA-224withRSAandMGF1";

            AlgorithmMap["SHA256WITHRSAANDMGF1"] = "SHA-256withRSAandMGF1";
            AlgorithmMap["SHA-256WITHRSAANDMGF1"] = "SHA-256withRSAandMGF1";
            AlgorithmMap["SHA256WITHRSA/PSS"] = "SHA-256withRSAandMGF1";
            AlgorithmMap["SHA-256WITHRSA/PSS"] = "SHA-256withRSAandMGF1";
            AlgorithmMap["SHA256WITHRSASSA-PSS"] = "SHA-256withRSAandMGF1";
            AlgorithmMap["SHA-256WITHRSASSA-PSS"] = "SHA-256withRSAandMGF1";

            AlgorithmMap["SHA384WITHRSAANDMGF1"] = "SHA-384withRSAandMGF1";
            AlgorithmMap["SHA-384WITHRSAANDMGF1"] = "SHA-384withRSAandMGF1";
            AlgorithmMap["SHA384WITHRSA/PSS"] = "SHA-384withRSAandMGF1";
            AlgorithmMap["SHA-384WITHRSA/PSS"] = "SHA-384withRSAandMGF1";
            AlgorithmMap["SHA384WITHRSASSA-PSS"] = "SHA-384withRSAandMGF1";
            AlgorithmMap["SHA-384WITHRSASSA-PSS"] = "SHA-384withRSAandMGF1";

            AlgorithmMap["SHA512WITHRSAANDMGF1"] = "SHA-512withRSAandMGF1";
            AlgorithmMap["SHA-512WITHRSAANDMGF1"] = "SHA-512withRSAandMGF1";
            AlgorithmMap["SHA512WITHRSA/PSS"] = "SHA-512withRSAandMGF1";
            AlgorithmMap["SHA-512WITHRSA/PSS"] = "SHA-512withRSAandMGF1";
            AlgorithmMap["SHA512WITHRSASSA-PSS"] = "SHA-512withRSAandMGF1";
            AlgorithmMap["SHA-512WITHRSASSA-PSS"] = "SHA-512withRSAandMGF1";

            AlgorithmMap["RIPEMD128WITHRSA"] = "RIPEMD128withRSA";
            AlgorithmMap["RIPEMD128WITHRSAENCRYPTION"] = "RIPEMD128withRSA";
            AlgorithmOidMap[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128] = "RIPEMD128withRSA";

            AlgorithmMap["RIPEMD160WITHRSA"] = "RIPEMD160withRSA";
            AlgorithmMap["RIPEMD160WITHRSAENCRYPTION"] = "RIPEMD160withRSA";
            AlgorithmOidMap[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160] = "RIPEMD160withRSA";

            AlgorithmMap["RIPEMD256WITHRSA"] = "RIPEMD256withRSA";
            AlgorithmMap["RIPEMD256WITHRSAENCRYPTION"] = "RIPEMD256withRSA";
            AlgorithmOidMap[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256] = "RIPEMD256withRSA";

            AlgorithmMap["NONEWITHRSA"] = "RSA";
            AlgorithmMap["RSAWITHNONE"] = "RSA";
            AlgorithmMap["RAWRSA"] = "RSA";

            AlgorithmMap["RAWRSAPSS"] = "RAWRSASSA-PSS";
            AlgorithmMap["NONEWITHRSAPSS"] = "RAWRSASSA-PSS";
            AlgorithmMap["NONEWITHRSASSA-PSS"] = "RAWRSASSA-PSS";

            AlgorithmMap["NONEWITHDSA"] = "NONEwithDSA";
            AlgorithmMap["DSAWITHNONE"] = "NONEwithDSA";
            AlgorithmMap["RAWDSA"] = "NONEwithDSA";

            AlgorithmMap["DSA"] = "SHA-1withDSA";
            AlgorithmMap["DSAWITHSHA1"] = "SHA-1withDSA";
            AlgorithmMap["DSAWITHSHA-1"] = "SHA-1withDSA";
            AlgorithmMap["SHA/DSA"] = "SHA-1withDSA";
            AlgorithmMap["SHA1/DSA"] = "SHA-1withDSA";
            AlgorithmMap["SHA-1/DSA"] = "SHA-1withDSA";
            AlgorithmMap["SHA1WITHDSA"] = "SHA-1withDSA";
            AlgorithmMap["SHA-1WITHDSA"] = "SHA-1withDSA";
            AlgorithmOidMap[X9ObjectIdentifiers.IdDsaWithSha1] = "SHA-1withDSA";
            AlgorithmOidMap[OiwObjectIdentifiers.DsaWithSha1] = "SHA-1withDSA";

            AlgorithmMap["DSAWITHSHA224"] = "SHA-224withDSA";
            AlgorithmMap["DSAWITHSHA-224"] = "SHA-224withDSA";
            AlgorithmMap["SHA224/DSA"] = "SHA-224withDSA";
            AlgorithmMap["SHA-224/DSA"] = "SHA-224withDSA";
            AlgorithmMap["SHA224WITHDSA"] = "SHA-224withDSA";
            AlgorithmMap["SHA-224WITHDSA"] = "SHA-224withDSA";
            AlgorithmOidMap[NistObjectIdentifiers.DsaWithSha224] = "SHA-224withDSA";

            AlgorithmMap["DSAWITHSHA256"] = "SHA-256withDSA";
            AlgorithmMap["DSAWITHSHA-256"] = "SHA-256withDSA";
            AlgorithmMap["SHA256/DSA"] = "SHA-256withDSA";
            AlgorithmMap["SHA-256/DSA"] = "SHA-256withDSA";
            AlgorithmMap["SHA256WITHDSA"] = "SHA-256withDSA";
            AlgorithmMap["SHA-256WITHDSA"] = "SHA-256withDSA";
            AlgorithmOidMap[NistObjectIdentifiers.DsaWithSha256] = "SHA-256withDSA";

            AlgorithmMap["DSAWITHSHA384"] = "SHA-384withDSA";
            AlgorithmMap["DSAWITHSHA-384"] = "SHA-384withDSA";
            AlgorithmMap["SHA384/DSA"] = "SHA-384withDSA";
            AlgorithmMap["SHA-384/DSA"] = "SHA-384withDSA";
            AlgorithmMap["SHA384WITHDSA"] = "SHA-384withDSA";
            AlgorithmMap["SHA-384WITHDSA"] = "SHA-384withDSA";
            AlgorithmOidMap[NistObjectIdentifiers.DsaWithSha384] = "SHA-384withDSA";

            AlgorithmMap["DSAWITHSHA512"] = "SHA-512withDSA";
            AlgorithmMap["DSAWITHSHA-512"] = "SHA-512withDSA";
            AlgorithmMap["SHA512/DSA"] = "SHA-512withDSA";
            AlgorithmMap["SHA-512/DSA"] = "SHA-512withDSA";
            AlgorithmMap["SHA512WITHDSA"] = "SHA-512withDSA";
            AlgorithmMap["SHA-512WITHDSA"] = "SHA-512withDSA";
            AlgorithmOidMap[NistObjectIdentifiers.DsaWithSha512] = "SHA-512withDSA";

            AlgorithmMap["DSAWITHSHA3-224"] = "SHA3-224withDSA";
            AlgorithmMap["SHA3-224/DSA"] = "SHA3-224withDSA";
            AlgorithmMap["SHA3-224WITHDSA"] = "SHA3-224withDSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdDsaWithSha3_224] = "SHA3-224withDSA";

            AlgorithmMap["DSAWITHSHA3-256"] = "SHA3-256withDSA";
            AlgorithmMap["SHA3-256/DSA"] = "SHA3-256withDSA";
            AlgorithmMap["SHA3-256WITHDSA"] = "SHA3-256withDSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdDsaWithSha3_256] = "SHA3-256withDSA";

            AlgorithmMap["DSAWITHSHA3-384"] = "SHA3-384withDSA";
            AlgorithmMap["SHA3-384/DSA"] = "SHA3-384withDSA";
            AlgorithmMap["SHA3-384WITHDSA"] = "SHA3-384withDSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdDsaWithSha3_384] = "SHA3-384withDSA";

            AlgorithmMap["DSAWITHSHA3-512"] = "SHA3-512withDSA";
            AlgorithmMap["SHA3-512/DSA"] = "SHA3-512withDSA";
            AlgorithmMap["SHA3-512WITHDSA"] = "SHA3-512withDSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdDsaWithSha3_512] = "SHA3-512withDSA";

            AlgorithmMap["NONEWITHECDSA"] = "NONEwithECDSA";
            AlgorithmMap["ECDSAWITHNONE"] = "NONEwithECDSA";

            AlgorithmMap["ECDSA"] = "SHA-1withECDSA";
            AlgorithmMap["SHA1/ECDSA"] = "SHA-1withECDSA";
            AlgorithmMap["SHA-1/ECDSA"] = "SHA-1withECDSA";
            AlgorithmMap["ECDSAWITHSHA1"] = "SHA-1withECDSA";
            AlgorithmMap["ECDSAWITHSHA-1"] = "SHA-1withECDSA";
            AlgorithmMap["SHA1WITHECDSA"] = "SHA-1withECDSA";
            AlgorithmMap["SHA-1WITHECDSA"] = "SHA-1withECDSA";
            AlgorithmOidMap[X9ObjectIdentifiers.ECDsaWithSha1] = "SHA-1withECDSA";
            AlgorithmOidMap[TeleTrusTObjectIdentifiers.ECSignWithSha1] = "SHA-1withECDSA";

            AlgorithmMap["SHA224/ECDSA"] = "SHA-224withECDSA";
            AlgorithmMap["SHA-224/ECDSA"] = "SHA-224withECDSA";
            AlgorithmMap["ECDSAWITHSHA224"] = "SHA-224withECDSA";
            AlgorithmMap["ECDSAWITHSHA-224"] = "SHA-224withECDSA";
            AlgorithmMap["SHA224WITHECDSA"] = "SHA-224withECDSA";
            AlgorithmMap["SHA-224WITHECDSA"] = "SHA-224withECDSA";
            AlgorithmOidMap[X9ObjectIdentifiers.ECDsaWithSha224] = "SHA-224withECDSA";

            AlgorithmMap["SHA256/ECDSA"] = "SHA-256withECDSA";
            AlgorithmMap["SHA-256/ECDSA"] = "SHA-256withECDSA";
            AlgorithmMap["ECDSAWITHSHA256"] = "SHA-256withECDSA";
            AlgorithmMap["ECDSAWITHSHA-256"] = "SHA-256withECDSA";
            AlgorithmMap["SHA256WITHECDSA"] = "SHA-256withECDSA";
            AlgorithmMap["SHA-256WITHECDSA"] = "SHA-256withECDSA";
            AlgorithmOidMap[X9ObjectIdentifiers.ECDsaWithSha256] = "SHA-256withECDSA";

            AlgorithmMap["SHA384/ECDSA"] = "SHA-384withECDSA";
            AlgorithmMap["SHA-384/ECDSA"] = "SHA-384withECDSA";
            AlgorithmMap["ECDSAWITHSHA384"] = "SHA-384withECDSA";
            AlgorithmMap["ECDSAWITHSHA-384"] = "SHA-384withECDSA";
            AlgorithmMap["SHA384WITHECDSA"] = "SHA-384withECDSA";
            AlgorithmMap["SHA-384WITHECDSA"] = "SHA-384withECDSA";
            AlgorithmOidMap[X9ObjectIdentifiers.ECDsaWithSha384] = "SHA-384withECDSA";

            AlgorithmMap["SHA512/ECDSA"] = "SHA-512withECDSA";
            AlgorithmMap["SHA-512/ECDSA"] = "SHA-512withECDSA";
            AlgorithmMap["ECDSAWITHSHA512"] = "SHA-512withECDSA";
            AlgorithmMap["ECDSAWITHSHA-512"] = "SHA-512withECDSA";
            AlgorithmMap["SHA512WITHECDSA"] = "SHA-512withECDSA";
            AlgorithmMap["SHA-512WITHECDSA"] = "SHA-512withECDSA";
            AlgorithmOidMap[X9ObjectIdentifiers.ECDsaWithSha512] = "SHA-512withECDSA";

            AlgorithmMap["RIPEMD160/ECDSA"] = "RIPEMD160withECDSA";
            AlgorithmMap["ECDSAWITHRIPEMD160"] = "RIPEMD160withECDSA";
            AlgorithmMap["RIPEMD160WITHECDSA"] = "RIPEMD160withECDSA";
            AlgorithmOidMap[TeleTrusTObjectIdentifiers.ECSignWithRipeMD160] = "RIPEMD160withECDSA";

            AlgorithmMap["SHA3-224/ECDSA"] = "SHA3-224withECDSA";
            AlgorithmMap["ECDSAWITHSHA3-224"] = "SHA3-224withECDSA";
            AlgorithmMap["SHA3-224WITHECDSA"] = "SHA3-224withECDSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdEcdsaWithSha3_224] = "SHA3-224withECDSA";

            AlgorithmMap["SHA3-256/ECDSA"] = "SHA3-256withECDSA";
            AlgorithmMap["ECDSAWITHSHA3-256"] = "SHA3-256withECDSA";
            AlgorithmMap["SHA3-256WITHECDSA"] = "SHA3-256withECDSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdEcdsaWithSha3_256] = "SHA3-256withECDSA";

            AlgorithmMap["SHA3-384/ECDSA"] = "SHA3-384withECDSA";
            AlgorithmMap["ECDSAWITHSHA3-384"] = "SHA3-384withECDSA";
            AlgorithmMap["SHA3-384WITHECDSA"] = "SHA3-384withECDSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdEcdsaWithSha3_384] = "SHA3-384withECDSA";

            AlgorithmMap["SHA3-512/ECDSA"] = "SHA3-512withECDSA";
            AlgorithmMap["ECDSAWITHSHA3-512"] = "SHA3-512withECDSA";
            AlgorithmMap["SHA3-512WITHECDSA"] = "SHA3-512withECDSA";
            AlgorithmOidMap[NistObjectIdentifiers.IdEcdsaWithSha3_512] = "SHA3-512withECDSA";

            AlgorithmMap["NONEWITHCVC-ECDSA"] = "NONEwithCVC-ECDSA";
            AlgorithmMap["CVC-ECDSAWITHNONE"] = "NONEwithCVC-ECDSA";

            AlgorithmMap["SHA1/CVC-ECDSA"] = "SHA-1withCVC-ECDSA";
            AlgorithmMap["SHA-1/CVC-ECDSA"] = "SHA-1withCVC-ECDSA";
            AlgorithmMap["CVC-ECDSAWITHSHA1"] = "SHA-1withCVC-ECDSA";
            AlgorithmMap["CVC-ECDSAWITHSHA-1"] = "SHA-1withCVC-ECDSA";
            AlgorithmMap["SHA1WITHCVC-ECDSA"] = "SHA-1withCVC-ECDSA";
            AlgorithmMap["SHA-1WITHCVC-ECDSA"] = "SHA-1withCVC-ECDSA";
            AlgorithmOidMap[EacObjectIdentifiers.id_TA_ECDSA_SHA_1] = "SHA-1withCVC-ECDSA";

            AlgorithmMap["SHA224/CVC-ECDSA"] = "SHA-224withCVC-ECDSA";
            AlgorithmMap["SHA-224/CVC-ECDSA"] = "SHA-224withCVC-ECDSA";
            AlgorithmMap["CVC-ECDSAWITHSHA224"] = "SHA-224withCVC-ECDSA";
            AlgorithmMap["CVC-ECDSAWITHSHA-224"] = "SHA-224withCVC-ECDSA";
            AlgorithmMap["SHA224WITHCVC-ECDSA"] = "SHA-224withCVC-ECDSA";
            AlgorithmMap["SHA-224WITHCVC-ECDSA"] = "SHA-224withCVC-ECDSA";
            AlgorithmOidMap[EacObjectIdentifiers.id_TA_ECDSA_SHA_224] = "SHA-224withCVC-ECDSA";

            AlgorithmMap["SHA256/CVC-ECDSA"] = "SHA-256withCVC-ECDSA";
            AlgorithmMap["SHA-256/CVC-ECDSA"] = "SHA-256withCVC-ECDSA";
            AlgorithmMap["CVC-ECDSAWITHSHA256"] = "SHA-256withCVC-ECDSA";
            AlgorithmMap["CVC-ECDSAWITHSHA-256"] = "SHA-256withCVC-ECDSA";
            AlgorithmMap["SHA256WITHCVC-ECDSA"] = "SHA-256withCVC-ECDSA";
            AlgorithmMap["SHA-256WITHCVC-ECDSA"] = "SHA-256withCVC-ECDSA";
            AlgorithmOidMap[EacObjectIdentifiers.id_TA_ECDSA_SHA_256] = "SHA-256withCVC-ECDSA";

            AlgorithmMap["SHA384/CVC-ECDSA"] = "SHA-384withCVC-ECDSA";
            AlgorithmMap["SHA-384/CVC-ECDSA"] = "SHA-384withCVC-ECDSA";
            AlgorithmMap["CVC-ECDSAWITHSHA384"] = "SHA-384withCVC-ECDSA";
            AlgorithmMap["CVC-ECDSAWITHSHA-384"] = "SHA-384withCVC-ECDSA";
            AlgorithmMap["SHA384WITHCVC-ECDSA"] = "SHA-384withCVC-ECDSA";
            AlgorithmMap["SHA-384WITHCVC-ECDSA"] = "SHA-384withCVC-ECDSA";
            AlgorithmOidMap[EacObjectIdentifiers.id_TA_ECDSA_SHA_384] = "SHA-384withCVC-ECDSA";

            AlgorithmMap["SHA512/CVC-ECDSA"] = "SHA-512withCVC-ECDSA";
            AlgorithmMap["SHA-512/CVC-ECDSA"] = "SHA-512withCVC-ECDSA";
            AlgorithmMap["CVC-ECDSAWITHSHA512"] = "SHA-512withCVC-ECDSA";
            AlgorithmMap["CVC-ECDSAWITHSHA-512"] = "SHA-512withCVC-ECDSA";
            AlgorithmMap["SHA512WITHCVC-ECDSA"] = "SHA-512withCVC-ECDSA";
            AlgorithmMap["SHA-512WITHCVC-ECDSA"] = "SHA-512withCVC-ECDSA";
            AlgorithmOidMap[EacObjectIdentifiers.id_TA_ECDSA_SHA_512] = "SHA-512withCVC-ECDSA";

            AlgorithmMap["NONEWITHPLAIN-ECDSA"] = "NONEwithPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHNONE"] = "NONEwithPLAIN-ECDSA";

            AlgorithmMap["SHA1/PLAIN-ECDSA"] = "SHA-1withPLAIN-ECDSA";
            AlgorithmMap["SHA-1/PLAIN-ECDSA"] = "SHA-1withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA1"] = "SHA-1withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA-1"] = "SHA-1withPLAIN-ECDSA";
            AlgorithmMap["SHA1WITHPLAIN-ECDSA"] = "SHA-1withPLAIN-ECDSA";
            AlgorithmMap["SHA-1WITHPLAIN-ECDSA"] = "SHA-1withPLAIN-ECDSA";
            AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA1] = "SHA-1withPLAIN-ECDSA";

            AlgorithmMap["SHA224/PLAIN-ECDSA"] = "SHA-224withPLAIN-ECDSA";
            AlgorithmMap["SHA-224/PLAIN-ECDSA"] = "SHA-224withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA224"] = "SHA-224withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA-224"] = "SHA-224withPLAIN-ECDSA";
            AlgorithmMap["SHA224WITHPLAIN-ECDSA"] = "SHA-224withPLAIN-ECDSA";
            AlgorithmMap["SHA-224WITHPLAIN-ECDSA"] = "SHA-224withPLAIN-ECDSA";
            AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA224] = "SHA-224withPLAIN-ECDSA";

            AlgorithmMap["SHA256/PLAIN-ECDSA"] = "SHA-256withPLAIN-ECDSA";
            AlgorithmMap["SHA-256/PLAIN-ECDSA"] = "SHA-256withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA256"] = "SHA-256withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA-256"] = "SHA-256withPLAIN-ECDSA";
            AlgorithmMap["SHA256WITHPLAIN-ECDSA"] = "SHA-256withPLAIN-ECDSA";
            AlgorithmMap["SHA-256WITHPLAIN-ECDSA"] = "SHA-256withPLAIN-ECDSA";
            AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA256] = "SHA-256withPLAIN-ECDSA";

            AlgorithmMap["SHA384/PLAIN-ECDSA"] = "SHA-384withPLAIN-ECDSA";
            AlgorithmMap["SHA-384/PLAIN-ECDSA"] = "SHA-384withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA384"] = "SHA-384withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA-384"] = "SHA-384withPLAIN-ECDSA";
            AlgorithmMap["SHA384WITHPLAIN-ECDSA"] = "SHA-384withPLAIN-ECDSA";
            AlgorithmMap["SHA-384WITHPLAIN-ECDSA"] = "SHA-384withPLAIN-ECDSA";
            AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA384] = "SHA-384withPLAIN-ECDSA";

            AlgorithmMap["SHA512/PLAIN-ECDSA"] = "SHA-512withPLAIN-ECDSA";
            AlgorithmMap["SHA-512/PLAIN-ECDSA"] = "SHA-512withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA512"] = "SHA-512withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA-512"] = "SHA-512withPLAIN-ECDSA";
            AlgorithmMap["SHA512WITHPLAIN-ECDSA"] = "SHA-512withPLAIN-ECDSA";
            AlgorithmMap["SHA-512WITHPLAIN-ECDSA"] = "SHA-512withPLAIN-ECDSA";
            AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA512] = "SHA-512withPLAIN-ECDSA";

            AlgorithmMap["RIPEMD160/PLAIN-ECDSA"] = "RIPEMD160withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHRIPEMD160"] = "RIPEMD160withPLAIN-ECDSA";
            AlgorithmMap["RIPEMD160WITHPLAIN-ECDSA"] = "RIPEMD160withPLAIN-ECDSA";
            AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_RIPEMD160] = "RIPEMD160withPLAIN-ECDSA";

            AlgorithmMap["SHA3-224/PLAIN-ECDSA"] = "SHA3-224withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA3-224"] = "SHA3-224withPLAIN-ECDSA";
            AlgorithmMap["SHA3-224WITHPLAIN-ECDSA"] = "SHA3-224withPLAIN-ECDSA";
            AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA3_224] = "SHA3-224withPLAIN-ECDSA";

            AlgorithmMap["SHA3-256/PLAIN-ECDSA"] = "SHA3-256withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA3-256"] = "SHA3-256withPLAIN-ECDSA";
            AlgorithmMap["SHA3-256WITHPLAIN-ECDSA"] = "SHA3-256withPLAIN-ECDSA";
            AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA3_256] = "SHA3-256withPLAIN-ECDSA";

            AlgorithmMap["SHA3-384/PLAIN-ECDSA"] = "SHA3-384withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA3-384"] = "SHA3-384withPLAIN-ECDSA";
            AlgorithmMap["SHA3-384WITHPLAIN-ECDSA"] = "SHA3-384withPLAIN-ECDSA";
            AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA3_384] = "SHA3-384withPLAIN-ECDSA";

            AlgorithmMap["SHA3-512/PLAIN-ECDSA"] = "SHA3-512withPLAIN-ECDSA";
            AlgorithmMap["PLAIN-ECDSAWITHSHA3-512"] = "SHA3-512withPLAIN-ECDSA";
            AlgorithmMap["SHA3-512WITHPLAIN-ECDSA"] = "SHA3-512withPLAIN-ECDSA";
            AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA3_512] = "SHA3-512withPLAIN-ECDSA";

            AlgorithmMap["SHA1WITHECNR"] = "SHA-1withECNR";
            AlgorithmMap["SHA-1WITHECNR"] = "SHA-1withECNR";
            AlgorithmMap["SHA224WITHECNR"] = "SHA-224withECNR";
            AlgorithmMap["SHA-224WITHECNR"] = "SHA-224withECNR";
            AlgorithmMap["SHA256WITHECNR"] = "SHA-256withECNR";
            AlgorithmMap["SHA-256WITHECNR"] = "SHA-256withECNR";
            AlgorithmMap["SHA384WITHECNR"] = "SHA-384withECNR";
            AlgorithmMap["SHA-384WITHECNR"] = "SHA-384withECNR";
            AlgorithmMap["SHA512WITHECNR"] = "SHA-512withECNR";
            AlgorithmMap["SHA-512WITHECNR"] = "SHA-512withECNR";

            AlgorithmMap["GOST-3410"] = "GOST3410";
            AlgorithmMap["GOST-3410-94"] = "GOST3410";
            AlgorithmMap["GOST3411WITHGOST3410"] = "GOST3410";
            AlgorithmMap["GOST3411/GOST3410"] = "GOST3410";
            AlgorithmOidMap[CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94] = "GOST3410";

            AlgorithmMap["ECGOST-3410"] = "ECGOST3410";
            AlgorithmMap["GOST-3410-2001"] = "ECGOST3410";
            AlgorithmMap["GOST3411WITHECGOST3410"] = "ECGOST3410";
            AlgorithmMap["GOST3411/ECGOST3410"] = "ECGOST3410";
            AlgorithmOidMap[CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001] = "ECGOST3410";

            AlgorithmMap["GOST-3410-2012-256"] = "ECGOST3410-2012-256";
            AlgorithmMap["GOST3411WITHECGOST3410-2012-256"] = "ECGOST3410-2012-256";
            AlgorithmMap["GOST3411-2012-256WITHECGOST3410"] = "ECGOST3410-2012-256";
            AlgorithmMap["GOST3411-2012-256WITHECGOST3410-2012-256"] = "ECGOST3410-2012-256";
            AlgorithmMap["GOST3411-2012-256/ECGOST3410"] = "ECGOST3410-2012-256";
            AlgorithmMap["GOST3411-2012-256/ECGOST3410-2012-256"] = "ECGOST3410-2012-256";
            AlgorithmOidMap[RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_256] =
                "ECGOST3410-2012-256";

            AlgorithmMap["GOST-3410-2012-512"] = "ECGOST3410-2012-512";
            AlgorithmMap["GOST3411WITHECGOST3410-2012-512"] = "ECGOST3410-2012-512";
            AlgorithmMap["GOST3411-2012-512WITHECGOST3410"] = "ECGOST3410-2012-512";
            AlgorithmMap["GOST3411-2012-512WITHECGOST3410-2012-512"] = "ECGOST3410-2012-512";
            AlgorithmMap["GOST3411-2012-512/ECGOST3410"] = "ECGOST3410-2012-512";
            AlgorithmMap["GOST3411-2012-512/ECGOST3410-2012-512"] = "ECGOST3410-2012-512";
            AlgorithmOidMap[RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_512] =
                "ECGOST3410-2012-512";

            AlgorithmMap["SHA256WITHSM2"] = "SHA256withSM2";
            AlgorithmOidMap[GMObjectIdentifiers.sm2sign_with_sha256] = "SHA256withSM2";
            AlgorithmMap["SM3WITHSM2"] = "SM3withSM2";
            AlgorithmOidMap[GMObjectIdentifiers.sm2sign_with_sm3] = "SM3withSM2";

            Oids["MD2withRSA"] = PkcsObjectIdentifiers.MD2WithRsaEncryption;
            Oids["MD4withRSA"] = PkcsObjectIdentifiers.MD4WithRsaEncryption;
            Oids["MD5withRSA"] = PkcsObjectIdentifiers.MD5WithRsaEncryption;

            Oids["SHA-1withRSA"] = PkcsObjectIdentifiers.Sha1WithRsaEncryption;
            Oids["SHA-224withRSA"] = PkcsObjectIdentifiers.Sha224WithRsaEncryption;
            Oids["SHA-256withRSA"] = PkcsObjectIdentifiers.Sha256WithRsaEncryption;
            Oids["SHA-384withRSA"] = PkcsObjectIdentifiers.Sha384WithRsaEncryption;
            Oids["SHA-512withRSA"] = PkcsObjectIdentifiers.Sha512WithRsaEncryption;
            Oids["SHA-512(224)withRSA"] = PkcsObjectIdentifiers.Sha512_224WithRSAEncryption;
            Oids["SHA-512(256)withRSA"] = PkcsObjectIdentifiers.Sha512_256WithRSAEncryption;
            Oids["SHA3-224withRSA"] = NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_224;
            Oids["SHA3-256withRSA"] = NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_256;
            Oids["SHA3-384withRSA"] = NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_384;
            Oids["SHA3-512withRSA"] = NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_512;

            Oids["PSSwithRSA"] = PkcsObjectIdentifiers.IdRsassaPss;
            Oids["SHA-1withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
            Oids["SHA-224withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
            Oids["SHA-256withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
            Oids["SHA-384withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
            Oids["SHA-512withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;

            Oids["RIPEMD128withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128;
            Oids["RIPEMD160withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160;
            Oids["RIPEMD256withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256;

            Oids["SHA-1withDSA"] = X9ObjectIdentifiers.IdDsaWithSha1;
            Oids["SHA-224withDSA"] = NistObjectIdentifiers.DsaWithSha224;
            Oids["SHA-256withDSA"] = NistObjectIdentifiers.DsaWithSha256;
            Oids["SHA-384withDSA"] = NistObjectIdentifiers.DsaWithSha384;
            Oids["SHA-512withDSA"] = NistObjectIdentifiers.DsaWithSha512;

            Oids["SHA3-224withDSA"] = NistObjectIdentifiers.IdDsaWithSha3_224;
            Oids["SHA3-256withDSA"] = NistObjectIdentifiers.IdDsaWithSha3_256;
            Oids["SHA3-384withDSA"] = NistObjectIdentifiers.IdDsaWithSha3_384;
            Oids["SHA3-512withDSA"] = NistObjectIdentifiers.IdDsaWithSha3_512;

            Oids["SHA-1withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha1;
            Oids["SHA-224withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha224;
            Oids["SHA-256withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha256;
            Oids["SHA-384withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha384;
            Oids["SHA-512withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha512;
            Oids["RIPEMD160withECDSA"] = TeleTrusTObjectIdentifiers.ECSignWithRipeMD160;

            Oids["SHA3-224withECDSA"] = NistObjectIdentifiers.IdEcdsaWithSha3_224;
            Oids["SHA3-256withECDSA"] = NistObjectIdentifiers.IdEcdsaWithSha3_256;
            Oids["SHA3-384withECDSA"] = NistObjectIdentifiers.IdEcdsaWithSha3_384;
            Oids["SHA3-512withECDSA"] = NistObjectIdentifiers.IdEcdsaWithSha3_512;

            Oids["SHA-1withCVC-ECDSA"] = EacObjectIdentifiers.id_TA_ECDSA_SHA_1;
            Oids["SHA-224withCVC-ECDSA"] = EacObjectIdentifiers.id_TA_ECDSA_SHA_224;
            Oids["SHA-256withCVC-ECDSA"] = EacObjectIdentifiers.id_TA_ECDSA_SHA_256;
            Oids["SHA-384withCVC-ECDSA"] = EacObjectIdentifiers.id_TA_ECDSA_SHA_384;
            Oids["SHA-512withCVC-ECDSA"] = EacObjectIdentifiers.id_TA_ECDSA_SHA_512;

            Oids["SHA-1withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA1;
            Oids["SHA-224withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA224;
            Oids["SHA-256withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA256;
            Oids["SHA-384withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA384;
            Oids["SHA-512withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA512;
            Oids["RIPEMD160withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_RIPEMD160;

            Oids["SHA3-224withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA3_224;
            Oids["SHA3-256withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA3_256;
            Oids["SHA3-384withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA3_384;
            Oids["SHA3-512withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA3_512;

            Oids["GOST3410"] = CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94;
            Oids["ECGOST3410"] = CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001;

            Oids["ECGOST3410-2012-256"] = RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_256;
            Oids["ECGOST3410-2012-512"] = RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_512;

            Oids["SHA256withSM2"] = GMObjectIdentifiers.sm2sign_with_sha256;
            Oids["SM3withSM2"] = GMObjectIdentifiers.sm2sign_with_sm3;

            /*
             * EdDSA
             */
            AddAlgorithm("Ed25519", EdECObjectIdentifiers.id_Ed25519, isNoRandom: true);
            AddAlgorithm("Ed25519ctx", oid: null, isNoRandom: true);
            AddAlgorithm("Ed25519ph", oid: null, isNoRandom: true);
            AddAlgorithm("Ed448", EdECObjectIdentifiers.id_Ed448, isNoRandom: true);
            AddAlgorithm("Ed448ph", oid: null, isNoRandom: true);

            /*
             * ML-DSA
             */
            foreach (MLDsaParameters mlDsa in MLDsaParameters.ByName.Values)
            {
                AddAlgorithm(mlDsa.Name, mlDsa.Oid, isNoRandom: false);
            }

            /*
             * SLH-DSA
             */
            foreach (SlhDsaParameters slhDsa in SlhDsaParameters.ByName.Values)
            {
                AddAlgorithm(slhDsa.Name, slhDsa.Oid, isNoRandom: false);
            }

#if DEBUG
            foreach (var key in AlgorithmMap.Keys)
            {
                if (DerObjectIdentifier.TryFromID(key, out var ignore))
                    throw new Exception("OID mapping belongs in AlgorithmOidMap: " + key);
            }

            var mechanisms = new HashSet<string>(AlgorithmMap.Values);
            mechanisms.UnionWith(AlgorithmOidMap.Values);

            foreach (var mechanism in mechanisms)
            {
                if (AlgorithmMap.TryGetValue(mechanism, out var check))
                {
                    if (mechanism != check)
                        throw new Exception("Mechanism mapping MUST be to self: " + mechanism);
                }
                else
                {
                    if (!mechanism.Equals(mechanism.ToUpperInvariant()))
                        throw new Exception("Unmapped mechanism MUST be uppercase: " + mechanism);
                }
            }
#endif
        }

        private static void AddAlgorithm(string name, DerObjectIdentifier oid, bool isNoRandom)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (name.ToUpperInvariant() != name)
            {
                AlgorithmMap.Add(name, name);
            }
            if (oid != null)
            {
                AlgorithmOidMap.Add(oid, name);
                Oids.Add(name, oid);
            }
            if (isNoRandom)
            {
                NoRandom.Add(name);
            }
        }

        public static ICollection<string> Algorithms => CollectionUtilities.ReadOnly(Oids.Keys);

        // TODO[api] Change parameter name to 'oid'
        public static Asn1Encodable GetDefaultX509Parameters(DerObjectIdentifier id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (!AlgorithmOidMap.TryGetValue(id, out var mechanism))
                return DerNull.Instance;

            return GetDefaultX509ParametersForMechanism(mechanism);
        }

        public static Asn1Encodable GetDefaultX509Parameters(string algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));

            string mechanism = GetMechanism(algorithm) ?? algorithm;

            return GetDefaultX509ParametersForMechanism(mechanism);
        }

        private static Asn1Encodable GetDefaultX509ParametersForMechanism(string mechanism)
        {
            if (mechanism == "PSSwithRSA")
            {
                // TODO The Sha1Digest here is a default. In JCE version, the actual digest
                // to be used can be overridden by subsequent parameter settings.
                return GetPssX509Parameters("SHA-1");
            }

            if (Platform.EndsWith(mechanism, "withRSAandMGF1"))
            {
                string digestName = mechanism.Substring(0, mechanism.Length - "withRSAandMGF1".Length);
                return GetPssX509Parameters(digestName);
            }

            return DerNull.Instance;
        }

        public static string GetEncodingName(DerObjectIdentifier oid)
        {
            return CollectionUtilities.GetValueOrNull(AlgorithmOidMap, oid);
        }

        private static string GetMechanism(string algorithm)
        {
            if (AlgorithmMap.TryGetValue(algorithm, out var mechanism1))
                return mechanism1;

            if (DerObjectIdentifier.TryFromID(algorithm, out var oid))
            {
                if (AlgorithmOidMap.TryGetValue(oid, out var mechanism2))
                    return mechanism2;
            }

            return null;
        }

        /// <summary>
        /// Returns an ObjectIdentifier for a given signature mechanism.
        /// </summary>
        /// <param name="mechanism">A string representation of the signature mechanism.</param>
        /// <returns>A DerObjectIdentifier, null if the OID is not available.</returns>
        public static DerObjectIdentifier GetObjectIdentifier(string mechanism)
        {
            if (mechanism == null)
                throw new ArgumentNullException(nameof(mechanism));

            mechanism = GetMechanism(mechanism) ?? mechanism;

            return CollectionUtilities.GetValueOrNull(Oids, mechanism);
        }

        private static Asn1Encodable GetPssX509Parameters(
            string	digestName)
        {
            AlgorithmIdentifier hashAlgorithm = new AlgorithmIdentifier(
                DigestUtilities.GetObjectIdentifier(digestName), DerNull.Instance);

            // TODO Is it possible for the MGF hash alg to be different from the PSS one?
            AlgorithmIdentifier maskGenAlgorithm = new AlgorithmIdentifier(
                PkcsObjectIdentifiers.IdMgf1, hashAlgorithm);

            int saltLen = DigestUtilities.GetDigest(digestName).GetDigestSize();
            return new RsassaPssParameters(hashAlgorithm, maskGenAlgorithm,
                new DerInteger(saltLen), RsassaPssParameters.DefaultTrailerField);
        }

        // TODO[api] Change parameter name to 'oid'
        public static ISigner GetSigner(DerObjectIdentifier id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            if (AlgorithmOidMap.TryGetValue(id, out var mechanism))
            {
                var signer = GetSignerForMechanism(mechanism);
                if (signer != null)
                    return signer;
            }

            throw new SecurityUtilityException("Signer OID not recognised.");
        }

        public static ISigner GetSigner(string algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));

            string mechanism = GetMechanism(algorithm) ?? algorithm.ToUpperInvariant();

            var signer = GetSignerForMechanism(mechanism);
            if (signer != null)
                return signer;

            throw new SecurityUtilityException("Signer " + algorithm + " not recognised.");
        }

        private static ISigner GetSignerForMechanism(string mechanism)
        {
            if (Platform.StartsWith(mechanism, "Ed"))
            {
                if (mechanism.Equals("Ed25519"))
                {
                    return new Ed25519Signer();
                }
                if (mechanism.Equals("Ed25519ctx"))
                {
                    return new Ed25519ctxSigner(Arrays.EmptyBytes);
                }
                if (mechanism.Equals("Ed25519ph"))
                {
                    return new Ed25519phSigner(Arrays.EmptyBytes);
                }
                if (mechanism.Equals("Ed448"))
                {
                    return new Ed448Signer(Arrays.EmptyBytes);
                }
                if (mechanism.Equals("Ed448ph"))
                {
                    return new Ed448phSigner(Arrays.EmptyBytes);
                }
            }

            if (mechanism.Equals("RSA"))
            {
                return new GenericSigner(new Pkcs1Encoding(new RsaBlindedEngine()), new NullDigest());
            }
            if (mechanism.Equals("RAWRSASSA-PSS"))
            {
                // TODO Add support for other parameter settings
                return PssSigner.CreateRawSigner(new RsaBlindedEngine(), new Sha1Digest());
            }
            if (mechanism.Equals("PSSwithRSA"))
            {
                // TODO The Sha1Digest here is a default. In JCE version, the actual digest
                // to be used can be overridden by subsequent parameter settings.
                return new PssSigner(new RsaBlindedEngine(), new Sha1Digest());
            }
            if (Platform.EndsWith(mechanism, "withRSA"))
            {
                string digestName = mechanism.Substring(0, mechanism.LastIndexOf("with"));
                IDigest digest = DigestUtilities.GetDigest(digestName);
                return new RsaDigestSigner(digest);
            }
            if (Platform.EndsWith(mechanism, "withRSAandMGF1"))
            {
                string digestName = mechanism.Substring(0, mechanism.LastIndexOf("with"));
                IDigest digest = DigestUtilities.GetDigest(digestName);
                return new PssSigner(new RsaBlindedEngine(), digest);
            }

            if (Platform.EndsWith(mechanism, "withDSA"))
            {
                string digestName = mechanism.Substring(0, mechanism.LastIndexOf("with"));
                IDigest digest = DigestUtilities.GetDigest(digestName);
                return new DsaDigestSigner(new DsaSigner(), digest);
            }

            if (Platform.EndsWith(mechanism, "withECDSA"))
            {
                string digestName = mechanism.Substring(0, mechanism.LastIndexOf("with"));
                IDigest digest = DigestUtilities.GetDigest(digestName);
                return new DsaDigestSigner(new ECDsaSigner(), digest);
            }

            if (Platform.EndsWith(mechanism, "withCVC-ECDSA")
                || Platform.EndsWith(mechanism, "withPLAIN-ECDSA"))
            {
                string digestName = mechanism.Substring(0, mechanism.LastIndexOf("with"));
                IDigest digest = DigestUtilities.GetDigest(digestName);
                return new DsaDigestSigner(new ECDsaSigner(), digest, PlainDsaEncoding.Instance);
            }

            if (Platform.EndsWith(mechanism, "withECNR"))
            {
                string digestName = mechanism.Substring(0, mechanism.LastIndexOf("with"));
                IDigest digest = DigestUtilities.GetDigest(digestName);
                return new DsaDigestSigner(new ECNRSigner(), digest);
            }

            if (Platform.EndsWith(mechanism, "withSM2"))
            {
                string digestName = mechanism.Substring(0, mechanism.LastIndexOf("with"));
                IDigest digest = DigestUtilities.GetDigest(digestName);
                return new SM2Signer(digest);
            }

            if (mechanism.Equals("GOST3410"))
            {
                return new Gost3410DigestSigner(new Gost3410Signer(), new Gost3411Digest());
            }

            if (Platform.StartsWith(mechanism, "ECGOST3410"))
            {
                if (mechanism.Equals("ECGOST3410"))
                {
                    return new Gost3410DigestSigner(new ECGost3410Signer(), new Gost3411Digest());
                }
                if (mechanism.Equals("ECGOST3410-2012-256"))
                {
                    return new Gost3410DigestSigner(new ECGost3410Signer(), new Gost3411_2012_256Digest());
                }
                if (mechanism.Equals("ECGOST3410-2012-512"))
                {
                    return new Gost3410DigestSigner(new ECGost3410Signer(), new Gost3411_2012_512Digest());
                }
            }

            if (Platform.EndsWith(mechanism, "/ISO9796-2"))
            {
                if (mechanism.Equals("SHA1WITHRSA/ISO9796-2"))
                {
                    return new Iso9796d2Signer(new RsaBlindedEngine(), new Sha1Digest(), true);
                }
                if (mechanism.Equals("MD5WITHRSA/ISO9796-2"))
                {
                    return new Iso9796d2Signer(new RsaBlindedEngine(), new MD5Digest(), true);
                }
                if (mechanism.Equals("RIPEMD160WITHRSA/ISO9796-2"))
                {
                    return new Iso9796d2Signer(new RsaBlindedEngine(), new RipeMD160Digest(), true);
                }
            }

            if (Platform.EndsWith(mechanism, "/X9.31"))
            {
                string x931 = mechanism.Substring(0, mechanism.Length - "/X9.31".Length);
                int withPos = Platform.IndexOf(x931, "WITH");
                if (withPos > 0)
                {
                    int endPos = withPos + "WITH".Length;

                    string cipherName = x931.Substring(endPos, x931.Length - endPos);
                    if (cipherName.Equals("RSA"))
                    {
                        IAsymmetricBlockCipher cipher = new RsaBlindedEngine();

                        string digestName = x931.Substring(0, withPos);
                        IDigest digest = DigestUtilities.GetDigest(digestName);

                        return new X931Signer(cipher, digest);
                    }
                }
            }

            if (MLDsaParameters.ByName.TryGetValue(mechanism, out MLDsaParameters mlDsaParameters))
            {
                var preHashOid = mlDsaParameters.PreHashOid;
                if (preHashOid == null)
                    return new MLDsaSigner(mlDsaParameters, deterministic: false);

                return new HashMLDsaSigner(mlDsaParameters, deterministic: false);
            }

            if (SlhDsaParameters.ByName.TryGetValue(mechanism, out SlhDsaParameters slhDsaParameters))
            {
                var preHashOid = slhDsaParameters.PreHashOid;
                if (preHashOid == null)
                    return new SlhDsaSigner(slhDsaParameters, deterministic: false);

                return new HashSlhDsaSigner(slhDsaParameters, deterministic: false);
            }

            return null;
        }

        // TODO[api] Rename 'privateKey' to 'key'
        public static ISigner InitSigner(DerObjectIdentifier algorithmOid, bool forSigning,
            AsymmetricKeyParameter privateKey, SecureRandom random)
        {
            if (algorithmOid == null)
                throw new ArgumentNullException(nameof(algorithmOid));

            if (!AlgorithmOidMap.TryGetValue(algorithmOid, out var mechanism))
                throw new SecurityUtilityException("Signer OID not recognised.");

            return InitSignerForMechanism(mechanism, forSigning, privateKey, random);
        }

        // TODO[api] Rename 'privateKey' to 'key'
        public static ISigner InitSigner(string algorithm, bool forSigning, AsymmetricKeyParameter privateKey,
            SecureRandom random)
        {
            if (algorithm == null)
                throw new ArgumentNullException(nameof(algorithm));

            string mechanism = GetMechanism(algorithm) ?? algorithm.ToUpperInvariant();

            return InitSignerForMechanism(mechanism, forSigning, privateKey, random);
        }

        private static ISigner InitSignerForMechanism(string mechanism, bool forSigning,
            AsymmetricKeyParameter key, SecureRandom random)
        {
            var signer = GetSignerForMechanism(mechanism) ??
                throw new SecurityUtilityException("Signing mechanism " + mechanism + " not recognised.");

            ICipherParameters cipherParameters = key;
            if (forSigning && !NoRandom.Contains(mechanism))
            {
                cipherParameters = ParameterUtilities.WithRandom(cipherParameters, random);
            }

            signer.Init(forSigning, cipherParameters);
            return signer;
        }
    }
}
