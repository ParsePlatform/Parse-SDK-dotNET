// Copyright (c) 2015-present, Parse, LLC.  All rights reserved.  This source code is licensed under the BSD-style license found in the LICENSE file in the root directory of this source tree.  An additional grant of patent rights can be found in the PATENTS file in the same directory.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Parse.Common.Internal;

namespace Parse.Core.Internal
{
    public class ParseFileController : IParseFileController
    {
        private readonly IParseCommandRunner commandRunner;

        public ParseFileController(IParseCommandRunner commandRunner) => this.commandRunner = commandRunner;

        public Task<FileState> SaveAsync(FileState state,
            Stream dataStream,
            string sessionToken,
            IProgress<ParseUploadProgressEventArgs> progress,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (state.Url != null)
            {
                // !isDirty
                return Task.FromResult(state);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                TaskCompletionSource<FileState> tcs = new TaskCompletionSource<FileState>();
                tcs.TrySetCanceled();
                return tcs.Task;
            }

            long oldPosition = dataStream.Position;
            ParseCommand command = new ParseCommand("files/" + state.Name,
                method: "POST",
                sessionToken: sessionToken,
                contentType: state.MimeType,
                stream: dataStream);

            return commandRunner.RunCommandAsync(command,
                uploadProgress: progress,
                cancellationToken: cancellationToken).OnSuccess(uploadTask =>
                {
                    Tuple<System.Net.HttpStatusCode, System.Collections.Generic.IDictionary<string, object>> result = uploadTask.Result;
                    System.Collections.Generic.IDictionary<string, object> jsonData = result.Item2;
                    cancellationToken.ThrowIfCancellationRequested();

                    return new FileState
                    {
                        Name = jsonData["name"] as string,
                        Url = new Uri(jsonData["url"] as string, UriKind.Absolute),
                        MimeType = state.MimeType
                    };
                }).ContinueWith(t =>
                {
                    // Rewind the stream on failure or cancellation (if possible)
                    if ((t.IsFaulted || t.IsCanceled) && dataStream.CanSeek)
                    {
                        dataStream.Seek(oldPosition, SeekOrigin.Begin);
                    }
                    return t;
                }).Unwrap();
        }
    }
}