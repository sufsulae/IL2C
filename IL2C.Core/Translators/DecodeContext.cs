﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using IL2C.Metadata;

namespace IL2C.Translators
{
    internal sealed class DecodeContext
    {
        #region Private types
        private sealed class StackInformationHolder
        {
            private readonly List<VariableInformation> typedStackInformation;
            private readonly int stackPointer;
            private int selectedStackInformation = -1;

            private StackInformationHolder(
                int stackPointer, List<VariableInformation> typedStackInformation)
            {
                this.stackPointer = stackPointer;
                this.typedStackInformation = typedStackInformation;
            }

            public StackInformationHolder(int stackPointer)
                : this(stackPointer, new List<VariableInformation>())
            {
            }

            public string GetOrAdd(ITypeInformation targetType, IMethodInformation method, object hintInformation)
            {
                var index = typedStackInformation.
                    FindIndex(si => si.TargetType == targetType);
                if (index >= 0)
                {
                    selectedStackInformation = index;
                    return typedStackInformation[index].SymbolName;
                }

                var symbolName = string.Format(
                    "stack{0}_{1}__",
                    stackPointer,
                    typedStackInformation.Count);

                selectedStackInformation = typedStackInformation.Count;
                var stackInformation = new VariableInformation(
                    method,
                    stackPointer,
                    symbolName,
                    targetType,
                    hintInformation);
                typedStackInformation.Add(stackInformation);

                return symbolName;
            }

            public VariableInformation GetCurrent()
            {
                Debug.Assert(selectedStackInformation >= 0);
                Debug.Assert(typedStackInformation.Any());

                return typedStackInformation[selectedStackInformation];
            }

            public IEnumerable<VariableInformation> ExtractStacks() => typedStackInformation;

            public override string ToString() =>
                string.Format("[{0}]", string.Join(", ", typedStackInformation.Select(si => si.TargetType.FriendlyName)));
        }

        private struct StackSnapshot
        {
            public readonly int Offset;
            public readonly VariableInformation[] StackInformations;

            public StackSnapshot(
                int offset,
                int stackInformationsPosition,
                IEnumerable<StackInformationHolder> stackInformationHolders)
            {
                this.Offset = offset;
                this.StackInformations = stackInformationHolders.
                    Take(stackInformationsPosition).
                    Select(stackInformationList => stackInformationList.GetCurrent()).
                    ToArray();
            }
        }
        #endregion

        #region Fields
        public readonly IMethodInformation Method;
        public readonly IPrepareContext PrepareContext;

        private int nextOffset = -1;
        private ICodeInformation currentCode;

        private int decodingPathNumber = 0;
        private readonly Dictionary<int, StackSnapshot> stackSnapshortsAtOffset =
            new Dictionary<int, StackSnapshot>();

        private readonly List<StackInformationHolder> stackList =
            new List<StackInformationHolder>();
        private int stackPointer = -1;
        private readonly Dictionary<int, string> labelNames = 
            new Dictionary<int, string>();
        private readonly Dictionary<int, string> catchExpressions = 
            new Dictionary<int, string>();
        private readonly Queue<StackSnapshot> pathRemains =
            new Queue<StackSnapshot>();
        #endregion

        public DecodeContext(
            IMethodInformation method,
            IPrepareContext prepareContext)
        {
            Debug.Assert(method.HasBody && (method.CodeStream.Count >= 1));

            this.Method = method;
            this.PrepareContext = prepareContext;

            // First valid process is TryDequeueNextPath.
            this.pathRemains.Enqueue(new StackSnapshot(0, 0, new StackInformationHolder[0]));

            // Add exception handler paths.
            if (method.CodeStream.ExceptionHandlers.Any(eh => eh.CatchHandlers.Any()))
            {
                var stackInformation = new StackInformationHolder(0);
                stackList.Add(stackInformation);

                foreach (var eh in method.CodeStream.ExceptionHandlers)
                {
                    foreach (var ech in eh.CatchHandlers)
                    {
                        switch (ech.CatchHandlerType)
                        {
                            case ExceptionCatchHandlerTypes.Catch:
                                var symbolName = stackInformation.GetOrAdd(ech.CatchType, method, null);
                                catchExpressions.Add(ech.CatchStart, symbolName);

                                // TODO: stack position rarely cause mismatched.
                                //   If already pushed some values before try block,
                                //   the stackpointer progressed.
                                this.pathRemains.Enqueue(new StackSnapshot(
                                    ech.CatchStart, 1 /* ??? */, stackList));
                                break;
                        }
                    }
                }
            }
        }

