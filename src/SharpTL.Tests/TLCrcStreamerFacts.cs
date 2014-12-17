// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TLCrcStreamerFacts.cs">
//   Copyright (c) 2013-2014 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SharpTL.Tests
{
    using System.Text;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class TLCrcStreamerFacts
    {
        [Test]
        [TestCase("boolFalse = Bool", 0xbc799737u)]
        [TestCase("boolTrue = Bool", 0x997275b5u)]
        [TestCase("vector t:Type # [ t ] = Vector t", 0x1cb5c415u)]
        [TestCase("error code:int text:string = Error", 0xc4b9f9bbu)]
        [TestCase("req_pq nonce:int128 = ResPQ", 0x60469778u)]
        public void Should_calculate_write_crc(string text, uint expectedCrc32)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            using (var crcStreamer = new TLCrcStreamer())
            {
                crcStreamer.Write(bytes);
                crcStreamer.WriteCrc.Should().Be(expectedCrc32);
            }
        }
    }
}
