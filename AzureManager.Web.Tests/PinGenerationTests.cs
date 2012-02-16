using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureManager.Web.Models;

namespace AzureManager.Web.Tests
{
  [TestClass]
  public class PinGenerationTests
  {
    [TestMethod]
    public void PinGeneration()
    {
      RegistrationModel model = new RegistrationModel();

      List<string> generatedPins = new List<string>();

      for (int i = 0; i < 10000; i++)
      {
        string newPin = model.GeneratePin();

        if (generatedPins.Contains(newPin))
        {
          Assert.Fail("Duplication pin found at index " + i);
          return;
        }

        generatedPins.Add(newPin);
      }
    }
  }
}
