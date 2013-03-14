#region Copyright (c) 2012 SmartVault, Inc.
//  Permission is hereby granted, free of charge, to any person obtaining a copy 
//  of this software and associated documentation files (the "Software"), to deal 
//  in the Software without restriction, including without limitation the rights 
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
//  copies of the Software, and to permit persons to whom the Software is 
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//  IN THE SOFTWARE.
#endregion
using System;
using System.Collections.Generic;
using NClassify.Library;

namespace Freshbooks.Library.Model
{
    partial struct Url
    {
        public Uri ToUri()
        {
            return new Uri(Value, UriKind.Absolute);
        }
    }

    #region RecurringFrequency
    public struct RecurringFrequency : IEquatable<RecurringFrequency>, IComparable<RecurringFrequency>, IValidate
    {
        #region TypeFields
        public enum TypeFields
        {
            Value = 0
        }
        #endregion
        #region _valuesPossible

        private static readonly Dictionary<string, string> _valuesPossible =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    //server-side values
                    {"w", "weekly"},
                    {"2w", "2 weeks"},
                    {"4w", "4 weeks"},
                    {"m", "monthly"},
                    {"2m", "2 months"},
                    {"3m", "3 months"},
                    {"6m", "6 months"},
                    {"y", "yearly"},
                    {"2y", "2 years"},
                    //client-side values
                    {"weekly", "weekly"},
                    {"2 weeks", "2 weeks"},
                    {"4 weeks", "4 weeks"},
                    {"monthly", "monthly"},
                    {"2 months", "2 months"},
                    {"3 months", "3 months"},
                    {"6 months", "6 months"},
                    {"yearly", "yearly"},
                    {"2 years", "2 years"},
                };
        #endregion
        #region Instance Fields and Members
        public static bool IsValidValue(string value, Action<ValidationError> onError)
        {
            if (ReferenceEquals(null, value))
            {
                if (onError != null) onError(new ValidationError(TypeFields.Value, ResourceMessages.MustNotBeNull, TypeFields.Value));
                return false;
            }
            if (!_valuesPossible.ContainsKey(value))
            {
                if (onError != null) onError(new ValidationError(TypeFields.Value, ResourceMessages.InvalidField, TypeFields.Value));
                return false;
            }
            return true;
        }
        private bool _hasValue;
        private string _fldValue;

        public RecurringFrequency(string value)
            : this()
        {
            _hasValue = true;
            if (null == value) throw new ArgumentNullException("value");
            if (!_valuesPossible.TryGetValue(value, out _fldValue))
                _fldValue = value;
        }
        public bool HasValue
        {
            get { return _hasValue; }
        }
        public string Value
        {
            get
            {
                if (!_hasValue) return "";
                return _fldValue;
            }
        }
        public bool IsValid() { return 0 == GetBrokenRules(null); }
        public void AssertValid() { GetBrokenRules(RaiseValidationError); }
        void RaiseValidationError(ValidationError e) { e.RaiseException(); }
        public int GetBrokenRules(Action<ValidationError> onError)
        {
            int errorCount = 0;
            if (!_hasValue)
            {
                if (onError != null) onError(new ValidationError(TypeFields.Value, ResourceMessages.MissingRequiredField, TypeFields.Value));
                errorCount++;
            }
            return errorCount;
        }
        #endregion
        #region Operators and Comparisons
        public override string ToString()
        {
            return Value;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj is RecurringFrequency ? Equals((RecurringFrequency)obj) : base.Equals(obj);
        }
        public bool Equals(RecurringFrequency other)
        {
            return _hasValue && other._hasValue ? _fldValue.Equals(other._fldValue) : _hasValue == other._hasValue;
        }
        public int CompareTo(RecurringFrequency other)
        {
            return _hasValue && other._hasValue ? _fldValue.CompareTo(other._fldValue) : _hasValue ? 1 : other._hasValue ? -1 : 0;
        }
        public static explicit operator RecurringFrequency(string value)
        {
            return new RecurringFrequency(value);
        }
        public static explicit operator string(RecurringFrequency value)
        {
            return value.Value;
        }
        public static bool operator ==(RecurringFrequency x, RecurringFrequency y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(RecurringFrequency x, RecurringFrequency y)
        {
            return !x.Equals(y);
        }
        #endregion

        public static readonly RecurringFrequency EveryWeek = new RecurringFrequency("weekly");
        public static readonly RecurringFrequency Every2Weeks = new RecurringFrequency("2 weeks");
        public static readonly RecurringFrequency Every4Weeks = new RecurringFrequency("4 weeks");
        public static readonly RecurringFrequency EveryMonth = new RecurringFrequency("monthly");
        public static readonly RecurringFrequency Every2Months = new RecurringFrequency("2 months");
        public static readonly RecurringFrequency Every3Months = new RecurringFrequency("3 months");
        public static readonly RecurringFrequency Every6Months = new RecurringFrequency("6 months");
        public static readonly RecurringFrequency EveryYear = new RecurringFrequency("yearly");
        public static readonly RecurringFrequency Every2Years = new RecurringFrequency("2 years");
    }
    #endregion
}
