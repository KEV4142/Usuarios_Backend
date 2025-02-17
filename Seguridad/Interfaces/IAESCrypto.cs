namespace Seguridad.Interfaces;
public interface IAESCrypto
{
    public string Encrypt(string plainText);
    public string Decrypt(string cipherText);
}
