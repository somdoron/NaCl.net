# NaCl.net

NaCl.net (pronounced "salt dotnet") is C# port of Box (Curve25519XSalsa20Poly1305), SecretBox (XSalsa20Poly1305), XSalsa20 and Poly1305 from [NaCl](http://nacl.cr.yp.to/) by Daniel J. Bernstein.

## Package

https://www.nuget.org/packages/NaCl.Net/

## How to use

### Curve25519XSalsa20Poly1305 - Public-key Authenticated Encryption

Using public-key authenticated encryption, Bob can encrypt a confidential message specifically for Alice, using Alice's public key.

Using Bob's public key, Alice can compute a shared secret key. Using Alice's public key and his secret key, Bob can compute the exact same shared secret key. That shared secret key can be used to verify that the encrypted message was not tampered with, before eventually decrypting it.

Alice only needs Bob's public key, the nonce and the ciphertext. Bob should never ever share his secret key, even with Alice.
And in order to send messages to Alice, Bob only needs Alice's public key. Alice should never ever share her secret key either, even with Bob.

Alice can reply to Bob using the same system, without having to generate a distinct key pair.
The nonce doesn't have to be confidential, but it should be used with just one invocation of crypto_box_easy() for a particular pair of public and secret keys.

One easy way to generate a nonce is to use [RandonNumberGenerator](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.randomnumbergenerator?view=netframework-4.8), considering the size of the nonces the risk of any random collisions is negligible. For some applications, if you wish to use nonces to detect missing messages or to ignore replayed messages, it is also acceptable to use a simple incrementing counter as a nonce.
When doing so you must ensure that the same value can never be re-used (for example you may have multiple threads or even hosts generating messages using the same key pairs).
As stated above, senders can decrypt their own messages, and compute a valid authentication tag for any messages encrypted with a given shared secret key. This is generally not an issue for online protocols.

Example:

```
using var rng = RandomNumberGenerator.Create();

Curve25519XSalsa20Poly1305.KeyPair(out var aliceSecretKey, out var alicePublicKey);
Curve25519XSalsa20Poly1305.KeyPair(out var bobSecretKey, out var bobPublicKey);

Curve25519XSalsa20Poly1305 aliceBox = new Curve25519XSalsa20Poly1305(aliceSecretKey, bobPublicKey);
Curve25519XSalsa20Poly1305 bobBox = new Curve25519XSalsa20Poly1305(bobSecretKey, alicePublicKey);

// Generating random nonce
byte[] nonce = new byte[Curve25519XSalsa20Poly1305.NonceLength];
rng.GetBytes(nonce);

// Plaintext message
byte[] message = Encoding.UTF8.GetBytes("Hey Bob");

// Prepare the buffer for the ciphertext, must be message length and extra 16 bytes for the authentication tag
byte[] cipher = new byte[message.Length + Curve25519XSalsa20Poly1305.TagLength];

// Encrypting using alice box
aliceBox.Encrypt(cipher, message, nonce);

// Decrypting using bob box
byte[] plain = new byte[cipher.Length - Curve25519XSalsa20Poly1305.TagLength];
bool isVerified = bobBox.TryDecrypt(plain, cipher, nonce);

Console.WriteLine("Verified: {0}", isVerified);
Console.WriteLine("Message: {0}", Encoding.UTF8.GetString(plain));
```


### XSalsa20Poly1305

## License
NaCl.net is using MPLv2, you can read more at the [FAQ](http://www.mozilla.org/MPL/2.0/FAQ.html) file.
