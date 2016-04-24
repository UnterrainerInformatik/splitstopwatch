// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System;
using System.Diagnostics;
using System.IO;

namespace SplitStopWatch
{
    /// <summary>
    ///     This class implements a stopwatch that may be used to debug out
    ///     split-times as well. It measures the split-times and keeps track of
    ///     the overall times in a variable. It may be used in the same way as one
    ///     would use a System.Diagnostic.Stopwatch. Don't be afraid to stop the
    ///     watch. Stopping doesn't mean you loose any value whatsoever. Think of
    ///     it as a real stopwatch where you may press the start-button at any
    ///     time after previously pressing the stop-button.
    ///     This class provides useful overloads that allow writing to a
    ///     <see cref="TextWriter" /> in a way that your measurement doesn't get
    ///     compromised (the stopwatch is paused while writing to the stream). You
    ///     may initialize it with a <see cref="TextWriter" /> so that you can
    ///     use all the overloads that take a string-argument or Console.Out is
    ///     used as a default. All the write-operations are performed as a
    ///     <c>WriteLine</c>, so you don't need to close your assigned text with a
    ///     newline-character.
    /// </summary>
    public class SplitStopwatch
    {
        /// <summary>
        ///     This format string displays the total time, split-time and your text separated by a pipe.
        /// </summary>
        public const string FORMAT_STRING_TOTAL_SPLIT_OWN = "| total: {0,10}ms | since last split: {1,10}ms | {2}";

        /// <summary>
        ///     This format string displays the split-time, total time and your text separated by a pipe.
        /// </summary>
        public const string FORMAT_STRING_SPLIT_TOTAL_OWN = "| since last split: {1,10}ms | total: {0,10}ms | {2}";

        private Stopwatch stopwatch = new Stopwatch();
        private long totalTimeInMilliseconds;
        private long totalTimeInTicks;
        private readonly TextWriter textWriter;

        public bool IsActive { get; set; }
        public bool IsFlushImmediately { get; set; }
        public int CurrentIndentLevel { get; set; }
        public string IndentString { get; set; }

