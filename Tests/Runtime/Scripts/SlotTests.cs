using System;
using NUnit.Framework;
using UnityEngine;

namespace ExpressoBits.Inventories.Tests
{
    public class SlotTests
    {

        [Test]
        public void TestAmountAndInitializationOfSlot()
        {
            // Slot slot = new Slot(1) { };
            // slot.Add(2);
            // Assert.AreEqual(slot.Amount, 2);
        }

        [Test]
        public void TestAddOnSlot()
        {
            // Slot slot = new Slot(1, 1) { };
            // Assert.AreEqual(slot.Add(1), 0);
            // Assert.AreEqual(slot.Amount, 1);
            // Assert.AreEqual(slot.Add(slot.MaxStack), 1);
            // Assert.AreEqual(slot.Amount, slot.MaxStack);
        }

        [Test]
        public void TestRemoveValues()
        {
            // Slot slot = new Slot(1, 1) { };

            // slot.Add(12);

            // Assert.AreEqual(slot.Remove(5), 0);
            // Assert.AreEqual(slot.Amount, 7);
            // Assert.AreEqual(slot.Remove(12), 5);
            // Assert.AreEqual(slot.Amount, 0);
        }

        [Test]
        public void TestSpace()
        {
            // Slot slot = new Slot(1, 1) { };

            // slot.Add(12);

            // Assert.IsTrue(slot.IsSpace);

            // slot.Add(slot.MaxStack);

            // Assert.IsFalse(slot.IsSpace);
        }

        [Test]
        public void TestSplitData()
        {
            ushort itemId = 1;
            ushort amount = 2;

            byte[] b1 = BitConverter.GetBytes(itemId);
            byte[] b2 = BitConverter.GetBytes(amount);
            uint s = (uint)(b1[0] | (b1[1] << 8) | (b2[0] << 16) | (b2[1] << 24));
            Assert.AreEqual(itemId, Slot.SplitInt(s).Item1);
            Assert.AreEqual(amount, Slot.SplitInt(s).Item2);
        }


    }
}