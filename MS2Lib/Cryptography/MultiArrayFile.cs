using System;
using System.IO;

namespace MS2Lib;

public class MultiArrayFile : IMultiArray {

    private readonly Lazy<byte[][]> LazyFile; // TODO: maybe array of lazy (Lazy<byte[]>[])

    public MultiArrayFile(string filePath, int count, int arraySize) {
        FilePath = filePath;
        Count = count;
        ArraySize = arraySize;

        LazyFile = new Lazy<byte[][]>(CreateLazyImplementation);
    }
    public string FilePath { get; }
    public int ArraySize { get; }
    public int Count { get; }

    public byte[] this[long index] => LazyFile.Value[index % Count];

    private byte[][] CreateLazyImplementation() {
        byte[][] result = new byte[Count][];

        using (var br = new BinaryReader(File.OpenRead(FilePath))) {
            for (int i = 0; i < Count; i++) {
                byte[] bytes = br.ReadBytes(ArraySize);
                if (bytes.Length == ArraySize) {
                    result[i] = bytes;
                }
            }
        }

        return result;
    }
}
