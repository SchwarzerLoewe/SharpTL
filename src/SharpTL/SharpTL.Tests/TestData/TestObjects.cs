// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestObjects.cs">
//   Copyright (c) 2013-2014 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using BigMath;

namespace SharpTL.Tests.TestData
{
    [TLObject(0xA1B2C3D4)]
    public class TestObject : IEquatable<TestObject>
    {
        [TLProperty(1)]
        public bool TestBoolean { get; set; }

        [TLProperty(2)]
        public double TestDouble { get; set; }

        [TLProperty(3)]
        public int TestInt { get; set; }

        [TLProperty(4)]
        public List<int> TestIntVector { get; set; }

        [TLProperty(5)]
        public long TestLong { get; set; }

        [TLProperty(6)]
        public string TestString { get; set; }

        [TLProperty(7)]
        public Int128 TestInt128 { get; set; }

        [TLProperty(8)]
        public Int256 TestInt256 { get; set; }

        [TLProperty(9)]
        public List<IUser> TestUsersVector { get; set; }

        [TLProperty(10, TLSerializationMode.Bare)]
        public List<int> TestIntBareVector { get; set; }

        [TLProperty(11)]
        public object TestInnerObject { get; set; }

        #region Equality
        public bool Equals(TestObject other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return TestBoolean.Equals(other.TestBoolean) && TestDouble.Equals(other.TestDouble) && TestInt == other.TestInt &&
                TestIntVector.SequenceEqual(other.TestIntVector) && TestLong == other.TestLong && string.Equals(TestString, other.TestString) &&
                TestInt128 == other.TestInt128 && TestInt256 == other.TestInt256 && TestUsersVector.SequenceEqual(other.TestUsersVector) &&
                TestIntBareVector.SequenceEqual(other.TestIntBareVector);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((TestObject) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = TestBoolean.GetHashCode();
                hashCode = (hashCode*397) ^ TestDouble.GetHashCode();
                hashCode = (hashCode*397) ^ TestInt;
                hashCode = (hashCode*397) ^ (TestIntVector != null ? TestIntVector.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ TestLong.GetHashCode();
                hashCode = (hashCode*397) ^ (TestString != null ? TestString.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ TestInt128.GetHashCode();
                hashCode = (hashCode*397) ^ TestInt256.GetHashCode();
                hashCode = (hashCode*397) ^ (TestUsersVector != null ? TestUsersVector.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (TestIntBareVector != null ? TestIntBareVector.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(TestObject left, TestObject right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestObject left, TestObject right)
        {
            return !Equals(left, right);
        }
        #endregion

        public override string ToString()
        {
            return "TestObject #" + GetHashCode().ToString(CultureInfo.InvariantCulture);
        }
    }

    [TLType(typeof (User), typeof (NoUser))]
    public interface IUser
    {
        int Id { get; set; }
    }

    /// <summary>
    ///     user#d23c81a3 id:int first_name:string last_name:string = User;
    /// </summary>
    [TLObject(0xD23C81A3)]
    [DebuggerDisplay("#{Id} {FirstName} {LastName}")]
    public class User : IUser, IEquatable<User>
    {
        [TLProperty(1)]
        public int Id { get; set; }

        [TLProperty(2)]
        public string FirstName { get; set; }

        [TLProperty(3)]
        public string LastName { get; set; }

        [TLProperty(4)]
        public byte[] Key { get; set; }

        #region Equality
        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Id == other.Id && string.Equals(FirstName, other.FirstName) && string.Equals(LastName, other.LastName) && Key.SequenceEqual(other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id;
                hashCode = (hashCode*397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (LastName != null ? LastName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Key != null ? Key.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(User left, User right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(User left, User right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    /// <summary>
    ///     no_user#c67599d1 id:int = User;
    /// </summary>
    [TLObject(0xC67599D1)]
    [DebuggerDisplay("#{Id}[NoUser]")]
    public class NoUser : IUser, IEquatable<NoUser>
    {
        [TLProperty(1)]
        public int Id { get; set; }

        #region Equality
        public bool Equals(NoUser other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((NoUser) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(NoUser left, NoUser right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NoUser left, NoUser right)
        {
            return !Equals(left, right);
        }
        #endregion
    }

    /// <summary>
    ///     getUsers#2d84d5f5 (Vector int) = Vector User;
    /// </summary>
    [TLObject(0x2D84D5F5)]
    public class GetUsersFunction : IEquatable<GetUsersFunction>
    {
        [TLProperty(1)]
        public List<int> Arg1 { get; set; }

        public List<IUser> Response { get; set; }

        #region Equality
        public bool Equals(GetUsersFunction other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Arg1.SequenceEqual(other.Arg1);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((GetUsersFunction) obj);
        }

        public override int GetHashCode()
        {
            return (Arg1 != null ? Arg1.GetHashCode() : 0);
        }

        public static bool operator ==(GetUsersFunction left, GetUsersFunction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GetUsersFunction left, GetUsersFunction right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
