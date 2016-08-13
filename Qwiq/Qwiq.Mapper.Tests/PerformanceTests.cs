﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.IE.IEPortal.BehaviorDrivenDevelopmentTools;
using Microsoft.IE.Qwiq.Mapper.Attributes;
using Microsoft.IE.Qwiq.Mocks;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace Microsoft.IE.Qwiq.Mapper.Tests
{
    // POCO serialization pulling static typed objects
    // When running benchmarks, be sure to compile in Release and not attach a debugger
    [TestClass]
    public class PerformanceContext : ContextSpecification
    {
        public override void Given()
        {
            var propertyInspector = new PropertyInspector(new PropertyReflector());
            var typeParser = new TypeParser();
            var mappingStrategies = new IWorkItemMapperStrategy[]
                                        { new AttributeMapperStrategy(propertyInspector, typeParser) };
            WorkItemMapper = new WorkItemMapper(mappingStrategies);

            //
            Func<Type, object> getRandomValue = (propertyType) =>
                {
                    const string Chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
                    var randomizer = Randomizer.Instance;

                    object value;
                    switch (propertyType.ToString())
                    {
                        case "System.Boolean":
                            value = Randomizer.ShouldEnter();
                            break;
                        case "System.Byte":
                            var @byte = new byte[1];
                            randomizer.NextBytes(@byte);
                            value = @byte[0];
                            break;
                        case "System.Double":
                            value = randomizer.NextDouble();
                            break;
                        case "System.Int32":
                            value = randomizer.Next();
                            break;
                        case "System.Int64":
                            value = (long)randomizer.Next();
                            break;
                        case "System.SByte":
                            var @sbyte = new byte[1];
                            randomizer.NextBytes(@sbyte);
                            value = (sbyte)@sbyte[0];
                            break;
                        case "System.Int16":
                            value = (short)randomizer.Next(1 << 16);
                            break;
                        case "System.Single":
                            var singleBytes = new byte[8];
                            randomizer.NextBytes(singleBytes);
                            value = BitConverter.ToSingle(singleBytes, 0);
                            break;
                        case "System.UInt16":
                            var shortBytes = new byte[2];
                            randomizer.NextBytes(shortBytes);
                            value = BitConverter.ToUInt16(shortBytes, 0);
                            break;
                        case "System.UInt32":
                            var bytes = new byte[4];
                            randomizer.NextBytes(bytes);
                            value = BitConverter.ToUInt32(bytes, 0);
                            break;
                        case "System.UInt64":
                            var longBytes = new byte[8];
                            randomizer.NextBytes(longBytes);
                            value = BitConverter.ToUInt64(longBytes, 0);
                            break;
                        case "System.String":
                            value =
                                new string(
                                    Enumerable.Repeat(Chars, randomizer.Next(5, 250))
                                              .Select(s => s[randomizer.Next(s.Length)])
                                              .ToArray());
                            break;
                        case "System.DateTime":
                            var range = DateTime.MaxValue - DateTime.MinValue;
                            var randTimeSpan = new TimeSpan((long)(randomizer.NextDouble() * range.Ticks));
                            value = DateTime.MinValue + randTimeSpan;
                            break;
                        case "System.Guid":
                            value = Guid.NewGuid();
                            break;
                        case "System.TimeSpan":
                            value = new TimeSpan(randomizer.Next());
                            break;

                        default:
                            value = new object();
                            break;
                    }
                    return value;

                };

            // Create 500 objects to map
            var items = new List<IWorkItem>(500);
            for (int i = 0; i < 500; i++)
            {
                var instance = new MockWorkItem("Baz");
                foreach (
                    var property in
                        typeof(MockWorkItem).GetProperties(
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty))
                {
                    var value = getRandomValue(property.PropertyType);
                    try
                    {
                        property.SetValue(instance, value);
                    }
                    catch (TargetParameterCountException)
                    {
                        // Best effort
                        // May fail with index properties
                    }
                    catch (ArgumentException)
                    {
                        // Best effort
                        // May fail because the setter is not available
                    }
                }

                items.Add(instance);
            }

            Items = items;
        }

        public override void When()
        {
            // Loop 25 times and collect data
            var results = new double[25];

            for (var i = 0; i < results.Length; i++)
            {
                var sw = new Stopwatch();
                sw.Start();

                var mockModels = WorkItemMapper.Create<MockModel>(Items).ToList();

                sw.Stop();
                results[i] = sw.Elapsed.TotalMilliseconds;
            }

            // Calculate the mean and standard deviation values through the benchmark
            var mean = results.Average();

            // see: http://www.mathsisfun.com/data/standard-deviation.html
            var sd = Math.Sqrt(results.Select(v => v - mean).Select(v => v * v).Average());

            Console.WriteLine("N = {0}, Mean = {1:F2}ms, SD = {2:F2}ms", results.Length, mean, sd);
            for (int i = 0; i < results.Length; i++)
            {
                Console.WriteLine("{0}  : {1:F2}", i+1, results[i]);
            }

        }


        protected IEnumerable<IWorkItem> Items { get; set; }

        protected WorkItemMapper WorkItemMapper { get; set; }

        private class Randomizer : Random
        {
            private static Randomizer random;

            public static Randomizer Instance => random ?? (random = new Randomizer());

            public static bool ShouldEnter()
            {
                return Instance.NextDouble() < 0.5;
            }


            public static int NextInt32()
            {
                unchecked
                {
                    var firstBits = Instance.Next(0, 1 << 4) << 28;
                    var lastBits = Instance.Next(0, 1 << 28);
                    return firstBits | lastBits;
                }
            }

            /// <summary>
            /// Taken from http://stackoverflow.com/questions/609501/generating-a-random-decimal-in-c-sharp Jon Skeet's answer
            /// </summary>
            public static decimal NextDecimal()
            {
                var scale = (byte)Instance.Next(29);
                var sign = Instance.Next(2) == 1;
                return new decimal(NextInt32(), NextInt32(), NextInt32(), sign, scale);
            }
        }

        [WorkItemType("Baz")]
        public class MockModel
        {
            private DateTime _openedDate;

            private DateTime _changedDate;

            private DateTime _revisedDate;

            private DateTime? _closedDate;

            private string _milestone;

            private string _title;

            private string _assignedTo;

            private string _status;

            private string _resolution;

            private string _database;

            private string _keywords;

            private string _openedBy;

            private string _treePath;

            private string _closedBy;

            private string _tags;

            private string _history;

            private string _issueType;

            private string _productFamily;

            private string _product;

            private string _release;

            private string _releaseType;

            [JsonIgnore]
            public int Id
            {
                get
                {
                    return ID;
                }
                set
                {
                    ID = value;
                }
            }

            [FieldDefinition("Id")]
            public virtual int ID { get; set; }

            /// <summary>
            /// The title of the issue.
            /// </summary>
            [FieldDefinition("Title")]
            public virtual string Title
            {
                get
                {
                    return _title ?? string.Empty;
                }
                set
                {
                    _title = value;
                }
            }

            /// <summary>
            /// The priority of the issue (e.g. 1, 2, etc.).
            /// </summary>
            [FieldDefinition("Priority")]
            public virtual int? Priority { get; set; }

            /// <summary>
            /// The alias of the user to which this issue is assigned.
            /// </summary>
            [FieldDefinition("Assigned To")]

            public virtual string AssignedTo
            {
                get
                {
                    return _assignedTo ?? string.Empty;
                }
                set
                {
                    _assignedTo = value;
                }
            }

            /// <summary>
            /// The string of this issue's status (e.g. 'Active', 'Closed - Completed', etc.).
            /// </summary>
            [FieldDefinition("State")]
            public virtual string Status
            {
                get
                {
                    return _status ?? string.Empty;
                }
                set
                {
                    _status = value;
                }
            }

            /// <summary>
            /// The string of this issue's status (e.g. 'By Design', 'Submitted', 'Won't Fix', etc.).
            /// </summary>
            [FieldDefinition("Resolved Reason")]
            public virtual string Resolution
            {
                get
                {
                    return _resolution ?? string.Empty;
                }
                set
                {
                    _resolution = value;
                }
            }

            /// <summary>
            /// The string of this issue's milestone (e.g. 'M1 Coding', 'M1', etc.).
            /// </summary>
            [FieldDefinition("Iteration Path")]
            public virtual string Milestone
            {
                get
                {
                    return _milestone ?? string.Empty;
                }
                set
                {
                    _milestone = value;
                }
            }

            /// <summary>
            /// The DateTime when this issue was opened in the local timezone.
            /// </summary>
            [FieldDefinition("Created Date")]
            public virtual DateTime OpenedDate
            {
                get
                {
                    return _openedDate;
                }
                set
                {
                    if (value.Kind == DateTimeKind.Unspecified)
                    {
                        _openedDate = TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.Local);
                    }
                    else
                    {
                        _openedDate = value;
                    }
                }
            }

            /// <summary>
            /// The user that opened this issue.
            /// </summary>
            [FieldDefinition("Created By")]

            public virtual string OpenedBy
            {
                get
                {
                    return _openedBy ?? string.Empty;
                }
                set
                {
                    _openedBy = value;
                }
            }

            /// <summary>
            /// The issue's keywords text field. If there are no keywords it returns an empty string.
            /// </summary>
            [FieldDefinition("Keywords")]
            public virtual string KeyWords
            {
                get
                {
                    return _keywords ?? string.Empty;
                }
                set
                {
                    _keywords = value;
                }
            }

            /// <summary>
            /// Used for "As Of" queries. For each revision of an issue, the ChangedDate is when the issue was modified
            /// (or opened for the first revision), and that bug is considered current until <see cref="RevisedDate"/>.
            /// If the revision is the lastest revision, the revised date is 9999/01/01 to avoid NULL values.
            /// </summary>
            [FieldDefinition("Changed Date")]
            public virtual DateTime ChangedDate
            {
                get
                {
                    return _changedDate;
                }
                set
                {
                    if (value.Kind == DateTimeKind.Unspecified)
                    {
                        _changedDate = TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.Local);
                    }
                    else
                    {
                        _changedDate = value;
                    }
                }
            }

            /// <summary>
            /// The DateTime when this issue was last modified.
            /// </summary>
            [FieldDefinition("Revised Date")]
            public virtual DateTime RevisedDate
            {
                get
                {
                    return _revisedDate;
                }
                set
                {
                    if (value.Kind == DateTimeKind.Unspecified)
                    {
                        _revisedDate = TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.Local);
                    }
                    else
                    {
                        _revisedDate = value;
                    }
                }
            }

            /// <summary>
            /// The database from which this issue comes.
            /// </summary>
            public virtual string Database
            {
                get
                {
                    if (_database == null)
                    {
                        _database = Regex.Replace(TreePath, @"\\.*", ""); // Remove everything after the first slash '\'

                        if (_database == "windows blue bugs")
                        {
                            _database = "Windows Blue Bugs";
                        }
                        else if (_database == "windows blue features")
                        {
                            _database = "Windows Blue Features";
                        }
                    }

                    return _database;
                }
            }

            /// <summary>
            /// The 'tree path' or 'area path' of the issue (e.g. \IE-Internet Explorer\COMP-Composition and Rendering\).
            /// </summary>
            [FieldDefinition("Area Path")]
            public virtual string TreePath
            {
                get
                {
                    return _treePath ?? string.Empty;
                }
                set
                {
                    _treePath = value;
                }
            }

            /// <summary>
            /// Type of issue this object represents (e.g. "Code Bug", "Dev Task", "Spec Bug", "Buffer", etc.)
            /// </summary>
            [FieldDefinition("Issue Type")]
            public virtual string IssueType
            {
                get
                {
                    return _issueType ?? string.Empty;
                }
                set
                {
                    _issueType = value;
                }
            }

            [FieldDefinition("Work Item Type")]
            public virtual string WorkItemType { get; set; }

            /// <summary>
            /// The alias of the user that closed this issue.
            /// </summary>
            [FieldDefinition("Closed By")]
            public virtual string ClosedBy
            {
                get
                {
                    return _closedBy ?? string.Empty;
                }
                set
                {
                    _closedBy = value;
                }
            }

            /// <summary>
            /// The DateTime when this issue was closed in the local timezone. Null if the issue is still open.
            /// </summary>
            [FieldDefinition("Closed Date")]
            public virtual DateTime? ClosedDate
            {
                get
                {
                    return _closedDate;
                }

                set
                {
                    if (value.HasValue)
                    {
                        if (value.Value.Kind == DateTimeKind.Unspecified)
                        {
                            _closedDate = TimeZoneInfo.ConvertTimeFromUtc(value.Value, TimeZoneInfo.Local);
                        }
                        else
                        {
                            _closedDate = value.Value;
                        }
                    }
                }
            }

            [FieldDefinition("Product Family")]
            public virtual string ProductFamily
            {
                get
                {
                    return _productFamily ?? string.Empty;
                }
                set
                {
                    _productFamily = value;
                }
            }

            [FieldDefinition("Product")]
            public virtual string Product
            {
                get
                {
                    return _product ?? string.Empty;
                }
                set
                {
                    _product = value;
                }
            }

            [FieldDefinition("Release")]
            public virtual string Release
            {
                get
                {
                    return _release ?? string.Empty;
                }
                set
                {
                    _release = value;
                }
            }

            [FieldDefinition("Tags")]
            public virtual string Tags
            {
                get
                {
                    return _tags ?? string.Empty;
                }
                set
                {
                    _tags = value;
                }
            }

            [FieldDefinition("Release Type")]
            public virtual string ReleaseType
            {
                get
                {
                    return _releaseType ?? string.Empty;
                }
                set
                {
                    _releaseType = value;
                }
            }

            [FieldDefinition("History")]
            public virtual string History
            {
                get
                {
                    return _history ?? string.Empty;
                }
                set
                {
                    _history = value;
                }
            }
        }

        [TestMethod]
        public void Execute_Performance()
        {
            // Intentionally left blank
        }
    }
}
