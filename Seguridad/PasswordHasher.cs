using System.Security.Cryptography;

namespace FulbitoBravo.Seguridad;

// Hasher de contraseñas basado en PBKDF2 (Rfc2898DeriveBytes), incluido en
// el propio .NET (System.Security.Cryptography), sin dependencias externas.
// Formato almacenado: {iteraciones}.{saltBase64}.{hashBase64}
public static class PasswordHasher
{
    private const int SaltSize = 16;       // 128 bits
    private const int HashSize = 32;       // 256 bits
    private const int Iteraciones = 100_000;

    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iteraciones,
            HashAlgorithmName.SHA256,
            HashSize);

        return $"{Iteraciones}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string hashAlmacenado)
    {
        var partes = hashAlmacenado.Split('.', 3);
        if (partes.Length != 3) return false;

        if (!int.TryParse(partes[0], out int iteraciones)) return false;

        byte[] salt = Convert.FromBase64String(partes[1]);
        byte[] hashEsperado = Convert.FromBase64String(partes[2]);

        byte[] hashCalculado = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iteraciones,
            HashAlgorithmName.SHA256,
            hashEsperado.Length);

        return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
    }
}