        /// <summary>
        ///     Gets or sets the prefix format string that generates the string
        ///     appearing in front of each of your text-calls.
        /// </summary>
        /// <value>
        ///     The prefix format string. With the following format-items:
        ///     <c>
        ///         {0} is the total time elapsed in milliseconds.
        ///         {1} is the time that has elapsed since the last split-time in milliseconds.
        ///         {2} is the text you specified via the text-parameter of the method you called.
        ///         Default value is:
        ///         | total: {0,10}ms | since last split: {1,10}ms | {2}
        ///     </c>
        /// </value>
        public string PrefixFormatString { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the prefix should be
        ///     displayed every time you use a method with a text-field.
        /// </summary>
        /// <value>
        ///     <c>true</c> if to display the prefix every time; otherwise, <c>false</c>.
        /// </value>
        public bool DisplayPrefix { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get { return stopwatch.IsRunning; }
        }

        /// <summary>
        ///     Gets the elapsed time since the last split.
        /// </summary>
        /// <value>The elapsed time since the last split.</value>
        public TimeSpan ElapsedSinceLastSplit
        {
            get { return stopwatch.Elapsed; }
        }

        /// <summary>
        ///     Gets the elapsed time since the last split in milliseconds.
        /// </summary>
        /// <value>The elapsed time since the last split in milliseconds.</value>
        public long ElapsedSinceLastSplitMilliseconds
        {
            get { return stopwatch.ElapsedMilliseconds; }
        }

        /// <summary>
        ///     Gets the elapsed time since the last split in timer ticks.
        /// </summary>
        /// <value>The elapsed time since the last split in timer ticks.</value>
        public long ElapsedSinceLastSplitTicks
        {
            get { return stopwatch.ElapsedTicks; }
        }

        /// <summary>
        ///     Gets the total elapsed time since the last start.
        /// </summary>
        /// <value>The elapsed total time.</value>
        public TimeSpan ElapsedTotal
        {
            get { return TimeSpan.FromMilliseconds(totalTimeInMilliseconds + stopwatch.ElapsedMilliseconds); }
        }

        /// <summary>
        ///     Gets the total elapsed time since the last start in milliseconds.
        /// </summary>
        /// <value>The elapsed total time in milliseconds.</value>
        public long ElapsedTotalMilliseconds
        {
            get { return totalTimeInMilliseconds + stopwatch.ElapsedMilliseconds; }
        }

        /// <summary>
        ///     Gets the total elapsed time since the last start in timer ticks.
        /// </summary>
        /// <value>The elapsed total time in timer ticks.</value>
        public long ElapsedTotalTicks
        {
            get { return totalTimeInTicks + stopwatch.ElapsedTicks; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplitStopwatch" />
        ///     class. The default <see cref="TextWriter" /> is Console.Out.
        /// </summary>
        public SplitStopwatch()
        {
            IsActive = true;
            PrefixFormatString = FORMAT_STRING_TOTAL_SPLIT_OWN;
            DisplayPrefix = true;
            IndentString = "  ";
            textWriter = Console.Out;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SplitStopwatch" />class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="isFlushImmediately">
        ///     if set to <c>true</c> the writer is
        ///     immediately flushed every time a write is done.
        /// </param>
        public SplitStopwatch(TextWriter textWriter, bool isFlushImmediately = false)
        {
            IsActive = true;
            PrefixFormatString = FORMAT_STRING_TOTAL_SPLIT_OWN;
            DisplayPrefix = true;
            IndentString = "  ";
            this.textWriter = textWriter;
            IsFlushImmediately = isFlushImmediately;
        }

        /// <summary>
        ///     Starts, or resumes measuring elapsed time for an interval. You may
        ///     start after a stop as well.
        /// </summary>
        public void Start()
        {
            if (IsActive)
            {
                stopwatch.Start();
            }
        }

        /// <summary>
        ///     Starts, or resumes measuring elapsed time for an interval. You may
        ///     start after a stop as well.
        /// </summary>
        /// <param name="text">
        ///     The text to output on the
        ///     <see cref="TextWriter" /> you specified when creating this instance
        ///     or Console.Out as the default
        ///     <see cref="TextWriter" />.
        /// </param>
        /// <param name="indentLevel">The indent level.</param>
        public void Start(string text, int indentLevel = 0)
        {
            if (IsActive)
            {
                var timeSinceLastSplitInMilliseconds = stopwatch.ElapsedMilliseconds;
                _WriteText(text, timeSinceLastSplitInMilliseconds, indentLevel);
                stopwatch.Start();
            }
        }

        /// <summary>
        ///     Initializes a new instance, sets the elapsed time property to
        ///     zero, and starts measuring elapsed time.
        /// </summary>
        public void StartNew()
        {
            if (IsActive)
            {
                totalTimeInMilliseconds = 0;
                totalTimeInTicks = 0;
                stopwatch = Stopwatch.StartNew();
            }
        }

        /// <summary>
        ///     Initializes a new instance, sets the elapsed time property to
        ///     zero, and starts measuring elapsed time.
        /// </summary>
        /// <param name="text">
        ///     The text to output on the
        ///     <see cref="TextWriter" /> you specified when creating this instance
        ///     or Console.Out as the default
        ///     <see cref="TextWriter" />.
        /// </param>
        /// <param name="indentLevel">The indent level.</param>
        public void StartNew(string text, int indentLevel = 0)
        {
            if (IsActive)
            {
                var timeSinceLastSplitInMilliseconds = stopwatch.ElapsedMilliseconds;
                _WriteText(text, timeSinceLastSplitInMilliseconds, indentLevel);
                totalTimeInMilliseconds = 0;
                totalTimeInTicks = 0;
                stopwatch = Stopwatch.StartNew();
            }
        }

        /// <summary>
        ///     Stops measuring elapsed time for an interval. You may stop and
        ///     restart afterward resulting in a pause of the timer.
        /// </summary>
        public void Stop()
        {
            if (IsActive)
            {
                stopwatch.Stop();
            }
        }

        /// <summary>
        ///     Stops measuring elapsed time for an interval. You may stop and
        ///     restart afterward resulting in a pause of the timer.
        /// </summary>
        /// <param name="text">
        ///     The text to output on the
        ///     <see cref="TextWriter" /> you specified when creating this instance
        ///     or Console.Out as the default
        ///     <see cref="TextWriter" />.
        /// </param>
        /// <param name="indentLevel">The indent level.</param>
        public void Stop(string text, int indentLevel = 0)
        {
            if (IsActive)
            {
                stopwatch.Stop();
                var timeSinceLastSplitInMilliseconds = stopwatch.ElapsedMilliseconds;
                _WriteText(text, timeSinceLastSplitInMilliseconds, indentLevel);
            }
        }

        /// <summary>
        ///     Stops time interval measurement and resets the time to zero.
        /// </summary>
        public void Reset()
        {
            if (IsActive)
            {
                stopwatch.Reset();
                totalTimeInMilliseconds = 0;
                totalTimeInTicks = 0;
            }
        }

        /// <summary>
        ///     Stops time interval measurement and resets the time to zero.
        /// </summary>
        /// <param name="text">
        ///     The text to output on the
        ///     <see cref="TextWriter" /> you specified when creating this instance
        ///     or Console.Out as the default
        ///     <see cref="TextWriter" />.
        /// </param>
        /// <param name="indentLevel">The indent level.</param>
        public void Reset(string text, int indentLevel = 0)
        {
            if (IsActive)
            {
                var timeSinceLastSplitInMilliseconds = SplitAndStop();
                stopwatch.Reset();
                _WriteText(text, timeSinceLastSplitInMilliseconds, indentLevel);
                totalTimeInMilliseconds = 0;
                totalTimeInTicks = 0;
            }
        }

        /// <summary>
        ///     Takes a split-time and restarts the timer.
        /// </summary>
        public long Split()
        {
            if (IsActive)
            {
                var timeSinceLastSplitInMilliseconds = SplitAndStop();
                stopwatch.Start();
                return timeSinceLastSplitInMilliseconds;
            }
            return 0L;
        }

        /// <summary>
        ///     Takes a split-time and restarts the timer.
        /// </summary>
        /// <param name="text">
        ///     The text to output on the
        ///     <see cref="TextWriter" /> you specified when creating this instance
        ///     or Console.Out as the default
        ///     <see cref="TextWriter" />.
        /// </param>
        /// <param name="indentLevel">The indent level.</param>
        /// <returns></returns>
        public long Split(string text, int indentLevel = 0)
        {
            if (IsActive)
            {
                var timeSinceLastSplitInMilliseconds = SplitAndStop();
                _WriteText(text, timeSinceLastSplitInMilliseconds, indentLevel);
                stopwatch.Start();
                return timeSinceLastSplitInMilliseconds;
            }
            return 0L;
        }

        /// <summary>
        ///     Takes a split-time and does not restart the timer. You may restart
        ///     it with start again at any time.
        /// </summary>
        /// <returns>The split-time.</returns>
        public long SplitAndStop()
        {
            if (IsActive)
            {
                stopwatch.Stop();
                var timeSinceLastSplitInMilliseconds = stopwatch.ElapsedMilliseconds;
                var timeSinceLastSplitInTicks = stopwatch.ElapsedTicks;
                totalTimeInMilliseconds += timeSinceLastSplitInMilliseconds;
                totalTimeInTicks += timeSinceLastSplitInTicks;
                stopwatch.Reset();
                return timeSinceLastSplitInMilliseconds;
            }
            return 0L;
        }

        /// <summary>
        ///     Takes a split-time and does not restart the timer. You may restart
        ///     it with start again at any time.
        /// </summary>
        /// <param name="text">
        ///     The text to output on the
        ///     <see cref="TextWriter" /> you specified when creating this instance
        ///     or Console.Out as the default
        ///     <see cref="TextWriter" />.
        /// </param>
        /// <param name="indentLevel">The indent level.</param>
        /// <returns>The split-time.</returns>
        public long SplitAndStop(string text, int indentLevel = 0)
        {
            if (IsActive)
            {
                stopwatch.Stop();
                var timeSinceLastSplitInMilliseconds = stopwatch.ElapsedMilliseconds;
                var timeSinceLastSplitInTicks = stopwatch.ElapsedTicks;
                totalTimeInMilliseconds += timeSinceLastSplitInMilliseconds;
                totalTimeInTicks += timeSinceLastSplitInTicks;
                stopwatch.Reset();
                _WriteText(text, timeSinceLastSplitInMilliseconds, indentLevel);
                return timeSinceLastSplitInMilliseconds;
            }
            return 0L;
        }

        private void _WriteText(string text, long elapsedTimeSinceLastSplitMilliseconds, int indentLevel)
        {
            if (IsActive)
            {
                if (indentLevel > 0)
                {
                    for (var i = 0; i < indentLevel; i++)
                    {
                        textWriter.Write(IndentString);
                    }
                }
                if (DisplayPrefix)
                {
                    textWriter.WriteLine(PrefixFormatString, ElapsedTotalMilliseconds, elapsedTimeSinceLastSplitMilliseconds, text);
                }
                else
                {
                    textWriter.WriteLine(text);
                }
                if (IsFlushImmediately)
                {
                    textWriter.Flush();
                }
            }
        }
    }
}