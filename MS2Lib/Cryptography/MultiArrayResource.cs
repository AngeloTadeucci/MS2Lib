using System;
using System.IO;
using System.Resources;

namespace MS2Lib;

public class MultiArrayResource : IMultiArray {

    private readonly Lazy<byte[][]> LazyResource; // TODO: maybe array of lazy (Lazy<byte[]>[])

    public MultiArrayResource(ResourceManager resourceManager, string resourceName, int count, int arraySize) {
        ResourceManager = resourceManager;
        ResourceName = resourceName;
        Count = count;
        ArraySize = arraySize;

        LazyResource = new Lazy<byte[][]>(CreateLazyImplementation);
    }
    public ResourceManager ResourceManager { get; }
    public string ResourceName { get; }
    public int ArraySize { get; }
    public int Count { get; }

    public byte[] this[long index] => LazyResource.Value[index % Count];

    private byte[][] CreateLazyImplementation() {
        byte[][] result = new byte[Count][];

        using (var br = new BinaryReader(new MemoryStream((byte[]) ResourceManager.GetObject(ResourceName)))) {
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
