namespace ServerBlockChain.Interface;

public interface ISerialization<T>
{
    byte[] ToBytes(T data);
    T FromBytes(byte[] data);
}