        #region Instruction
        public bool MoveNext()
        {
            // Finish if current position already decoded.
            if (stackSnapshortsAtOffset.TryGetValue(nextOffset, out var stackSnapshot))
            {
                currentCode = null;
                return false;
            }

            stackSnapshot = new StackSnapshot(nextOffset, stackPointer, stackList);
            stackSnapshortsAtOffset.Add(nextOffset, stackSnapshot);

            if (this.Method.CodeStream.TryGetValue(nextOffset, out var codeInformation) == false)
            {
                throw new InvalidProgramSequenceException(
                    "End of method body reached: Method={0}",
                    this.Method.FriendlyName);
            }
            currentCode = codeInformation;

            nextOffset = codeInformation.Offset + codeInformation.Size;

            return true;
        }

        public ICodeInformation CurrentCode => currentCode;

        public int CalculateByRelativeOffset(int offsetValue)
        {
            return nextOffset + offsetValue;
        }

        public void SetOffset(int newOffset)
        {
            Debug.Assert(decodingPathNumber >= 1);
            Debug.Assert(stackList != null);
            Debug.Assert(stackPointer >= 0);

            if (this.Method.CodeStream.Contains(newOffset) == false)
            {
                throw new InvalidProgramSequenceException(
                    "Invalid branch target: Location={0}, Target={1}",
                    this.CurrentCode.RawLocation,
                    newOffset);
            }

            nextOffset = newOffset;
        }
        #endregion

        #region Stack
        public string PushStack(ITypeInformation targetType, object hintInformation = null)
        {
            Debug.Assert(decodingPathNumber >= 1);
            Debug.Assert(stackList != null);
            Debug.Assert(stackPointer >= 0);

            StackInformationHolder stackInformationHolder;
            if (stackPointer >= stackList.Count)
            {
                stackInformationHolder = new StackInformationHolder(stackPointer);
                stackList.Add(stackInformationHolder);
            }
            else
            {
                stackInformationHolder = stackList[stackPointer];
            }

            stackPointer++;

            return stackInformationHolder.GetOrAdd(targetType, this.Method, hintInformation);
        }

        public VariableInformation PopStack()
        {
            Debug.Assert(decodingPathNumber >= 1);
            Debug.Assert(stackList != null);
            Debug.Assert(stackPointer >= 1);

            if (stackPointer <= 0)
            {
                throw new InvalidProgramSequenceException(
                    "Evaluation stack underflow: Method={0}, CurrentIndex={1}",
                    this.Method.FriendlyName,
                    nextOffset);
            }

            stackPointer--;
            return stackList[stackPointer].GetCurrent();
        }
        #endregion

        #region Path
        public int UniqueCodeBlockIndex { get; private set; }

        public int DecodingPathNumber => decodingPathNumber;

        public string EnqueueNewPath(int targetOffset)
        {
            Debug.Assert(decodingPathNumber >= 1);
            Debug.Assert(stackList != null);
            Debug.Assert(stackPointer >= 0);

            pathRemains.Enqueue(new StackSnapshot(
                targetOffset, stackPointer, stackList));

            if (labelNames.TryGetValue(
                targetOffset, out var labelName) == false)
            {
                labelName = MetadataUtilities.GetLabelName(targetOffset);
                labelNames.Add(targetOffset, labelName);
            }

            return labelName;
        }

        public bool TryDequeueNextPath()
        {
            // Finish if remains path is empty.
            while (pathRemains.Count >= 1)
            {
                // Get queued branch target.
                var beforeBranchStackSnapshot = pathRemains.Dequeue();

                // If current position already decoded:
                if (stackSnapshortsAtOffset.TryGetValue(
                    beforeBranchStackSnapshot.Offset,
                    out var stackSnapshot))
                {
                    // Skip if stack information equals.
                    if (stackSnapshot.StackInformations.SequenceEqual(
                        beforeBranchStackSnapshot.StackInformations))
                    {
                        continue;
                    }

                    // Same but has to reinterpret code block:
                    //   It will interpret already through (but bit different) path,
                    //   these code blocks assigned at same IL offset.
                    //   IL2C has to split blocks by this unique code block index.
                    this.UniqueCodeBlockIndex++;
                }

                // Start next path.
                decodingPathNumber++;
                nextOffset = beforeBranchStackSnapshot.Offset;

                // Retreive stack informations.
                for (var index = 0;
                    index < beforeBranchStackSnapshot.StackInformations.Length;
                    index++)
                {
                    stackList[index].GetOrAdd(
                        beforeBranchStackSnapshot.StackInformations[index].TargetType,
                        this.Method,
                        beforeBranchStackSnapshot.StackInformations[index].HintInformation);
                }
                stackPointer = beforeBranchStackSnapshot.StackInformations.Length;

                return true;
            }

            nextOffset = -1;
            return false;
        }
        #endregion

        #region Extractors
        public IEnumerable<VariableInformation> ExtractStacks()
        {
            return stackList
                .SelectMany(stackInformations => stackInformations.ExtractStacks());
        }

        public IReadOnlyDictionary<int, string> ExtractLabelNames()
        {
            return labelNames;
        }

        public IReadOnlyDictionary<int, string> ExtractCatchExpressions()
        {
            return catchExpressions;
        }
        #endregion
    }
}
