using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;

namespace MS2Lib;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class MS2File : IMS2File {

    public MS2File(IMS2Archive archive, Stream dataStream, IMS2FileInfo info, IMS2FileHeader header, bool isStreamEncrypted) {
        if (!dataStream.CanSeek) {
            throw new ArgumentException("The given stream must be seekable.", nameof(dataStream));
        }

        Archive = archive ?? throw new ArgumentNullException(nameof(archive));
        DataStream = dataStream ?? throw new ArgumentNullException(nameof(dataStream));
        Info = info ?? throw new ArgumentNullException(nameof(info));
        Header = header ?? throw new ArgumentNullException(nameof(header));
        IsDataEncrypted = isStreamEncrypted;
        Id = InternalGetId(info, header);
    }

    public MS2File(IMS2Archive archive, MemoryMappedFile dataMemoryMappedFile, IMS2FileInfo info, IMS2FileHeader header, bool isStreamEncrypted) {
        Archive = archive ?? throw new ArgumentNullException(nameof(archive));
        DataMemoryMappedFile = dataMemoryMappedFile ?? throw new ArgumentNullException(nameof(dataMemoryMappedFile));
        Info = info ?? throw new ArgumentNullException(nameof(info));
        Header = header ?? throw new ArgumentNullException(nameof(header));
        IsDataEncrypted = isStreamEncrypted;
        Id = InternalGetId(info, header);
    }

    protected Stream DataStream { get; }
    protected MemoryMappedFile DataMemoryMappedFile { get; }
    public bool IsDataEncrypted { get; }

    protected CompressionType CompressionType => Header.CompressionType;
    protected bool IsZlibCompressed =>
        Header.CompressionType switch {
            CompressionType.None => false,
            CompressionType.Usm => false,
            CompressionType.Png => false,
            CompressionType.Zlib => true,
            _ => throw new Exception($"Unrecognised compression type [{CompressionType}]."),
        };

    [ExcludeFromCodeCoverage]
    private string DebuggerDisplay
        => $"Name = {Name}";
    public IMS2Archive Archive { get; }
    public IMS2FileInfo Info { get; }
    public IMS2FileHeader Header { get; }

    public long Id { get; }
    public string Name => Info.Path;

    public virtual Task<Stream> GetStreamAsync() {
        if (IsDisposed) {
            throw new ObjectDisposedException(nameof(MS2File));
        }

        Stream dataStream = GetDataStream();
        if (IsDataEncrypted) {
            return Archive.CryptoRepository.GetDecryptionStreamAsync(dataStream, Header.Size, IsZlibCompressed);
        }

        return Task.FromResult(dataStream);
    }

    public virtual Task<(Stream stream, IMS2SizeHeader size)> GetStreamForArchivingAsync() {
        if (IsDisposed) {
            throw new ObjectDisposedException(nameof(MS2File));
        }

        Stream dataStream = GetDataStream();
        if (IsDataEncrypted) {
            return Task.FromResult((dataStream, Header.Size));
        }

        return Archive.CryptoRepository.GetEncryptionStreamAsync(dataStream, Header.Size.EncodedSize, IsZlibCompressed);
    }

    protected virtual Stream GetDataStream() {
        if (DataStream != null) {
            DataStream.Position = Header.Offset;

            return new KeepOpenStreamProxy(DataStream);
        }

        if (DataMemoryMappedFile != null) {
            return DataMemoryMappedFile.CreateViewStream(Header.Offset, Header.Size.EncodedSize, MemoryMappedFileAccess.Read);
        }

        throw new InvalidOperationException();
    }

    protected static long InternalGetId(IMS2FileInfo info, IMS2FileHeader header) {
        if (long.TryParse(info.Id, out long result)) {
            Debug.Assert(result == header.Id);

            return result;
        }
        return header.Id;
    }

    #region IDisposable interface
    private bool IsDisposed;

    protected virtual void Dispose(bool disposing) {
        if (!IsDisposed) {
            if (disposing) {
                // managed
                DataStream?.Dispose();
            }

            // unmanaged

            IsDisposed = true;
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
