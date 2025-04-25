using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MS2Lib;

[ExcludeFromCodeCoverage]
public class KeepOpenStreamProxy : Stream {

    public KeepOpenStreamProxy(Stream stream) {
        Stream = stream ?? throw new ArgumentNullException(nameof(stream));
    }
    public Stream Stream { get; }

    public override bool CanRead => Stream.CanRead;
    public override bool CanSeek => Stream.CanSeek;
    public override bool CanWrite => Stream.CanWrite;
    public override long Length => Stream.Length;
    public override bool CanTimeout => Stream.CanTimeout;

    public override long Position {
        get => Stream.Position;
        set => Stream.Position = value;
    }
    public override int ReadTimeout {
        get => Stream.ReadTimeout;
        set => Stream.ReadTimeout = value;
    }
    public override int WriteTimeout {
        get => Stream.WriteTimeout;
        set => Stream.WriteTimeout = value;
    }

    public override void Flush() {
        Stream.Flush();
    }
    public override int Read(byte[] buffer, int offset, int count) {
        return Stream.Read(buffer, offset, count);
    }
    public override long Seek(long offset, SeekOrigin origin) {
        return Stream.Seek(offset, origin);
    }
    public override void SetLength(long value) {
        Stream.SetLength(value);
    }
    public override void Write(byte[] buffer, int offset, int count) {
        Stream.Write(buffer, offset, count);
    }
    public override object InitializeLifetimeService() {
        return Stream.InitializeLifetimeService();
    }
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
        return Stream.BeginRead(buffer, offset, count, callback, state);
    }
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
        return Stream.BeginWrite(buffer, offset, count, callback, state);
    }
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) {
        return Stream.CopyToAsync(destination, bufferSize, cancellationToken);
    }
    public override int EndRead(IAsyncResult asyncResult) {
        return Stream.EndRead(asyncResult);
    }
    public override void EndWrite(IAsyncResult asyncResult) {
        Stream.EndWrite(asyncResult);
    }
    public override Task FlushAsync(CancellationToken cancellationToken) {
        return Stream.FlushAsync(cancellationToken);
    }
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
        return Stream.ReadAsync(buffer, offset, count, cancellationToken);
    }
    public override int ReadByte() {
        return Stream.ReadByte();
    }
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
        return Stream.WriteAsync(buffer, offset, count, cancellationToken);
    }
    public override void WriteByte(byte value) {
        Stream.WriteByte(value);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) {
        return Stream.WriteAsync(buffer, cancellationToken);
    }
    public override void Write(ReadOnlySpan<byte> buffer) {
        Stream.Write(buffer);
    }
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) {
        return Stream.ReadAsync(buffer, cancellationToken);
    }
    public override int Read(Span<byte> buffer) {
        return Stream.Read(buffer);
    }
    public override void CopyTo(Stream destination, int bufferSize) {
        Stream.CopyTo(destination, bufferSize);
    }
}
