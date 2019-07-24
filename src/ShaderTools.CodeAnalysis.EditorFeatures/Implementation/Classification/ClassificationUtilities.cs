﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using ShaderTools.CodeAnalysis.Editor.Shared.Utilities;
using ShaderTools.CodeAnalysis.Text.Shared.Extensions;

namespace ShaderTools.CodeAnalysis.Editor.Implementation.Classification
{
    internal static class ClassificationUtilities
    {
        private static readonly ConcurrentQueue<List<ClassifiedSpan>> s_spanCache = new ConcurrentQueue<List<ClassifiedSpan>>();

        public static List<ClassifiedSpan> GetOrCreateClassifiedSpanList()
        {
            return s_spanCache.TryDequeue(out var result)
                ? result
                : new List<ClassifiedSpan>();
        }

        public static void ReturnClassifiedSpanList(List<ClassifiedSpan> list)
        {
            if (list == null)
            {
                return;
            }

            list.Clear();
            s_spanCache.Enqueue(list);
        }

        public static void Convert(ClassificationTypeMap typeMap, ITextSnapshot snapshot, List<ClassifiedSpan> list, Action<ITagSpan<IClassificationTag>> addTag)
        {
            foreach (var classifiedSpan in list)
            {
                addTag(new TagSpan<IClassificationTag>(
                    classifiedSpan.TextSpan.ToSnapshotSpan(snapshot),
                    new ClassificationTag(typeMap.GetClassificationType(classifiedSpan.ClassificationType))));
            }
        }
    }
}
